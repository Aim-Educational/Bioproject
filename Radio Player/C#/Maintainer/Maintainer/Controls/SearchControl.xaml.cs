using System;
using System.Collections.Generic;
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
using Maintainer.Interfaces;

namespace Maintainer.Controls
{
    /// <summary>
    /// Interaction logic for SearchControl.xaml
    /// </summary>
    public partial class SearchControl : UserControl
    {
        private ISearchProvider _provider;
        public ISearchProvider provider
        {
            set
            {
                this._provider = value;
                this.dataGrid.Columns.Clear();
                this.dataGrid.Items.Clear();
                value.populateDataGrid(this.dataGrid);
                this.populateSearchList();
            }

            get => this._provider;
        }

        private ISearchAction _action;
        public ISearchAction action
        {
            set => this._action = value;
            get => this._action;
        }

        public SearchControl()
        {
            InitializeComponent();
        }

        private void populateSearchList()
        {
            this.txtSearch.Text = "";
            this.dropdownColumn.Items.Clear();
            foreach(var column in this.dataGrid.Columns.Select(c => c.Header as string))
                this.dropdownColumn.Items.Add(column);

            if(this.dropdownColumn.Items.Count > 0)
                this.dropdownColumn.SelectedIndex = 0;
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Shamelessly stolen from StackOverflow
            DataGridRow row = ItemsControl.ContainerFromElement((DataGrid)sender, e.OriginalSource as DependencyObject) as DataGridRow;
            if (row == null) return;

            if(this.action != null)
            { 
                var result = this.action.onSearchAction(row.Item);
                if(result == RefreshSearchList.yes)
                    this.provider = this.provider;
            }
        }

        // Sort the search grid as the user enters text.
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // No value in drop down
            if(this.dropdownColumn.SelectedIndex == -1)
                return;

            // Check the column name is valid, and get it.
            var columnName = this.dropdownColumn.SelectedItem as string;
            DataGridTextColumn column = this.dataGrid.Columns.Where(c => c.Header as string == columnName).FirstOrDefault() as DataGridTextColumn;
            if(column == null)
                return;

            // Default-sort the grid if the searchbox is empty
            if (String.IsNullOrWhiteSpace(this.txtSearch.Text))
            {
                this.dataGrid.Items.SortDescriptions.Clear();
                this.dataGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, ListSortDirection.Ascending));
                return;
            }

            Action<int, int> swapItems = (indexA, indexB) =>
            {
                var itemA = this.dataGrid.Items[indexA];
                this.dataGrid.Items[indexA] = this.dataGrid.Items[indexB];
                this.dataGrid.Items[indexB] = itemA;
            };

            // Sort the data by [Item_Starts_With] -> [Item_Contains] -> [Everything else]
            int cursor = 0;
            var searchText = this.txtSearch.Text.ToLower();
            foreach(var itemIndex in Enumerable.Range(0, this.dataGrid.Items.Count))
            {
                var item    = this.dataGrid.Items[itemIndex];
                var content = column.GetCellContent(item) as TextBlock;
                var text    = content.Text;

                if(text.ToLower().StartsWith(searchText))
                    swapItems(cursor++, itemIndex);
                else if(text.ToLower().Contains(searchText))
                    swapItems(cursor++, itemIndex);
            }
        }
    }
}
