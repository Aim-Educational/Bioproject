using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using CodeGenerator.Database;
using CodeGenerator.Views;

namespace CodeGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, UserControl> _viewCache { get; set; }
        private readonly Brush TAB_BACKGROUND_CURRENT = new SolidColorBrush(Color.FromRgb(0xFF, 0xDD, 0xDD));
        private readonly Brush TAB_BACKGROUND_DEFAULT = new SolidColorBrush(Color.FromRgb(0xDD, 0xDD, 0xDD));

        public MainWindow()
        {
            InitializeComponent();
            this._viewCache = new Dictionary<string, UserControl>();

            this.changeViewEvent(this.buttonGenerator, null);
        }

        /// <summary>
        /// Updates the status label.
        /// </summary>
        /// <param name="newStatus">The new status to show. Will be prefixed with "Last Status: "</param>
        public void updateStatus(string newStatus)
        {
            this.labelStatus.Content = $"Last Status: {newStatus}";
            this.labelStatus.ToolTip = newStatus;
        }

        /// <summary>
        /// Changes the current view to the given view type.
        /// 
        /// If a view of `ViewType` hasn't been made already, an object of it is made and then cached.
        /// If a view of `ViewType` has been cached already, then that object is reused.
        /// </summary>
        /// 
        /// <typeparam name="ViewType">
        ///     The type of the view to create, e.g. `changeView<GeneratorView>`.
        ///     
        ///     This type must inherit from `UserControl`.
        ///     This type must take a single parameter of `MainWindow` for a constructor.
        /// </typeparam>
        public void changeView<ViewType>() where ViewType : UserControl
        {
            var typeName = typeof(ViewType).ToString();
            UserControl view = null;

            if(this._viewCache.ContainsKey(typeName))
                view = this._viewCache[typeName];
            else
            {
                view = (UserControl)Activator.CreateInstance(typeof(ViewType), this);
                this._viewCache[typeName] = view;
            }

            this.currentView.Content = view;
        }

        private void changeViewEvent(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if(button == null)
                throw new Exception("Bug");

            // Change the view
            if(button == this.buttonApplications)   this.changeView<ApplicationEditor>();
            else if(button == this.buttonDevices)   this.changeView<DeviceEditor>();
            else if(button == this.buttonGenerator) this.changeView<GeneratorView>();
            else if(button == this.buttonLanguages) this.changeView<LanguageEditor>();
            else throw new Exception($"Did you forget to add a new button to the event handler? Name: {button.Name}");

            // Set all buttons to the default background colour
            foreach(var but in this.panelTabs.Children)
                ((Button)but).Background = TAB_BACKGROUND_DEFAULT;

            // Then set the current button to the selected background colour
            button.Background = TAB_BACKGROUND_CURRENT;
        }
    }
}
