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
    /// Interaction logic for Editordevice_value.xaml
    /// </summary>
    public partial class Editordevice_value : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editordevice_value(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.device.provider = new SearchProviderdevice();
this.device.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.device_value_id.Text = "(CREATE NEW)";
            this.device_value_id.Text = "";
this.value.Text = "";
this.datetime.SelectedDate = null;
this.response_recieved.IsChecked = false;
this.is_active.IsChecked = false;
this.comment.Text = "";
this.extra_data.Text = "";
this.device.item = null;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as device_value;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a device_value.", "selectedItem");

            this.device_value_id.Text = $"{data.device_value_id}";
this.value.Text = $"{data.value}";
this.datetime.SelectedDate = data.datetime;
this.response_recieved.IsChecked = data.response_recieved;
this.is_active.IsChecked = data.is_active;
this.comment.Text = $"{data.comment}";
this.extra_data.Text = $"{data.extra_data}";
this.device.item = data.device;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                device_value data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.device_value_id.Text);
                    data = db.device_value.Where(d => d.device_value_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.device_value_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new device_value();

                data.device_value_id = Convert.ToInt32(this.device_value_id.Text);
data.value = Convert.ToDouble(this.value.Text);
data.datetime = (DateTime)this.datetime.SelectedDate;
data.response_recieved = (bool)this.response_recieved.IsChecked;
data.is_active = (bool)this.is_active.IsChecked;
data.comment = (this.comment.Text);
data.extra_data = (this.extra_data.Text);
data.device = this.device.item as device;


                if (this._isCreateMode)
                    db.device_value.Add(data);
                db.SaveChanges();
            }
        }
    }
}
