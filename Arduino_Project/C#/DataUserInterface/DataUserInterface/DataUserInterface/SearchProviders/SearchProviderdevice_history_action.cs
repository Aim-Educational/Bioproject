using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderdevice_history_action : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as device_history_action).device_history_action1 == (item2 as device_history_action).device_history_action1;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            device_history_action data = item as device_history_action;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.device_history_action.Remove(db.device_history_action.Where(d => d.device_history_action1 == data.device_history_action1).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as device_history_action).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(device_history_action.device_history_action1))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "description",
                    Binding = new Binding(nameof(device_history_action.description))
                });
                

                foreach (var data in db.device_history_action)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    if(data.device != null) data.device.ToString();
foreach(var val0 in data.device_history){
if(val0.device != null) val0.device.ToString();
if(val0.device_history_action != null) val0.device_history_action.ToString();
if(val0.supplier != null) val0.supplier.ToString();
}

                }
            }
        }
    }
}
