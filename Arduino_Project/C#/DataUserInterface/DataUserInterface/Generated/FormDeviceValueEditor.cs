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
    public partial class FormDeviceValueEditor : Form
    {
        private static device_value _defaultObject = new device_value();

        public int id { private set; get; }
        private device_value _cached { set; get; }

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
    this.listDevice.Items.Add(val.name);
    
                }

                if (this.mode != EnumEditorMode.Create)
                {
                    var obj = db.device_value.SingleOrDefault(v => v.device_value_id == this.id);
                    if (obj != null)
                    {
                        this.textboxDeviceValueId.Text = Convert.ToString(obj.device_value_id);
foreach (var value in db.devices.OrderBy(v => v.name))
{
    this.listDevice.Items.Add(value.name);
    if (value.device_id == obj.device_id)
        this.listDevice.SelectedIndex = this.listDevice.Items.Count - 1;
}
this.numericValue.Value = (decimal)obj.value;
this.datetimeDatetime.Value = obj.datetime;
this.checkboxResponseRecieved.Checked = obj.response_recieved;this.textboxComment.Text = obj.comment;
this.textboxExtraData.Text = obj.extra_data;


                        this._cached  = obj;
                        this._isDirty = false;
                    }
                }
            }
        }
        
        public FormDeviceValueEditor(EnumEditorMode mode, int id = -1) // ID isn't always needed, e.g. Create
        {
            InitializeComponent();

            FormHelper.unlimitNumericBox(this.numericValue, AllowDecimals.yes);


            this.mode = mode;
            if (mode != EnumEditorMode.Create)
                this.id = id;
            else
                this._cached = FormDeviceValueEditor._defaultObject;

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
                var obj = db.device_value.SingleOrDefault(v => v.device_value_id == this.id);

                if (!obj.isDeletable(db))
                {
                    MessageBox.Show("Cannot delete this record as other records are still linked to it.", 
                                    "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this record?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    return;

                db.device_value.Remove(obj);
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
                var obj = db.device_value.SingleOrDefault(v => v.device_value_id == this.id);

                
var selectedDevice = this.listDevice.Items[this.listDevice.SelectedIndex] as string;
obj.device = db.devices.Single(v => v.name == selectedDevice);
obj.value = (double)this.numericValue.Value;
this.datetimeDatetime.Value = DateTime.Now;
obj.datetime = this.datetimeDatetime.Value;
obj.response_recieved = this.checkboxResponseRecieved.Checked;obj.comment = this.textboxComment.Text;
obj.extra_data = this.textboxExtraData.Text;


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
                var obj = new device_value();

                
var selectedDevice = this.listDevice.Items[this.listDevice.SelectedIndex] as string;
obj.device = db.devices.Single(v => v.name == selectedDevice);
obj.value = (double)this.numericValue.Value;
this.datetimeDatetime.Value = DateTime.Now;
obj.datetime = this.datetimeDatetime.Value;
obj.response_recieved = this.checkboxResponseRecieved.Checked;obj.comment = this.textboxComment.Text;
obj.extra_data = this.textboxExtraData.Text;


                db.device_value.Add(obj);
                db.SaveChanges();
                this._cached  = obj;
                this.id       = obj.device_value_id;
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

                private void textboxDeviceValueId_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxDeviceValueId.Text != Convert.ToString(this._cached.device_value_id))
                this._isDirty = true;
        }
                private void listDevice_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listDevice.SelectedIndex;
            var value = this.listDevice.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || (this._cached.device != null && value != this._cached.device.name))
                this._isDirty = true;
        }
        private void buttonShowDevice_Click(object sender, EventArgs e)
{
    var form = new SearchForm(EnumSearchFormType.Device);
    form.MdiParent = this.MdiParent;
    form.Show();
}
        private void numericValue_Enter(object sender, EventArgs e)
        {
            FormHelper.selectAllText(this.numericValue);
        }
        private void numericValue_ValueChanged(object sender, EventArgs e)
        {
            if(this._cached == null)
                return;

            if (Convert.ToDouble(this.numericValue.Value) != this._cached.value)
                this._isDirty = true;
        }
private void checkboxResponseRecieved_CheckedChanged(object sender, EventArgs e)
{
    if (this._cached != null && this.checkboxResponseRecieved.Checked != this._cached.response_recieved)
        this._isDirty = true;
}        private void textboxComment_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxComment.Text != Convert.ToString(this._cached.comment))
                this._isDirty = true;
        }
                private void textboxExtraData_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxExtraData.Text != Convert.ToString(this._cached.extra_data))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDeviceValueEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelDirty = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.buttonAction = new System.Windows.Forms.Button();
            this.textboxDeviceValueId = new System.Windows.Forms.TextBox();
this.labelDeviceValueId = new System.Windows.Forms.Label();
this.listDevice = new System.Windows.Forms.ComboBox();
this.buttonShowDevice = new System.Windows.Forms.Button();
this.labelDevice = new System.Windows.Forms.Label();
this.numericValue = new System.Windows.Forms.NumericUpDown();
this.labelValue = new System.Windows.Forms.Label();
this.datetimeDatetime = new System.Windows.Forms.DateTimePicker();
this.labelDatetime = new System.Windows.Forms.Label();
this.checkboxResponseRecieved = new System.Windows.Forms.CheckBox();
this.labelResponseRecieved = new System.Windows.Forms.Label();
this.textboxComment = new System.Windows.Forms.TextBox();
this.labelComment = new System.Windows.Forms.Label();
this.textboxExtraData = new System.Windows.Forms.TextBox();
this.labelExtraData = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericValue)).BeginInit();

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
            this.splitContainer1.Panel1.Controls.Add(labelDeviceValueId);
this.splitContainer1.Panel1.Controls.Add(labelDevice);
this.splitContainer1.Panel1.Controls.Add(labelValue);
this.splitContainer1.Panel1.Controls.Add(labelDatetime);
this.splitContainer1.Panel1.Controls.Add(labelResponseRecieved);
this.splitContainer1.Panel1.Controls.Add(labelComment);
this.splitContainer1.Panel1.Controls.Add(labelExtraData);

            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.buttonDelete);
            this.splitContainer1.Panel2.Controls.Add(this.buttonReload);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAction);
            this.splitContainer1.Panel2.Controls.Add(textboxDeviceValueId);
this.splitContainer1.Panel2.Controls.Add(listDevice);
this.splitContainer1.Panel2.Controls.Add(buttonShowDevice);
this.splitContainer1.Panel2.Controls.Add(numericValue);
this.splitContainer1.Panel2.Controls.Add(datetimeDatetime);
this.splitContainer1.Panel2.Controls.Add(checkboxResponseRecieved);
this.splitContainer1.Panel2.Controls.Add(textboxComment);
this.splitContainer1.Panel2.Controls.Add(textboxExtraData);

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
            this.buttonDelete.Location = new System.Drawing.Point(85, 208);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Text = "[X]";
            this.buttonDelete.Size = new System.Drawing.Size(50, 23);
            this.buttonDelete.TabIndex = 11;
            this.buttonDelete.UseVisualStyleBackColor = false;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonReload
            // 
            this.buttonReload.Location = new System.Drawing.Point(4, 208);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(75, 23);
            this.buttonReload.TabIndex = 6;
            this.buttonReload.Text = "Reload";
            this.buttonReload.UseVisualStyleBackColor = true;
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            // 
            // buttonAction
            // 
            this.buttonAction.Location = new System.Drawing.Point(141, 208);
            this.buttonAction.Name = "buttonAction";
            this.buttonAction.Size = new System.Drawing.Size(75, 23);
            this.buttonAction.TabIndex = 2;
            this.buttonAction.Text = "Save";
            this.buttonAction.UseVisualStyleBackColor = true;
            this.buttonAction.Click += new System.EventHandler(this.buttonSave_Click);
                        // 
            // textboxDeviceValueId
            // 
            this.textboxDeviceValueId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxDeviceValueId.Location = new System.Drawing.Point(4, 12);
            this.textboxDeviceValueId.Name = "textboxDeviceValueId";
            this.textboxDeviceValueId.Size = new System.Drawing.Size(208, 20);
            this.textboxDeviceValueId.TabIndex = 0;
            this.textboxDeviceValueId.Leave += new System.EventHandler(this.textboxDeviceValueId_Leave);
            this.textboxDeviceValueId.Enabled = false;
                        // 
            // labelDeviceValueId
            // 
            this.labelDeviceValueId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDeviceValueId.AutoSize = true;
            this.labelDeviceValueId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDeviceValueId.Location = new System.Drawing.Point(0, 12);
            this.labelDeviceValueId.Name = "labelDeviceValueId";
            this.labelDeviceValueId.Size = new System.Drawing.Size(30, 20);
            this.labelDeviceValueId.TabIndex = 0;
            this.labelDeviceValueId.Text = "ID";
            this.labelDeviceValueId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // listDevice
            // 
            this.listDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listDevice.FormattingEnabled = true;
            this.listDevice.Location = new System.Drawing.Point(4, 38);
            this.listDevice.Name = "listDevice";
            this.listDevice.Size = new System.Drawing.Size(165, 21);
            this.listDevice.TabIndex = 1;
            this.listDevice.SelectionChangeCommitted += new System.EventHandler(this.listDevice_SelectionChangeCommitted);
                        // 
            // buttonShowDevice
            // 
            this.buttonShowDevice.Location = new System.Drawing.Point(174, 38);
            this.buttonShowDevice.Name = "buttonShowDevice";
            this.buttonShowDevice.Size = new System.Drawing.Size(40, 23);
            this.buttonShowDevice.TabIndex = 2;
            this.buttonShowDevice.Text = "...";
            this.buttonShowDevice.Click += new System.EventHandler(this.buttonShowDevice_Click);
            // 
            // labelDevice
            // 
            this.labelDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDevice.AutoSize = true;
            this.labelDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDevice.Location = new System.Drawing.Point(0, 38);
            this.labelDevice.Name = "labelDevice";
            this.labelDevice.Size = new System.Drawing.Size(30, 20);
            this.labelDevice.TabIndex = 0;
            this.labelDevice.Text = "Device";
            this.labelDevice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // numericValue
            // 
            this.numericValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericValue.Location = new System.Drawing.Point(4, 64);
            this.numericValue.Name = "numericValue";
            this.numericValue.Size = new System.Drawing.Size(211, 20);
            this.numericValue.TabIndex = 3;
            this.numericValue.ValueChanged += new System.EventHandler(this.numericValue_ValueChanged);
            this.numericValue.Click += new System.EventHandler(this.numericValue_Enter);
            this.numericValue.Enter += new System.EventHandler(this.numericValue_Enter);
                        // 
            // labelValue
            // 
            this.labelValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelValue.AutoSize = true;
            this.labelValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelValue.Location = new System.Drawing.Point(0, 64);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(30, 20);
            this.labelValue.TabIndex = 0;
            this.labelValue.Text = "Value";
            this.labelValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // datetimeDatetime
            // 
            this.datetimeDatetime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.datetimeDatetime.Enabled = false;
            this.datetimeDatetime.Location = new System.Drawing.Point(4, 90);
            this.datetimeDatetime.Name = "datetimeDatetime";
            this.datetimeDatetime.Size = new System.Drawing.Size(208, 20);
            this.datetimeDatetime.TabIndex = 4;
            // 
            // labelDatetime
            // 
            this.labelDatetime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDatetime.AutoSize = true;
            this.labelDatetime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDatetime.Location = new System.Drawing.Point(0, 90);
            this.labelDatetime.Name = "labelDatetime";
            this.labelDatetime.Size = new System.Drawing.Size(30, 20);
            this.labelDatetime.TabIndex = 0;
            this.labelDatetime.Text = "Datetime";
            this.labelDatetime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // checkboxResponseRecieved
            // 
            this.checkboxResponseRecieved.AutoSize = true;
            this.checkboxResponseRecieved.Location = new System.Drawing.Point(4, 118);
            this.checkboxResponseRecieved.Name = "checkboxResponseRecieved";
            this.checkboxResponseRecieved.Size = new System.Drawing.Size(53, 17);
            this.checkboxResponseRecieved.TabIndex = 5;
            this.checkboxResponseRecieved.Text = "ResponseRecieved";
            this.checkboxResponseRecieved.UseVisualStyleBackColor = true;
            this.checkboxResponseRecieved.CheckedChanged += new System.EventHandler(this.checkboxResponseRecieved_CheckedChanged);            // 
            // labelResponseRecieved
            // 
            this.labelResponseRecieved.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelResponseRecieved.AutoSize = true;
            this.labelResponseRecieved.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResponseRecieved.Location = new System.Drawing.Point(0, 116);
            this.labelResponseRecieved.Name = "labelResponseRecieved";
            this.labelResponseRecieved.Size = new System.Drawing.Size(30, 20);
            this.labelResponseRecieved.TabIndex = 0;
            this.labelResponseRecieved.Text = "ResponseRecieved";
            this.labelResponseRecieved.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxComment
            // 
            this.textboxComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxComment.Location = new System.Drawing.Point(4, 142);
            this.textboxComment.Name = "textboxComment";
            this.textboxComment.Size = new System.Drawing.Size(208, 20);
            this.textboxComment.TabIndex = 6;
            this.textboxComment.Leave += new System.EventHandler(this.textboxComment_Leave);
            this.textboxComment.Enabled = true;
                        // 
            // labelComment
            // 
            this.labelComment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelComment.AutoSize = true;
            this.labelComment.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelComment.Location = new System.Drawing.Point(0, 142);
            this.labelComment.Name = "labelComment";
            this.labelComment.Size = new System.Drawing.Size(30, 20);
            this.labelComment.TabIndex = 0;
            this.labelComment.Text = "Comment";
            this.labelComment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxExtraData
            // 
            this.textboxExtraData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxExtraData.Location = new System.Drawing.Point(4, 168);
            this.textboxExtraData.Name = "textboxExtraData";
            this.textboxExtraData.Size = new System.Drawing.Size(208, 20);
            this.textboxExtraData.TabIndex = 7;
            this.textboxExtraData.Leave += new System.EventHandler(this.textboxExtraData_Leave);
            this.textboxExtraData.Enabled = true;
                        // 
            // labelExtraData
            // 
            this.labelExtraData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelExtraData.AutoSize = true;
            this.labelExtraData.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelExtraData.Location = new System.Drawing.Point(0, 168);
            this.labelExtraData.Name = "labelExtraData";
            this.labelExtraData.Size = new System.Drawing.Size(30, 20);
            this.labelExtraData.TabIndex = 0;
            this.labelExtraData.Text = "ExtraData";
            this.labelExtraData.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            
            // 
            // FormDeviceValueEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 234);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormDeviceValueEditor";
            this.Text = "DeviceValue Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDeviceEditor_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericValue)).EndInit();

            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonAction;
        private System.Windows.Forms.Label labelDirty;
        private System.Windows.Forms.Button buttonReload;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.TextBox textboxDeviceValueId;
private System.Windows.Forms.Label labelDeviceValueId;
private System.Windows.Forms.ComboBox listDevice;
private System.Windows.Forms.Button buttonShowDevice;
private System.Windows.Forms.Label labelDevice;
private System.Windows.Forms.NumericUpDown numericValue;
private System.Windows.Forms.Label labelValue;
private System.Windows.Forms.DateTimePicker datetimeDatetime;
private System.Windows.Forms.Label labelDatetime;
private System.Windows.Forms.CheckBox checkboxResponseRecieved;
private System.Windows.Forms.Label labelResponseRecieved;
private System.Windows.Forms.TextBox textboxComment;
private System.Windows.Forms.Label labelComment;
private System.Windows.Forms.TextBox textboxExtraData;
private System.Windows.Forms.Label labelExtraData;

        #endregion
    }
}