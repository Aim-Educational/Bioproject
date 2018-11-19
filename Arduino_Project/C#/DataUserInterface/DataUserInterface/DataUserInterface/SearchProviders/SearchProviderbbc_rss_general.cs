using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderbbc_rss_general : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as bbc_rss_general).bbc_rss_general_id == (item2 as bbc_rss_general).bbc_rss_general_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            bbc_rss_general data = item as bbc_rss_general;
            if(data == null)
                return;

            using (var db = new RadioPlayer())
            {
                db.bbc_rss_general.Remove(db.bbc_rss_general.Where(d => d.bbc_rss_general_id == data.bbc_rss_general_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as bbc_rss_general).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new RadioPlayer())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(bbc_rss_general.bbc_rss_general_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "description",
                    Binding = new Binding(nameof(bbc_rss_general.description))
                });
                

                foreach (var data in db.bbc_rss_general)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    
                }
            }
        }
    }
}
