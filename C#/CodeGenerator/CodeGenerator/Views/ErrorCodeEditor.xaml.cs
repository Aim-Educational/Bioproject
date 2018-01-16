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
using System.Windows.Forms;

using CodeGenerator.Common;
using CodeGenerator.Database;

namespace CodeGenerator.Views
{
    /// <summary>
    /// Interaction logic for ErrorCodeEditor.xaml
    /// </summary>
    public partial class ErrorCodeEditor : System.Windows.Controls.UserControl
    {
        // Maybe I should make a global cache for these...
        private List<error_code> _errors;
        private List<application> _applications;
        private List<device_type> _devices;
        private List<severity> _severity;
        private MainWindow _window;

        private const string NEW_ITEM_TEXT = "[NEW ERROR]";

        public ErrorCodeEditor(MainWindow window)
        {
            InitializeComponent();
            this._window = window;
            this._errors = new List<error_code>();

            this._updateDropdowns();
        }

        private void _updateDropdowns()
        {
            this.dropDownErrors.Items.Clear();
            this.dropDownApplications.Items.Clear();
            this.dropDownDevices.Items.Clear();
            this.dropDownSeverity.Items.Clear();

            this.panelApplications.Children.Clear();
            this.panelDevices.Children.Clear();
            this.panelErrors.Children.Clear();

            // populteDropDownWithT will clear the other lists for us.
            this._errors.Clear();

            this.dropDownErrors.Items.Add(NEW_ITEM_TEXT);

            using (var db = new DatabaseCon())
            {
                foreach(var error in db.error_code.OrderBy(e => e.error_code_mneumonic))
                {
                    this.panelErrors.Children.Add(
                    new ErrorInfoControl(db, error){
                        Margin = new Thickness(0, 2, 0, 0),
                        Width = double.NaN
                    });
                    this.dropDownErrors.Items.Add(error.error_code_mneumonic);
                    this._errors.Add(error);
                }

                ViewHelper.populateDropDownWithT<application>(db, this.dropDownApplications, out this._applications);
                ViewHelper.populateDropDownWithT<device_type>(db, this.dropDownDevices, out this._devices);
                ViewHelper.populateDropDownWithT<severity>(db, this.dropDownSeverity, out this._severity);
            }
        }

        // Makes it so only numbers can be entered into the textbox.
        private void textboxCode_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // false = don't allow text
            e.Handled = !e.Text.All(c => char.IsDigit(c));
        }

        private void dropDownErrors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            this._window.updateStatus("Updating UI to reflect chosen error");
            var errorName = e.AddedItems[0].ToString();
            var error = this._errors.SingleOrDefault(er => er.error_code_mneumonic == errorName);

            if (error != null)
            {
                this.textboxMneumonic.Text = error.error_code_mneumonic;
                this.textboxCode.Text = error.error_code1;
                this.textboxNarrative.Text = error.narrative;
                
                for(int i = 0; i < this.dropDownSeverity.Items.Count; i++)
                {
                    if(this.dropDownSeverity.Items[i].ToString() == error.severity.description)
                    {
                        this.dropDownSeverity.SelectedIndex = i;
                        break;
                    }
                }
                
                // DataHelper has a function for this, but that requires connecting back to the database.
                // And I'd rather not do that for this function, since having it connect to the database just for changing -
                // - your selection seems a bit odd.
                var apps = this._getFromBitmask<application>(this._applications, error.application_ids);
                var devices = this._getFromBitmask<device_type>(this._devices, error.device_ids);

                this.panelApplications.Children.Clear();
                this.panelDevices.Children.Clear();

                // SHould probably make a function for this...
                foreach(var app in apps)
                {
                    this.panelApplications.Children.Add(new ApplicationInfoControl(app)
                    {
                        Margin = new Thickness(0,2,0,0),
                        Width = double.NaN
                    });
                }

                foreach(var device in devices)
                {
                    this.panelDevices.Children.Add(new DeviceInfoControl(device)
                    {
                        Margin = new Thickness(0,2,0,0),
                        Width = double.NaN
                    });
                }
            }
        }

        private List<T> _getFromBitmask<T>(List<T> list, byte bitmask)
        {
            var output = new List<T>();
            foreach(var value in list)
            {
                dynamic dyValue = value;
                if((bitmask & (1 << dyValue.bit_index)) > 0)
                {
                    output.Add(value);
                }
            }

            return output;
        }

        private void buttonSetAppOrDevice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // TODO: A function to reduce code reusage....
                if(sender == this.buttonSetApplication)
                {
                    var appName = this.dropDownApplications.SelectedItem.ToString();
                    if(appName == null)
                        return;

                    foreach(var child in this.panelApplications.Children)
                    {
                        if((child as ApplicationInfoControl).labelName.Content.ToString() == appName)
                            throw new Exception($"The application '{appName}' has already been added");
                    }

                    var app = this._applications.Where(a => a.description == appName).First();
                    this.panelApplications.Children.Add(new ApplicationInfoControl(app)
                    {
                        Margin = new Thickness(0,2,0,0),
                        Width = double.NaN
                    });
                }
                else if(sender == this.buttonSetDevice)
                {
                    var devName = this.dropDownDevices.SelectedItem.ToString();
                    if (devName == null)
                        return;

                    foreach (var child in this.panelDevices.Children)
                    {
                        if ((child as DeviceInfoControl).labelName.Content.ToString() == devName)
                            throw new Exception($"The device '{devName}' has already been added");
                    }

                    var dev = this._devices.Where(d => d.description == devName).First();
                    this.panelDevices.Children.Add(new DeviceInfoControl(dev)
                    {
                        Margin = new Thickness(0, 2, 0, 0),
                        Width = double.NaN
                    });
                }
            }
            catch(Exception ex)
            {
                this._window.updateStatus($"[ERROR] {ex.ToString()}");
            }
        }

        private void buttonUnsetAppOrDevice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(sender == this.buttonUnsetApplication)
                    this._removeAppOrDevice<application>(this.dropDownApplications, this.panelApplications);
                else if(sender == this.buttonUnsetDevice)
                    this._removeAppOrDevice<device_type>(this.dropDownDevices, this.panelDevices);
            }
            catch (Exception ex)
            {
                this._window.updateStatus($"[ERROR] {ex.ToString()}");
            }
        }

        private void _removeAppOrDevice<T>(System.Windows.Controls.ComboBox dropdown, System.Windows.Controls.Panel panel) where T : class
        {
            var name = dropdown.SelectedItem?.ToString();
            if(name == null)
                throw new Exception($"No {typeof(T).Name} was selected");

            foreach(var child in panel.Children)
            {
                // As long as both ApplicationInfo and DEviceInfo have a label called labelName, then this will work.
                // But when that changes, this will need a rewrite...
                dynamic dyChild = child;
                if(dyChild.labelName.Content.ToString() == name)
                {
                    this._window.updateStatus($"Removing the {typeof(T).Name} called '{name}' from the error");
                    panel.Children.Remove(child as System.Windows.Controls.UserControl);
                    return;
                }
            }

            this._window.updateStatus($"No {typeof(T).Name} called '{name}' is being used for this error. No removal is taking place.");
        }

        private void buttonDeleteError_Click(object sender, RoutedEventArgs e)
        {
            // Again, since error_code doesn't use 'description' but rather 'error_code_mneumonic' I can't use ViewHelper.deleteTByDescription for this.
            // So I've pretty much just copied to code into here and tweaked it a bit.
            var window = this._window;
            var name_description = this.dropDownErrors.SelectedItem?.ToString();

            if (window == null)
                throw new ArgumentNullException("window");

            if (name_description == null)
                throw new ArgumentNullException("name_description");

            var TName = "error_code";
            var result = System.Windows.Forms.MessageBox.Show($"Are you sure you want to remove the {TName} '{name_description}'?",
                                                              "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                window.updateStatus($"Not going through with removal of the {TName}");
                return;
            }

            using (var db = new DatabaseCon())
            {
                var set = db.error_code;
                var value = db.error_code.SingleOrDefault(v => v.error_code_mneumonic == name_description);

                if (value == null)
                {
                    window.updateStatus($"Can't delete an {TName} that doesn't exist");
                    return;
                }

                window.updateStatus($"Removing {TName} '{name_description}' from the database");
                set.Remove(value);
                db.SaveChanges();
                this._updateDropdowns();
            }
        }

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedErrorName = this.dropDownErrors.SelectedItem?.ToString();
                if (selectedErrorName == null)
                    throw new Exception("No error was selected");

                using (var db = new DatabaseCon())
                {
                    var error = this._createObjectFromData(db);
                    if(selectedErrorName == NEW_ITEM_TEXT)
                    {
                        this._window.updateStatus($"Adding new error called '{error.error_code_mneumonic}' to the database");
                        db.error_code.Add(error);
                        db.SaveChanges();
                    }
                    else
                    {
                        this._window.updateStatus($"Updating error called '{selectedErrorName}'");

                        var dbError = db.error_code.SingleOrDefault(er => er.error_code_mneumonic == selectedErrorName);                        
                        dbError.application_ids = error.application_ids;
                        dbError.default_severity_id = error.default_severity_id;
                        dbError.device_ids = error.device_ids;
                        dbError.error_code1 = error.error_code1;
                        dbError.error_code_mneumonic = error.error_code_mneumonic;
                        dbError.narrative = error.narrative;
                        db.SaveChanges();
                    }
                }

                this._updateDropdowns();
            }
            catch (Exception ex)
            {
                this._window.updateStatus($"[ERROR] {ex.ToString()}");
            }
        }

        private error_code _createObjectFromData(DatabaseCon db)
        {
            // Fun fact: If we don't get the data from the database, and use the cache instead (for the severity) then it randomly duplicates entries.
            var error = new error_code();
            error.error_code_mneumonic = this.textboxMneumonic.Text;
            error.error_code1 = this.textboxCode.Text;
            error.narrative = this.textboxNarrative.Text;

            var severityName = this.dropDownSeverity.SelectedItem?.ToString();
            if(severityName == null)
                throw new Exception("No default severity was chosen");

            var defaultSeverity = db.severities.SingleOrDefault(s => s.description == severityName);
            if(defaultSeverity == null)
                throw new Exception("Bug");

            error.default_severity_id = defaultSeverity.severity_id;
            error.device_ids = this._createBitmask(this.panelDevices);
            error.application_ids = this._createBitmask(this.panelApplications);

            return error;
        }

        private byte _createBitmask(System.Windows.Controls.Panel panel)
        {
            byte mask = 0;
            foreach(var child in panel.Children)
            {
                dynamic dyChild = child;
                mask |= 1 << Convert.ToByte(dyChild.labelIndex.Content.ToString()); // Will only work as long as ApplicationInfo and DeviceInfo have this field.
            }

            return mask;
        }
    }
}
