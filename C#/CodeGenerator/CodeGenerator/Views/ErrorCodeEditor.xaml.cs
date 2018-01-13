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

namespace CodeGenerator.Views
{
    /// <summary>
    /// Interaction logic for ErrorCodeEditor.xaml
    /// </summary>
    public partial class ErrorCodeEditor : UserControl
    {
        public ErrorCodeEditor(MainWindow window)
        {
            InitializeComponent();
        }

        // Makes it so only numbers can be entered into the textbox.
        private void textboxCode_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // false = don't allow text
            e.Handled = !e.Text.All(c => char.IsDigit(c));
        }
    }
}
