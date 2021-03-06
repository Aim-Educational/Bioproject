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
    /// Interaction logic for Editoralarm.xaml
    /// </summary>
    public partial class Editoralarm : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editoralarm(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.alarm_type.provider = new SearchProvideralarm_type();
this.alarm_type.mainInterface = mi;
this.device.provider = new SearchProviderdevice();
this.device.mainInterface = mi;
this.group_type.provider = new SearchProvidergroup_type();
this.group_type.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.alarm_id.Text = "(CREATE NEW)";
            this.alarm_id.Text = "";
this.value.Text = "";
this.comment.Text = "";
this.is_active.IsChecked = false;
this.alarm_type.item = null;
this.device.item = null;
this.group_type.item = null;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as alarm;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a alarm.", "selectedItem");

            this.alarm_id.Text = $"{data.alarm_id}";
this.value.Text = $"{data.value}";
this.comment.Text = $"{data.comment}";
this.is_active.IsChecked = data.is_active;
this.alarm_type.item = data.alarm_type;
this.device.item = data.device;
this.group_type.item = data.group_type;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                alarm data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.alarm_id.Text);
                    data = db.alarms.Where(d => d.alarm_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.alarm_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new alarm();

                data.alarm_id = Convert.ToInt32(this.alarm_id.Text);
data.value = Convert.ToDouble(this.value.Text);
data.comment = /**/(this.comment.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.alarm_type = new Func<alarm_type>(() => { foreach(var v in db.alarm_type){ if(v.alarm_type_id == (this.alarm_type.item as alarm_type).alarm_type_id) return v; } return null; })();
data.device = new Func<device>(() => { foreach(var v in db.devices){ if(v.device_id == (this.device.item as device).device_id) return v; } return null; })();
data.group_type = new Func<group_type>(() => { foreach(var v in db.group_type){ if(v.group_type_id == (this.group_type.item as group_type).group_type_id) return v; } return null; })();


                if (this._isCreateMode)
                    db.alarms.Add(data);
                db.SaveChanges();
            }
        }
    }
}
