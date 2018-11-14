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

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Shamelessly stolen from StackOverflow
            DataGridRow row = ItemsControl.ContainerFromElement((DataGrid)sender, e.OriginalSource as DependencyObject) as DataGridRow;
            if (row == null) return;

            if(this.action != null)
                this.action.onSearchAction(row.Item);
        }
    }
}
