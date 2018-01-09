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

namespace CodeGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            using (var db = new DatabaseCon())
            {
                this.populateDropdowns(db);
            }
        }

        private void populateDropdowns(DatabaseCon db)
        {
            this.updateStatus("Populating Dropdown menus");

            foreach(var language in db.languages.OrderBy(lang => lang.description))
            {
                languageDropDown.Items.Add(language.description);
            }

            foreach(var device in db.device_type.OrderBy(lang => lang.description))
            {
                deviceTypeDropDown.Items.Add(device.description);
            }

            foreach(var application in db.applications.OrderBy(app => app.description))
            {
                applicationDropDown.Items.Add(application.description);
            }
        }

        private void updateStatus(string status)
        {
            this.statusLabel.Content = "Status: " + status;
        }

        private void buttonGenerate_Click(object sender, RoutedEventArgs e)
        {
            this.updateStatus("Generating files...");

            var selectedLanguage = languageDropDown.SelectedItem.ToString();
            var selectedDevice = deviceTypeDropDown.SelectedItem.ToString();
            var selectedApplication = applicationDropDown.SelectedItem.ToString();
            using (var db = new DatabaseCon())
            {
                switch(selectedLanguage)
                {
                    case "C++":
                        var lang = (from db_lang in db.languages
                                   where db_lang.description == selectedLanguage
                                   select db_lang).First();
                        var application = (from db_app in db.applications
                                           where db_app.description == selectedApplication
                                           select db_app).First();

                        var templatePath = lang.path_to_templates;
                        var outputPath = application.path_to_output_file; // Might have to remove 'file' from that name, it's not accurate

                        this.ReadWriteFile(Path.Combine(templatePath, "Template.h"), Path.Combine(outputPath, "Template.h"));
                        this.ReadWriteFile(Path.Combine(templatePath, "Template.cpp"), Path.Combine(outputPath, "Template.cpp"));
                        break;

                    default: MessageBox.Show("Unsupported language: " + selectedLanguage); return;
                }
            }
        }

        private void ReadWriteFile(string template, string output)
        {
            var templateStream = new StreamReader(File.OpenRead(template));
            var outputStream = new StreamWriter(File.OpenWrite(output));

            while(templateStream.Peek() >= 0)
            {
                var line = templateStream.ReadLine();
                outputStream.WriteLine(line);
            }

            templateStream.Close();
            outputStream.Close();
        }
    }
}
