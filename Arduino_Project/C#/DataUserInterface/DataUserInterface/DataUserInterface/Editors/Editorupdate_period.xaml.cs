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
    /// Interaction logic for Editorupdate_period.xaml
    /// </summary>
    public partial class Editorupdate_period : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editorupdate_period(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            
        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.update_period_id.Text = "(CREATE NEW)";
            this.update_period_id.Text = "";
this.description.Text = "";
this.version.Text = "";
this.is_active.IsChecked = false;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as update_period;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a update_period.", "selectedItem");

            this.update_period_id.Text = $"{data.update_period_id}";
this.description.Text = $"{data.description}";
this.version.Text = $"{data.version}";
this.is_active.IsChecked = data.is_active;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                update_period data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.update_period_id.Text);
                    data = db.update_period.Where(d => d.update_period_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.update_period_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new update_period();

                data.update_period_id = Convert.ToInt32(this.update_period_id.Text);
data.description = (this.description.Text);
data.version = Convert.ToInt32(this.version.Text);
data.is_active = (bool)this.is_active.IsChecked;


                if (this._isCreateMode)
                    db.update_period.Add(data);
                db.SaveChanges();
            }
        }
    }
}
