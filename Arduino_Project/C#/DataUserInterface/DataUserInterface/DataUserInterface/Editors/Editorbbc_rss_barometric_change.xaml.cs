using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Aim.DatabaseInterface.Controls;
using Aim.DatabaseInterface.Interfaces;
using Aim.DatabaseInterface.Windows;
using DataManager.Model;
using DataUserInterface.SearchProviders;

namespace DataUserInterface.Editors
{
    /// <summary>
    /// Interaction logic for Editorbbc_rss_barometric_change.xaml
    /// </summary>
    public partial class Editorbbc_rss_barometric_change : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editorbbc_rss_barometric_change(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            
        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.bbc_rss_barometric_change_id.Text = "(CREATE NEW)";
            this.bbc_rss_barometric_change_id.Text = "";
this.description.Text = "";
this.comment.Text = "";
this.is_active.IsChecked = false;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as bbc_rss_barometric_change;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a bbc_rss_barometric_change.", "selectedItem");

            this.bbc_rss_barometric_change_id.Text = $"{data.bbc_rss_barometric_change_id}";
this.description.Text = $"{data.description}";
this.comment.Text = $"{data.comment}";
this.is_active.IsChecked = data.is_active;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                bbc_rss_barometric_change data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.bbc_rss_barometric_change_id.Text);
                    data = db.bbc_rss_barometric_change.Where(d => d.bbc_rss_barometric_change_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.bbc_rss_barometric_change_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new bbc_rss_barometric_change();

                data.bbc_rss_barometric_change_id = Convert.ToInt32(this.bbc_rss_barometric_change_id.Text);
data.description = (this.description.Text);
data.comment = (this.comment.Text);
data.is_active = (bool)this.is_active.IsChecked;


                if (this._isCreateMode)
                    db.bbc_rss_barometric_change.Add(data);
                db.SaveChanges();
            }
        }
    }
}
