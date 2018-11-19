using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderdevice : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as device).device_id == (item2 as device).device_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            device data = item as device;
            if(data == null)
                return;

            using (var db = new RadioPlayer())
            {
                db.device.Remove(db.device.Where(d => d.device_id == data.device_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as device).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new RadioPlayer())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(device.device_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "name",
                    Binding = new Binding(nameof(device.name))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "description",
                    Binding = new Binding(nameof(device.description))
                });
                

                foreach (var data in db.device)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    data.device2.ToString();
data.device_type.ToString();
foreach(var val0 in data.action_level){
val0.action_type.ToString();
val0.device.ToString();
foreach(var val0 in data.alarms){
val0.alarm_type.ToString();
val0.device.ToString();
val0.group_type.ToString();
foreach(var val0 in data.device_address){
val0.device.ToString();
val0.device_address_type.ToString();
foreach(var val0 in data.device1){
val0.device2.ToString();
val0.device_type.ToString();
foreach(var val1 in val0.action_level){
}
foreach(var val1 in val0.alarms){
}
foreach(var val1 in val0.device_address){
}
foreach(var val1 in val0.device1){
}
foreach(var val1 in val0.device_history_action){
}
foreach(var val1 in val0.device_history){
}
foreach(var val1 in val0.device_url){
}
foreach(var val1 in val0.device_value){
}
foreach(var val1 in val0.rss_configuration){
}
}
foreach(var val0 in data.device_history_action){
val0.device.ToString();
foreach(var val1 in val0.device_history){
}
}
foreach(var val0 in data.device_history){
val0.device.ToString();
val0.device_history_action.ToString();
val0.supplier.ToString();
foreach(var val0 in data.device_url){
val0.device.ToString();
foreach(var val0 in data.device_value){
val0.device.ToString();
foreach(var val0 in data.rss_configuration){
val0.device.ToString();
val0.update_period.ToString();
}

                }
            }
        }
    }
}
