using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProvidercontact_history : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as contact_history).contact_history_id == (item2 as contact_history).contact_history_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            contact_history data = item as contact_history;
            if(data == null)
                return;

            using (var db = new RadioPlayer())
            {
                db.contact_history.Remove(db.contact_history.Where(d => d.contact_history_id == data.contact_history_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as contact_history).contact_history_id}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new RadioPlayer())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(contact_history.contact_history_id))
                });
                

                foreach (var data in db.contact_history)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    data.contact.ToString();
data.history_event.ToString();

                }
            }
        }
    }
}
