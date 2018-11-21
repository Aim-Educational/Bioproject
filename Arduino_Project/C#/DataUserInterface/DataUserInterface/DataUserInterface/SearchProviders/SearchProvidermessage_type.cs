using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProvidermessage_type : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as message_type).message_type_id == (item2 as message_type).message_type_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            message_type data = item as message_type;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.message_type.Remove(db.message_type.Where(d => d.message_type_id == data.message_type_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as message_type).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(message_type.message_type_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "description",
                    Binding = new Binding(nameof(message_type.description))
                });
                

                foreach (var data in db.message_type)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    foreach(var val0 in data.application_log){
if(val0.application != null) val0.application.ToString();
if(val0.message_type != null) val0.message_type.ToString();
}

                }
            }
        }
    }
}
