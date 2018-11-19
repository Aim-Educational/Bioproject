using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderdevice_value : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as device_value).device_value_id == (item2 as device_value).device_value_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            device_value data = item as device_value;
            if(data == null)
                return;

            using (var db = new RadioPlayer())
            {
                db.device_value.Remove(db.device_value.Where(d => d.device_value_id == data.device_value_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as device_value).device_value_id}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new RadioPlayer())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(device_value.device_value_id))
                });
                

                foreach (var data in db.device_value)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    data.device.ToString();

                }
            }
        }
    }
}
