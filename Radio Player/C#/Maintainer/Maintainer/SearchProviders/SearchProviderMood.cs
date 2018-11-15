using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Maintainer.Interfaces;
using EFLayer.Model;
using System.Windows.Data;
using System.Windows;

namespace Maintainer.SearchProviders
{
    public class SearchProviderMood : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as tbl_mood).mood_id == (item2 as tbl_mood).mood_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            tbl_mood mood = item as tbl_mood;
            if(mood == null)
                return;

            using (var db = new RadioPlayer())
            {
                db.tbl_mood.Remove(db.tbl_mood.Where(m => m.mood_id == mood.mood_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return (item as tbl_mood).description;
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new RadioPlayer())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(tbl_mood.mood_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Description",
                    Binding = new Binding(nameof(tbl_mood.description))
                });

                foreach(var mood in db.tbl_mood)
                    grid.Items.Add(mood);
            }
        }
    }
}
