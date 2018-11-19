using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderbackup_log : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as backup_log).backup_log_id == (item2 as backup_log).backup_log_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            backup_log data = item as backup_log;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.backup_log.Remove(db.backup_log.Where(d => d.backup_log_id == data.backup_log_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as backup_log).backup_log_id}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(backup_log.backup_log_id))
                });
                

                foreach (var data in db.backup_log)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    
                }
            }
        }
    }
}
