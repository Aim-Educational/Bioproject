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
    /// Interaction logic for Editorcontact_telephone.xaml
    /// </summary>
    public partial class Editorcontact_telephone : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editorcontact_telephone(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.contact.provider = new SearchProvidercontact();
this.contact.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.contact_telephone_id.Text = "(CREATE NEW)";
            this.contact_telephone_id.Text = "";
this.telephone_number.Text = "";
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
            var data = selectedItem as contact_telephone;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a contact_telephone.", "selectedItem");

            this.contact_telephone_id.Text = $"{data.contact_telephone_id}";
this.telephone_number.Text = $"{data.telephone_number}";
this.is_active.IsChecked = data.is_active;
this.comment.Text = $"{data.comment}";
this.contact.item = data.contact;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                contact_telephone data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.contact_telephone_id.Text);
                    data = db.contact_telephone.Where(d => d.contact_telephone_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.contact_telephone_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new contact_telephone();

                data.contact_telephone_id = Convert.ToInt32(this.contact_telephone_id.Text);
data.telephone_number = (this.telephone_number.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.comment = (this.comment.Text);
data.contact = new Func<contact>(() => { foreach(var v in db.contacts){ if(v.contact_id == (this.contact.item as contact).contact_id) return v; } return null; })();


                if (this._isCreateMode)
                    db.contact_telephone.Add(data);
                db.SaveChanges();
            }
        }
    }
}
