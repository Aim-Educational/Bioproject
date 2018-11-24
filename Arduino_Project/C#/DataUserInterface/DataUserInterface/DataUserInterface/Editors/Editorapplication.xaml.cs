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
    /// Interaction logic for Editorapplication.xaml
    /// </summary>
    public partial class Editorapplication : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editorapplication(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            
        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.application_id.Text = "(CREATE NEW)";
            this.application_id.Text = "";
this.name.Text = "";
this.description.Text = "";
this.application_version.Text = "";
this.is_active.IsChecked = false;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as application;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a application.", "selectedItem");

            this.application_id.Text = $"{data.application_id}";
this.name.Text = $"{data.name}";
this.description.Text = $"{data.description}";
this.application_version.Text = $"{data.application_version}";
this.is_active.IsChecked = data.is_active;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                application data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.application_id.Text);
                    data = db.applications.Where(d => d.application_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.application_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new application();

                data.application_id = Convert.ToInt32(this.application_id.Text);
data.name = /**/(this.name.Text);
data.description = /**/(this.description.Text);
data.application_version = Convert.ToInt32(this.application_version.Text);
data.is_active = (bool)this.is_active.IsChecked;


                if (this._isCreateMode)
                    db.applications.Add(data);
                db.SaveChanges();
            }
        }
    }
}
