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
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
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
                        this._mainInterface.statusText = $"ERROR: ID '{this.id.Text}' does not exist.";
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
