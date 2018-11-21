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
    /// Interaction logic for Editorhistory_event.xaml
    /// </summary>
    public partial class Editorhistory_event : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editorhistory_event(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            
        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.history_event_id.Text = "(CREATE NEW)";
            this.history_event_id.Text = "";
this.description.Text = "";
this.is_active.IsChecked = false;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as history_event;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a history_event.", "selectedItem");

            this.history_event_id.Text = $"{data.history_event_id}";
this.description.Text = $"{data.description}";
this.is_active.IsChecked = data.is_active;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                history_event data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.history_event_id.Text);
                    data = db.history_event.Where(d => d.history_event_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.history_event_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new history_event();

                data.history_event_id = Convert.ToInt32(this.history_event_id.Text);
data.description = (this.description.Text);
data.is_active = (bool)this.is_active.IsChecked;


                if (this._isCreateMode)
                    db.history_event.Add(data);
                db.SaveChanges();
            }
        }
    }
}
