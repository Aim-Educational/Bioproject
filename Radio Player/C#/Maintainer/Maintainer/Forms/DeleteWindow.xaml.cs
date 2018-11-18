using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
using Maintainer.Interfaces;

namespace Maintainer.Forms
{
    /// <summary>
    /// Interaction logic for DeleteWindow.xaml
    /// </summary>
    public partial class DeleteWindow : Window
    {
        public DeleteWindow()
        {
            InitializeComponent();
        }

        public static MessageBoxResult Show(IEditor editor, object value)
        {
            var window = new DeleteWindow();
            window.content.Content = editor;
            window.content.IsEnabled = false;
            window.ShowActivated = true;
            editor.onSearchAction(value);

            return ((bool)window.ShowDialog()) ? MessageBoxResult.Yes : MessageBoxResult.No;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
