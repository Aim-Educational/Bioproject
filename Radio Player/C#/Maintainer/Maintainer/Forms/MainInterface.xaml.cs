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

        private IEditor editorControl
        {
            get => this.editorContent.Content as IEditor;
        }

        public MainInterface()
        {
            InitializeComponent();

            this.searchContent.Content = new SearchControl();

            this.openEditor(new SearchProviderGenre(), new EditorGenre(this));
        }

        void openEditor(ISearchProvider provider, UserControl editor)
        {
            if((editor as IEditor) == null)
                throw new ArgumentException("The Editor given does not implement the IEditor interface.", "editor");

            this.searchControl.provider = provider;
            this.searchControl.action = editor as IEditor;
            this.editorContent.Content = editor;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(this.editorControl == null)
                return;
            
            if(this.editorControl.isDataDirty)
            {
                this.editorControl.saveChanges();
                this.searchControl.provider = this.searchControl.provider; // This refreshes the data.
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(this.editorControl == null)
                return;

            this.editorControl.flagAsCreateMode();
        }
    }
}
