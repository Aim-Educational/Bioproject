using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProvidergroup_member : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as group_member).group_member_id == (item2 as group_member).group_member_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            group_member data = item as group_member;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.group_member.Remove(db.group_member.Where(d => d.group_member_id == data.group_member_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as group_member).group_member_id}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(group_member.group_member_id))
                });
                

                foreach (var data in db.group_member)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    data.contact.ToString();
data.group_type.ToString();

                }
            }
        }
    }
}
