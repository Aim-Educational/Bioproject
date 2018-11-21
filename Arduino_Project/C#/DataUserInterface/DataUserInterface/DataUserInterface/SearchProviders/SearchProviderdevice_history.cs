using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderdevice_history : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as device_history).device_history_id == (item2 as device_history).device_history_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            device_history data = item as device_history;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.device_history.Remove(db.device_history.Where(d => d.device_history_id == data.device_history_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as device_history).device_history_id}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(device_history.device_history_id))
                });
                

                foreach (var data in db.device_history)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    if(data.device != null) data.device.ToString();
if(data.device_history_action != null) data.device_history_action.ToString();
if(data.supplier != null) data.supplier.ToString();

                }
            }
        }
    }
}
