using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Aim.DatabaseInterface.Controls;
using Aim.DatabaseInterface.Interfaces;

namespace Aim.DatabaseInterface.Windows
{
    /// <summary>
    /// The main interface.
    /// 
    /// This control contains the following:
    ///     * A <see cref="SearchControl"/> to allow the user to select an item to modify.
    ///     * An <see cref="IEditor"/> to provide the actual functionality to create/modify data.
    ///     * A <see cref="SearchControl"/> to allow the user to select a data for controls such as <see cref="ListboxSelectorEditorControl"/>.
    ///     * A delete button that should 'just work'.
    ///     * A create button that should 'just work'.
    ///     * An apply/save button that should 'just work'.
    ///     * The ability to resize parts of the interface, to allow the user to customise what they're seeing.
    ///     * A status bar.
    /// </summary>
    public partial class MainInterface : UserControl
    {
        private SearchControl searchControl   => this.searchContent.Content as SearchControl;
        private SearchControl selectorControl => this.selectorContent.Content as SearchControl;
        private IEditor       editorControl   => this.editorContent.Content as IEditor;

        /// <summary>
        /// Sets the status text in the status bar.
        /// </summary>
        public string statusText
        {
            set => this.labelStatus.Content = value;
        }

        public MainInterface()
        {
            InitializeComponent();

            this.searchContent.Content = new SearchControl();
            this.selectorContent.Content = new SearchControl();
            this.selectorContent.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Opens the 'search' search form, as well as an editor for the user to make use of.
        /// </summary>
        /// <param name="provider">
        ///     This provider will provide the data for both the user and <paramref name="editor"/> to use,
        ///     as well as the ability to delete items.
        /// </param>
        /// <param name="editor">
        ///     This editor will provide the ability to for a user save/create data.
        ///     This class *must* implement <see cref="IEditor"/>
        /// </param>
        public void openEditor(ISearchProvider provider, UserControl editor)
        {
            if ((editor as IEditor) == null)
                throw new ArgumentException("The Editor given does not implement the IEditor interface.", "editor");

            this.searchControl.provider = provider;
            this.searchControl.action = editor as IEditor;
            this.editorContent.Content = editor;
            this.selectorContent.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Opens the 'selection' search form to allow the user to select a specific item, which is then
        /// passed to the given action.
        /// </summary>
        /// <param name="provider">The provider used to provide the data that the user can select.</param>
        /// <param name="onSelect">An action to be performed on the item that the user selects.</param>
        public void openSelector(ISearchProvider provider, Action<Object> onSelect)
        {
            this.selectorContent.Visibility = Visibility.Visible;
            this.selectorControl.provider = provider;
            this.selectorControl.action = new SelectorSearchAction(this, onSelect);
        }

        /// <summary>
        /// A special helper function that will call the given <paramref name="func"/>, and if
        /// an exception is caught it will update the <see cref="statusText"/> to the exception's message.
        /// 
        /// Note that if 'DEBUG' is defined, then this function won't catch exceptions (to make debugging easier).
        /// </summary>
        /// <param name="func">The function to execute.</param>
        public void protectedExecute(Action func)
        {
            if (func == null)
                throw new ArgumentNullException("func");

#if DEBUG
            func();
#else
            try
            {
                func();
            }
            catch(Exception ex)
            {
                this.statusText = $"AN ERROR HAS OCCURED: {ex.Message}";
            }
#endif
        }

        // Apply
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.editorControl == null)
                return;

            if (this.editorControl.isDataDirty)
            {
                this.protectedExecute(() => this.editorControl.saveChanges());
                this.searchControl.provider = this.searchControl.provider; // This refreshes the data.
            }
        }

        // Create
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.editorControl == null)
                return;

            this.editorControl.flagAsCreateMode();
        }

        // Delete
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (this.searchControl == null || this.searchControl.provider == null || this.searchControl.dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Please select an item from the 'Search' grid.", "Invalid operation", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            var result = DeleteBox.Show(this.editorControl, this.searchControl.dataGrid.SelectedItem);
            if (result == MessageBoxResult.No)
                return;

            this.protectedExecute(() => this.searchControl.provider.deleteItem(this.searchControl.dataGrid.SelectedItem));
            this.searchControl.provider = this.searchControl.provider; // This refreshes the data.
        }
    }

    internal class SelectorSearchAction : ISearchAction
    {
        private MainInterface _mi;
        private Action<Object> _onSelect;

        public SelectorSearchAction(MainInterface mi, Action<Object> onSelect)
        {
            this._mi = mi;
            this._onSelect = onSelect;
        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            this._mi.selectorContent.Visibility = Visibility.Hidden;
            this._onSelect(selectedItem);

            return RefreshSearchList.no;
        }
    }
}
