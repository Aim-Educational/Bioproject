using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Aim.DatabaseInterface.Controls;
using Aim.DatabaseInterface.Interfaces;
using Aim.DatabaseInterface.Windows;
using EFLayer.Model;
using Maintainer.SearchProviders;

namespace Maintainer.Controls
{
    /// <summary>
    /// Interaction logic for EditorTrack.xaml
    /// </summary>
    public partial class EditorTrack : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool _isCreateMode;
        public bool isDataDirty => true;

        public EditorTrack(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.selectorFormat.provider = new SearchProviderFormat();
            this.selectorFormat.mainInterface = mi;
            
            this.listMoods.provider = new SearchProviderMood();
            this.listMoods.mainInterface = mi;

            this.listGenres.provider = new SearchProviderGenre();
            this.listGenres.mainInterface = mi;
        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.txtID.Text = "(CREATE NEW)";
            var textboxes = new TextBox[]
            {
                txtTitle, txtSubtitle, txtBitrate,
                txtPublisher, txtServerName, txtFolder,
                txtFileName, txtDuration, txtFileSize,
                txtLikes, txtDislikes
            };
            foreach(var tb in textboxes)
                tb.Text = "";

            this.selectorFormat.item = null;
            this.dateRecorded.SelectedDate = null;
            this.dateReleased.SelectedDate = null;
            this.listArtists.clear();
            this.listComposers.clear();
            this.listKeywords.clear();
            this.listMoods.clear();
            this.listGenres.clear();
            this.checkboxPAL.IsChecked = false;
        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var track = selectedItem as tbl_track;
            if (track == null)
                throw new ArgumentException("Invalid type. Expected a tbl_track.", "selectedItem");

            this.txtID.Text                 = $"{track.track_id}";
            this.txtTitle.Text              = track.title;
            this.txtSubtitle.Text           = track.subtitle;
            this.txtBitrate.Text            = $"{track.bitrate}";
            this.txtPublisher.Text          = track.publisher;
            this.checkboxPAL.IsChecked      = track.parental_advisory;
            this.txtServerName.Text         = track.server_name;
            this.txtFolder.Text             = track.folder_path;
            this.txtFileName.Text           = track.file_name;
            this.txtDuration.Text           = $"{track.duration}";
            this.selectorFormat.item        = track.tbl_format;
            this.txtFileSize.Text           = $"{track.filesize}";
            this.dateRecorded.SelectedDate  = track.date_recorded;
            this.dateReleased.SelectedDate  = track.date_released;
            this.txtLikes.Text              = $"{track.likes}";
            this.txtDislikes.Text           = $"{track.dislikes}";
            this.populateList(this.listArtists, track.artists);
            this.populateList(this.listComposers, track.composers);
            this.populateList(this.listKeywords, track.keywords);
            this.populateSelectorList(this.listMoods, track.tbl_moodmap.Select(m => m.tbl_mood));
            this.populateSelectorList(this.listGenres, track.tbl_genremap.Select(g => g.tbl_genre));

            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new RadioPlayer())
            {
                tbl_track track = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.txtID.Text);
                    track = db.tbl_track.Where(t => t.track_id == id).FirstOrDefault();
                    if (track == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.txtID.Text}' does not exist.";
                        return;
                    }
                }
                else
                    track = new tbl_track();

                track.title             = this.txtTitle.Text;
                track.subtitle          = this.txtSubtitle.Text;
                track.bitrate           = this.txtBitrate.Text;
                track.publisher         = this.txtPublisher.Text;
                track.parental_advisory = (bool)this.checkboxPAL.IsChecked;
                track.server_name       = this.txtServerName.Text;
                track.folder_path       = this.txtFolder.Text;
                track.file_name         = this.txtFileName.Text;
                track.duration          = Convert.ToInt32(this.txtDuration.Text);
                track.format_id         = ((tbl_format)this.selectorFormat.item).format_id;
                track.filesize          = Convert.ToInt64(this.txtFileSize.Text);
                track.date_recorded     = this.dateRecorded.SelectedDate;
                track.date_released     = this.dateReleased.SelectedDate;
                track.likes             = Convert.ToInt32(this.txtLikes.Text);
                track.dislikes          = Convert.ToInt32(this.txtDislikes.Text);
                track.artists           = this.stringifyList(this.listArtists);
                track.composers         = this.stringifyList(this.listComposers);
                track.keywords          = this.stringifyList(this.listKeywords);

                // Sort out the genre mappings.
                List<tbl_genremap> genresToUnmap;
                List<tbl_genre> genresToMap;
                IEditorHelper.getMappings<tbl_genremap, tbl_genre>(
                    out genresToUnmap, out genresToMap, this.listGenres.items.Select(i => i as tbl_genre),
                    track.tbl_genremap, (map, value) => map.genre_id == value.genre_id
                );
                foreach (var toUnmap in genresToUnmap)
                    db.tbl_genremap.Remove(toUnmap);
                foreach (var toMap in genresToMap)
                {
                    tbl_genremap map = new tbl_genremap();
                    map.tbl_track = track;
                    map.tbl_genre = toMap;
                    db.tbl_genremap.Add(map);
                }

                // Sort out the mood mappings.
                List<tbl_moodmap> moodsToUnmap;
                List<tbl_mood>    moodsToMap;
                IEditorHelper.getMappings<tbl_moodmap, tbl_mood>(
                    out moodsToUnmap, out moodsToMap, this.listMoods.items.Select(i => i as tbl_mood),
                    track.tbl_moodmap, (map, value) => map.mood_id == value.mood_id
                );
                foreach (var toUnmap in moodsToUnmap)
                    db.tbl_moodmap.Remove(toUnmap);
                foreach (var toMap in moodsToMap)
                {
                    tbl_moodmap map = new tbl_moodmap();
                    map.tbl_track = track;
                    map.tbl_mood  = toMap;
                    db.tbl_moodmap.Add(map);
                }

                if (this._isCreateMode)
                    db.tbl_track.Add(track);
                db.SaveChanges();
            }
        }

        private string stringifyList(ListboxEditorControl list)
        {
            string output = "";

            foreach(var value in list.list.Items)
                output += value + ",";

            return output;
        }

        private void populateList(ListboxEditorControl list, string csv)
        {
            list.clear();

            foreach (var value in csv.Split(','))
                list.list.Items.Add(value);
        }

        private void populateSelectorList(ListboxSelectorEditorControl list, IEnumerable<Object> objs)
        {
            list.clear();
            foreach(var obj in objs)
                list.addItem(obj);
        }
    }
}
