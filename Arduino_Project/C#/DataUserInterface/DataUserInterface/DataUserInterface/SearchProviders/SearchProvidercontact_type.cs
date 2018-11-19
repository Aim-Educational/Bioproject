using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProvidercontact_type : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as contact_type).contact_type_id == (item2 as contact_type).contact_type_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            contact_type data = item as contact_type;
            if(data == null)
                return;

            using (var db = new RadioPlayer())
            {
                db.contact_type.Remove(db.contact_type.Where(d => d.contact_type_id == data.contact_type_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as contact_type).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new RadioPlayer())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(contact_type.contact_type_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "description",
                    Binding = new Binding(nameof(contact_type.description))
                });
                

                foreach (var data in db.contact_type)
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
