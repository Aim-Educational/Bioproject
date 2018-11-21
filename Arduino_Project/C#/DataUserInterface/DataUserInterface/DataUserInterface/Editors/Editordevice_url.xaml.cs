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
    /// Interaction logic for Editordevice_url.xaml
    /// </summary>
    public partial class Editordevice_url : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editordevice_url(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.device.provider = new SearchProviderdevice();
this.device.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.device_url_id.Text = "(CREATE NEW)";
            this.device_url_id.Text = "";
this.url.Text = "";
this.is_active.IsChecked = false;
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
            var data = selectedItem as device_url;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a device_url.", "selectedItem");

            this.device_url_id.Text = $"{data.device_url_id}";
this.url.Text = $"{data.url}";
this.is_active.IsChecked = data.is_active;
this.comment.Text = $"{data.comment}";
this.device.item = data.device;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                device_url data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.device_url_id.Text);
                    data = db.device_url.Where(d => d.device_url_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.device_url_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new device_url();

                data.device_url_id = Convert.ToInt32(this.device_url_id.Text);
data.url = (this.url.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.comment = (this.comment.Text);
data.device = new Func<device>(() => { foreach(var v in db.devices){ if(v.device_id == (this.device.item as device).device_id) return v; } return null; })();


                if (this._isCreateMode)
                    db.device_url.Add(data);
                db.SaveChanges();
            }
        }
    }
}
