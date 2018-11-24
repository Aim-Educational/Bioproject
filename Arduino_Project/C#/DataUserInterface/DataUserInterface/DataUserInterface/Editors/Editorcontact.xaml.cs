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
    /// Interaction logic for Editorcontact.xaml
    /// </summary>
    public partial class Editorcontact : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editorcontact(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.contact_type.provider = new SearchProvidercontact_type();
this.contact_type.mainInterface = mi;
this.supplier.provider = new SearchProvidersupplier();
this.supplier.mainInterface = mi;
this.user.provider = new SearchProvideruser();
this.user.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.contact_id.Text = "(CREATE NEW)";
            this.contact_id.Text = "";
this.is_active.IsChecked = false;
this.comment.Text = "";
this.contact_type.item = null;
this.supplier.item = null;
this.user.item = null;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as contact;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a contact.", "selectedItem");

            this.contact_id.Text = $"{data.contact_id}";
this.is_active.IsChecked = data.is_active;
this.comment.Text = $"{data.comment}";
this.contact_type.item = data.contact_type;
this.supplier.item = data.supplier;
this.user.item = data.user;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                contact data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.contact_id.Text);
                    data = db.contacts.Where(d => d.contact_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.contact_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new contact();

                data.contact_id = Convert.ToInt32(this.contact_id.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.comment = /**/(this.comment.Text);
data.contact_type = new Func<contact_type>(() => { foreach(var v in db.contact_type){ if(v.contact_type_id == (this.contact_type.item as contact_type).contact_type_id) return v; } return null; })();
data.supplier = new Func<supplier>(() => { foreach(var v in db.suppliers){ if(v.supplier_id == (this.supplier.item as supplier).supplier_id) return v; } return null; })();
data.user = new Func<user>(() => { foreach(var v in db.users){ if(v.user_id == (this.user.item as user).user_id) return v; } return null; })();


                if (this._isCreateMode)
                    db.contacts.Add(data);
                db.SaveChanges();
            }
        }
    }
}
