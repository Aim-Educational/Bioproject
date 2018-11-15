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
    /// Interaction logic for EditorMood.xaml
    /// </summary>
    public partial class EditorFormat : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool _isCreateMode;

        public bool isDataDirty
        {
            get => true;
        }

        public EditorFormat(MainInterface mainInterface)
        {
            InitializeComponent();
            this._mainInterface = mainInterface;
        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = this.askToSave();
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var format = selectedItem as tbl_format;
            if (format == null)
                throw new ArgumentException("Invalid type. Expected a tbl_mood.", "selectedItem");

            this.id.Text = $"{format.format_id}";
            this.description.Text = format.description;

            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new RadioPlayer())
            {
                tbl_format format = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.id.Text);
                    format = db.tbl_format.Where(m => m.format_id == id).FirstOrDefault();
                    if (format == null)
                    {
                        this._mainInterface.labelStatus.Content = $"ERROR: ID '{this.id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    format = new tbl_format();

                format.description = this.description.Text;

                if (this._isCreateMode)
                    db.tbl_format.Add(format);
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
