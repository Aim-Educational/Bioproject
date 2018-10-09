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
    /// Interaction logic for DeviceEditor.xaml
    /// </summary>
    public partial class DeviceEditor : UserControl
    {
        private MainWindow _window;
        private List<device_type> _devices;

        private const string NEW_ITEM_TEXT = "[NEW DEVICE]";

        public DeviceEditor(MainWindow window)
        {
            InitializeComponent();
            this._window = window;
            
            this._updateDevices();
        }

        private void _updateDevices()
        {
            this.dropDownDevices.Items.Clear();
            this.panelDevices.Children.Clear();
            this.dropDownDevices.Items.Add(NEW_ITEM_TEXT);

            using (var db = new DatabaseCon())
                ViewHelper.populateDropDownAndPanelWithT<device_type, DeviceInfoControl>(db, this.dropDownDevices, this.panelDevices, out this._devices);
        }

        private void sliderBitIndex_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.labelBitIndex.Content = $"{this.sliderBitIndex.Value}";
        }

        private void buttonUpdateDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(this.dropDownDevices.SelectedItem == null)
                    throw new Exception("No device was selected!");

                var name = this.textboxName.Text;
                var bitIndex = (byte)this.sliderBitIndex.Value;

                if (string.IsNullOrEmpty(name))
                    throw new Exception("No name has been given");

                using (var db = new DatabaseCon())
                {
                    if(this.dropDownDevices.SelectedItem.ToString() == NEW_ITEM_TEXT)
                    {
                        db.enforceNameIsUnique<device_type>(name);
                        db.enforceBitIndexIsUnique<device_type>(bitIndex);
                        this._addDevice(db, name, bitIndex);
                    }
                    else
                    {
                        var cached = this._devices.Single(d => d.description == this.dropDownDevices.SelectedItem.ToString());

                        if(name != cached.description)
                            db.enforceNameIsUnique<device_type>(name);

                        if(bitIndex != cached.bit_index)
                            db.enforceBitIndexIsUnique<device_type>(bitIndex);

                        this._updateDevice(db, name, bitIndex);
                    }
                }
            }
            catch(Exception ex)
            {
                this._window.updateStatus($"[ERROR] {ex.ToString()}");
            }
        }

        private void _addDevice(DatabaseCon db, string name, byte bitIndex)
        {
            this._window.updateStatus($"Adding new device '{name}'");
            db.device_type.Add(
            new device_type()
            {
                bit_index = bitIndex,
                description = name
            });
            db.SaveChanges();

            this._updateDevices();
        }

        private void _updateDevice(DatabaseCon db, string name, byte bitIndex)
        {
            this._window.updateStatus($"Updating existing device '{name}'");
            var oldName = this.dropDownDevices.SelectedItem.ToString();

            var device = db.device_type.SingleOrDefault(d => d.description == oldName);
            if(device == null)
                throw new Exception($"For some reason the device {oldName} no longer exists in the database.");

            device.description = name;
            device.bit_index = bitIndex;
            db.SaveChanges();

            this._updateDevices();
        }

        private void dropDownApplications_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count == 0)
                return;

            this._window.updateStatus("Updating UI to reflect chosen device");
            var deviceName = e.AddedItems[0].ToString();
            var device = this._devices.SingleOrDefault(d => d.description == deviceName);

            if(device != null)
            {
                this.textboxName.Text = deviceName;
                this.sliderBitIndex.Value = device.bit_index;
            }
        }

        private void buttonDeleteDevice_Click(object sender, RoutedEventArgs e)
        {
            var item = this.dropDownDevices.SelectedItem;
            if (item == null || item.ToString() == NEW_ITEM_TEXT)
                return;

            var wasDeletion = ViewHelper.deleteTByDescription<device_type>(this._window, item.ToString());
            if(wasDeletion)
                this._updateDevices();
        }
    }
}
