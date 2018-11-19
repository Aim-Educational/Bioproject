using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProviderhistory_event : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as history_event).history_event_id == (item2 as history_event).history_event_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            history_event data = item as history_event;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.history_event.Remove(db.history_event.Where(d => d.history_event_id == data.history_event_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as history_event).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(history_event.history_event_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "description",
                    Binding = new Binding(nameof(history_event.description))
                });
                

                foreach (var data in db.history_event)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    foreach(var val0 in data.contact_history){
val0.contact.ToString();
val0.history_event.ToString();
}

                }
            }
        }
    }
}
