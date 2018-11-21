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
    /// Interaction logic for Editorgroup_type.xaml
    /// </summary>
    public partial class Editorgroup_type : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editorgroup_type(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.group_type2.provider = new SearchProvidergroup_type();
this.group_type2.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.group_type_id.Text = "(CREATE NEW)";
            this.group_type_id.Text = "";
this.name.Text = "";
this.description.Text = "";
this.is_active.IsChecked = false;
this.comment.Text = "";
this.group_type2.item = null;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as group_type;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a group_type.", "selectedItem");

            this.group_type_id.Text = $"{data.group_type_id}";
this.name.Text = $"{data.name}";
this.description.Text = $"{data.description}";
this.is_active.IsChecked = data.is_active;
this.comment.Text = $"{data.comment}";
this.group_type2.item = data.group_type2;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                group_type data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.group_type_id.Text);
                    data = db.group_type.Where(d => d.group_type_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.group_type_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new group_type();

                data.group_type_id = Convert.ToInt32(this.group_type_id.Text);
data.name = (this.name.Text);
data.description = (this.description.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.comment = (this.comment.Text);
data.group_type2 = this.group_type2.item as group_type;


                if (this._isCreateMode)
                    db.group_type.Add(data);
                db.SaveChanges();
            }
        }
    }
}
