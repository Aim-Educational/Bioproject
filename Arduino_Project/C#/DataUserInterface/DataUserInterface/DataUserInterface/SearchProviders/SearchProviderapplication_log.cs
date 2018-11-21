using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderapplication_log : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as application_log).application_log_id == (item2 as application_log).application_log_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            application_log data = item as application_log;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.application_log.Remove(db.application_log.Where(d => d.application_log_id == data.application_log_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as application_log).application_log_id}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(application_log.application_log_id))
                });
                

                foreach (var data in db.application_log)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    if(data.application != null) data.application.ToString();
if(data.message_type != null) data.message_type.ToString();

                }
            }
        }
    }
}
