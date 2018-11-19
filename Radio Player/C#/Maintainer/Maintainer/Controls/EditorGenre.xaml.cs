using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Aim.DatabaseInterface.Interfaces;
using Aim.DatabaseInterface.Windows;
using EFLayer.Model;

namespace Maintainer.Controls
{
    /// <summary>
    /// Interaction logic for EditorGenre.xaml
    /// </summary>
    public partial class EditorGenre : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool _isCreateMode;

        public bool isDataDirty
        {
            get => true;
        }

        public EditorGenre(MainInterface mainInterface)
        {
            InitializeComponent();
            this._mainInterface = mainInterface;
        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if(saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var genre = selectedItem as tbl_genre;
            if(genre == null)
                throw new ArgumentException("Invalid type. Expected a tbl_genre.", "selectedItem");

            this.id.Text          = $"{genre.genre_id}";
            this.description.Text = genre.description;

            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new RadioPlayer())
            {
                tbl_genre genre = null;

                if(!this._isCreateMode)
                {
                    var id    = Convert.ToInt32(this.id.Text);
                        genre = db.tbl_genre.Where(g => g.genre_id == id).FirstOrDefault();
                    if(genre == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    genre = new tbl_genre();

                genre.description = this.description.Text;

                if(this._isCreateMode)
                    db.tbl_genre.Add(genre);
                db.SaveChanges();
            }
        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.id.Text = "(CREATE NEW)";
            this.description.Text = "";
        }
    }
}
