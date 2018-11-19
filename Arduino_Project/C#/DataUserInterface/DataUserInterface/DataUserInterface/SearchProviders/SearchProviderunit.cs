using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderunit : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as unit).unit_id == (item2 as unit).unit_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            unit data = item as unit;
            if(data == null)
                return;

            using (var db = new RadioPlayer())
            {
                db.unit.Remove(db.unit.Where(d => d.unit_id == data.unit_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as unit).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new RadioPlayer())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(unit.unit_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "description",
                    Binding = new Binding(nameof(unit.description))
                });
                

                foreach (var data in db.unit)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    foreach(var val0 in data.device_type){
val0.unit.ToString();
foreach(var val1 in val0.devices){
}
}
}

                }
            }
        }
    }
}
