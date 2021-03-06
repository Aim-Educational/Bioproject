﻿using System;
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
using System.Reflection;
using System.Windows.Forms;
using System.IO;

using System.ComponentModel.DataAnnotations;

using CodeGenerator.Common;
using CodeGenerator.Database;

namespace CodeGenerator.Views
{
    // A lot of code reusage here (literally a copy of DeviceEditor.xaml)
    // TODO: See if I can put some of this stuff into DataHelper or something.

    /// <summary>
    /// Interaction logic for ApplicationEditor.xaml
    /// </summary>
    public partial class ApplicationEditor : System.Windows.Controls.UserControl
    {
        private MainWindow _window;
        private List<application> _applications;
        private int _maxPathLength;

        private const string PATH_MEMBER_NAME = "path_to_output_file"; // The name of the member inside the application class that contains the output path.
        private const string NEW_ITEM_TEXT = "[NEW APPLICATION]";

        public ApplicationEditor(MainWindow window)
        {
            InitializeComponent();
            this._window = window;
            
            this._updateApplications();
            this._findPathLength();
        }

        private void _updateApplications()
        {
            this.dropDownApplications.Items.Clear();
            this.panelApplications.Children.Clear();
            this.dropDownApplications.Items.Add(NEW_ITEM_TEXT);

            using (var db = new DatabaseCon())
                ViewHelper.populateDropDownAndPanelWithT<application, ApplicationInfoControl>(db, this.dropDownApplications, this.panelApplications, out this._applications);
        }

        private void _findPathLength()
        {
            // The 'path_to_output_file' member has a StringLength attribute that we can use to make sure the file path is a good size.
            // We probably *could* hardcode the max length, since it probably won't ever change, but this is more fun/stupid (but stupid is fun).
            // The only actual benefit of this method, is that it keeps the max length up to date with the database model, without having to manually do it ourselves.
            var members = typeof(application).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var lengthAttrib = (from mem in members
                                where mem.Name == PATH_MEMBER_NAME
                                select mem.GetCustomAttribute(typeof(StringLengthAttribute), true)).FirstOrDefault();

            var length = lengthAttrib as StringLengthAttribute;
            if (length == null)
                throw new Exception($"There is no member called '{PATH_MEMBER_NAME}' anymore, please update the variable's value.");

            this._maxPathLength = length.MaximumLength;
        }

        private void sliderBitIndex_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.labelBitIndex.Content = $"{this.sliderBitIndex.Value}";
        }

        private void buttonUpdateDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(this.dropDownApplications.SelectedItem == null)
                    throw new Exception("No application was selected!");

                var name = this.textboxName.Text;
                var bitIndex = (byte)this.sliderBitIndex.Value;
                var path = this.textboxPath.Text;

                if (string.IsNullOrEmpty(name))
                    throw new Exception("No name has been given");

                if(path.Length > this._maxPathLength)
                    throw new Exception($"The output path is too large, it has ${path.Length} characters while the limit is ${this._maxPathLength} characters");

                if(!Directory.Exists(path))
                    throw new Exception("The output path does not exist");

                using (var db = new DatabaseCon())
                {
                    if(this.dropDownApplications.SelectedItem.ToString() == NEW_ITEM_TEXT)
                    {
                        db.enforceNameIsUnique<application>(name);
                        db.enforceBitIndexIsUnique<application>(bitIndex);
                        this._addApplication(db, name, bitIndex, path);
                    }
                    else
                    {
                        var cached = this._applications.Single(a => a.description == this.dropDownApplications.SelectedItem.ToString());

                        if(name != cached.description)
                            db.enforceNameIsUnique<application>(name);

                        if(bitIndex != cached.bit_index)
                            db.enforceBitIndexIsUnique<application>(bitIndex);

                        this._updateApplication(db, name, bitIndex, path);
                    }
                }
            }
            catch(Exception ex)
            {
                this._window.updateStatus($"[ERROR] {ex.ToString()}");
            }
        }

        private void _addApplication(DatabaseCon db, string name, byte bitIndex, string path)
        {
            this._window.updateStatus($"Adding new application '{name}'");
            db.applications.Add(
            new application()
            {
                bit_index = bitIndex,
                description = name,
                path_to_output_file = path
            });
            db.SaveChanges();

            this._updateApplications();
        }

        private void _updateApplication(DatabaseCon db, string name, byte bitIndex, string path)
        {
            this._window.updateStatus($"Updating existing application '{name}'");
            var oldName = this.dropDownApplications.SelectedItem.ToString();

            var app = db.applications.SingleOrDefault(a => a.description == oldName);
            if(app == null)
                throw new Exception($"For some reason the application {oldName} no longer exists in the database.");

            app.description = name;
            app.path_to_output_file = path;
            app.bit_index = bitIndex;
            db.SaveChanges();

            this._updateApplications();
        }

        private void dropDownApplications_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count == 0)
                return;

            this._window.updateStatus("Updating UI to reflect chosen application");
            var appName = e.AddedItems[0].ToString();
            var app = this._applications.SingleOrDefault(a => a.description == appName);

            if(app != null)
            {
                this.textboxName.Text = appName;
                this.sliderBitIndex.Value = app.bit_index;
                this.textboxPath.Text = app.path_to_output_file;
            }
        }

        private void buttonDeleteApplication_Click(object sender, RoutedEventArgs e)
        {
            var item = this.dropDownApplications.SelectedItem;
            if(item == null || item.ToString() == NEW_ITEM_TEXT)
                return;

            var wasDeletion = ViewHelper.deleteTByDescription<application>(this._window, item.ToString());
            if(wasDeletion)
                this._updateApplications();
        }

        private void buttonSelectPath_Click(object sender, RoutedEventArgs e)
        {
            var path = ViewHelper.askForPath(this.textboxPath.Text);
            if (path != null)
                this.textboxPath.Text = path;
        }
    }
}
