using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Aim.DatabaseInterface.Interfaces;
using Aim.DatabaseInterface.Windows;

namespace Aim.DatabaseInterface.Controls
{
    /// <summary>
    /// Similar to the <see cref="ListboxEditorControl"/> in that it contains a list of items the user can mess around with,
    /// but with some unique features.
    /// 
    /// The list can be reordered using buttons.
    /// 
    /// The values have to be searched with similar to how <see cref="SelectorEditorControl"/> functions.
    /// 
    /// Please make sure <see cref="ListboxSelectorEditorControl.provider"/> and <see cref="ListboxSelectorEditorControl.mainInterface"/> are
    /// set before this control is used by the user.
    /// </summary>
    public partial class ListboxSelectorEditorControl : UserControl
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// A reference to the <see cref="MainInterface"/> being used.
        /// </summary>
        public MainInterface mainInterface;

        /// <summary>
        /// The <see cref="ISearchProvider"/> to use for providing the data to the list.
        /// 
        /// Any item provided by the search provider that is then added to the list can be retrieved from
        /// <see cref="items"/>.
        /// </summary>
        public ISearchProvider provider { get; set; }

        // Purposefully undocumented. Cannot be private due to data bindings, but doesn't need to be used by the user.
        public ObservableCollection<string> itemNames { get; set; }

        /// <summary>
        /// The items in the list, in the same order they appear on screen.
        /// 
        /// If you want to add to this list PLEASE USE the <see cref="addItem(object)"/> function.
        /// </summary>
        public List<Object> items { get; set; }

        private Object _selectedItem;
        private Object selectedItem
        {
            get => _selectedItem;
            set
            {
                this._selectedItem = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("selectedItemName"));
            }
        }
        
        private string selectedItemName
        {
            get => this.provider.getDisplayStringForItem(this.selectedItem);
            set
            {
                string dummy = value;
            }
        }

        public ListboxSelectorEditorControl()
        {
            InitializeComponent();
            DataContext = this;

            this.itemNames = new ObservableCollection<string>();
            this.items = new List<object>();
        }

        /// <summary>
        /// Clears the input box, as well as the value list.
        /// </summary>
        public void clear()
        {
            this.items.Clear();
            this.itemNames.Clear();
            this.selectedItem = null;
        }

        /// <summary>
        /// Adds an item into the value list, this function affects the <see cref="items"/> property.
        /// 
        /// A display name for the object will be retrived via the associated <see cref="provider"/>.
        /// </summary>
        /// <param name="item">The item to use.</param>
        public void addItem(Object item)
        {
            this.items.Add(item);
            this.itemNames.Add(this.provider.getDisplayStringForItem(item));
        }

        private void removeItemAt(int index)
        {
            if (index < 0 || index >= this.list.Items.Count)
                return;

            this.items.RemoveAt(index);
            this.itemNames.RemoveAt(index);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (this.selectedItem == null)
                return;

            if(this.provider == null)
                throw new InvalidOperationException("The ISearchProvider (.provider field) is null.");

            foreach (var item in this.items)
            {
                if (this.provider.areSameItems(item, this.selectedItem))
                {
                    MessageBox.Show("Cannot add duplicate items.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            this.addItem(this.selectedItem);
            this.selectedItem = null;
        }

        private void list_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && this.list.SelectedIndex != -1)
            {
                e.Handled = true;

                this.removeItemAt(this.list.SelectedIndex);
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            this.mainInterface.openSelector(this.provider, (obj) => this.selectedItem = obj);
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (this.list.SelectedIndex == -1 || this.list.SelectedIndex == 0)
                return;

            this.moveLists(this.list.SelectedIndex - 1, false);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.list.SelectedIndex == -1 || this.list.SelectedIndex == this.items.Count() - 1)
                return;

            this.moveLists(this.list.SelectedIndex, true);
        }

        private void moveLists(int leftIndex, bool selectRight)
        {
            var rightIndex = leftIndex + 1;

            var left1 = this.items[leftIndex];
            this.items[leftIndex] = this.items[rightIndex];
            this.items[rightIndex] = left1;

            var left2 = this.itemNames[leftIndex];
            this.itemNames[leftIndex] = this.itemNames[rightIndex];
            this.itemNames[rightIndex] = left2;

            this.list.SelectedIndex = (selectRight) ? rightIndex : leftIndex;
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            this.removeItemAt(this.list.SelectedIndex);
        }
    }
}
