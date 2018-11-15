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

namespace Maintainer.Controls
{
    /// <summary>
    /// Interaction logic for ListboxEditorControl.xaml
    /// </summary>
    public partial class ListboxEditorControl : UserControl
    {
        public ListboxEditorControl()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrWhiteSpace(this.inputBox.Text))
                return;

            if(this.inputBox.Text.Contains(","))
            {
                MessageBox.Show("Input string cannot contain a comma.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.list.Items.Add(this.inputBox.Text);
            this.inputBox.Text = "";
        }

        private void inputBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                e.Handled = true;
                this.btnAdd_Click(sender, null);
            }
        }

        private void list_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete && this.list.SelectedIndex != -1)
            {
                e.Handled = true;

                this.list.Items.RemoveAt(this.list.SelectedIndex);
            }
        }
    }
}
