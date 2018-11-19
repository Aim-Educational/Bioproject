using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProvidercontact_telephone : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as contact_telephone).contact_telephone_id == (item2 as contact_telephone).contact_telephone_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            contact_telephone data = item as contact_telephone;
            if(data == null)
                return;

            using (var db = new RadioPlayer())
            {
                db.contact_telephone.Remove(db.contact_telephone.Where(d => d.contact_telephone_id == data.contact_telephone_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as contact_telephone).contact_telephone_id}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new RadioPlayer())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(contact_telephone.contact_telephone_id))
                });
                

                foreach (var data in db.contact_telephone)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    data.contact.ToString();

                }
            }
        }
    }
}
