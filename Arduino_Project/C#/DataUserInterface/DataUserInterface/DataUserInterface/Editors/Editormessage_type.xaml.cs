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
    /// Interaction logic for Editormessage_type.xaml
    /// </summary>
    public partial class Editormessage_type : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editormessage_type(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            
        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.message_type_id.Text = "(CREATE NEW)";
            this.message_type_id.Text = "";
this.description.Text = "";
this.version.Text = "";
this.is_active.IsChecked = false;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as message_type;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a message_type.", "selectedItem");

            this.message_type_id.Text = $"{data.message_type_id}";
this.description.Text = $"{data.description}";
this.version.Text = $"{data.version}";
this.is_active.IsChecked = data.is_active;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                message_type data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.message_type_id.Text);
                    data = db.message_type.Where(d => d.message_type_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.message_type_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new message_type();

                data.message_type_id = Convert.ToInt32(this.message_type_id.Text);
data.description = (this.description.Text);
data.version = Convert.ToInt32(this.version.Text);
data.is_active = (bool)this.is_active.IsChecked;


                if (this._isCreateMode)
                    db.message_type.Add(data);
                db.SaveChanges();
            }
        }
    }
}
