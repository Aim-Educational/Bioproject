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
