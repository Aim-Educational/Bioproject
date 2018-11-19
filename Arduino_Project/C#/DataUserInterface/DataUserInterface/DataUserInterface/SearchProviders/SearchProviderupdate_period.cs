using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderupdate_period : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as update_period).update_period_id == (item2 as update_period).update_period_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            update_period data = item as update_period;
            if(data == null)
                return;

            using (var db = new RadioPlayer())
            {
                db.update_period.Remove(db.update_period.Where(d => d.update_period_id == data.update_period_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as update_period).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new RadioPlayer())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(update_period.update_period_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "description",
                    Binding = new Binding(nameof(update_period.description))
                });
                

                foreach (var data in db.update_period)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    foreach(var val0 in data.rss_configuration){
val0.device.ToString();
val0.update_period.ToString();
}

                }
            }
        }
    }
}
