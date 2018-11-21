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
    /// Interaction logic for Editorcontact_email.xaml
    /// </summary>
    public partial class Editorcontact_email : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editorcontact_email(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.contact.provider = new SearchProvidercontact();
this.contact.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.contact_email_id.Text = "(CREATE NEW)";
            this.contact_email_id.Text = "";
this.email.Text = "";
this.is_active.IsChecked = false;
this.comment.Text = "";
this.contact.item = null;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as contact_email;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a contact_email.", "selectedItem");

            this.contact_email_id.Text = $"{data.contact_email_id}";
this.email.Text = $"{data.email}";
this.is_active.IsChecked = data.is_active;
this.comment.Text = $"{data.comment}";
this.contact.item = data.contact;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                contact_email data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.contact_email_id.Text);
                    data = db.contact_email.Where(d => d.contact_email_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.contact_email_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new contact_email();

                data.contact_email_id = Convert.ToInt32(this.contact_email_id.Text);
data.email = (this.email.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.comment = (this.comment.Text);
data.contact = this.contact.item as contact;


                if (this._isCreateMode)
                    db.contact_email.Add(data);
                db.SaveChanges();
            }
        }
    }
}
