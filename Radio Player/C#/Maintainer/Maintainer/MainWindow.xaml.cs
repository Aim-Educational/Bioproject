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

namespace Maintainer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new EditorMood(EditorType.Create);
            window.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            (new EditorMood(EditorType.Delete, Convert.ToInt32(this.txt_id.Text))).Show();
        }

        // Close all other windows when this one is closed.
        private void Window_Closed(object sender, EventArgs e)
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window == this)
                    continue;

                window.Close();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            (new EditorMood(EditorType.Update, Convert.ToInt32(this.txt_id.Text))).Show();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            (new SearchForm()).Show();
        }
    }
}
