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
    public class SearchProviderGenre : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as tbl_genre).genre_id == (item2 as tbl_genre).genre_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            tbl_genre genre = item as tbl_genre;
            if(genre == null)
                return;

            using (var db = new RadioPlayer())
            {
                db.tbl_genre.Remove(db.tbl_genre.Where(g => g.genre_id == genre.genre_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return (item as tbl_genre).description;
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new RadioPlayer())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(tbl_genre.genre_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Description",
                    Binding = new Binding(nameof(tbl_genre.description))
                });

                foreach(var genre in db.tbl_genre)
                    grid.Items.Add(genre);
            }
        }
    }
}
