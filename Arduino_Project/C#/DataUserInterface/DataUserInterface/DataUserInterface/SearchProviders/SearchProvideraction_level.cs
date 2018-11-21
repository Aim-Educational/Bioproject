using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProvideraction_level : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as action_level).action_level_id == (item2 as action_level).action_level_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            action_level data = item as action_level;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.action_level.Remove(db.action_level.Where(d => d.action_level_id == data.action_level_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as action_level).action_level_id}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(action_level.action_level_id))
                });
                

                foreach (var data in db.action_level)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    if(data.action_type != null) data.action_type.ToString();
if(data.device != null) data.device.ToString();

                }
            }
        }
    }
}
