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
    /// Interaction logic for Editoraction_level.xaml
    /// </summary>
    public partial class Editoraction_level : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editoraction_level(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.action_type.provider = new SearchProvideraction_type();
this.action_type.mainInterface = mi;
this.device.provider = new SearchProviderdevice();
this.device.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.action_level_id.Text = "(CREATE NEW)";
            this.action_level_id.Text = "";
this.value.Text = "";
this.is_active.IsChecked = false;
this.comment.Text = "";
this.action_type.item = null;
this.device.item = null;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as action_level;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a action_level.", "selectedItem");

            this.action_level_id.Text = $"{data.action_level_id}";
this.value.Text = $"{data.value}";
this.is_active.IsChecked = data.is_active;
this.comment.Text = $"{data.comment}";
this.action_type.item = data.action_type;
this.device.item = data.device;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                action_level data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.action_level_id.Text);
                    data = db.action_level.Where(d => d.action_level_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.action_level_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new action_level();

                data.action_level_id = Convert.ToInt32(this.action_level_id.Text);
data.value = Convert.ToDouble(this.value.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.comment = (this.comment.Text);
data.action_type = this.action_type.item as action_type;
data.device = this.device.item as device;


                if (this._isCreateMode)
                    db.action_level.Add(data);
                db.SaveChanges();
            }
        }
    }
}
