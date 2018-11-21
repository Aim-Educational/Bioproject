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
    /// Interaction logic for Editorsupplier.xaml
    /// </summary>
    public partial class Editorsupplier : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editorsupplier(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            
        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.supplier_id.Text = "(CREATE NEW)";
            this.supplier_id.Text = "";
this.name.Text = "";
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
            var data = selectedItem as supplier;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a supplier.", "selectedItem");

            this.supplier_id.Text = $"{data.supplier_id}";
this.name.Text = $"{data.name}";
this.is_active.IsChecked = data.is_active;
this.comment.Text = $"{data.comment}";


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                supplier data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.supplier_id.Text);
                    data = db.suppliers.Where(d => d.supplier_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.supplier_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new supplier();

                data.supplier_id = Convert.ToInt32(this.supplier_id.Text);
data.name = (this.name.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.comment = (this.comment.Text);


                if (this._isCreateMode)
                    db.suppliers.Add(data);
                db.SaveChanges();
            }
        }
    }
}
