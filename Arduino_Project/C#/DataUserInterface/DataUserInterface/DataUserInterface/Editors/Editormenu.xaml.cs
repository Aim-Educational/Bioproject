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
    /// Interaction logic for Editormenu.xaml
    /// </summary>
    public partial class Editormenu : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public Editormenu(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            this.menu2.provider = new SearchProvidermenu();
this.menu2.mainInterface = mi;

        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.menu_id.Text = "(CREATE NEW)";
            this.menu_id.Text = "";
this.description.Text = "";
this.is_active.IsChecked = false;
this.search_provider.Text = "";
this.editor.Text = "";
this.menu2.item = null;

        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as menu;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a menu.", "selectedItem");

            this.menu_id.Text = $"{data.menu_id}";
this.description.Text = $"{data.description}";
this.is_active.IsChecked = data.is_active;
this.search_provider.Text = $"{data.search_provider}";
this.editor.Text = $"{data.editor}";
this.menu2.item = data.menu2;


            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new PlanningContext())
            {
                menu data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.menu_id.Text);
                    data = db.menus.Where(d => d.menu_id == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.menu_id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new menu();

                data.menu_id = Convert.ToInt32(this.menu_id.Text);
data.description = /**/(this.description.Text);
data.is_active = (bool)this.is_active.IsChecked;
data.search_provider = /**/(this.search_provider.Text);
data.editor = /**/(this.editor.Text);
data.menu2 = new Func<menu>(() => { foreach(var v in db.menus){ if(v.menu_id == (this.menu2.item as menu).menu_id) return v; } return null; })();


                if (this._isCreateMode)
                    db.menus.Add(data);
                db.SaveChanges();
            }
        }
    }
}
