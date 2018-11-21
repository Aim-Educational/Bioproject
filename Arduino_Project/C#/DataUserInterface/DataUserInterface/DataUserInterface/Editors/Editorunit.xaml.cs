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
    /// Interaction logic for Editorunit.xaml
    /// </summary>
    public partial class Editorunit : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editorunit(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            
        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.unit_id.Text = "(CREATE NEW)";
            this.unit_id.Text = "";
this.description.Text = "";
this.abbreviation.Text = "";
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
            var data = selectedItem as unit;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a unit.", "selectedItem");

            this.unit_id.Text = $"{data.unit_id}";
this.description.Text = $"{data.description}";
this.abbreviation.Text = $"{data.abbreviation}";
this.is_active.IsChecked = data.is_active;
this.comment.Text = $"{data.comment}";


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                unit data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.unit_id.Text);
                    data = db.units.Where(d => d.unit_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.unit_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new unit();

                data.unit_id = Convert.ToInt32(this.unit_id.Text);
data.description = (this.description.Text);
data.abbreviation = (this.abbreviation.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.comment = (this.comment.Text);


                if (this._isCreateMode)
                    db.units.Add(data);
                db.SaveChanges();
            }
        }
    }
}
