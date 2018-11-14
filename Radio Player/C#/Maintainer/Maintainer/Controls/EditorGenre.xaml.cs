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
using System.Windows.Navigation;
using System.Windows.Shapes;
using EFLayer.Model;
using Maintainer.Forms;
using Maintainer.Interfaces;

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

        public void onSearchAction(object selectedItem)
        {
            this._isCreateMode = false;
            var genre = selectedItem as tbl_genre;
            if(genre == null)
                throw new ArgumentException("Invalid type. Expected a tbl_genre.", "selectedItem");

            this.id.Text          = $"{genre.genre_id}";
            this.description.Text = genre.description;
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
                        this._mainInterface.labelStatus.Content = $"ERROR: ID '{this.id.Text}' does not exist.";
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
