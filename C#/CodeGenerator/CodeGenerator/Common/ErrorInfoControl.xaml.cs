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
    /// Interaction logic for ErrorInfoControl.xaml
    /// </summary>
    public partial class ErrorInfoControl : UserControl
    {
        public ErrorInfoControl(DatabaseCon con, error_code error)
        {
            InitializeComponent();

            this.labelID.Content = $"{error.error_code_id}";
            this.labelCode.Content = $"{error.error_code1}";
            this.labelMneumonic.Content = error.error_code_mneumonic;
            this.labelNarrative.Content = error.narrative;
            this.labelDefaultSeverity.Content = error.severity.description;

            var apps = con.getFromBitmask<application>(error.application_ids);
            this.labelApplications.Content = string.Join(",", apps.Select(a => a.description));

            var devices = con.getFromBitmask<device_type>(error.device_ids);
            this.labelDevices.Content = string.Join(",", devices.Select(d => d.description));
        }
    }
}
