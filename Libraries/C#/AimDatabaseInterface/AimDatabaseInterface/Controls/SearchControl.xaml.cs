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
using Aim.DatabaseInterface.Interfaces;

namespace Aim.DatabaseInterface.Controls
{
    /// <summary>
    /// A control that takes the data given by a <see cref="ISearchProvider"/> to provide a list of
    /// data to the user, which is then used to execute an <see cref="ISearchAction"/>.
    /// </summary>
    public partial class SearchControl : UserControl
    {
        private ISearchProvider _provider;

        /// <summary>
        /// The <see cref="ISearchProvider"/> to use to provide the data to the user.
        /// 
        /// Setting this field will cause the search data to be refreshed.
        /// </summary>
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

        /// <summary>
        /// The <see cref="ISearchAction"/> to perform once the user has selected which piece of data to use.
        /// </summary>
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
            foreach (var column in this.dataGrid.Columns.Select(c => c.Header as string))
                this.dropdownColumn.Items.Add(column);

            if (this.dropdownColumn.Items.Count > 0)
                this.dropdownColumn.SelectedIndex = 0;
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Shamelessly stolen from StackOverflow
            DataGridRow row = ItemsControl.ContainerFromElement((DataGrid)sender, e.OriginalSource as DependencyObject) as DataGridRow;
            if (row == null) return;

            if (this.action != null)
            {
                var result = this.action.onSearchAction(row.Item);
                if (result == RefreshSearchList.yes)
                    this.provider = this.provider;
            }
        }

        // Sort the search grid as the user enters text.
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // No value in drop down
            if (this.dropdownColumn.SelectedIndex == -1)
                return;

            // Check the column name is valid, and get it.
            var columnName = this.dropdownColumn.SelectedItem as string;
            DataGridTextColumn column = this.dataGrid.Columns.Where(c => c.Header as string == columnName).FirstOrDefault() as DataGridTextColumn;
            if (column == null)
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
            foreach (var itemIndex in Enumerable.Range(0, this.dataGrid.Items.Count))
            {
                var item = this.dataGrid.Items[itemIndex];
                var content = column.GetCellContent(item) as TextBlock;
                var text = content.Text;

                if (text.ToLower().StartsWith(searchText))
                    swapItems(cursor++, itemIndex);
                else if (text.ToLower().Contains(searchText))
                    swapItems(cursor++, itemIndex);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (this.provider == null)
                return;

            this.provider = this.provider;
        }
    }
}
