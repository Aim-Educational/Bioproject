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
    /// Interaction logic for EditorCollection.xaml
    /// </summary>
    public partial class EditorCollection : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool _isCreateMode;

        public EditorCollection(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.listTracks.provider = new SearchProviderTrack();
            this.listTracks.mainInterface = mi;
        }   

        public bool isDataDirty => true;

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.txtID.Text = "(CREATE NEW)";
            this.txtTitle.Clear();
            this.listTracks.clear();
        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var collection = selectedItem as tbl_collection;
            if (collection == null)
                throw new ArgumentException("Invalid type. Expected a tbl_collection.", "selectedItem");

            this.txtID.Text = $"{collection.collection_id}";
            this.txtTitle.Text = collection.title;

            this.listTracks.clear();
            foreach(var track in collection.tbl_collectionmap.OrderBy(c => c.sequence_index).Select(c => c.tbl_track))
                this.listTracks.addItem(track);

            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new RadioPlayer())
            {
                tbl_collection collection = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.txtID.Text);
                    collection = db.tbl_collection.Where(c => c.collection_id == id).FirstOrDefault();
                    if (collection == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.txtID.Text}' does not exist.";
                        return;
                    }
                }
                else
                    collection = new tbl_collection();
                
                collection.title = this.txtTitle.Text;

                // Any values left in mapList need to be removed.
                var mapList   = collection.tbl_collectionmap.ToList();
                var trackList = this.listTracks.items.Select(i => i as tbl_track).ToList();
                for (int index = 0; index < trackList.Count(); index++)
                {
                    var track = trackList[index];
                    var map   = mapList.Where(m => m.track_id == track.track_id).FirstOrDefault();
                    if(map == null)
                    {
                        db.tbl_collectionmap.Add(new tbl_collectionmap()
                        {
                            tbl_collection = collection,
                            track_id = track.track_id,
                            sequence_index = index
                        });
                        continue;
                    }

                    map.sequence_index = index;
                    mapList.Remove(map);
                }

                foreach(var map in mapList)
                    db.tbl_collectionmap.Remove(map);

                if (this._isCreateMode)
                    db.tbl_collection.Add(collection);
                db.SaveChanges();
            }
        }
    }
}
