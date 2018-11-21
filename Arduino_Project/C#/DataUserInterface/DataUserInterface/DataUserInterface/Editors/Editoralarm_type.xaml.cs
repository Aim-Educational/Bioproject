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
    /// Interaction logic for Editoralarm_type.xaml
    /// </summary>
    public partial class Editoralarm_type : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editoralarm_type(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            
        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.alarm_type_id.Text = "(CREATE NEW)";
            this.alarm_type_id.Text = "";
this.description.Text = "";
this.comment.Text = "";
this.is_active.IsChecked = false;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as alarm_type;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a alarm_type.", "selectedItem");

            this.alarm_type_id.Text = $"{data.alarm_type_id}";
this.description.Text = $"{data.description}";
this.comment.Text = $"{data.comment}";
this.is_active.IsChecked = data.is_active;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                alarm_type data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.alarm_type_id.Text);
                    data = db.alarm_type.Where(d => d.alarm_type_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.alarm_type_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new alarm_type();

                data.alarm_type_id = Convert.ToInt32(this.alarm_type_id.Text);
data.description = (this.description.Text);
data.comment = (this.comment.Text);
data.is_active = (bool)this.is_active.IsChecked;


                if (this._isCreateMode)
                    db.alarm_type.Add(data);
                db.SaveChanges();
            }
        }
    }
}
