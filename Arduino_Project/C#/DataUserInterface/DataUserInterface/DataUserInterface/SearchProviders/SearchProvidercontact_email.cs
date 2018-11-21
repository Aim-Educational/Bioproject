using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProvidercontact_email : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as contact_email).contact_email_id == (item2 as contact_email).contact_email_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            contact_email data = item as contact_email;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.contact_email.Remove(db.contact_email.Where(d => d.contact_email_id == data.contact_email_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as contact_email).contact_email_id}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(contact_email.contact_email_id))
                });
                

                foreach (var data in db.contact_email)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    if(data.contact != null) data.contact.ToString();

                }
            }
        }
    }
}
