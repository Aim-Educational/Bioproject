using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using DataManager.Model;

namespace DataUserInterface.SearchProviders
{
    public class SearchProvidermenu : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as menu).menu_id == (item2 as menu).menu_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            menu data = item as menu;
            if(data == null)
                return;

            using (var db = new PlanningContext())
            {
                db.menus.Remove(db.menus.Where(d => d.menu_id == data.menu_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as menu).description}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new PlanningContext())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(menu.menu_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "description",
                    Binding = new Binding(nameof(menu.description))
                });
                

                foreach (var data in db.menus)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    if(data.menu2 != null) data.menu2.ToString();
foreach(var val0 in data.menu1){
if(val0.menu2 != null) val0.menu2.ToString();
foreach(var val1 in val0.menu1){
}
}

                }
            }
        }
    }
}
