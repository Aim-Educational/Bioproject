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
    public partial class FormDeviceTypeEditor : Form
    {
        private static device_type _defaultType = new device_type();

        public int id { private set; get; }
        private device_type _device_type { set; get; }

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
                        // Disable all controls except the Reload and Save button
                        // Make the 'delete' button visible, disable/hide the 'save' button.
                        // After deletion, close the form.
                        break;
                        
                    case EnumEditorMode.Create:
                        // The button's onClick event will now perform a create instead of a save.
                        // Change the button's text to 'create'
                        // After creating something successfully with the form, the mode changes to Modify.
                        // After creating, reload the form.
                        this.buttonAction.Text = "Create";
                        this.buttonReload.Visible = false;
                        this.textboxID.Text = "New";
                        break;

                    case EnumEditorMode.Modify:
                        // The save button's onClick event will do what it's currently does.
                        this.buttonAction.Text = "Save";
                        this.buttonReload.Visible = true;
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
                    foreach (var unit_type in db.units.OrderBy(u => u.description))
                        this.listTypes.Items.Add(unit_type.description);
                }

                if (this.mode != EnumEditorMode.Create)
                {
                    var dev = db.device_type.SingleOrDefault(d => d.device_type_id == this.id);
                    if (dev != null)
                    {
                        this.textboxID.Text = Convert.ToString(id);
                        this.textboxDescription.Text = dev.description;
                        this.textboxComment.Text = dev.comment;

                        foreach (var unit_type in db.units.OrderBy(u => u.description))
                        {
                            this.listTypes.Items.Add(unit_type.description);
                            if (unit_type.unit_id == dev.unit_id)
                                this.listTypes.SelectedIndex = this.listTypes.Items.Count - 1;
                        }

                        this._device_type = dev;
                        this._isDirty = false;
                    }
                }
            }
        }
        
        public FormDeviceTypeEditor(EnumEditorMode mode, int id = -1) // ID isn't always needed, e.g. Create
        {
            InitializeComponent();

            this.mode = mode;
            if (mode != EnumEditorMode.Create)
                this.id = id;
            else
                this._device_type = FormDeviceTypeEditor._defaultType;

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

        #region Modify/Create/Reload Events
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

            if (result == DialogResult.Yes)
                this.reload();
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
                var dev = db.device_type.SingleOrDefault(d => d.device_type_id == this.id);
                
                dev.description = this.textboxDescription.Text;
                dev.comment = this.textboxComment.Text;

                var selectedType = this.listTypes.Items[this.listTypes.SelectedIndex] as string;
                dev.unit = db.units.Single(u => u.description == selectedType);

                if (dev.isValidForUpdate(IncrementVersion.yes))
                {
                    db.SaveChanges();
                    this._device_type = dev;
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
                var dev = new device_type();
                dev.description = this.textboxDescription.Text;
                dev.comment = this.textboxComment.Text;

                var selectedType = this.listTypes.Items[this.listTypes.SelectedIndex] as string;
                dev.unit = db.units.Single(u => u.description == selectedType);

                db.device_type.Add(dev);
                db.SaveChanges();
                this._device_type = dev;
                this.id = dev.device_type_id;
                this._isDirty = false;
                this.mode = EnumEditorMode.Modify;

                this.reload();
            }
        }
        #endregion

        #region Input Control Events
        private void textboxDescription_Leave(object sender, EventArgs e)
        {
            if (textboxDescription.Text != this._device_type.description)
                this._isDirty = true;
        }

        private void listTypes_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listTypes.SelectedIndex;
            var value = this.listTypes.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || value != this._device_type.unit.description)
                this._isDirty = true;
        }

        private void textboxComment_Leave(object sender, EventArgs e)
        {
            if (textboxComment.Text != this._device_type.comment)
                this._isDirty = true;
        }
        #endregion
    }
}
