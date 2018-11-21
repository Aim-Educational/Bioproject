using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProvideralarm_type : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as alarm_type).alarm_type_id == (item2 as alarm_type).alarm_type_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            alarm_type data = item as alarm_type;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.alarm_type.Remove(db.alarm_type.Where(d => d.alarm_type_id == data.alarm_type_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as alarm_type).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(alarm_type.alarm_type_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "description",
                    Binding = new Binding(nameof(alarm_type.description))
                });
                

                foreach (var data in db.alarm_type)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    foreach(var val0 in data.alarms){
if(val0.alarm_type != null) val0.alarm_type.ToString();
if(val0.device != null) val0.device.ToString();
if(val0.group_type != null) val0.group_type.ToString();
}

                }
            }
        }
    }
}
