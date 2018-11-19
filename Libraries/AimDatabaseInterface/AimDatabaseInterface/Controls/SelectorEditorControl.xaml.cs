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
using Aim.DatabaseInterface.Interfaces;
using Aim.DatabaseInterface.Windows;

namespace Aim.DatabaseInterface.Controls
{
    /// <summary>
    /// A control which has the ability to use an <see cref="ISearchProvider"/> to allow the
    /// user to select some other piece of data.
    /// </summary>
    public partial class SelectorEditorControl : UserControl
    {
        /// <summary>
        /// The <see cref="ISearchProvider"/> to use for providing the data to the list.
        /// 
        /// The item provided by this provider will be accessbile by using <see cref="item"/>.
        /// </summary>
        public ISearchProvider provider;

        /// <summary>
        /// A reference to the <see cref="MainInterface"/> being used.
        /// </summary>
        public MainInterface mainInterface;

        private Object _item;

        /// <summary>
        /// The item selected by the user. May be null.
        /// </summary>
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
