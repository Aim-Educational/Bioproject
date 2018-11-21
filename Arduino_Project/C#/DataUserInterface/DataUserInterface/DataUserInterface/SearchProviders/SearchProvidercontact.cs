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
                    if(data.contact_type != null) data.contact_type.ToString();
if(data.supplier != null) data.supplier.ToString();
if(data.user != null) data.user.ToString();
foreach(var val0 in data.contact_email){
if(val0.contact != null) val0.contact.ToString();
}
foreach(var val0 in data.contact_history){
if(val0.contact != null) val0.contact.ToString();
if(val0.history_event != null) val0.history_event.ToString();
}
foreach(var val0 in data.contact_telephone){
if(val0.contact != null) val0.contact.ToString();
}
foreach(var val0 in data.group_member){
if(val0.contact != null) val0.contact.ToString();
if(val0.group_type != null) val0.group_type.ToString();
}

                }
            }
        }
    }
}
