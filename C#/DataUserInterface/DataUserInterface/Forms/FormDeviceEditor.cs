using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DataManager.Model;

namespace DataUserInterface.Forms
{
    public partial class FormDeviceEditor : Form
    {
        private static device _defaultDevice = new device();

        public int id { private set; get; }
        private device _device { set; get; }

        private bool _isDirty_value;
        private bool _isDirty
        {
            get
            {
                return this._isDirty_value;
            }

            set
            {
                labelDirty.Visible = value;
                buttonAction.Enabled = value;
                this._isDirty_value = value;
            }
        }

        private EnumEditorMode _mode_value;
        public EnumEditorMode mode
        {
            set
            {
                this._mode_value = value;
                switch(value)
                {
                    case EnumEditorMode.Delete:
                        // Disable all controls except the Reload
                        // Make the 'delete' button visible, disable/hide the 'save' button.
                        // After deletion, close the form.
                        foreach (var control in this.splitContainer1.Panel2.Controls)
                            FormHelper.disableControl(control);

                        this.buttonReload.Enabled = true;
                        this.buttonDelete.Enabled = true;
                        this.buttonDelete.Visible = true;
                        this.buttonAction.Visible = false;
                        break;
                        
                    case EnumEditorMode.Create:
                        // The button's onClick event will now perform a create instead of a save.
                        // Change the button's text to 'create'
                        // After creating something successfully with the form, the mode changes to Modify.
                        // After creating, reload the form.
                        this.buttonAction.Text = "Create";
                        this.buttonReload.Visible = false;
                        this.buttonDelete.Visible = false;
                        this.textboxID.Text = "New";
                        break;

                    case EnumEditorMode.Modify:
                        // The save button's onClick event will do what it's currently does.
                        this.buttonAction.Text = "Save";
                        this.buttonReload.Visible = true;
                        this.buttonDelete.Visible = false;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            get
            {
                return this._mode_value;
            }
        }

        private void reload()
        {
            using (var db = new PlanningContext())
            {
                if (this.mode == EnumEditorMode.Create)
                {
                    foreach (var dev_type in db.device_type.OrderBy(d => d.description))
                        this.listTypes.Items.Add(dev_type.description);
                }

                if (this.mode != EnumEditorMode.Create)
                {
                    var dev = db.devices.SingleOrDefault(d => d.device_id == id);
                    if (dev != null)
                    {
                        this.textboxID.Text = Convert.ToString(id);
                        this.textboxName.Text = dev.name;
                        this.textboxDescription.Text = dev.description;
                        this.textboxLocation.Text = dev.location;
                        this.textboxComment.Text = dev.comment;
                        this.textboxSerial.Text = dev.serial_number;
                        this.numericMin.Value = (decimal)dev.min_value;
                        this.numericMax.Value = (decimal)dev.max_value;

                        foreach (var dev_type in db.device_type.OrderBy(d => d.description))
                        {
                            this.listTypes.Items.Add(dev_type.description);
                            if (dev_type.device_type_id == dev.device_type_id)
                                this.listTypes.SelectedIndex = this.listTypes.Items.Count - 1;
                        }

                        this._device = dev;
                        this._isDirty = false;
                    }
                }
            }
        }
        
        public FormDeviceEditor(EnumEditorMode mode, int id = -1) // ID isn't always needed, e.g. Create
        {
            InitializeComponent();

            FormHelper.unlimitNumericBox(this.numericMin);
            FormHelper.unlimitNumericBox(this.numericMax);

            // temp
            if (mode == EnumEditorMode.Modify)
            {
                using (var db = new PlanningContext())
                {
                    var type = (from dt in db.devices
                                where dt.device_id == id
                                select dt).Single();

                    MessageBox.Show($"{type.isDeletable(db)}");
                }
            }

            this.mode = mode;
            if (mode != EnumEditorMode.Create)
                this.id = id;
            else
                this._device = FormDeviceEditor._defaultDevice;

            this.reload();
        }

        private void FormDeviceEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._isDirty)
            {
                var result = MessageBox.Show("Closing this form will cause you to lose all of your changes. Are you sure?", "Attention",
                                             MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                e.Cancel = (result != DialogResult.Yes);
            }
        }

        #region Modify/Create/Delete Events
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (this.mode != EnumEditorMode.Delete)
                return;

            using (var db = new PlanningContext())
            {
                var dev = db.devices.SingleOrDefault(d => d.device_id == this.id);

                if (!dev.isDeletable(db))
                {
                    MessageBox.Show("Cannot delete this record as other records are still linked to it.", 
                                    "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this record?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    return;

                db.devices.Remove(dev);
                db.SaveChanges();
                this.Close();
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            switch(this.mode)
            {
                case EnumEditorMode.Modify:
                    this.modifyOnClick();
                    break;

                case EnumEditorMode.Create:
                    this.createOnClick();
                    break;

                case EnumEditorMode.Delete:
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void modifyOnClick()
        {
            if (!this._isDirty)
                return;

            using (var db = new PlanningContext())
            {
                var dev = db.devices.SingleOrDefault(d => d.device_id == id);
                
                dev.name = this.textboxName.Text;
                dev.description = this.textboxDescription.Text;
                dev.location = this.textboxLocation.Text;
                dev.comment = this.textboxComment.Text;
                dev.serial_number = this.textboxSerial.Text;
                dev.min_value = (double)this.numericMin.Value;
                dev.max_value = (double)this.numericMax.Value;

                var selectedType = this.listTypes.Items[this.listTypes.SelectedIndex] as string;
                dev.device_type = db.device_type.Single(t => t.description == selectedType);

                if (dev.isValidForUpdate(IncrementVersion.yes))
                {
                    db.SaveChanges();
                    this._device = dev;
                    this._isDirty = false;
                }
                else
                    MessageBox.Show("Someone edited this value before you did :).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void createOnClick()
        {
            if (!this._isDirty)
                return;

            // TODO: Stop the user from creating the object if all the fields aren't filled out.
            using (var db = new PlanningContext())
            {
                var dev = new device();
                dev.name = this.textboxName.Text;
                dev.description = this.textboxDescription.Text;
                dev.location = this.textboxLocation.Text;
                dev.comment = this.textboxComment.Text;
                dev.serial_number = this.textboxSerial.Text;
                dev.min_value = (double)this.numericMin.Value;
                dev.max_value = (double)this.numericMax.Value;

                var selectedType = this.listTypes.Items[this.listTypes.SelectedIndex] as string;
                dev.device_type = db.device_type.Single(t => t.description == selectedType);

                db.devices.Add(dev);
                db.SaveChanges();
                this._device = dev;
                this.id = dev.device_id;
                this._isDirty = false;
                this.mode = EnumEditorMode.Modify;

                this.reload();
            }
        }
        #endregion

        #region Input Control Events
        private void textboxName_Leave(object sender, EventArgs e)
        {
            if (textboxName.Text != this._device.name)
                this._isDirty = true;
        }

        private void textboxDescription_Leave(object sender, EventArgs e)
        {
            if (textboxDescription.Text != this._device.description)
                this._isDirty = true;
        }

        private void listTypes_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listTypes.SelectedIndex;
            var value = this.listTypes.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || value != this._device.device_type.description)
                this._isDirty = true;
        }

        private void textboxLocation_Leave(object sender, EventArgs e)
        {
            if (textboxLocation.Text != this._device.location)
                this._isDirty = true;
        }

        private void numericMin_ValueChanged(object sender, EventArgs e)
        {
            if (this._device == null)
                return;

            if (Convert.ToDouble(numericMin.Value) != this._device.min_value)
                this._isDirty = true;
        }

        private void numericMax_ValueChanged(object sender, EventArgs e)
        {
            if (this._device == null)
                return;

            if (Convert.ToDouble(numericMax.Value) != this._device.max_value)
                this._isDirty = true;
        }

        private void buttonReload_Click(object sender, EventArgs e)
        {
            if (this.mode == EnumEditorMode.Create)
                return;

            DialogResult result = DialogResult.No;
            if (this._isDirty)
            {
                result = MessageBox.Show("Reloading the form will cause your changes to be lost, continue?", "Confirmation", 
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }

            if(result == DialogResult.Yes)
                this.reload();
        }

        private void textboxSerial_Leave(object sender, EventArgs e)
        {
            if (textboxSerial.Text != this._device.serial_number)
                this._isDirty = true;
        }

        private void textboxComment_Leave(object sender, EventArgs e)
        {
            if (textboxComment.Text != this._device.comment)
                this._isDirty = true;
        }

        private void numericMin_Enter(object sender, EventArgs e)
        {
            FormHelper.selectAllText(this.numericMin);
        }

        private void numericMax_Enter(object sender, EventArgs e)
        {
            FormHelper.selectAllText(this.numericMax);
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            var form = new SearchForm(EnumSearchFormType.DeviceType);
            form.MdiParent = this.MdiParent;
            form.Show();
        }
    }
}
