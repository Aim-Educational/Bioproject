using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderapplication : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as application).application_id == (item2 as application).application_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            application data = item as application;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.application.Remove(db.application.Where(d => d.application_id == data.application_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as application).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(application.application_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "name",
                    Binding = new Binding(nameof(application.name))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "description",
                    Binding = new Binding(nameof(application.description))
                });
                

                foreach (var data in db.application)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    foreach(var val0 in data.application_log){
val0.application.ToString();
val0.message_type.ToString();
}

                }
            }
        }
    }
}
