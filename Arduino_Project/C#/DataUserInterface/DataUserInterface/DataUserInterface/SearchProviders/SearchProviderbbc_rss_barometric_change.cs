using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderbbc_rss_barometric_change : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as bbc_rss_barometric_change).bbc_rss_barometric_change_id == (item2 as bbc_rss_barometric_change).bbc_rss_barometric_change_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            bbc_rss_barometric_change data = item as bbc_rss_barometric_change;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.bbc_rss_barometric_change.Remove(db.bbc_rss_barometric_change.Where(d => d.bbc_rss_barometric_change_id == data.bbc_rss_barometric_change_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as bbc_rss_barometric_change).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(bbc_rss_barometric_change.bbc_rss_barometric_change_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "description",
                    Binding = new Binding(nameof(bbc_rss_barometric_change.description))
                });
                

                foreach (var data in db.bbc_rss_barometric_change)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    
                }
            }
        }
    }
}
