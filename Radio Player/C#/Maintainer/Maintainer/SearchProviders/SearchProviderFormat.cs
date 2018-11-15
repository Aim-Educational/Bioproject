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
    public class SearchProviderFormat : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as tbl_format).format_id == (item2 as tbl_format).format_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            tbl_format format = item as tbl_format;
            if(format == null)
                return;

            using (var db = new RadioPlayer())
            {
                db.tbl_format.Remove(db.tbl_format.Where(f => f.format_id == format.format_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return (item as tbl_format).description;
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new RadioPlayer())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(tbl_format.format_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Description",
                    Binding = new Binding(nameof(tbl_format.description))
                });

                foreach(var format in db.tbl_format)
                    grid.Items.Add(format);
            }
        }
    }
}
