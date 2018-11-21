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
    /// Interaction logic for Editoraction_type.xaml
    /// </summary>
    public partial class Editoraction_type : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editoraction_type(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            
        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.action_type_id.Text = "(CREATE NEW)";
            this.action_type_id.Text = "";
this.description.Text = "";
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
            var data = selectedItem as action_type;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a action_type.", "selectedItem");

            this.action_type_id.Text = $"{data.action_type_id}";
this.description.Text = $"{data.description}";
this.is_active.IsChecked = data.is_active;
this.comment.Text = $"{data.comment}";


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                action_type data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.action_type_id.Text);
                    data = db.action_type.Where(d => d.action_type_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.action_type_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new action_type();

                data.action_type_id = Convert.ToInt32(this.action_type_id.Text);
data.description = (this.description.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.comment = (this.comment.Text);


                if (this._isCreateMode)
                    db.action_type.Add(data);
                db.SaveChanges();
            }
        }
    }
}
