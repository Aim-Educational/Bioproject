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
using System.IO;

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
            try
            {
                var app = this.dropDownApplication.SelectedItem?.ToString();
                var dev = this.dropDownDevice.SelectedItem?.ToString();
                var lang = this.dropDownLanguage.SelectedItem?.ToString();

                if (app == null || dev == null || lang == null)
                    throw new Exception("One of selection boxes haven't been used");

                this.generate(app, dev, lang);
            }
            catch(Exception ex)
            {
                this._window.updateStatus($"[ERROR] {ex.ToString()}");
            }
        }

        private void generate(string application, string device, string language)
        {
            this._window.updateStatus("Generating...");
            using (var db = new DatabaseCon())
            {
                var dbApp = db.getFromDescription<application>(application);
                var dbLang = db.getFromDescription<language>(language);
                var errors = db.getFilteredErrors(application, device).ToList();

                this.showExportedErrors(db, errors);

                switch(dbLang.description)
                {
                    case "C++":
                        this.copyTemplates(
                                dbLang.path_to_templates,
                                dbApp.path_to_output_file,
                                this.generateCPP(db, errors),
                                (uint)errors.Count(),
                                "Template.cpp",
                                "Template.h"
                            );
                        break;

                    default:
                        throw new Exception($"Unsupported language: {language}");
                }
            }
        }

        private void copyTemplates(string templateDir, string outputDir, string errorListCode, uint errorCount, params string[] files)
        {
            if (!Directory.Exists(templateDir))
                throw new DirectoryNotFoundException(templateDir);

            if(!Directory.Exists(outputDir))
                throw new DirectoryNotFoundException(outputDir);

            foreach (var file in files)
            {
                var template = Path.Combine(templateDir, file);
                var generated = Path.Combine(outputDir, file.Replace("Template", "Gen-ErrorList"));

                var text = File.ReadAllText(template);
                text = text.Replace("<ErrorCodes>", errorListCode);
                text = text.Replace("<FileName>", "Gen-ErrorList");
                text = text.Replace("<ErrorCount>", $"{errorCount}");

                File.WriteAllText(generated, text);
            }
        }

        private string generateCPP(DatabaseCon con, List<error_code> codes)
        {
            List<string> objects = new List<string>();
            int i = 0;
            foreach(var error in codes)
            {
                var severity = from sev in con.severities where sev.severity_id == error.default_severity_id select sev;
                objects.Add(
                    $"ErrorList::errorCodes[{i++}] = \n" +
                    $"ErrorCode(\n" +
                    $"  F(\"{error.error_code1}\"),\n" +
                    $"  F(\"{error.error_code_mneumonic}\"),\n" +
                    $"  F(\"{error.narrative}\"),\n" +
                    $"  Severity::{severity.First().description}\n" +
                    $");");
            }

            return string.Join("\n", objects);
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
