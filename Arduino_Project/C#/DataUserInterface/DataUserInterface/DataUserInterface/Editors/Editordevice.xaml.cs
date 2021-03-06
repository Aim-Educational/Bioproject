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
    /// Interaction logic for Editordevice.xaml
    /// </summary>
    public partial class Editordevice : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editordevice(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.device2.provider = new SearchProviderdevice();
this.device2.mainInterface = mi;
this.device_type.provider = new SearchProviderdevice_type();
this.device_type.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.device_id.Text = "(CREATE NEW)";
            this.device_id.Text = "";
this.name.Text = "";
this.description.Text = "";
this.location.Text = "";
this.min_value.Text = "";
this.max_value.Text = "";
this.accuracy.Text = "";
this.serial_number.Text = "";
this.cost.Text = "";
this.reliability.Text = "";
this.strikes.Text = "";
this.is_active.IsChecked = false;
this.comment.Text = "";
this.is_allowed_for_use.IsChecked = false;
this.device2.item = null;
this.device_type.item = null;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as device;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a device.", "selectedItem");

            this.device_id.Text = $"{data.device_id}";
this.name.Text = $"{data.name}";
this.description.Text = $"{data.description}";
this.location.Text = $"{data.location}";
this.min_value.Text = $"{data.min_value}";
this.max_value.Text = $"{data.max_value}";
this.accuracy.Text = $"{data.accuracy}";
this.serial_number.Text = $"{data.serial_number}";
this.cost.Text = $"{data.cost}";
this.reliability.Text = $"{data.reliability}";
this.strikes.Text = $"{data.strikes}";
this.is_active.IsChecked = data.is_active;
this.comment.Text = $"{data.comment}";
this.is_allowed_for_use.IsChecked = data.is_allowed_for_use;
this.device2.item = data.device2;
this.device_type.item = data.device_type;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                device data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.device_id.Text);
                    data = db.devices.Where(d => d.device_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.device_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new device();

                data.device_id = Convert.ToInt32(this.device_id.Text);
data.name = /**/(this.name.Text);
data.description = /**/(this.description.Text);
data.location = /**/(this.location.Text);
data.min_value = Convert.ToDouble(this.min_value.Text);
data.max_value = Convert.ToDouble(this.max_value.Text);
data.accuracy = Convert.ToDouble(this.accuracy.Text);
data.serial_number = /**/(this.serial_number.Text);
data.cost = Convert.ToDecimal(this.cost.Text);
data.reliability = Convert.ToInt32(this.reliability.Text);
data.strikes = Convert.ToInt32(this.strikes.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.comment = /**/(this.comment.Text);
data.is_allowed_for_use = (bool)this.is_allowed_for_use.IsChecked;
data.device2 = new Func<device>(() => { foreach(var v in db.devices){ if(v.device_id == (this.device2.item as device).device_id) return v; } return null; })();
data.device_type = new Func<device_type>(() => { foreach(var v in db.device_type){ if(v.device_type_id == (this.device_type.item as device_type).device_type_id) return v; } return null; })();


                if (this._isCreateMode)
                    db.devices.Add(data);
                db.SaveChanges();
            }
        }
    }
}
