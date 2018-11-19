using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderdevice_address_type : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as device_address_type).device_address_type_id == (item2 as device_address_type).device_address_type_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            device_address_type data = item as device_address_type;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.device_address_type.Remove(db.device_address_type.Where(d => d.device_address_type_id == data.device_address_type_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as device_address_type).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(device_address_type.device_address_type_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "description",
                    Binding = new Binding(nameof(device_address_type.description))
                });
                

                foreach (var data in db.device_address_type)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    foreach(var val0 in data.device_address){
val0.device.ToString();
val0.device_address_type.ToString();
}

                }
            }
        }
    }
}
