using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using EFLayer.Model;

namespace Maintainer.SearchProviders
{
    public class SearchProviderTrack : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as tbl_track).track_id == (item2 as tbl_track).track_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            tbl_track track = item as tbl_track;
            if(track == null)
                return;

            using (var db = new RadioPlayer())
            {
                db.tbl_track.Remove(db.tbl_track.Where(t => t.track_id == track.track_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return (item as tbl_track).title;
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new RadioPlayer())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(tbl_track.track_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Title",
                    Binding = new Binding(nameof(tbl_track.title))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Subtitle",
                    Binding = new Binding(nameof(tbl_track.subtitle))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Publisher",
                    Binding = new Binding(nameof(tbl_track.publisher))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Likes",
                    Binding = new Binding(nameof(tbl_track.likes))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Dislikes",
                    Binding = new Binding(nameof(tbl_track.dislikes))
                });

                foreach (var track in db.tbl_track)
                {
                    grid.Items.Add(track);

                    // Cache some of the data we need
                    track.tbl_format.ToString();

                    foreach(var v in track.tbl_moodmap)
                    {
                        v.ToString();
                        v.tbl_mood.ToString();
                    }

                    foreach(var v in track.tbl_genremap)
                    {
                        v.ToString();
                        v.tbl_genre.ToString();
                    }
                }
            }
        }
    }
}
