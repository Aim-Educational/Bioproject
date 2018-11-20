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
    /// Interaction logic for Editordevice_history_action.xaml
    /// </summary>
    public partial class Editordevice_history_action : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editordevice_history_action(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.device.provider = new SearchProviderdevice_history_action();
this.device.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.device_history_action1.Text = "(CREATE NEW)";
            this.device_history_action1.Text = "";
this.description.Text = "";
this.is_active.IsChecked = false;
this.version.Text = "";
this.comment.Text = "";
this.device.item = null;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as device_history_action;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a device_history_action.", "selectedItem");

            this.device_history_action1.Text = $"{data.device_history_action1}";
this.description.Text = $"{data.description}";
this.is_active.IsChecked = data.is_active;
this.version.Text = $"{data.version}";
this.comment.Text = $"{data.comment}";
this.device.item = data.device;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                device_history_action data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.device_history_action1.Text);
                    data = db.device_history_action.Where(d => d.device_history_action1 == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.device_history_action1.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new device_history_action();

                data.device_history_action1 = Convert.ToInt32(this.device_history_action1.Text);
data.description = (this.description.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.version = Convert.ToInt32(this.version.Text);
data.comment = (this.comment.Text);
data.device = this.device.item as device;


                if (this._isCreateMode)
                    db.device_history_action.Add(data);
                db.SaveChanges();
            }
        }
    }
}
