using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Aim.DatabaseInterface.Interfaces;
using Aim.DatabaseInterface.Windows;
using EFLayer.Model;
using Maintainer.SearchProviders;

namespace Maintainer.Controls
{
    /// <summary>
    /// Interaction logic for EditorPlaylist.xaml
    /// </summary>
    public partial class EditorPlaylist : UserControl, IEditor
    {
        private bool _isCreateMode;
        private MainInterface _mainInterface;
        public bool isDataDirty => true;

        public EditorPlaylist(MainInterface mi)
        {
            InitializeComponent();

            this._mainInterface = mi;
            this.listTracks.mainInterface = mi;
            this.listTracks.provider = new SearchProviderTrack();
        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.txtID.Text = "(CREATE NEW)";
            this.txtDescription.Clear();
            this.txtLikes.Clear();
            this.txtDislikes.Clear();
            this.txtCreator.Clear();
            this.checkboxRandomise.IsChecked = false;
            this.listTracks.clear();
        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var playlist = selectedItem as tbl_playlist_header;
            if (playlist == null)
                throw new ArgumentException("Invalid type. Expected a tbl_playlist_header.", "selectedItem");

            this.txtID.Text                  = $"{playlist.playlist_header_id}";
            this.txtDescription.Text         = playlist.description;
            this.txtLikes.Text               = $"{playlist.likes}";
            this.txtDislikes.Text            = $"{playlist.dislikes}";
            this.txtCreator.Text             = playlist.creator;
            this.checkboxRandomise.IsChecked = playlist.play_random;

            this.listTracks.clear();
            foreach (var track in playlist.tbl_playlist_lines.OrderBy(p => p.sequence_index).Select(p => p.tbl_track))
                this.listTracks.addItem(track);

            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new RadioPlayer())
            {
                tbl_playlist_header playlist = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.txtID.Text);
                    playlist = db.tbl_playlist_header.Where(p => p.playlist_header_id== id).FirstOrDefault();
                    if (playlist == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.txtID.Text}' does not exist.";
                        return;
                    }
                }
                else
                    playlist = new tbl_playlist_header();

                playlist.description = this.txtDescription.Text;
                playlist.likes = Convert.ToInt32(this.txtLikes.Text);
                playlist.dislikes = Convert.ToInt32(this.txtDislikes.Text);
                playlist.creator = this.txtCreator.Text;
                playlist.play_random = (bool)this.checkboxRandomise.IsChecked;

                // Any values left in mapList need to be removed.
                var mapList = playlist.tbl_playlist_lines.ToList();
                var trackList = this.listTracks.items.Select(i => i as tbl_track).ToList();
                for (int index = 0; index < trackList.Count(); index++)
                {
                    var track = trackList[index];
                    var map = mapList.Where(m => m.track_id == track.track_id).FirstOrDefault();
                    if (map == null)
                    {
                        db.tbl_playlist_lines.Add(new tbl_playlist_lines()
                        {
                            tbl_playlist_header = playlist,
                            track_id            = track.track_id,
                            sequence_index      = index
                        });
                        continue;
                    }

                    map.sequence_index = index;
                    mapList.Remove(map);
                }

                foreach (var map in mapList)
                    db.tbl_playlist_lines.Remove(map);

                if (this._isCreateMode)
                    db.tbl_playlist_header.Add(playlist);
                db.SaveChanges();
            }
        }
    }
}
