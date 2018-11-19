using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using EFLayer.Model;

namespace Maintainer.SearchProviders
{
    public class SearchProviderCollection : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as tbl_collection).collection_id == (item2 as tbl_collection).collection_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            tbl_collection collection = item as tbl_collection;
            if(collection == null)
                return;

            using (var db = new RadioPlayer())
            {
                db.tbl_collection.Remove(db.tbl_collection.Where(g => g.collection_id == collection.collection_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return (item as tbl_collection).title;
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new RadioPlayer())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(tbl_collection.collection_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Title",
                    Binding = new Binding(nameof(tbl_collection.title))
                });

                foreach(var collection in db.tbl_collection)
                {
                    grid.Items.Add(collection);

                    // Cache data
                    foreach(var map in collection.tbl_collectionmap)
                    {
                        map.ToString();
                        map.tbl_collection.ToString();
                        map.tbl_track.ToString();
                    }
                }
            }
        }
    }
}
