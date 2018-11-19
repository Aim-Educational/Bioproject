using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderdatabase_config : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as database_config).database_config_id == (item2 as database_config).database_config_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            database_config data = item as database_config;
            if(data == null)
                return;

            using (var db = new RadioPlayer())
            {
                db.database_config.Remove(db.database_config.Where(d => d.database_config_id == data.database_config_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as database_config).database_config_id}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new RadioPlayer())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(database_config.database_config_id))
                });
                

                foreach (var data in db.database_config)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    
                }
            }
        }
    }
}
