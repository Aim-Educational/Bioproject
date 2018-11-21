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

            using (var db = new PlanningContext())
            {
                db.devices.Remove(db.devices.Where(d => d.device_id == data.device_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as device).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
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
                

                foreach (var data in db.devices)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    if(data.device2 != null) data.device2.ToString();
if(data.device_type != null) data.device_type.ToString();
foreach(var val0 in data.action_level){
if(val0.action_type != null) val0.action_type.ToString();
if(val0.device != null) val0.device.ToString();
}
foreach(var val0 in data.alarms){
if(val0.alarm_type != null) val0.alarm_type.ToString();
if(val0.device != null) val0.device.ToString();
if(val0.group_type != null) val0.group_type.ToString();
}
foreach(var val0 in data.device_address){
if(val0.device != null) val0.device.ToString();
if(val0.device_address_type != null) val0.device_address_type.ToString();
}
foreach(var val0 in data.device1){
if(val0.device2 != null) val0.device2.ToString();
if(val0.device_type != null) val0.device_type.ToString();
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
if(val0.device != null) val0.device.ToString();
foreach(var val1 in val0.device_history){
}
}
foreach(var val0 in data.device_history){
if(val0.device != null) val0.device.ToString();
if(val0.device_history_action != null) val0.device_history_action.ToString();
if(val0.supplier != null) val0.supplier.ToString();
}
foreach(var val0 in data.device_url){
if(val0.device != null) val0.device.ToString();
}
foreach(var val0 in data.device_value){
if(val0.device != null) val0.device.ToString();
}
foreach(var val0 in data.rss_configuration){
if(val0.device != null) val0.device.ToString();
if(val0.update_period != null) val0.update_period.ToString();
}

                }
            }
        }
    }
}
