using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Aim.DatabaseInterface.Interfaces;
using Aim.DatabaseInterface.Windows;
using EFLayer.Model;
using Microsoft.Win32;
using NAudio.Wave;
using TagLib;
using System.IO;
using Maintainer.Util;

namespace Maintainer.Controls
{
    /// <summary>
    /// Interaction logic for EditorGenre.xaml
    /// </summary>
    public partial class EditorTrackPicker : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private Tag           _tag;
        private string        _extension;
        private WaveFormat    _format;
        private string        _fileName;
        private int           _totalSeconds;

        public bool isDataDirty
        {
            get => true;
        }

        public EditorTrackPicker(MainInterface mainInterface)
        {
            InitializeComponent();
            this._mainInterface = mainInterface;
        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            return RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new RadioPlayer())
            {
                var track               = new tbl_track();
                track.title             = _tag.Title;
                track.subtitle          = "N/A";
                track.artists           = String.Join(",", _tag.AlbumArtists);
                track.composers         = String.Join(",", _tag.Composers);
                track.bitrate           = Convert.ToString(_format.AverageBytesPerSecond * 8);
                track.publisher         = "N/A";
                track.parental_advisory = false;
                track.folder_path       = Path.GetDirectoryName(_fileName).Replace(Config.PathToRoot, "").Replace(Config.ServerName, "").TrimStart('\\', '/');
                track.file_name         = Path.GetFileName(_fileName);
                track.duration          = _totalSeconds;
                track.tbl_format        = db.tbl_format.First(f => f.description == _extension);
                track.date_recorded     = null;
                track.date_released     = null;
                track.likes             = 0;
                track.dislikes          = 0;
                track.keywords          = "N/A";

                var lengthStream = System.IO.File.Open(_fileName, FileMode.Open);
                track.filesize   = lengthStream.Length;
                lengthStream.Dispose();

                if (db.tbl_track.Any(t => t.title == track.title))
                {
                    var result = MessageBox.Show(
                        $"A track called '{track.title}' already exists, are you sure you want to" +
                        $" continue with adding this track?",
                        "Confirmation",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                    );

                    if (result == MessageBoxResult.No)
                        return;
                }

                db.tbl_track.Add(track);

                // Auto-add it to a collection.
                var collection = db.tbl_collection.FirstOrDefault(c => c.title.ToLower() == _tag.Album.ToLower());
                if(collection == null && !String.IsNullOrWhiteSpace(_tag.Album))
                {
                    var result = MessageBox.Show(
                        $"There is no collection called '{_tag.Album}', would you " +
                        $"like to create one for this track?",
                        "Confirmation",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                    );

                    if(result == MessageBoxResult.Yes)
                    {
                        collection       = new tbl_collection();
                        collection.title = _tag.Album;
                        db.tbl_collection.Add(collection);
                    }
                }

                if (collection != null)
                {
                    var map = new tbl_collectionmap();
                    map.tbl_collection = collection;
                    map.tbl_track = track;
                    map.sequence_index = 999;
                    db.tbl_collectionmap.Add(map);
                }
                db.SaveChanges();
            }
        }

        public void flagAsCreateMode()
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var picker = new OpenFileDialog();
            picker.Multiselect = false;

            var success = picker.ShowDialog();
            if(!success ?? false)
                return;
            
            this._tag          = TagLib.File.Create(picker.FileName).Tag; // TODO: Check for corruption
            var reader         = new MediaFoundationReader(picker.FileName);
            this._format       = reader.WaveFormat;
            this._extension    = Path.GetExtension(picker.FileName).ToUpper().Substring(1);
            this._totalSeconds = (int)reader.TotalTime.TotalSeconds;
            this._fileName     = picker.FileName;
            this.path.Text     = picker.FileName;
            reader.Dispose();
        }
    }
}
