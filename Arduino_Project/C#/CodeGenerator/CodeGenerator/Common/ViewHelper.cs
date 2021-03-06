﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.IO;

using Microsoft.WindowsAPICodePack.Dialogs;

using CodeGenerator.Database;

namespace CodeGenerator.Common
{
    /// <summary>
    /// Contains helper functions/variables for views.
    /// </summary>
    public static class ViewHelper
    {
        /// <summary>
        /// The standard width for the 0th column of a view.
        /// 
        /// This exists so any user controls made for views, and all other views, can be designed in a consistent way.
        /// </summary>
        public static GridLength View_Column_0_Width
        {
            private set { }
            get { return new GridLength(88); }
        }

        /// <summary>
        /// The standard width for the 1st column of a view.
        /// </summary>
        public static GridLength View_Column_1_Width
        {
            private set {}
            get { return new GridLength(140, GridUnitType.Star); }
        }

        /// <summary>
        /// The standard width for the 2nd column of a view.
        /// </summary>
        public static GridLength View_Column_2_Width
        {
            private set { }
            get { return new GridLength(25); }
        }

        /// <summary>
        /// The standard height for a row of a view.
        /// 
        /// This exists so any user controls made for views, and all other views, can be designed in a consistent way.
        /// </summary>
        public static GridLength View_Row_Height
        {
            private set { }
            get { return new GridLength(25); }
        }

        /// <summary>
        /// Populates a drop down box (combobox) with the descriptions of `T` from the database.
        /// 
        /// For example, if `T` were `device_type`, then the drop down would contain all of the descriptions/names of 
        /// all the entries in the device_type table.
        /// 
        /// It is a requirement that `T` contains a `description` field.
        /// </summary>
        /// <typeparam name="T">The database type to use</typeparam>
        /// <param name="db">The database connection</param>
        /// <param name="dropDown">The dropdown to populate</param>
        /// <param name="list">This list will be filled with all of the records from the table</param>
        /// <param name="additionalProcessing">
        ///     This function will be called with each record of `T` from the database, in case any additional action is required.
        ///     
        ///     The records are passed in order of description.
        ///     
        ///     This alleviates the need to then create another foreach right after this function, just to do some additional action.
        /// </param>
        public static void populateDropDownWithT<T>(this DatabaseCon db, System.Windows.Controls.ComboBox dropDown, out List<T> list, Action<T> additionalProcessing = null) where T : class
        {
            if(db == null)
                throw new ArgumentNullException("db");

            if(dropDown == null)
                throw new ArgumentNullException("dropDown");

            list = db._getSetFromT<T>().ToList();
            foreach(var value in list.OrderBy(v => { dynamic dyV = v; return dyV.description; }))
            {
                dynamic dyValue = value;
                dropDown.Items.Add(dyValue.description);

                additionalProcessing?.Invoke(value);
            }
        }

        // The parameters are formatted like that because it's just way too long otherwise.
        /// <summary>
        /// Functions like `populateDropDownWithT` but also populates a `Panel` using `TInfoControl`.
        /// 
        /// Note: `TInfoControl` must have a constructor that takes `T` as it's only parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TInfoControl"></typeparam>
        /// <param name="panel"></param>
        public static void populateDropDownAndPanelWithT<T, TInfoControl>(this DatabaseCon                  db, 
                                                                          System.Windows.Controls.ComboBox  dropDown, 
                                                                          System.Windows.Controls.Panel     panel, 
                                                                          out List<T>                       list, 
                                                                          Action<T>                         additionalProcessing = null) where T : class
        {
            if(panel == null)
                throw new ArgumentNullException("panel");

            Action<T> action = 
            (tValue) =>
            {
                if(additionalProcessing != null)
                    additionalProcessing(tValue);

                var info = Activator.CreateInstance(typeof(TInfoControl), tValue) as System.Windows.Controls.UserControl;
                info.Margin = new Thickness(0,2,0,0);
                info.Width = double.NaN; // auto width

                panel.Children.Add(info);
            };
            db.populateDropDownWithT<T>(dropDown, out list, action);
        }

        // The reason this function is in ViewHelper, instead of DataHelper, is because of the user-interaction is requires, as well as the requirement of the MainWindow, making it specialised for use in views.
        /// <summary>
        /// Deletes a `T` from the database that has a description matching the given description.
        /// (note: in a lot of cases the description is just a name, hence 'name_description')
        /// 
        /// This function will first prompt the user with a message box, asking them to confirm the deletion of the object.
        /// 
        /// It is a requirement that `T` has a field called `description`.
        /// </summary>
        /// <typeparam name="T">The database type to remove (and also determines the database table that's used)</typeparam>
        /// <param name="window">The main window, so it's status can be updated</param>
        /// <param name="name_description">The name/description of the object to remove</param>
        /// <returns>True if something was delete.</returns>
        public static bool deleteTByDescription<T>(MainWindow window, string name_description) where T : class
        {
            if(window == null)
                throw new ArgumentNullException("window");

            if(name_description == null)
                throw new ArgumentNullException("name_description");

            var TName = typeof(T).Name;
            var result = System.Windows.Forms.MessageBox.Show($"Are you sure you want to remove the {TName} '{name_description}'?",
                                                              "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                window.updateStatus($"Not going through with removal of the {TName}");
                return false;
            }

            using (var db = new DatabaseCon())
            {
                var set = db._getSetFromT<T>();
                T value = null;

                // I need to use dynamic, so can't use SingleOrDefault since it doesn't like lambdas apparently
                foreach(var val in set)
                {
                    dynamic dyVal = val;
                    if(dyVal.description == name_description)
                    {
                        value = val;
                        break;
                    }
                }

                if (value == null)
                {
                    window.updateStatus($"Can't delete a(n) {TName} that doesn't exist");
                    return false;
                }

                window.updateStatus($"Removing {TName} '{name_description}' from the database");
                set.Remove(value);
                db.SaveChanges();
            }

            return true;
        }

        public static string askForPath(string defaultPath = null)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.DefaultFileName = "";
                dialog.IsFolderPicker = true;
                dialog.EnsurePathExists = true;

                if (Directory.Exists(defaultPath))
                    dialog.InitialDirectory = defaultPath;

                var result = dialog.ShowDialog();

                if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(dialog.FileName))
                    return dialog.FileName;
                else if (result != CommonFileDialogResult.Cancel)
                    System.Windows.Forms.MessageBox.Show("An invalid path was chosen", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }
    }
}
