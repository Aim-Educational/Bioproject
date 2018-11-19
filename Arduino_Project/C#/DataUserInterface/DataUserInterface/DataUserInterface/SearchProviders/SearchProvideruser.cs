using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProvideruser : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as user).user_id == (item2 as user).user_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            user data = item as user;
            if(data == null)
                return;

            using (var db = new RadioPlayer())
            {
                db.user.Remove(db.user.Where(d => d.user_id == data.user_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as user).user_id}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new RadioPlayer())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(user.user_id))
                });
                

                foreach (var data in db.user)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    foreach(var val0 in data.contacts){
val0.contact_type.ToString();
val0.supplier.ToString();
val0.user.ToString();
foreach(var val1 in val0.contact_email){
}
foreach(var val1 in val0.contact_history){
}
foreach(var val1 in val0.contact_telephone){
}
foreach(var val1 in val0.group_member){
}
}
}

                }
            }
        }
    }
}
