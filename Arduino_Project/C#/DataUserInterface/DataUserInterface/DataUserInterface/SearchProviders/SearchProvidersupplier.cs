using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProvidersupplier : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as supplier).supplier_id == (item2 as supplier).supplier_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            supplier data = item as supplier;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.supplier.Remove(db.supplier.Where(d => d.supplier_id == data.supplier_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as supplier).supplier_id}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(supplier.supplier_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "name",
                    Binding = new Binding(nameof(supplier.name))
                });
                

                foreach (var data in db.supplier)
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
foreach(var val0 in data.device_history){
val0.device.ToString();
val0.device_history_action.ToString();
val0.supplier.ToString();
}

                }
            }
        }
    }
}
