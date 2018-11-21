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
    /// Interaction logic for Editordevice_address.xaml
    /// </summary>
    public partial class Editordevice_address : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editordevice_address(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.device.provider = new SearchProviderdevice();
this.device.mainInterface = mi;
this.device_address_type.provider = new SearchProviderdevice_address_type();
this.device_address_type.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.device_address_id.Text = "(CREATE NEW)";
            this.device_address_id.Text = "";
this.ip_address.Text = "";
this.comment.Text = "";
this.is_active.IsChecked = false;
this.device.item = null;
this.device_address_type.item = null;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as device_address;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a device_address.", "selectedItem");

            this.device_address_id.Text = $"{data.device_address_id}";
this.ip_address.Text = $"{data.ip_address}";
this.comment.Text = $"{data.comment}";
this.is_active.IsChecked = data.is_active;
this.device.item = data.device;
this.device_address_type.item = data.device_address_type;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                device_address data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.device_address_id.Text);
                    data = db.device_address.Where(d => d.device_address_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.device_address_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new device_address();

                data.device_address_id = Convert.ToInt32(this.device_address_id.Text);
data.ip_address = (this.ip_address.Text);
data.comment = (this.comment.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.device = new Func<device>(() => { foreach(var v in db.devices){ if(v.device_id == (this.device.item as device).device_id) return v; } return null; })();
data.device_address_type = new Func<device_address_type>(() => { foreach(var v in db.device_address_type){ if(v.device_address_type_id == (this.device_address_type.item as device_address_type).device_address_type_id) return v; } return null; })();


                if (this._isCreateMode)
                    db.device_address.Add(data);
                db.SaveChanges();
            }
        }
    }
}
