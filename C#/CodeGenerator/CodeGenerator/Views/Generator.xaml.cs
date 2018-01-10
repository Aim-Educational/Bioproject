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

using CodeGenerator.Common;
using CodeGenerator.Database;

namespace CodeGenerator.Views
{
    /// <summary>
    /// Interaction logic for Generator.xaml
    /// </summary>
    public partial class GeneratorView : UserControl
    {
        private MainWindow _window;

        public GeneratorView(MainWindow window)
        {
            InitializeComponent();
            this._window = window;

            this.populateDropDowns();
        }

        private void populateDropDowns()
        {
            this._window.updateStatus("Populating drop down menus");
            using (var db = new DatabaseCon())
            {
                foreach(var app in db.applications.OrderBy(a => a.description))
                    this.dropDownApplication.Items.Add(app.description);

                foreach(var device in db.device_type.OrderBy(d => d.description))
                    this.dropDownDevice.Items.Add(device.description);

                foreach(var language in db.languages.OrderBy(l => l.description))
                    this.dropDownLanguage.Items.Add(language.description);
            }
        }

        private void buttonGenerate_Click(object sender, RoutedEventArgs e)
        {
            var app = this.dropDownApplication.SelectedItem.ToString();
            var dev = this.dropDownDevice.SelectedItem.ToString();
            var lang = this.dropDownLanguage.SelectedItem.ToString();
            this.generate(app, dev, lang);
        }

        private void generate(string application, string device, string language)
        {
            this._window.updateStatus("Generating...");
            using (var db = new DatabaseCon())
            {
                var errors = db.getFilteredErrors(application, device).ToList();
                this.showExportedErrors(db, errors);
            }
        }

        private void showExportedErrors(DatabaseCon db, List<error_code> errors)
        {
            this.panelErrors.Children.Clear();
            foreach (var error in errors.OrderBy(e => e.error_code1))
            {
                var info = new ErrorInfoControl(db, error)
                {
                    Margin = new Thickness(0, 2, 0, 0)
                };
                this.panelErrors.Children.Add(info);
            }
        }
    }
}
