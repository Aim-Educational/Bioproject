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
    /// Interaction logic for Editorrss_configuration.xaml
    /// </summary>
    public partial class Editorrss_configuration : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editorrss_configuration(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.device.provider = new SearchProviderdevice();
this.device.mainInterface = mi;
this.update_period.provider = new SearchProviderupdate_period();
this.update_period.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.rss_configuration_id.Text = "(CREATE NEW)";
            this.rss_configuration_id.Text = "";
this.description.Text = "";
this.is_active.IsChecked = false;
this.last_update.SelectedDate = null;
this.update_frequency.Text = "";
this.rss_url.Text = "";
this.device.item = null;
this.update_period.item = null;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as rss_configuration;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a rss_configuration.", "selectedItem");

            this.rss_configuration_id.Text = $"{data.rss_configuration_id}";
this.description.Text = $"{data.description}";
this.is_active.IsChecked = data.is_active;
this.last_update.SelectedDate = data.last_update;
this.update_frequency.Text = $"{data.update_frequency}";
this.rss_url.Text = $"{data.rss_url}";
this.device.item = data.device;
this.update_period.item = data.update_period;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                rss_configuration data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.rss_configuration_id.Text);
                    data = db.rss_configuration.Where(d => d.rss_configuration_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.rss_configuration_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new rss_configuration();

                data.rss_configuration_id = Convert.ToInt32(this.rss_configuration_id.Text);
data.description = (this.description.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.last_update = (DateTime)this.last_update.SelectedDate;
data.update_frequency = Convert.ToDouble(this.update_frequency.Text);
data.rss_url = (this.rss_url.Text);
data.device = new Func<device>(() => { foreach(var v in db.devices){ if(v.device_id == (this.device.item as device).device_id) return v; } return null; })();
data.update_period = new Func<update_period>(() => { foreach(var v in db.update_period){ if(v.update_period_id == (this.update_period.item as update_period).update_period_id) return v; } return null; })();


                if (this._isCreateMode)
                    db.rss_configuration.Add(data);
                db.SaveChanges();
            }
        }
    }
}
