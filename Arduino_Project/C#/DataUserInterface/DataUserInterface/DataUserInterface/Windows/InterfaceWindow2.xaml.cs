using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using DataManager.Model;

using DataUserInterface.Editors;
using DataUserInterface.SearchProviders;
using Aim.DatabaseInterface.Interfaces;

namespace DataUserInterface.DataUserInterface.Windows
{
    class MenuInfo
    {
        public MenuItem item;
        public int parentID;
    }

    /// <summary>
    /// Interaction logic for InterfaceWindow2.xaml
    /// </summary>
    public partial class InterfaceWindow2 : Window
    {
        public InterfaceWindow2()
        {
            InitializeComponent();
            this.generateMenu();
        }

        void generateMenu()
        {
            var itemMap = new Dictionary<int, MenuItem>();
            var itemsMissingParents = new List<MenuInfo>(); // In case we add a child before it's parent, we need
                                                           // to keep track of them somehow.
            using (var db = new PlanningContext())
            {
                foreach(var menu in db.menus)
                {
                    var item = new MenuItem();
                    item.Header = menu.description;

                    if (menu.parent_menu_id == -1)
                        this.menu.Items.Add(item);
                    else if (itemMap.ContainsKey(menu.parent_menu_id))
                        itemMap[menu.parent_menu_id].Items.Add(item);
                    else
                        itemsMissingParents.Add(new MenuInfo() { item = item, parentID = menu.parent_menu_id });

                    itemMap[menu.menu_id] = item;

                    var editorType   = Type.GetType($"DataUserInterface.Editors.{menu.editor}");
                    var providerType = Type.GetType($"DataUserInterface.SearchProviders.{menu.search_provider}");

                    if (editorType == null || providerType == null)
                        continue;

                    var editor   = Activator.CreateInstance(editorType, this.mainInterface) as UserControl;
                    var provider = Activator.CreateInstance(providerType) as ISearchProvider;

                    if (editor == null)
                    {
                        MessageBox.Show($"Invalid editor for {menu.description}");
                        continue;
                    }
                    if (provider == null)
                    {
                        MessageBox.Show($"Invalid search provider for {menu.description}");
                        continue;
                    }

                    item.Click += (sender, e) => this.mainInterface.openEditor(provider, editor);
                }

                foreach(var info in itemsMissingParents)
                {
                    if (itemMap.ContainsKey(info.parentID))
                        itemMap[info.parentID].Items.Add(info.item);
                    else
                        MessageBox.Show($"Orphan menu: {info.item.Header}");
                }
            }
        }
    }
}
