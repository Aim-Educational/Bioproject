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
    /// Interaction logic for DeviceInfoControl.xaml
    /// </summary>
    public partial class DeviceInfoControl : UserControl
    {
        public DeviceInfoControl(device_type device)
        {
            InitializeComponent();

            this.labelID.Content = $"{device.device_type_id}";
            this.labelIndex.Content = $"{device.bit_index}";
            this.labelName.Content = $"{device.description}";
        }

        public DeviceInfoControl(application app)
        {
            InitializeComponent();

            this.labelID.Content = $"{app.application_id}";
            this.labelIndex.Content = $"{app.bit_index}";
            this.labelName.Content = $"{app.description}";
        }
    }
}
