using System.Windows;
using System.Windows.Controls;
using Aim.DatabaseInterface.Interfaces;
using Aim.DatabaseInterface.Windows;
using Maintainer.Controls;
using Maintainer.SearchProviders;
using Maintainer.Util;
using System;

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

            this.addEditorButton("Collection",  new SearchProviderCollection(), new EditorCollection(this._mainInterface));
            this.addEditorButton("Format",      new SearchProviderFormat(),     new EditorFormat(this._mainInterface));
            this.addEditorButton("Genre",       new SearchProviderGenre(),      new EditorGenre(this._mainInterface));
            this.addEditorButton("Mood",        new SearchProviderMood(),       new EditorMood(this._mainInterface));
            this.addEditorButton("Playlist",    new SearchProviderPlaylist(),   new EditorPlaylist(this._mainInterface));
            this.addEditorButton("Track",       new SearchProviderTrack(),      new EditorTrack(this._mainInterface));
            this.addEditorButton("TrackParser", new SearchProviderTrack(),      new EditorTrackPicker(this._mainInterface));

            MessageBox.Show(Environment.UserDomainName);

            var button = new Button();
            button.Content = "[New Window]";
            button.Click += (sender, e) => (new MainWindow()).Show();
            this.toolbar.Items.Add(button);
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
