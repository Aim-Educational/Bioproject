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
using System.IO;

using CodeGenerator.Common;
using CodeGenerator.Database;

namespace CodeGenerator.Views
{
    /// <summary>
    /// Interaction logic for LanguageEditor.xaml
    /// </summary>
    public partial class LanguageEditor : UserControl
    {
        private MainWindow _window;
        private List<language> _languages;

        public LanguageEditor(MainWindow window)
        {
            InitializeComponent();
            this._window = window;

            this._updateLanguages();
        }

        private void _updateLanguages()
        {
            this.dropDownLanguages.Items.Clear();
            this.panelLanguages.Children.Clear();
            this.dropDownLanguages.Items.Add("[NEW LANGUAGE]");

            Action<language> addToPanel =
            (lang) =>
            {
                this.panelLanguages.Children.Add(
                new LanguageInfoControl(lang)
                {
                    Margin = new Thickness(0, 2, 0, 0),
                    Width = Double.NaN // auto
                });
            };

            using (var db = new DatabaseCon())
                ViewHelper.populateDropDownWithT<language>(db, this.dropDownLanguages, out this._languages, addToPanel);
        }

        private void dropDownLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            this._window.updateStatus("Updating UI to reflect chosen language");
            var langName = e.AddedItems[0].ToString();
            var lang = this._languages.SingleOrDefault(l => l.description == langName);

            if (lang != null)
            {
                this.textboxName.Text = lang.description;
                this.textboxPath.Text = lang.path_to_templates;
            }
        }

        private void buttonDeleteLanguage_Click(object sender, RoutedEventArgs e)
        {
            var item = this.dropDownLanguages.SelectedItem;
            if (item == null || item.ToString() == "[NEW LANGUAGE]")
                return;

            var wasDeletion = ViewHelper.deleteTByDescription<language>(this._window, item.ToString());
            if (wasDeletion)
                this._updateLanguages();
        }

        private void buttonSelectPath_Click(object sender, RoutedEventArgs e)
        {
            var path = ViewHelper.askForPath(this.textboxPath.Text);
            if(path != null)
                this.textboxPath.Text = path;
        }

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.dropDownLanguages.SelectedItem == null)
                    throw new Exception("No language was selected!");

                var name = this.textboxName.Text;
                var path = this.textboxPath.Text;

                if (string.IsNullOrEmpty(name))
                    throw new Exception("No name has been given");

                if (path.Length > 255)
                    throw new Exception($"The output path is too large, it has ${path.Length} characters while the limit is 255 characters");

                if (!Directory.Exists(path))
                    throw new Exception("The output path does not exist");

                using (var db = new DatabaseCon())
                {
                    if (this.dropDownLanguages.SelectedItem.ToString() == "[NEW LANGUAGE]")
                    {
                        db.enforceNameIsUnique<language>(name);
                        this._addLanguage(db, name, path);
                    }
                    else
                    {
                        var cached = this._languages.Single(l => l.description == this.dropDownLanguages.SelectedItem.ToString());
                        if (name != cached.description)
                            db.enforceNameIsUnique<application>(name);

                        this._updateLanguage(db, name, path);
                    }
                }
            }
            catch (Exception ex)
            {
                this._window.updateStatus($"[ERROR] {ex.ToString()}");
            }
        }

        private void _addLanguage(DatabaseCon db, string name, string path)
        {
            this._window.updateStatus($"Adding new language '{name}'");
            db.languages.Add(
            new language()
            {
                description = name,
                path_to_templates = path
            });
            db.SaveChanges();

            this._updateLanguages();
        }

        private void _updateLanguage(DatabaseCon db, string name, string path)
        {
            this._window.updateStatus($"Updating existing language '{name}'");
            var oldName = this.dropDownLanguages.SelectedItem.ToString();

            var lang = db.languages.SingleOrDefault(l => l.description == oldName);
            if (lang == null)
                throw new Exception($"For some reason the language {oldName} no longer exists in the database.");

            lang.description = name;
            lang.path_to_templates = path;
            db.SaveChanges();

            this._updateLanguages();
        }
    }
}
