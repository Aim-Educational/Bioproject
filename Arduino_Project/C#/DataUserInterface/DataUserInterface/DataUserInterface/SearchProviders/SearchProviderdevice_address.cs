using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderdevice_address : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as device_address).device_address_id == (item2 as device_address).device_address_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            device_address data = item as device_address;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.device_address.Remove(db.device_address.Where(d => d.device_address_id == data.device_address_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as device_address).device_address_id}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(device_address.device_address_id))
                });
                

                foreach (var data in db.device_address)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    if(data.device != null) data.device.ToString();
if(data.device_address_type != null) data.device_address_type.ToString();

                }
            }
        }
    }
}
