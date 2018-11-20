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
    /// Interaction logic for Editorcontact_history.xaml
    /// </summary>
    public partial class Editorcontact_history : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editorcontact_history(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.contact.provider = new SearchProvidercontact_history();
this.contact.mainInterface = mi;
this.history_event.provider = new SearchProvidercontact_history();
this.history_event.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.contact_history_id.Text = "(CREATE NEW)";
            this.contact_history_id.Text = "";
this.date_and_time.SelectedDate = null;
this.is_active.IsChecked = false;
this.version.Text = "";
this.comment.Text = "";
this.contact.item = null;
this.history_event.item = null;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as contact_history;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a contact_history.", "selectedItem");

            this.contact_history_id.Text = $"{data.contact_history_id}";
this.date_and_time.SelectedDate = data.date_and_time;
this.is_active.IsChecked = data.is_active;
this.version.Text = $"{data.version}";
this.comment.Text = $"{data.comment}";
this.contact.item = data.contact;
this.history_event.item = data.history_event;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                contact_history data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.contact_history_id.Text);
                    data = db.contact_history.Where(d => d.contact_history_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.contact_history_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new contact_history();

                data.contact_history_id = Convert.ToInt32(this.contact_history_id.Text);
data.date_and_time = (DateTime)this.date_and_time.SelectedDate;
data.is_active = (bool)this.is_active.IsChecked;
data.version = Convert.ToInt32(this.version.Text);
data.comment = (this.comment.Text);
data.contact = this.contact.item as contact;
data.history_event = this.history_event.item as history_event;


                if (this._isCreateMode)
                    db.contact_history.Add(data);
                db.SaveChanges();
            }
        }
    }
}
