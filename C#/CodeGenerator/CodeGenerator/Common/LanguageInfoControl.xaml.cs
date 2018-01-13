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
    /// Interaction logic for LanguageInfoControl.xaml
    /// </summary>
    public partial class LanguageInfoControl : UserControl
    {
        public LanguageInfoControl(language lang)
        {
            InitializeComponent();

            this.labelID.Content = $"{lang.language_id}";
            this.labelName.Content = lang.description;
            this.labelPath.Content = lang.path_to_templates;
        }
    }
}
