using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProvidergroup_type : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as group_type).group_type_id == (item2 as group_type).group_type_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            group_type data = item as group_type;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.group_type.Remove(db.group_type.Where(d => d.group_type_id == data.group_type_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as group_type).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(group_type.group_type_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "name",
                    Binding = new Binding(nameof(group_type.name))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "description",
                    Binding = new Binding(nameof(group_type.description))
                });
                

                foreach (var data in db.group_type)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    if(data.group_type2 != null) data.group_type2.ToString();
foreach(var val0 in data.alarms){
if(val0.alarm_type != null) val0.alarm_type.ToString();
if(val0.device != null) val0.device.ToString();
if(val0.group_type != null) val0.group_type.ToString();
}
foreach(var val0 in data.group_action){
if(val0.action_type != null) val0.action_type.ToString();
if(val0.group_type != null) val0.group_type.ToString();
}
foreach(var val0 in data.group_member){
if(val0.contact != null) val0.contact.ToString();
if(val0.group_type != null) val0.group_type.ToString();
}
foreach(var val0 in data.group_type1){
if(val0.group_type2 != null) val0.group_type2.ToString();
foreach(var val1 in val0.alarms){
}
foreach(var val1 in val0.group_action){
}
foreach(var val1 in val0.group_member){
}
foreach(var val1 in val0.group_type1){
}
}

                }
            }
        }
    }
}
