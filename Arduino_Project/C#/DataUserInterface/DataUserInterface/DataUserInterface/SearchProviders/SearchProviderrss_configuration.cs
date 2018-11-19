using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderrss_configuration : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as rss_configuration).rss_configuration_id == (item2 as rss_configuration).rss_configuration_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            rss_configuration data = item as rss_configuration;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.rss_configuration.Remove(db.rss_configuration.Where(d => d.rss_configuration_id == data.rss_configuration_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as rss_configuration).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(rss_configuration.rss_configuration_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "description",
                    Binding = new Binding(nameof(rss_configuration.description))
                });
                

                foreach (var data in db.rss_configuration)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    data.device.ToString();
data.update_period.ToString();

                }
            }
        }
    }
}
