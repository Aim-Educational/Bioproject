using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProvidergroup_action : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as group_action).group_action_id == (item2 as group_action).group_action_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            group_action data = item as group_action;
            if(data == null)
                return;

            using (var db = new RadioPlayer())
            {
                db.group_action.Remove(db.group_action.Where(d => d.group_action_id == data.group_action_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as group_action).group_action_id}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new RadioPlayer())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(group_action.group_action_id))
                });
                

                foreach (var data in db.group_action)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    data.action_type.ToString();
data.group_type.ToString();

                }
            }
        }
    }
}
