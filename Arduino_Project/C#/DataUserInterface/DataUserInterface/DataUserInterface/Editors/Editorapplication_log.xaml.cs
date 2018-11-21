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
    /// Interaction logic for Editorapplication_log.xaml
    /// </summary>
    public partial class Editorapplication_log : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editorapplication_log(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.application.provider = new SearchProviderapplication();
this.application.mainInterface = mi;
this.message_type.provider = new SearchProvidermessage_type();
this.message_type.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.application_log_id.Text = "(CREATE NEW)";
            this.application_log_id.Text = "";
this.message.Text = "";
this.datetime.SelectedDate = null;
this.is_active.IsChecked = false;
this.application.item = null;
this.message_type.item = null;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as application_log;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a application_log.", "selectedItem");

            this.application_log_id.Text = $"{data.application_log_id}";
this.message.Text = $"{data.message}";
this.datetime.SelectedDate = data.datetime;
this.is_active.IsChecked = data.is_active;
this.application.item = data.application;
this.message_type.item = data.message_type;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                application_log data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.application_log_id.Text);
                    data = db.application_log.Where(d => d.application_log_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.application_log_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new application_log();

                data.application_log_id = Convert.ToInt32(this.application_log_id.Text);
data.message = (this.message.Text);
data.datetime = (DateTime)this.datetime.SelectedDate;
data.is_active = (bool)this.is_active.IsChecked;
data.application = new Func<application>(() => { foreach(var v in db.applications){ if(v.application_id == (this.application.item as application).application_id) return v; } return null; })();
data.message_type = new Func<message_type>(() => { foreach(var v in db.message_type){ if(v.message_type_id == (this.message_type.item as message_type).message_type_id) return v; } return null; })();


                if (this._isCreateMode)
                    db.application_log.Add(data);
                db.SaveChanges();
            }
        }
    }
}
