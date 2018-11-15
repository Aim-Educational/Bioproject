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
using Maintainer.Forms;
using Maintainer.Interfaces;

namespace Maintainer.Controls
{
    /// <summary>
    /// Interaction logic for SelectorEditorControl.xaml
    /// </summary>
    public partial class SelectorEditorControl : UserControl
    {
        public ISearchProvider provider;
        public MainInterface mainInterface;

        private Object _item;
        public Object item
        {
            set
            {
                this._item = value;
                this.itemName.Text = (value == null) ? "" : this.provider.getDisplayStringForItem(value);
            }

            get => this._item;
        }

        public SelectorEditorControl()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            this.mainInterface.openSelector(this.provider, (obj) => this.item = obj);
        }
    }
}
