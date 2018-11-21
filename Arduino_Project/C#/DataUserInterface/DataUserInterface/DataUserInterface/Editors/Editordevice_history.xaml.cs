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
    /// Interaction logic for Editordevice_history.xaml
    /// </summary>
    public partial class Editordevice_history : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editordevice_history(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.device.provider = new SearchProviderdevice();
this.device.mainInterface = mi;
this.device_history_action.provider = new SearchProviderdevice_history_action();
this.device_history_action.mainInterface = mi;
this.supplier.provider = new SearchProvidersupplier();
this.supplier.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.device_history_id.Text = "(CREATE NEW)";
            this.device_history_id.Text = "";
this.datetime.SelectedDate = null;
this.is_active.IsChecked = false;
this.comment.Text = "";
this.device.item = null;
this.device_history_action.item = null;
this.supplier.item = null;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as device_history;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a device_history.", "selectedItem");

            this.device_history_id.Text = $"{data.device_history_id}";
this.datetime.SelectedDate = data.datetime;
this.is_active.IsChecked = data.is_active;
this.comment.Text = $"{data.comment}";
this.device.item = data.device;
this.device_history_action.item = data.device_history_action;
this.supplier.item = data.supplier;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                device_history data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.device_history_id.Text);
                    data = db.device_history.Where(d => d.device_history_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.device_history_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new device_history();

                data.device_history_id = Convert.ToInt32(this.device_history_id.Text);
data.datetime = (DateTime)this.datetime.SelectedDate;
data.is_active = (bool)this.is_active.IsChecked;
data.comment = (this.comment.Text);
data.device = new Func<device>(() => { foreach(var v in db.devices){ if(v.device_id == (this.device.item as device).device_id) return v; } return null; })();
data.device_history_action = new Func<device_history_action>(() => { foreach(var v in db.device_history_action){ if(v.device_history_action1 == (this.device_history_action.item as device_history_action).device_history_action1) return v; } return null; })();
data.supplier = new Func<supplier>(() => { foreach(var v in db.suppliers){ if(v.supplier_id == (this.supplier.item as supplier).supplier_id) return v; } return null; })();


                if (this._isCreateMode)
                    db.device_history.Add(data);
                db.SaveChanges();
            }
        }
    }
}
