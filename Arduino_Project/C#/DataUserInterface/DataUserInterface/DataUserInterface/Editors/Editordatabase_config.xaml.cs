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
    /// Interaction logic for Editordatabase_config.xaml
    /// </summary>
    public partial class Editordatabase_config : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editordatabase_config(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            
        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.database_config_id.Text = "(CREATE NEW)";
            this.database_config_id.Text = "";
this.database_backup_directory.Text = "";
this.is_active.IsChecked = false;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as database_config;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a database_config.", "selectedItem");

            this.database_config_id.Text = $"{data.database_config_id}";
this.database_backup_directory.Text = $"{data.database_backup_directory}";
this.is_active.IsChecked = data.is_active;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                database_config data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.database_config_id.Text);
                    data = db.database_config.Where(d => d.database_config_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.database_config_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new database_config();

                data.database_config_id = Convert.ToInt32(this.database_config_id.Text);
data.database_backup_directory = /**/(this.database_backup_directory.Text);
data.is_active = (bool)this.is_active.IsChecked;


                if (this._isCreateMode)
                    db.database_config.Add(data);
                db.SaveChanges();
            }
        }
    }
}
