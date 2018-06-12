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
    public partial class FormActionLevelEditor : Form
    {
        private device _device { set; get; }
        public int id { private set; get; }

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
                buttonSave.Enabled = value;
                this._isDirty_value = value;
            }
        }

        public EnumEditorMode mode { set; get; }

        private void reload()
        {
            if (this.mode == EnumEditorMode.Create)
                return;

            using (var db = new PlanningContext())
            {
                var dev = db.devices.SingleOrDefault(d => d.device_id == id);
                if (dev != null)
                {
                    this.textboxID.Text = Convert.ToString(id);
                    this.textboxName.Text = dev.name;
                    this.textboxDescription.Text = dev.description;
                    this.textboxLocation.Text = dev.location;
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

        public FormActionLevelEditor(int id)
        {
            InitializeComponent();

            // TODO: Maybe make a helper function to do this for us
            this.numericMin.Minimum = decimal.MinValue;
            this.numericMin.Maximum = decimal.MaxValue;
            this.numericMax.Minimum = decimal.MinValue;
            this.numericMax.Maximum = decimal.MaxValue;

            this.id = id;
            this.reload();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if(this.mode == EnumEditorMode.Delete)
            {
                MessageBox.Show("Cannot update the database in delete mode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!this._isDirty)
                return;

            using (var db = new PlanningContext())
            {
                var dev = db.devices.SingleOrDefault(d => d.device_id == id);

                dev.name        = this.textboxName.Text;
                dev.description = this.textboxDescription.Text;
                dev.location    = this.textboxLocation.Text;
                dev.min_value   = (double)this.numericMin.Value;
                dev.max_value   = (double)this.numericMax.Value;

                var selectedType = this.listTypes.Items[this.listTypes.SelectedIndex] as string;
                dev.device_type  = db.device_type.Single(t => t.description == selectedType);

                // I want to be able to control when we make it save, so it's easier to test quickly
                MessageBox.Show("Press Ok to save");

                var before = DateTime.Now;
                for (int i = 0; i < 500; i++)
                    dev.isValidForUpdate(IncrementVersion.no);
                var duration = DateTime.Now - before;
                MessageBox.Show($"It took {duration.TotalMilliseconds} ms to complete.");

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

            if (value != this._device.device_type.description)
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
            DialogResult result = DialogResult.No;
            if (this._isDirty)
            {
                result = MessageBox.Show("Reloading the form will cause your changes to be lost, continue?", "Confirmation", 
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }

            if(result == DialogResult.Yes)
                this.reload();
        }
    }
}
