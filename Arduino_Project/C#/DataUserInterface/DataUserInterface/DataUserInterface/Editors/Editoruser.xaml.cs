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
    /// Interaction logic for Editoruser.xaml
    /// </summary>
    public partial class Editoruser : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editoruser(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            
        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.user_id.Text = "(CREATE NEW)";
            this.user_id.Text = "";
this.forename.Text = "";
this.surname.Text = "";
this.username.Text = "";
this.password.Text = "";
this.is_active.IsChecked = false;
this.comment.Text = "";

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as user;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a user.", "selectedItem");

            this.user_id.Text = $"{data.user_id}";
this.forename.Text = $"{data.forename}";
this.surname.Text = $"{data.surname}";
this.username.Text = $"{data.username}";
this.password.Text = $"{data.password}";
this.is_active.IsChecked = data.is_active;
this.comment.Text = $"{data.comment}";


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                user data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.user_id.Text);
                    data = db.users.Where(d => d.user_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.user_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new user();

                data.user_id = Convert.ToInt32(this.user_id.Text);
data.forename = /**/(this.forename.Text);
data.surname = /**/(this.surname.Text);
data.username = /**/(this.username.Text);
data.password = /**/(this.password.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.comment = /**/(this.comment.Text);


                if (this._isCreateMode)
                    db.users.Add(data);
                db.SaveChanges();
            }
        }
    }
}
