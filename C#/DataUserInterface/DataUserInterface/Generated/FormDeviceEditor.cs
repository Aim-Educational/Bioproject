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
        private static device _defaultObject = new device();

        public int id { private set; get; }
        private device _cached { set; get; }

        private bool _isDirty_value;
        private bool _isDirty
        {
            get
            {
                return this._isDirty_value;
            }

            set
            {
                labelDirty.Visible   = value;
                buttonAction.Enabled = value;
                this._isDirty_value  = value;
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
                    foreach (var val in db.devices.OrderBy(v => v.name))
    this.listDevice2.Items.Add(val.name);
    foreach (var val in db.device_type.OrderBy(v => v.description))
    this.listDeviceType.Items.Add(val.description);
    
                }

                if (this.mode != EnumEditorMode.Create)
                {
                    var obj = db.devices.SingleOrDefault(v => v.device_id == this.id);
                    if (obj != null)
                    {
                        this.textboxDeviceId.Text = Convert.ToString(obj.device_id);
foreach (var value in db.devices.OrderBy(v => v.name))
{
    this.listDevice2.Items.Add(value.name);
    if (value.device_id == obj.parent_device_id)
        this.listDevice2.SelectedIndex = this.listDevice2.Items.Count - 1;
}
foreach (var value in db.device_type.OrderBy(v => v.description))
{
    this.listDeviceType.Items.Add(value.description);
    if (value.device_type_id == obj.device_type_id)
        this.listDeviceType.SelectedIndex = this.listDeviceType.Items.Count - 1;
}
this.textboxName.Text = obj.name;
this.textboxDescription.Text = obj.description;
this.textboxLocation.Text = obj.location;
this.numericMinValue.Value = (decimal)obj.min_value;
this.numericMaxValue.Value = (decimal)obj.max_value;
this.numericAccuracy.Value = (decimal)obj.accuracy;
this.textboxSerialNumber.Text = obj.serial_number;
this.numericCost.Value = (decimal)obj.cost;
this.numericReliability.Value = (decimal)obj.reliability;
this.numericStrikes.Value = (decimal)obj.strikes;
this.textboxComment.Text = obj.comment;
this.checkboxIsAllowedForUse.Checked = obj.is_allowed_for_use;

                        this._cached  = obj;
                        this._isDirty = false;
                    }
                }
            }
        }
        
        public FormDeviceEditor(EnumEditorMode mode, int id = -1) // ID isn't always needed, e.g. Create
        {
            InitializeComponent();

            FormHelper.unlimitNumericBox(this.numericMinValue, AllowDecimals.yes);
FormHelper.unlimitNumericBox(this.numericMaxValue, AllowDecimals.yes);
FormHelper.unlimitNumericBox(this.numericAccuracy, AllowDecimals.yes);
FormHelper.unlimitNumericBox(this.numericCost, AllowDecimals.yes);
FormHelper.unlimitNumericBox(this.numericReliability, AllowDecimals.no);
FormHelper.unlimitNumericBox(this.numericStrikes, AllowDecimals.no);


            this.mode = mode;
            if (mode != EnumEditorMode.Create)
                this.id = id;
            else
                this._cached = FormDeviceEditor._defaultObject;

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
                var obj = db.devices.SingleOrDefault(v => v.device_id == this.id);

                if (!obj.isDeletable(db))
                {
                    MessageBox.Show("Cannot delete this record as other records are still linked to it.", 
                                    "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this record?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    return;

                db.devices.Remove(obj);
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
                var obj = db.devices.SingleOrDefault(v => v.device_id == this.id);

                
var selectedDevice2 = this.listDevice2.Items[this.listDevice2.SelectedIndex] as string;
obj.device2 = db.devices.Single(v => v.name == selectedDevice2);
var selectedDeviceType = this.listDeviceType.Items[this.listDeviceType.SelectedIndex] as string;
obj.device_type = db.device_type.Single(v => v.description == selectedDeviceType);
obj.name = this.textboxName.Text;
obj.description = this.textboxDescription.Text;
obj.location = this.textboxLocation.Text;
obj.min_value = (double)this.numericMinValue.Value;
obj.max_value = (double)this.numericMaxValue.Value;
obj.accuracy = (double)this.numericAccuracy.Value;
obj.serial_number = this.textboxSerialNumber.Text;
obj.cost = (decimal)this.numericCost.Value;
obj.reliability = (int)this.numericReliability.Value;
obj.strikes = (int)this.numericStrikes.Value;
obj.comment = this.textboxComment.Text;
obj.is_allowed_for_use = this.checkboxIsAllowedForUse.Checked;

                if (obj.isValidForUpdate(IncrementVersion.yes))
                {
                    db.SaveChanges();
                    this._cached  = obj;
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

            using (var db = new PlanningContext())
            {
                var obj = new device();

                
var selectedDevice2 = this.listDevice2.Items[this.listDevice2.SelectedIndex] as string;
obj.device2 = db.devices.Single(v => v.name == selectedDevice2);
var selectedDeviceType = this.listDeviceType.Items[this.listDeviceType.SelectedIndex] as string;
obj.device_type = db.device_type.Single(v => v.description == selectedDeviceType);
obj.name = this.textboxName.Text;
obj.description = this.textboxDescription.Text;
obj.location = this.textboxLocation.Text;
obj.min_value = (double)this.numericMinValue.Value;
obj.max_value = (double)this.numericMaxValue.Value;
obj.accuracy = (double)this.numericAccuracy.Value;
obj.serial_number = this.textboxSerialNumber.Text;
obj.cost = (decimal)this.numericCost.Value;
obj.reliability = (int)this.numericReliability.Value;
obj.strikes = (int)this.numericStrikes.Value;
obj.comment = this.textboxComment.Text;
obj.is_allowed_for_use = this.checkboxIsAllowedForUse.Checked;

                db.devices.Add(obj);
                db.SaveChanges();
                this._cached  = obj;
                this.id       = obj.device_id;
                this._isDirty = false;
                this.mode     = EnumEditorMode.Modify;
                this.reload();
            }
        }
        #endregion

        #region Input Control Events
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

                private void textboxDeviceId_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxDeviceId.Text != Convert.ToString(this._cached.device_id))
                this._isDirty = true;
        }
                private void listDevice2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listDevice2.SelectedIndex;
            var value = this.listDevice2.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || (this._cached.device2 != null && value != this._cached.device2.name))
                this._isDirty = true;
        }
        private void buttonShowDevice2_Click(object sender, EventArgs e)
{
    var form = new SearchForm(EnumSearchFormType.Device);
    form.MdiParent = this.MdiParent;
    form.Show();
}
        private void listDeviceType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listDeviceType.SelectedIndex;
            var value = this.listDeviceType.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || (this._cached.device_type != null && value != this._cached.device_type.description))
                this._isDirty = true;
        }
        private void buttonShowDeviceType_Click(object sender, EventArgs e)
{
    var form = new SearchForm(EnumSearchFormType.DeviceType);
    form.MdiParent = this.MdiParent;
    form.Show();
}
        private void textboxName_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxName.Text != Convert.ToString(this._cached.name))
                this._isDirty = true;
        }
                private void textboxDescription_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxDescription.Text != Convert.ToString(this._cached.description))
                this._isDirty = true;
        }
                private void textboxLocation_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxLocation.Text != Convert.ToString(this._cached.location))
                this._isDirty = true;
        }
                private void numericMinValue_Enter(object sender, EventArgs e)
        {
            FormHelper.selectAllText(this.numericMinValue);
        }
        private void numericMinValue_ValueChanged(object sender, EventArgs e)
        {
            if(this._cached == null)
                return;

            if (Convert.ToDouble(this.numericMinValue.Value) != this._cached.min_value)
                this._isDirty = true;
        }
        private void numericMaxValue_Enter(object sender, EventArgs e)
        {
            FormHelper.selectAllText(this.numericMaxValue);
        }
        private void numericMaxValue_ValueChanged(object sender, EventArgs e)
        {
            if(this._cached == null)
                return;

            if (Convert.ToDouble(this.numericMaxValue.Value) != this._cached.max_value)
                this._isDirty = true;
        }
        private void numericAccuracy_Enter(object sender, EventArgs e)
        {
            FormHelper.selectAllText(this.numericAccuracy);
        }
        private void numericAccuracy_ValueChanged(object sender, EventArgs e)
        {
            if(this._cached == null)
                return;

            if (Convert.ToDouble(this.numericAccuracy.Value) != this._cached.accuracy)
                this._isDirty = true;
        }
        private void textboxSerialNumber_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxSerialNumber.Text != Convert.ToString(this._cached.serial_number))
                this._isDirty = true;
        }
                private void numericCost_Enter(object sender, EventArgs e)
        {
            FormHelper.selectAllText(this.numericCost);
        }
        private void numericCost_ValueChanged(object sender, EventArgs e)
        {
            if(this._cached == null)
                return;

            if (Convert.ToDouble(this.numericCost.Value) != this._cached.cost)
                this._isDirty = true;
        }
        private void numericReliability_Enter(object sender, EventArgs e)
        {
            FormHelper.selectAllText(this.numericReliability);
        }
        private void numericReliability_ValueChanged(object sender, EventArgs e)
        {
            if(this._cached == null)
                return;

            if (Convert.ToDouble(this.numericReliability.Value) != this._cached.reliability)
                this._isDirty = true;
        }
        private void numericStrikes_Enter(object sender, EventArgs e)
        {
            FormHelper.selectAllText(this.numericStrikes);
        }
        private void numericStrikes_ValueChanged(object sender, EventArgs e)
        {
            if(this._cached == null)
                return;

            if (Convert.ToDouble(this.numericStrikes.Value) != this._cached.strikes)
                this._isDirty = true;
        }
        private void textboxComment_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxComment.Text != Convert.ToString(this._cached.comment))
                this._isDirty = true;
        }
        private void checkboxIsAllowedForUse_CheckedChanged(object sender, EventArgs e)
{
    if (this._cached != null && this.checkboxIsAllowedForUse.Checked != this._cached.is_allowed_for_use)
        this._isDirty = true;
}
        #endregion




        #region DESIGNER_CODE
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDeviceEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelDirty = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.buttonAction = new System.Windows.Forms.Button();
            this.textboxDeviceId = new System.Windows.Forms.TextBox();
this.labelDeviceId = new System.Windows.Forms.Label();
this.listDevice2 = new System.Windows.Forms.ComboBox();
this.buttonShowDevice2 = new System.Windows.Forms.Button();
this.labelDevice2 = new System.Windows.Forms.Label();
this.listDeviceType = new System.Windows.Forms.ComboBox();
this.buttonShowDeviceType = new System.Windows.Forms.Button();
this.labelDeviceType = new System.Windows.Forms.Label();
this.textboxName = new System.Windows.Forms.TextBox();
this.labelName = new System.Windows.Forms.Label();
this.textboxDescription = new System.Windows.Forms.TextBox();
this.labelDescription = new System.Windows.Forms.Label();
this.textboxLocation = new System.Windows.Forms.TextBox();
this.labelLocation = new System.Windows.Forms.Label();
this.numericMinValue = new System.Windows.Forms.NumericUpDown();
this.labelMinValue = new System.Windows.Forms.Label();
this.numericMaxValue = new System.Windows.Forms.NumericUpDown();
this.labelMaxValue = new System.Windows.Forms.Label();
this.numericAccuracy = new System.Windows.Forms.NumericUpDown();
this.labelAccuracy = new System.Windows.Forms.Label();
this.textboxSerialNumber = new System.Windows.Forms.TextBox();
this.labelSerialNumber = new System.Windows.Forms.Label();
this.numericCost = new System.Windows.Forms.NumericUpDown();
this.labelCost = new System.Windows.Forms.Label();
this.numericReliability = new System.Windows.Forms.NumericUpDown();
this.labelReliability = new System.Windows.Forms.Label();
this.numericStrikes = new System.Windows.Forms.NumericUpDown();
this.labelStrikes = new System.Windows.Forms.Label();
this.textboxComment = new System.Windows.Forms.TextBox();
this.labelComment = new System.Windows.Forms.Label();
this.checkboxIsAllowedForUse = new System.Windows.Forms.CheckBox();
this.labelIsAllowedForUse = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericMinValue)).BeginInit();
((System.ComponentModel.ISupportInitialize)(this.numericMaxValue)).BeginInit();
((System.ComponentModel.ISupportInitialize)(this.numericAccuracy)).BeginInit();
((System.ComponentModel.ISupportInitialize)(this.numericCost)).BeginInit();
((System.ComponentModel.ISupportInitialize)(this.numericReliability)).BeginInit();
((System.ComponentModel.ISupportInitialize)(this.numericStrikes)).BeginInit();

            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AutoScroll = true;
            this.splitContainer1.Panel1.Controls.Add(this.labelDirty);
            this.splitContainer1.Panel1.Controls.Add(labelDeviceId);
this.splitContainer1.Panel1.Controls.Add(labelDevice2);
this.splitContainer1.Panel1.Controls.Add(labelDeviceType);
this.splitContainer1.Panel1.Controls.Add(labelName);
this.splitContainer1.Panel1.Controls.Add(labelDescription);
this.splitContainer1.Panel1.Controls.Add(labelLocation);
this.splitContainer1.Panel1.Controls.Add(labelMinValue);
this.splitContainer1.Panel1.Controls.Add(labelMaxValue);
this.splitContainer1.Panel1.Controls.Add(labelAccuracy);
this.splitContainer1.Panel1.Controls.Add(labelSerialNumber);
this.splitContainer1.Panel1.Controls.Add(labelCost);
this.splitContainer1.Panel1.Controls.Add(labelReliability);
this.splitContainer1.Panel1.Controls.Add(labelStrikes);
this.splitContainer1.Panel1.Controls.Add(labelComment);
this.splitContainer1.Panel1.Controls.Add(labelIsAllowedForUse);

            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.buttonDelete);
            this.splitContainer1.Panel2.Controls.Add(this.buttonReload);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAction);
            this.splitContainer1.Panel2.Controls.Add(textboxDeviceId);
this.splitContainer1.Panel2.Controls.Add(listDevice2);
this.splitContainer1.Panel2.Controls.Add(buttonShowDevice2);
this.splitContainer1.Panel2.Controls.Add(listDeviceType);
this.splitContainer1.Panel2.Controls.Add(buttonShowDeviceType);
this.splitContainer1.Panel2.Controls.Add(textboxName);
this.splitContainer1.Panel2.Controls.Add(textboxDescription);
this.splitContainer1.Panel2.Controls.Add(textboxLocation);
this.splitContainer1.Panel2.Controls.Add(numericMinValue);
this.splitContainer1.Panel2.Controls.Add(numericMaxValue);
this.splitContainer1.Panel2.Controls.Add(numericAccuracy);
this.splitContainer1.Panel2.Controls.Add(textboxSerialNumber);
this.splitContainer1.Panel2.Controls.Add(numericCost);
this.splitContainer1.Panel2.Controls.Add(numericReliability);
this.splitContainer1.Panel2.Controls.Add(numericStrikes);
this.splitContainer1.Panel2.Controls.Add(textboxComment);
this.splitContainer1.Panel2.Controls.Add(checkboxIsAllowedForUse);

            this.splitContainer1.Size = new System.Drawing.Size(330, 341);
            this.splitContainer1.SplitterDistance = 109;
            this.splitContainer1.TabIndex = 0;
            // 
            // labelDirty
            // 
            this.labelDirty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelDirty.AutoSize = true;
            this.labelDirty.Location = new System.Drawing.Point(3, 9);
            this.labelDirty.Name = "labelDirty";
            this.labelDirty.Size = new System.Drawing.Size(50, 13);
            this.labelDirty.TabIndex = 3;
            this.labelDirty.Text = "Changed";
            this.labelDirty.Visible = false;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(85, 416);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Text = "[X]";
            this.buttonDelete.Size = new System.Drawing.Size(50, 23);
            this.buttonDelete.TabIndex = 11;
            this.buttonDelete.UseVisualStyleBackColor = false;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonReload
            // 
            this.buttonReload.Location = new System.Drawing.Point(4, 416);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(75, 23);
            this.buttonReload.TabIndex = 6;
            this.buttonReload.Text = "Reload";
            this.buttonReload.UseVisualStyleBackColor = true;
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            // 
            // buttonAction
            // 
            this.buttonAction.Location = new System.Drawing.Point(141, 416);
            this.buttonAction.Name = "buttonAction";
            this.buttonAction.Size = new System.Drawing.Size(75, 23);
            this.buttonAction.TabIndex = 2;
            this.buttonAction.Text = "Save";
            this.buttonAction.UseVisualStyleBackColor = true;
            this.buttonAction.Click += new System.EventHandler(this.buttonSave_Click);
                        // 
            // textboxDeviceId
            // 
            this.textboxDeviceId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxDeviceId.Location = new System.Drawing.Point(4, 12);
            this.textboxDeviceId.Name = "textboxDeviceId";
            this.textboxDeviceId.Size = new System.Drawing.Size(208, 20);
            this.textboxDeviceId.TabIndex = 31;
            this.textboxDeviceId.Leave += new System.EventHandler(this.textboxDeviceId_Leave);
            this.textboxDeviceId.Enabled = false;
                        // 
            // labelDeviceId
            // 
            this.labelDeviceId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDeviceId.AutoSize = true;
            this.labelDeviceId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDeviceId.Location = new System.Drawing.Point(0, 12);
            this.labelDeviceId.Name = "labelDeviceId";
            this.labelDeviceId.Size = new System.Drawing.Size(30, 20);
            this.labelDeviceId.TabIndex = 14;
            this.labelDeviceId.Text = "ID";
            this.labelDeviceId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // listDevice2
            // 
            this.listDevice2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listDevice2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listDevice2.FormattingEnabled = true;
            this.listDevice2.Location = new System.Drawing.Point(4, 38);
            this.listDevice2.Name = "listDevice2";
            this.listDevice2.Size = new System.Drawing.Size(165, 21);
            this.listDevice2.TabIndex = 25;
            this.listDevice2.SelectionChangeCommitted += new System.EventHandler(this.listDevice2_SelectionChangeCommitted);
                        // 
            // buttonShowDevice2
            // 
            this.buttonShowDevice2.Location = new System.Drawing.Point(174, 38);
            this.buttonShowDevice2.Name = "buttonShowDevice2";
            this.buttonShowDevice2.Size = new System.Drawing.Size(40, 23);
            this.buttonShowDevice2.TabIndex = 10;
            this.buttonShowDevice2.Text = "...";
            this.buttonShowDevice2.Click += new System.EventHandler(this.buttonShowDevice2_Click);
            // 
            // labelDevice2
            // 
            this.labelDevice2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDevice2.AutoSize = true;
            this.labelDevice2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDevice2.Location = new System.Drawing.Point(0, 38);
            this.labelDevice2.Name = "labelDevice2";
            this.labelDevice2.Size = new System.Drawing.Size(30, 20);
            this.labelDevice2.TabIndex = 14;
            this.labelDevice2.Text = "Parent Device";
            this.labelDevice2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // listDeviceType
            // 
            this.listDeviceType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listDeviceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listDeviceType.FormattingEnabled = true;
            this.listDeviceType.Location = new System.Drawing.Point(4, 64);
            this.listDeviceType.Name = "listDeviceType";
            this.listDeviceType.Size = new System.Drawing.Size(165, 21);
            this.listDeviceType.TabIndex = 25;
            this.listDeviceType.SelectionChangeCommitted += new System.EventHandler(this.listDeviceType_SelectionChangeCommitted);
                        // 
            // buttonShowDeviceType
            // 
            this.buttonShowDeviceType.Location = new System.Drawing.Point(174, 64);
            this.buttonShowDeviceType.Name = "buttonShowDeviceType";
            this.buttonShowDeviceType.Size = new System.Drawing.Size(40, 23);
            this.buttonShowDeviceType.TabIndex = 10;
            this.buttonShowDeviceType.Text = "...";
            this.buttonShowDeviceType.Click += new System.EventHandler(this.buttonShowDeviceType_Click);
            // 
            // labelDeviceType
            // 
            this.labelDeviceType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDeviceType.AutoSize = true;
            this.labelDeviceType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDeviceType.Location = new System.Drawing.Point(0, 64);
            this.labelDeviceType.Name = "labelDeviceType";
            this.labelDeviceType.Size = new System.Drawing.Size(30, 20);
            this.labelDeviceType.TabIndex = 14;
            this.labelDeviceType.Text = "DeviceType";
            this.labelDeviceType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxName
            // 
            this.textboxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxName.Location = new System.Drawing.Point(4, 90);
            this.textboxName.Name = "textboxName";
            this.textboxName.Size = new System.Drawing.Size(208, 20);
            this.textboxName.TabIndex = 31;
            this.textboxName.Leave += new System.EventHandler(this.textboxName_Leave);
            this.textboxName.Enabled = true;
                        // 
            // labelName
            // 
            this.labelName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelName.AutoSize = true;
            this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName.Location = new System.Drawing.Point(0, 90);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(30, 20);
            this.labelName.TabIndex = 14;
            this.labelName.Text = "Name";
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxDescription
            // 
            this.textboxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxDescription.Location = new System.Drawing.Point(4, 116);
            this.textboxDescription.Name = "textboxDescription";
            this.textboxDescription.Size = new System.Drawing.Size(208, 20);
            this.textboxDescription.TabIndex = 31;
            this.textboxDescription.Leave += new System.EventHandler(this.textboxDescription_Leave);
            this.textboxDescription.Enabled = true;
                        // 
            // labelDescription
            // 
            this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDescription.AutoSize = true;
            this.labelDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDescription.Location = new System.Drawing.Point(0, 116);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(30, 20);
            this.labelDescription.TabIndex = 14;
            this.labelDescription.Text = "Description";
            this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxLocation
            // 
            this.textboxLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxLocation.Location = new System.Drawing.Point(4, 142);
            this.textboxLocation.Name = "textboxLocation";
            this.textboxLocation.Size = new System.Drawing.Size(208, 20);
            this.textboxLocation.TabIndex = 31;
            this.textboxLocation.Leave += new System.EventHandler(this.textboxLocation_Leave);
            this.textboxLocation.Enabled = true;
                        // 
            // labelLocation
            // 
            this.labelLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelLocation.AutoSize = true;
            this.labelLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLocation.Location = new System.Drawing.Point(0, 142);
            this.labelLocation.Name = "labelLocation";
            this.labelLocation.Size = new System.Drawing.Size(30, 20);
            this.labelLocation.TabIndex = 14;
            this.labelLocation.Text = "Location";
            this.labelLocation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // numericMinValue
            // 
            this.numericMinValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericMinValue.Location = new System.Drawing.Point(4, 168);
            this.numericMinValue.Name = "numericMinValue";
            this.numericMinValue.Size = new System.Drawing.Size(211, 20);
            this.numericMinValue.TabIndex = 32;
            this.numericMinValue.ValueChanged += new System.EventHandler(this.numericMinValue_ValueChanged);
            this.numericMinValue.Click += new System.EventHandler(this.numericMinValue_Enter);
            this.numericMinValue.Enter += new System.EventHandler(this.numericMinValue_Enter);
                        // 
            // labelMinValue
            // 
            this.labelMinValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMinValue.AutoSize = true;
            this.labelMinValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMinValue.Location = new System.Drawing.Point(0, 168);
            this.labelMinValue.Name = "labelMinValue";
            this.labelMinValue.Size = new System.Drawing.Size(30, 20);
            this.labelMinValue.TabIndex = 14;
            this.labelMinValue.Text = "MinValue";
            this.labelMinValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // numericMaxValue
            // 
            this.numericMaxValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericMaxValue.Location = new System.Drawing.Point(4, 194);
            this.numericMaxValue.Name = "numericMaxValue";
            this.numericMaxValue.Size = new System.Drawing.Size(211, 20);
            this.numericMaxValue.TabIndex = 32;
            this.numericMaxValue.ValueChanged += new System.EventHandler(this.numericMaxValue_ValueChanged);
            this.numericMaxValue.Click += new System.EventHandler(this.numericMaxValue_Enter);
            this.numericMaxValue.Enter += new System.EventHandler(this.numericMaxValue_Enter);
                        // 
            // labelMaxValue
            // 
            this.labelMaxValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMaxValue.AutoSize = true;
            this.labelMaxValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMaxValue.Location = new System.Drawing.Point(0, 194);
            this.labelMaxValue.Name = "labelMaxValue";
            this.labelMaxValue.Size = new System.Drawing.Size(30, 20);
            this.labelMaxValue.TabIndex = 14;
            this.labelMaxValue.Text = "MaxValue";
            this.labelMaxValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // numericAccuracy
            // 
            this.numericAccuracy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericAccuracy.Location = new System.Drawing.Point(4, 220);
            this.numericAccuracy.Name = "numericAccuracy";
            this.numericAccuracy.Size = new System.Drawing.Size(211, 20);
            this.numericAccuracy.TabIndex = 32;
            this.numericAccuracy.ValueChanged += new System.EventHandler(this.numericAccuracy_ValueChanged);
            this.numericAccuracy.Click += new System.EventHandler(this.numericAccuracy_Enter);
            this.numericAccuracy.Enter += new System.EventHandler(this.numericAccuracy_Enter);
                        // 
            // labelAccuracy
            // 
            this.labelAccuracy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelAccuracy.AutoSize = true;
            this.labelAccuracy.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAccuracy.Location = new System.Drawing.Point(0, 220);
            this.labelAccuracy.Name = "labelAccuracy";
            this.labelAccuracy.Size = new System.Drawing.Size(30, 20);
            this.labelAccuracy.TabIndex = 14;
            this.labelAccuracy.Text = "Accuracy";
            this.labelAccuracy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxSerialNumber
            // 
            this.textboxSerialNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxSerialNumber.Location = new System.Drawing.Point(4, 246);
            this.textboxSerialNumber.Name = "textboxSerialNumber";
            this.textboxSerialNumber.Size = new System.Drawing.Size(208, 20);
            this.textboxSerialNumber.TabIndex = 31;
            this.textboxSerialNumber.Leave += new System.EventHandler(this.textboxSerialNumber_Leave);
            this.textboxSerialNumber.Enabled = true;
                        // 
            // labelSerialNumber
            // 
            this.labelSerialNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSerialNumber.AutoSize = true;
            this.labelSerialNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSerialNumber.Location = new System.Drawing.Point(0, 246);
            this.labelSerialNumber.Name = "labelSerialNumber";
            this.labelSerialNumber.Size = new System.Drawing.Size(30, 20);
            this.labelSerialNumber.TabIndex = 14;
            this.labelSerialNumber.Text = "SerialNumber";
            this.labelSerialNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // numericCost
            // 
            this.numericCost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericCost.Location = new System.Drawing.Point(4, 272);
            this.numericCost.Name = "numericCost";
            this.numericCost.Size = new System.Drawing.Size(211, 20);
            this.numericCost.TabIndex = 32;
            this.numericCost.ValueChanged += new System.EventHandler(this.numericCost_ValueChanged);
            this.numericCost.Click += new System.EventHandler(this.numericCost_Enter);
            this.numericCost.Enter += new System.EventHandler(this.numericCost_Enter);
                        // 
            // labelCost
            // 
            this.labelCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCost.AutoSize = true;
            this.labelCost.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCost.Location = new System.Drawing.Point(0, 272);
            this.labelCost.Name = "labelCost";
            this.labelCost.Size = new System.Drawing.Size(30, 20);
            this.labelCost.TabIndex = 14;
            this.labelCost.Text = "Cost";
            this.labelCost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // numericReliability
            // 
            this.numericReliability.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericReliability.Location = new System.Drawing.Point(4, 298);
            this.numericReliability.Name = "numericReliability";
            this.numericReliability.Size = new System.Drawing.Size(211, 20);
            this.numericReliability.TabIndex = 32;
            this.numericReliability.ValueChanged += new System.EventHandler(this.numericReliability_ValueChanged);
            this.numericReliability.Click += new System.EventHandler(this.numericReliability_Enter);
            this.numericReliability.Enter += new System.EventHandler(this.numericReliability_Enter);
                        // 
            // labelReliability
            // 
            this.labelReliability.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelReliability.AutoSize = true;
            this.labelReliability.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelReliability.Location = new System.Drawing.Point(0, 298);
            this.labelReliability.Name = "labelReliability";
            this.labelReliability.Size = new System.Drawing.Size(30, 20);
            this.labelReliability.TabIndex = 14;
            this.labelReliability.Text = "Reliability";
            this.labelReliability.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // numericStrikes
            // 
            this.numericStrikes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericStrikes.Location = new System.Drawing.Point(4, 324);
            this.numericStrikes.Name = "numericStrikes";
            this.numericStrikes.Size = new System.Drawing.Size(211, 20);
            this.numericStrikes.TabIndex = 32;
            this.numericStrikes.ValueChanged += new System.EventHandler(this.numericStrikes_ValueChanged);
            this.numericStrikes.Click += new System.EventHandler(this.numericStrikes_Enter);
            this.numericStrikes.Enter += new System.EventHandler(this.numericStrikes_Enter);
                        // 
            // labelStrikes
            // 
            this.labelStrikes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStrikes.AutoSize = true;
            this.labelStrikes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStrikes.Location = new System.Drawing.Point(0, 324);
            this.labelStrikes.Name = "labelStrikes";
            this.labelStrikes.Size = new System.Drawing.Size(30, 20);
            this.labelStrikes.TabIndex = 14;
            this.labelStrikes.Text = "Strikes";
            this.labelStrikes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxComment
            // 
            this.textboxComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxComment.Location = new System.Drawing.Point(4, 350);
            this.textboxComment.Name = "textboxComment";
            this.textboxComment.Size = new System.Drawing.Size(208, 20);
            this.textboxComment.TabIndex = 31;
            this.textboxComment.Leave += new System.EventHandler(this.textboxComment_Leave);
            this.textboxComment.Enabled = true;
                        // 
            // labelComment
            // 
            this.labelComment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelComment.AutoSize = true;
            this.labelComment.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelComment.Location = new System.Drawing.Point(0, 350);
            this.labelComment.Name = "labelComment";
            this.labelComment.Size = new System.Drawing.Size(30, 20);
            this.labelComment.TabIndex = 14;
            this.labelComment.Text = "Comment";
            this.labelComment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // checkboxIsAllowedForUse
            // 
            this.checkboxIsAllowedForUse.AutoSize = true;
            this.checkboxIsAllowedForUse.Location = new System.Drawing.Point(4, 378);
            this.checkboxIsAllowedForUse.Name = "checkboxIsAllowedForUse";
            this.checkboxIsAllowedForUse.Size = new System.Drawing.Size(53, 17);
            this.checkboxIsAllowedForUse.TabIndex = 33;
            this.checkboxIsAllowedForUse.Text = "IsAllowedForUse";
            this.checkboxIsAllowedForUse.UseVisualStyleBackColor = true;
            this.checkboxIsAllowedForUse.CheckedChanged += new System.EventHandler(this.checkboxIsAllowedForUse_CheckedChanged);            // 
            // labelIsAllowedForUse
            // 
            this.labelIsAllowedForUse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelIsAllowedForUse.AutoSize = true;
            this.labelIsAllowedForUse.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelIsAllowedForUse.Location = new System.Drawing.Point(0, 376);
            this.labelIsAllowedForUse.Name = "labelIsAllowedForUse";
            this.labelIsAllowedForUse.Size = new System.Drawing.Size(30, 20);
            this.labelIsAllowedForUse.TabIndex = 14;
            this.labelIsAllowedForUse.Text = "IsAllowedForUse";
            this.labelIsAllowedForUse.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            
            // 
            // FormDeviceEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 442);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormDeviceEditor";
            this.Text = "Device Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDeviceEditor_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericMinValue)).EndInit();
((System.ComponentModel.ISupportInitialize)(this.numericMaxValue)).EndInit();
((System.ComponentModel.ISupportInitialize)(this.numericAccuracy)).EndInit();
((System.ComponentModel.ISupportInitialize)(this.numericCost)).EndInit();
((System.ComponentModel.ISupportInitialize)(this.numericReliability)).EndInit();
((System.ComponentModel.ISupportInitialize)(this.numericStrikes)).EndInit();

            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonAction;
        private System.Windows.Forms.Label labelDirty;
        private System.Windows.Forms.Button buttonReload;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.TextBox textboxDeviceId;
private System.Windows.Forms.Label labelDeviceId;
private System.Windows.Forms.ComboBox listDevice2;
private System.Windows.Forms.Button buttonShowDevice2;
private System.Windows.Forms.Label labelDevice2;
private System.Windows.Forms.ComboBox listDeviceType;
private System.Windows.Forms.Button buttonShowDeviceType;
private System.Windows.Forms.Label labelDeviceType;
private System.Windows.Forms.TextBox textboxName;
private System.Windows.Forms.Label labelName;
private System.Windows.Forms.TextBox textboxDescription;
private System.Windows.Forms.Label labelDescription;
private System.Windows.Forms.TextBox textboxLocation;
private System.Windows.Forms.Label labelLocation;
private System.Windows.Forms.NumericUpDown numericMinValue;
private System.Windows.Forms.Label labelMinValue;
private System.Windows.Forms.NumericUpDown numericMaxValue;
private System.Windows.Forms.Label labelMaxValue;
private System.Windows.Forms.NumericUpDown numericAccuracy;
private System.Windows.Forms.Label labelAccuracy;
private System.Windows.Forms.TextBox textboxSerialNumber;
private System.Windows.Forms.Label labelSerialNumber;
private System.Windows.Forms.NumericUpDown numericCost;
private System.Windows.Forms.Label labelCost;
private System.Windows.Forms.NumericUpDown numericReliability;
private System.Windows.Forms.Label labelReliability;
private System.Windows.Forms.NumericUpDown numericStrikes;
private System.Windows.Forms.Label labelStrikes;
private System.Windows.Forms.TextBox textboxComment;
private System.Windows.Forms.Label labelComment;
private System.Windows.Forms.CheckBox checkboxIsAllowedForUse;
private System.Windows.Forms.Label labelIsAllowedForUse;

        #endregion
    }
}
