using EFLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Maintainer.Forms
{
    public enum SearchType
    {
        Collection,
        Format,
        Genre,
        Mood,
        PlayHistory,
        PlaylistHeader,
        PlaylistLines,
        Track
    }

    /// <summary>
    /// Interaction logic for SearchForm.xaml
    /// </summary>
    public partial class SearchForm : Window
    {
        public class SearchInfo
        {
            public int objectID { get; set; }
            public string objectDescription { get; set; }

            public SearchInfo(int objectID, string objectDescription)
            {
                this.objectID = objectID;
                this.objectDescription = objectDescription;
            }
        }

        public SearchForm()
        {
            InitializeComponent();

            // Testing
            using (var db = new RadioPlayer())
            {
                foreach (var info in this.getInfoFor(db, SearchType.Mood))
                    this.list.Items.Add(info);
            }
        }

        IEnumerable<SearchInfo> getInfoFor(RadioPlayer db, SearchType type)
        {
            switch (type)
            {
                case SearchType.Collection:
                    return (from m in db.tbl_collection select new SearchInfo(m.collection_id, m.title));

                case SearchType.Format:
                    return (from m in db.tbl_format select new SearchInfo(m.format_id, m.description));

                case SearchType.Genre:
                    return (from m in db.tbl_genre select new SearchInfo(m.genre_id, m.description));

                case SearchType.Mood:
                    return db.tbl_mood.ToList().Select(m => new SearchInfo(m.mood_id, m.description));

                case SearchType.PlayHistory:
                    return (from m in db.tbl_play_history select new SearchInfo(m.play_history_id, m.tbl_track.title));

                case SearchType.PlaylistHeader:
                    return (from m in db.tbl_playlist_header select new SearchInfo(m.playlist_header_id, m.description));

                case SearchType.PlaylistLines:
                    return (from m in db.tbl_playlist_lines select new SearchInfo(m.playlist_lines_id, m.tbl_track.title));

                case SearchType.Track:
                    return (from m in db.tbl_track select new SearchInfo(m.track_id, m.title));

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
