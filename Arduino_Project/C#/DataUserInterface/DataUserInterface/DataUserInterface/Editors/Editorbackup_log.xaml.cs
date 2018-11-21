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
    /// Interaction logic for Editorbackup_log.xaml
    /// </summary>
    public partial class Editorbackup_log : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editorbackup_log(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            
        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.backup_log_id.Text = "(CREATE NEW)";
            this.backup_log_id.Text = "";
this.filename.Text = "";
this.datetime.SelectedDate = null;
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
            var data = selectedItem as backup_log;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a backup_log.", "selectedItem");

            this.backup_log_id.Text = $"{data.backup_log_id}";
this.filename.Text = $"{data.filename}";
this.datetime.SelectedDate = data.datetime;
this.comment.Text = $"{data.comment}";
this.is_active.IsChecked = data.is_active;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                backup_log data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.backup_log_id.Text);
                    data = db.backup_log.Where(d => d.backup_log_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.backup_log_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new backup_log();

                data.backup_log_id = Convert.ToInt32(this.backup_log_id.Text);
data.filename = (this.filename.Text);
data.datetime = (DateTime)this.datetime.SelectedDate;
data.comment = (this.comment.Text);
data.is_active = (bool)this.is_active.IsChecked;


                if (this._isCreateMode)
                    db.backup_log.Add(data);
                db.SaveChanges();
            }
        }
    }
}
