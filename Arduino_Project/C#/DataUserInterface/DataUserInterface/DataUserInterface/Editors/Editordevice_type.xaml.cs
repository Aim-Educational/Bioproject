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
    /// Interaction logic for Editordevice_type.xaml
    /// </summary>
    public partial class Editordevice_type : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editordevice_type(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.unit.provider = new SearchProviderunit();
this.unit.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.device_type_id.Text = "(CREATE NEW)";
            this.device_type_id.Text = "";
this.description.Text = "";
this.is_active.IsChecked = false;
this.comment.Text = "";
this.unit.item = null;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as device_type;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a device_type.", "selectedItem");

            this.device_type_id.Text = $"{data.device_type_id}";
this.description.Text = $"{data.description}";
this.is_active.IsChecked = data.is_active;
this.comment.Text = $"{data.comment}";
this.unit.item = data.unit;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                device_type data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.device_type_id.Text);
                    data = db.device_type.Where(d => d.device_type_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.device_type_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new device_type();

                data.device_type_id = Convert.ToInt32(this.device_type_id.Text);
data.description = /**/(this.description.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.comment = /**/(this.comment.Text);
data.unit = new Func<unit>(() => { foreach(var v in db.units){ if(v.unit_id == (this.unit.item as unit).unit_id) return v; } return null; })();


                if (this._isCreateMode)
                    db.device_type.Add(data);
                db.SaveChanges();
            }
        }
    }
}
