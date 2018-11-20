using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProvidercontact : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as contact).contact_id == (item2 as contact).contact_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            contact data = item as contact;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.contacts.Remove(db.contacts.Where(d => d.contact_id == data.contact_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as contact).contact_id}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(contact.contact_id))
                });
                

                foreach (var data in db.contacts)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    data.contact_type.ToString();
data.supplier.ToString();
data.user.ToString();
foreach(var val0 in data.contact_email){
val0.contact.ToString();
}
foreach(var val0 in data.contact_history){
val0.contact.ToString();
val0.history_event.ToString();
}
foreach(var val0 in data.contact_telephone){
val0.contact.ToString();
}
foreach(var val0 in data.group_member){
val0.contact.ToString();
val0.group_type.ToString();
}

                }
            }
        }
    }
}
