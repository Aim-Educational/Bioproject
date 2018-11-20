using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProvideralarm : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as alarm).alarm_id == (item2 as alarm).alarm_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            alarm data = item as alarm;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.alarms.Remove(db.alarms.Where(d => d.alarm_id == data.alarm_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as alarm).alarm_id}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(alarm.alarm_id))
                });
                

                foreach (var data in db.alarms)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    data.alarm_type.ToString();
data.device.ToString();
data.group_type.ToString();

                }
            }
        }
    }
}
