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
    /// Interaction logic for Editorgroup_action.xaml
    /// </summary>
    public partial class Editorgroup_action : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editorgroup_action(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.action_type.provider = new SearchProvideraction_type();
this.action_type.mainInterface = mi;
this.group_type.provider = new SearchProvidergroup_type();
this.group_type.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.group_action_id.Text = "(CREATE NEW)";
            this.group_action_id.Text = "";
this.is_active.IsChecked = false;
this.comment.Text = "";
this.action_type.item = null;
this.group_type.item = null;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as group_action;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a group_action.", "selectedItem");

            this.group_action_id.Text = $"{data.group_action_id}";
this.is_active.IsChecked = data.is_active;
this.comment.Text = $"{data.comment}";
this.action_type.item = data.action_type;
this.group_type.item = data.group_type;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                group_action data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.group_action_id.Text);
                    data = db.group_action.Where(d => d.group_action_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.group_action_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new group_action();

                data.group_action_id = Convert.ToInt32(this.group_action_id.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.comment = (this.comment.Text);
data.action_type = new Func<action_type>(() => { foreach(var v in db.action_type){ if(v.action_type_id == (this.action_type.item as action_type).action_type_id) return v; } return null; })();
data.group_type = new Func<group_type>(() => { foreach(var v in db.group_type){ if(v.group_type_id == (this.group_type.item as group_type).group_type_id) return v; } return null; })();


                if (this._isCreateMode)
                    db.group_action.Add(data);
                db.SaveChanges();
            }
        }
    }
}
