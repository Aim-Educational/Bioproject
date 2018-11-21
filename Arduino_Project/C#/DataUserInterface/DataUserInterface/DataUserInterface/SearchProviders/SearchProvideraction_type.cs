using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProvideraction_type : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as action_type).action_type_id == (item2 as action_type).action_type_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            action_type data = item as action_type;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.action_type.Remove(db.action_type.Where(d => d.action_type_id == data.action_type_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as action_type).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(action_type.action_type_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "description",
                    Binding = new Binding(nameof(action_type.description))
                });
                

                foreach (var data in db.action_type)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    foreach(var val0 in data.action_level){
if(val0.action_type != null) val0.action_type.ToString();
if(val0.device != null) val0.device.ToString();
}
foreach(var val0 in data.group_action){
if(val0.action_type != null) val0.action_type.ToString();
if(val0.group_type != null) val0.group_type.ToString();
}

                }
            }
        }
    }
}
