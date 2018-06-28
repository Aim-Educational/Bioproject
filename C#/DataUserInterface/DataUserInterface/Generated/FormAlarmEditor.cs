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
    public partial class FormAlarmEditor : Form
    {
        private static alarm _defaultObject = new alarm();

        public int id { private set; get; }
        private alarm _cached { set; get; }

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
                    foreach (var val in db.alarm_type.OrderBy(v => v.description))
    this.listAlarmType.Items.Add(val.description);
    foreach (var val in db.devices.OrderBy(v => v.name))
    this.listDevice.Items.Add(val.name);
    foreach (var val in db.group_type.OrderBy(v => v.name))
    this.listGroupType.Items.Add(val.name);
    
                }

                if (this.mode != EnumEditorMode.Create)
                {
                    var obj = db.alarms.SingleOrDefault(v => v.alarm_id == this.id);
                    if (obj != null)
                    {
                        this.textboxAlarmId.Text = Convert.ToString(obj.alarm_id);
this.numericValue.Value = (decimal)obj.value;
this.textboxComment.Text = obj.comment;
foreach (var value in db.alarm_type.OrderBy(v => v.description))
{
    this.listAlarmType.Items.Add(value.description);
    if (value.alarm_type_id == obj.alarm_type_id)
        this.listAlarmType.SelectedIndex = this.listAlarmType.Items.Count - 1;
}
foreach (var value in db.devices.OrderBy(v => v.name))
{
    this.listDevice.Items.Add(value.name);
    if (value.device_id == obj.device_id)
        this.listDevice.SelectedIndex = this.listDevice.Items.Count - 1;
}
foreach (var value in db.group_type.OrderBy(v => v.name))
{
    this.listGroupType.Items.Add(value.name);
    if (value.group_type_id == obj.group_type_id)
        this.listGroupType.SelectedIndex = this.listGroupType.Items.Count - 1;
}


                        this._cached  = obj;
                        this._isDirty = false;
                    }
                }
            }
        }
        
        public FormAlarmEditor(EnumEditorMode mode, int id = -1) // ID isn't always needed, e.g. Create
        {
            InitializeComponent();

            FormHelper.unlimitNumericBox(this.numericValue, AllowDecimals.yes);


            this.mode = mode;
            if (mode != EnumEditorMode.Create)
                this.id = id;
            else
                this._cached = FormAlarmEditor._defaultObject;

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
                var obj = db.alarms.SingleOrDefault(v => v.alarm_id == this.id);

                if (!obj.isDeletable(db))
                {
                    MessageBox.Show("Cannot delete this record as other records are still linked to it.", 
                                    "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this record?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    return;

                db.alarms.Remove(obj);
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
                var obj = db.alarms.SingleOrDefault(v => v.alarm_id == this.id);

                
obj.value = (double)this.numericValue.Value;
obj.comment = this.textboxComment.Text;
var selectedAlarmType = this.listAlarmType.Items[this.listAlarmType.SelectedIndex] as string;
obj.alarm_type = db.alarm_type.Single(v => v.description == selectedAlarmType);
var selectedDevice = this.listDevice.Items[this.listDevice.SelectedIndex] as string;
obj.device = db.devices.Single(v => v.name == selectedDevice);
var selectedGroupType = this.listGroupType.Items[this.listGroupType.SelectedIndex] as string;
obj.group_type = db.group_type.Single(v => v.name == selectedGroupType);


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
                var obj = new alarm();

                
obj.value = (double)this.numericValue.Value;
obj.comment = this.textboxComment.Text;
var selectedAlarmType = this.listAlarmType.Items[this.listAlarmType.SelectedIndex] as string;
obj.alarm_type = db.alarm_type.Single(v => v.description == selectedAlarmType);
var selectedDevice = this.listDevice.Items[this.listDevice.SelectedIndex] as string;
obj.device = db.devices.Single(v => v.name == selectedDevice);
var selectedGroupType = this.listGroupType.Items[this.listGroupType.SelectedIndex] as string;
obj.group_type = db.group_type.Single(v => v.name == selectedGroupType);


                db.alarms.Add(obj);
                db.SaveChanges();
                this._cached  = obj;
                this.id       = obj.alarm_id;
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

                private void textboxAlarmId_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxAlarmId.Text != Convert.ToString(this._cached.alarm_id))
                this._isDirty = true;
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
        private void textboxComment_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxComment.Text != Convert.ToString(this._cached.comment))
                this._isDirty = true;
        }
                private void listAlarmType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listAlarmType.SelectedIndex;
            var value = this.listAlarmType.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || (this._cached.alarm_type != null && value != this._cached.alarm_type.description))
                this._isDirty = true;
        }
        private void buttonShowAlarmType_Click(object sender, EventArgs e)
{
    var form = new SearchForm(EnumSearchFormType.AlarmType);
    form.MdiParent = this.MdiParent;
    form.Show();
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
        private void listGroupType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listGroupType.SelectedIndex;
            var value = this.listGroupType.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || (this._cached.group_type != null && value != this._cached.group_type.name))
                this._isDirty = true;
        }
        private void buttonShowGroupType_Click(object sender, EventArgs e)
{
    var form = new SearchForm(EnumSearchFormType.GroupType);
    form.MdiParent = this.MdiParent;
    form.Show();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAlarmEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelDirty = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.buttonAction = new System.Windows.Forms.Button();
            this.textboxAlarmId = new System.Windows.Forms.TextBox();
this.labelAlarmId = new System.Windows.Forms.Label();
this.numericValue = new System.Windows.Forms.NumericUpDown();
this.labelValue = new System.Windows.Forms.Label();
this.textboxComment = new System.Windows.Forms.TextBox();
this.labelComment = new System.Windows.Forms.Label();
this.listAlarmType = new System.Windows.Forms.ComboBox();
this.buttonShowAlarmType = new System.Windows.Forms.Button();
this.labelAlarmType = new System.Windows.Forms.Label();
this.listDevice = new System.Windows.Forms.ComboBox();
this.buttonShowDevice = new System.Windows.Forms.Button();
this.labelDevice = new System.Windows.Forms.Label();
this.listGroupType = new System.Windows.Forms.ComboBox();
this.buttonShowGroupType = new System.Windows.Forms.Button();
this.labelGroupType = new System.Windows.Forms.Label();

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
            this.splitContainer1.Panel1.Controls.Add(labelAlarmId);
this.splitContainer1.Panel1.Controls.Add(labelValue);
this.splitContainer1.Panel1.Controls.Add(labelComment);
this.splitContainer1.Panel1.Controls.Add(labelAlarmType);
this.splitContainer1.Panel1.Controls.Add(labelDevice);
this.splitContainer1.Panel1.Controls.Add(labelGroupType);

            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.buttonDelete);
            this.splitContainer1.Panel2.Controls.Add(this.buttonReload);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAction);
            this.splitContainer1.Panel2.Controls.Add(textboxAlarmId);
this.splitContainer1.Panel2.Controls.Add(numericValue);
this.splitContainer1.Panel2.Controls.Add(textboxComment);
this.splitContainer1.Panel2.Controls.Add(listAlarmType);
this.splitContainer1.Panel2.Controls.Add(buttonShowAlarmType);
this.splitContainer1.Panel2.Controls.Add(listDevice);
this.splitContainer1.Panel2.Controls.Add(buttonShowDevice);
this.splitContainer1.Panel2.Controls.Add(listGroupType);
this.splitContainer1.Panel2.Controls.Add(buttonShowGroupType);

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
            this.buttonDelete.Location = new System.Drawing.Point(85, 182);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Text = "[X]";
            this.buttonDelete.Size = new System.Drawing.Size(50, 23);
            this.buttonDelete.TabIndex = 11;
            this.buttonDelete.UseVisualStyleBackColor = false;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonReload
            // 
            this.buttonReload.Location = new System.Drawing.Point(4, 182);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(75, 23);
            this.buttonReload.TabIndex = 6;
            this.buttonReload.Text = "Reload";
            this.buttonReload.UseVisualStyleBackColor = true;
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            // 
            // buttonAction
            // 
            this.buttonAction.Location = new System.Drawing.Point(141, 182);
            this.buttonAction.Name = "buttonAction";
            this.buttonAction.Size = new System.Drawing.Size(75, 23);
            this.buttonAction.TabIndex = 2;
            this.buttonAction.Text = "Save";
            this.buttonAction.UseVisualStyleBackColor = true;
            this.buttonAction.Click += new System.EventHandler(this.buttonSave_Click);
                        // 
            // textboxAlarmId
            // 
            this.textboxAlarmId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxAlarmId.Location = new System.Drawing.Point(4, 12);
            this.textboxAlarmId.Name = "textboxAlarmId";
            this.textboxAlarmId.Size = new System.Drawing.Size(208, 20);
            this.textboxAlarmId.TabIndex = 31;
            this.textboxAlarmId.Leave += new System.EventHandler(this.textboxAlarmId_Leave);
            this.textboxAlarmId.Enabled = false;
                        // 
            // labelAlarmId
            // 
            this.labelAlarmId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelAlarmId.AutoSize = true;
            this.labelAlarmId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAlarmId.Location = new System.Drawing.Point(0, 12);
            this.labelAlarmId.Name = "labelAlarmId";
            this.labelAlarmId.Size = new System.Drawing.Size(30, 20);
            this.labelAlarmId.TabIndex = 14;
            this.labelAlarmId.Text = "ID";
            this.labelAlarmId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // numericValue
            // 
            this.numericValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericValue.Location = new System.Drawing.Point(4, 38);
            this.numericValue.Name = "numericValue";
            this.numericValue.Size = new System.Drawing.Size(211, 20);
            this.numericValue.TabIndex = 32;
            this.numericValue.ValueChanged += new System.EventHandler(this.numericValue_ValueChanged);
            this.numericValue.Click += new System.EventHandler(this.numericValue_Enter);
            this.numericValue.Enter += new System.EventHandler(this.numericValue_Enter);
                        // 
            // labelValue
            // 
            this.labelValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelValue.AutoSize = true;
            this.labelValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelValue.Location = new System.Drawing.Point(0, 38);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(30, 20);
            this.labelValue.TabIndex = 14;
            this.labelValue.Text = "Value";
            this.labelValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxComment
            // 
            this.textboxComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxComment.Location = new System.Drawing.Point(4, 64);
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
            this.labelComment.Location = new System.Drawing.Point(0, 64);
            this.labelComment.Name = "labelComment";
            this.labelComment.Size = new System.Drawing.Size(30, 20);
            this.labelComment.TabIndex = 14;
            this.labelComment.Text = "Comment";
            this.labelComment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // listAlarmType
            // 
            this.listAlarmType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listAlarmType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listAlarmType.FormattingEnabled = true;
            this.listAlarmType.Location = new System.Drawing.Point(4, 90);
            this.listAlarmType.Name = "listAlarmType";
            this.listAlarmType.Size = new System.Drawing.Size(165, 21);
            this.listAlarmType.TabIndex = 25;
            this.listAlarmType.SelectionChangeCommitted += new System.EventHandler(this.listAlarmType_SelectionChangeCommitted);
                        // 
            // buttonShowAlarmType
            // 
            this.buttonShowAlarmType.Location = new System.Drawing.Point(174, 90);
            this.buttonShowAlarmType.Name = "buttonShowAlarmType";
            this.buttonShowAlarmType.Size = new System.Drawing.Size(40, 23);
            this.buttonShowAlarmType.TabIndex = 10;
            this.buttonShowAlarmType.Text = "...";
            this.buttonShowAlarmType.Click += new System.EventHandler(this.buttonShowAlarmType_Click);
            // 
            // labelAlarmType
            // 
            this.labelAlarmType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelAlarmType.AutoSize = true;
            this.labelAlarmType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAlarmType.Location = new System.Drawing.Point(0, 90);
            this.labelAlarmType.Name = "labelAlarmType";
            this.labelAlarmType.Size = new System.Drawing.Size(30, 20);
            this.labelAlarmType.TabIndex = 14;
            this.labelAlarmType.Text = "AlarmType";
            this.labelAlarmType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // listDevice
            // 
            this.listDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listDevice.FormattingEnabled = true;
            this.listDevice.Location = new System.Drawing.Point(4, 116);
            this.listDevice.Name = "listDevice";
            this.listDevice.Size = new System.Drawing.Size(165, 21);
            this.listDevice.TabIndex = 25;
            this.listDevice.SelectionChangeCommitted += new System.EventHandler(this.listDevice_SelectionChangeCommitted);
                        // 
            // buttonShowDevice
            // 
            this.buttonShowDevice.Location = new System.Drawing.Point(174, 116);
            this.buttonShowDevice.Name = "buttonShowDevice";
            this.buttonShowDevice.Size = new System.Drawing.Size(40, 23);
            this.buttonShowDevice.TabIndex = 10;
            this.buttonShowDevice.Text = "...";
            this.buttonShowDevice.Click += new System.EventHandler(this.buttonShowDevice_Click);
            // 
            // labelDevice
            // 
            this.labelDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDevice.AutoSize = true;
            this.labelDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDevice.Location = new System.Drawing.Point(0, 116);
            this.labelDevice.Name = "labelDevice";
            this.labelDevice.Size = new System.Drawing.Size(30, 20);
            this.labelDevice.TabIndex = 14;
            this.labelDevice.Text = "Device";
            this.labelDevice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // listGroupType
            // 
            this.listGroupType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listGroupType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listGroupType.FormattingEnabled = true;
            this.listGroupType.Location = new System.Drawing.Point(4, 142);
            this.listGroupType.Name = "listGroupType";
            this.listGroupType.Size = new System.Drawing.Size(165, 21);
            this.listGroupType.TabIndex = 25;
            this.listGroupType.SelectionChangeCommitted += new System.EventHandler(this.listGroupType_SelectionChangeCommitted);
                        // 
            // buttonShowGroupType
            // 
            this.buttonShowGroupType.Location = new System.Drawing.Point(174, 142);
            this.buttonShowGroupType.Name = "buttonShowGroupType";
            this.buttonShowGroupType.Size = new System.Drawing.Size(40, 23);
            this.buttonShowGroupType.TabIndex = 10;
            this.buttonShowGroupType.Text = "...";
            this.buttonShowGroupType.Click += new System.EventHandler(this.buttonShowGroupType_Click);
            // 
            // labelGroupType
            // 
            this.labelGroupType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelGroupType.AutoSize = true;
            this.labelGroupType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGroupType.Location = new System.Drawing.Point(0, 142);
            this.labelGroupType.Name = "labelGroupType";
            this.labelGroupType.Size = new System.Drawing.Size(30, 20);
            this.labelGroupType.TabIndex = 14;
            this.labelGroupType.Text = "GroupType";
            this.labelGroupType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            
            // 
            // FormAlarmEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 208);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormAlarmEditor";
            this.Text = "Alarm Editor";
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
        private System.Windows.Forms.TextBox textboxAlarmId;
private System.Windows.Forms.Label labelAlarmId;
private System.Windows.Forms.NumericUpDown numericValue;
private System.Windows.Forms.Label labelValue;
private System.Windows.Forms.TextBox textboxComment;
private System.Windows.Forms.Label labelComment;
private System.Windows.Forms.ComboBox listAlarmType;
private System.Windows.Forms.Button buttonShowAlarmType;
private System.Windows.Forms.Label labelAlarmType;
private System.Windows.Forms.ComboBox listDevice;
private System.Windows.Forms.Button buttonShowDevice;
private System.Windows.Forms.Label labelDevice;
private System.Windows.Forms.ComboBox listGroupType;
private System.Windows.Forms.Button buttonShowGroupType;
private System.Windows.Forms.Label labelGroupType;

        #endregion
    }
}
