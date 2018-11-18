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
    public class SearchProviderPlaylist : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as tbl_playlist_header).playlist_header_id == (item2 as tbl_playlist_header).playlist_header_id;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            tbl_playlist_header playlist = item as tbl_playlist_header;
            if(playlist == null)
                return;

            using (var db = new RadioPlayer())
            {
                db.tbl_playlist_header.Remove(db.tbl_playlist_header.Where(p => p.playlist_header_id == playlist.playlist_header_id).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return (item as tbl_playlist_header).description;
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new RadioPlayer())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof(tbl_playlist_header.playlist_header_id))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Description",
                    Binding = new Binding(nameof(tbl_playlist_header.description))
                });
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Creator",
                    Binding = new Binding(nameof(tbl_playlist_header.creator))
                });

                foreach (var playlist in db.tbl_playlist_header)
                {
                    grid.Items.Add(playlist);

                    // Cache data
                    foreach(var map in playlist.tbl_playlist_lines)
                    {
                        map.ToString();
                        map.tbl_playlist_header.ToString();
                        map.tbl_track.ToString();
                    }
                }
            }
        }
    }
}
