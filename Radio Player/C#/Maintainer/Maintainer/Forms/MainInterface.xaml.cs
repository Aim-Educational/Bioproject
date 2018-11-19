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
using Maintainer.Controls;
using Maintainer.Interfaces;
using Maintainer.SearchProviders;

namespace Maintainer.Forms
{
    /// <summary>
    /// Interaction logic for MainInterface.xaml
    /// </summary>
    public partial class MainInterface : UserControl
    {
        private SearchControl searchControl
        {
            get => this.searchContent.Content as SearchControl;
        }

        private SearchControl selectorControl
        {
            get => this.selectorContent.Content as SearchControl;
        }

        private IEditor editorControl
        {
            get => this.editorContent.Content as IEditor;
        }

        public MainInterface()
        {
            InitializeComponent();

            this.searchContent.Content = new SearchControl();
            this.selectorContent.Content = new SearchControl();
            this.selectorContent.Visibility = Visibility.Hidden;
        }

        public void openEditor(ISearchProvider provider, UserControl editor)
        {
            if((editor as IEditor) == null)
                throw new ArgumentException("The Editor given does not implement the IEditor interface.", "editor");

            this.searchControl.provider = provider;
            this.searchControl.action = editor as IEditor;
            this.editorContent.Content = editor;
            this.selectorContent.Visibility = Visibility.Hidden;
        }

        public void openSelector(ISearchProvider provider, Action<Object> onSelect)
        {
            this.selectorContent.Visibility = Visibility.Visible;
            this.selectorControl.provider = provider;
            this.selectorControl.action = new SelectorSearchAction(this, onSelect);
        }

        public void protectedExecute(Action func)
        {
            if(func == null)
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
                this.labelStatus.Content = $"AN ERROR HAS OCCURED: {ex.Message}";
            }
#endif
        }

        // Apply
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(this.editorControl == null)
                return;
            
            if(this.editorControl.isDataDirty)
            {
                this.protectedExecute(() => this.editorControl.saveChanges());
                this.searchControl.provider = this.searchControl.provider; // This refreshes the data.
            }
        }

        // Create
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(this.editorControl == null)
                return;

            this.editorControl.flagAsCreateMode();
        }

        // Delete
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if(this.searchControl == null || this.searchControl.provider == null || this.searchControl.dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Please select an item from the 'Search' grid.", "Invalid operation", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            var result = DeleteWindow.Show(this.editorControl, this.searchControl.dataGrid.SelectedItem);
            if(result == MessageBoxResult.No)
                return;

            this.protectedExecute(() => this.searchControl.provider.deleteItem(this.searchControl.dataGrid.SelectedItem));
            this.searchControl.provider = this.searchControl.provider; // This refreshes the data.
        }
    }

    public class SelectorSearchAction : ISearchAction
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
