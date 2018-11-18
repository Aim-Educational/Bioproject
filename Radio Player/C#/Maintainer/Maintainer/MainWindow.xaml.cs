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
using Maintainer.Forms;
using Maintainer.Interfaces;
using Maintainer.SearchProviders;

namespace Maintainer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainInterface _mainInterface;

        public MainWindow()
        {
            InitializeComponent();

            this._mainInterface = new MainInterface();
            this.content.Content = this._mainInterface;

            // TODO: Put in alphabetical order.
            this.addEditorButton("Genre",       new SearchProviderGenre(),      new EditorGenre(this._mainInterface));
            this.addEditorButton("Mood",        new SearchProviderMood(),       new EditorMood(this._mainInterface));
            this.addEditorButton("Format",      new SearchProviderFormat(),     new EditorFormat(this._mainInterface));
            this.addEditorButton("Track",       new SearchProviderTrack(),      new EditorTrack(this._mainInterface));
            this.addEditorButton("Collection",  new SearchProviderCollection(), new EditorCollection(this._mainInterface));
            this.addEditorButton("Playlist",    new SearchProviderPlaylist(),   new EditorPlaylist(this._mainInterface));
        }

        private void addEditorButton(string name, ISearchProvider provider, UserControl editor)
        {
            var button = new Button();
            button.Content = name;
            button.Click += (sender, e) => this._mainInterface.openEditor(provider, editor);
            this.toolbar.Items.Add(button);
        }
    }
}
