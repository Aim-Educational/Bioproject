using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderdevice_type : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as device_type).device_type_id == (item2 as device_type).device_type_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            device_type data = item as device_type;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.device_type.Remove(db.device_type.Where(d => d.device_type_id == data.device_type_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as device_type).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(device_type.device_type_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "description",
                    Binding = new Binding(nameof(device_type.description))
                });
                

                foreach (var data in db.device_type)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    data.unit.ToString();
foreach(var val0 in data.devices){
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
}

                }
            }
        }
    }
}
