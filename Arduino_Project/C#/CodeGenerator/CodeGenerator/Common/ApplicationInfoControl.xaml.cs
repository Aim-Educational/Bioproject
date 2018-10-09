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

using CodeGenerator.Database;

namespace CodeGenerator.Common
{
    /// <summary>
    /// Interaction logic for ApplicationInfoControl.xaml
    /// </summary>
    public partial class ApplicationInfoControl : UserControl
    {
        public ApplicationInfoControl(application app)
        {
            InitializeComponent();

            this.labelID.Content = $"{app.application_id}";
            this.labelIndex.Content = $"{app.bit_index}";
            this.labelName.Content = app.description;
            this.labelPath.Content = app.path_to_output_file;
        }
    }
}
