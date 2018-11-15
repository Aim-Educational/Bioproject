using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
using Maintainer.Forms;
using Maintainer.Interfaces;

namespace Maintainer.Controls
{
    /// <summary>
    /// Interaction logic for ListboxSelectorEditorControl.xaml
    /// </summary>
    public partial class ListboxSelectorEditorControl : UserControl, INotifyPropertyChanged
    {
        public MainInterface mainInterface;
        public event PropertyChangedEventHandler PropertyChanged;

        public ISearchProvider provider { get; set; }
        public ObservableCollection<string> itemNames { get; set; }

        /// <summary>
        /// USE `addItem` TO ADD TO THIS COLLECTION.
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

        public string selectedItemName
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

        public void clear()
        {
            this.items.Clear();
            this.itemNames.Clear();
            this.selectedItem = null;
        }

        public void addItem(Object item)
        {
            this.items.Add(item);
            this.itemNames.Add(this.provider.getDisplayStringForItem(item));
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if(this.selectedItem == null)
                return;

            foreach(var item in this.items)
            {
                if(this.provider.areSameItems(item, this.selectedItem))
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

                this.items.RemoveAt(this.list.SelectedIndex);
                this.itemNames.RemoveAt(this.list.SelectedIndex);
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            this.mainInterface.openSelector(this.provider, (obj) => this.selectedItem = obj);
        }
    }
}
