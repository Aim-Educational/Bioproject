using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderdevice_url : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as device_url).device_url_id == (item2 as device_url).device_url_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            device_url data = item as device_url;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.device_url.Remove(db.device_url.Where(d => d.device_url_id == data.device_url_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as device_url).device_url_id}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(device_url.device_url_id))
                });
                

                foreach (var data in db.device_url)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    if(data.device != null) data.device.ToString();

                }
            }
        }
    }
}
