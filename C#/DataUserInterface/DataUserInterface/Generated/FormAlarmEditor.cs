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
                        this.listTypes.Items.Add(val.description);

                    foreach (var val in db.group_type.OrderBy(v => v.name))
                        this.listGroups.Items.Add(val.name);

                    foreach (var val in db.devices.OrderBy(v => v.name))
                        this.listDevices.Items.Add(val.name);
                }

                if (this.mode != EnumEditorMode.Create)
                {
                    var obj = db.alarms.SingleOrDefault(v => v.alarm_id == this.id);
                    if (obj != null)
                    {
                        this.textboxID.Text = Convert.ToString(obj.alarm_id);
                        this.textboxComment.Text = obj.comment;
                        this.numericValue.Value = (decimal)obj.value;

                        foreach (var value in db.alarm_type.OrderBy(v => v.description))
                        {
                            this.listTypes.Items.Add(value.description);
                            if (value.alarm_type_id == obj.alarm_type_id)
                                this.listTypes.SelectedIndex = this.listTypes.Items.Count - 1;
                        }

                        foreach (var value in db.group_type.OrderBy(v => v.name))
                        {
                            this.listGroups.Items.Add(value.name);
                            if (value.group_type_id == obj.group_type_id)
                                this.listGroups.SelectedIndex = this.listGroups.Items.Count - 1;
                        }

                        foreach (var value in db.devices.OrderBy(v => v.name))
                        {
                            this.listDevices.Items.Add(value.name);
                            if (value.device_id == obj.device_id)
                                this.listDevices.SelectedIndex = this.listDevices.Items.Count - 1;
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

            FormHelper.unlimitNumericBox(this.numericValue);

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

                obj.comment = this.textboxComment.Text;
                obj.value = (double)this.numericValue.Value;

                var selectedType = this.listTypes.Items[this.listTypes.SelectedIndex] as string;
                obj.alarm_type = db.alarm_type.Single(v => v.description == selectedType);

                var selectedGroup = this.listGroups.Items[this.listGroups.SelectedIndex] as string;
                obj.group_type = db.group_type.Single(v => v.name == selectedGroup);

                var selectedDevice = this.listDevices.Items[this.listDevices.SelectedIndex] as string;
                obj.device = db.devices.Single(v => v.name == selectedDevice);

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

                obj.comment = this.textboxComment.Text;
                obj.value = (double)this.numericValue.Value;

                var selectedType = this.listTypes.Items[this.listTypes.SelectedIndex] as string;
                obj.alarm_type = db.alarm_type.Single(v => v.description == selectedType);

                var selectedGroup = this.listGroups.Items[this.listGroups.SelectedIndex] as string;
                obj.group_type = db.group_type.Single(v => v.name == selectedGroup);

                var selectedDevice = this.listDevices.Items[this.listDevices.SelectedIndex] as string;
                obj.device = db.devices.Single(v => v.name == selectedDevice);

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

        private void listTypes_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listTypes.SelectedIndex;
            var value = this.listTypes.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || value != this._cached.alarm_type.description)
                this._isDirty = true;
        }

        private void listGroups_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listGroups.SelectedIndex;
            var value = this.listGroups.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || value != this._cached.group_type.name)
                this._isDirty = true;
        }

        private void listDevices_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listDevices.SelectedIndex;
            var value = this.listDevices.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || value != this._cached.device.name)
                this._isDirty = true;
        }

        private void buttonModifyTypes_Click(object sender, EventArgs e)
        {
            var form = new SearchForm(EnumSearchFormType.AlarmType);
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        private void buttonModifyGroups_Click(object sender, EventArgs e)
        {
            var form = new SearchForm(EnumSearchFormType.GroupType);
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        private void buttonModifyDevices_Click(object sender, EventArgs e)
        {
            var form = new SearchForm(EnumSearchFormType.Device);
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        private void numericValue_ValueChanged(object sender, EventArgs e)
        {
            if (this._cached == null)
                return;

            if (Convert.ToDouble(numericValue.Value) != this._cached.value)
                this._isDirty = true;
        }

        private void numericValue_Enter(object sender, EventArgs e)
        {
            FormHelper.selectAllText(this.numericValue);
        }

        private void textboxComment_Leave(object sender, EventArgs e)
        {
            if (textboxComment.Text != this._cached.comment)
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelDirty = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.buttonAction = new System.Windows.Forms.Button();
            this.textboxID = new System.Windows.Forms.TextBox();
            this.buttonModifyGroups = new System.Windows.Forms.Button();
            this.listGroups = new System.Windows.Forms.ComboBox();
            this.buttonModifyDevices = new System.Windows.Forms.Button();
            this.listDevices = new System.Windows.Forms.ComboBox();
            this.buttonModifyTypes = new System.Windows.Forms.Button();
            this.listTypes = new System.Windows.Forms.ComboBox();
            this.textboxComment = new System.Windows.Forms.TextBox();
            this.numericValue = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
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
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.labelDirty);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.numericValue);
            this.splitContainer1.Panel2.Controls.Add(this.textboxComment);
            this.splitContainer1.Panel2.Controls.Add(this.buttonModifyTypes);
            this.splitContainer1.Panel2.Controls.Add(this.listTypes);
            this.splitContainer1.Panel2.Controls.Add(this.buttonModifyDevices);
            this.splitContainer1.Panel2.Controls.Add(this.listDevices);
            this.splitContainer1.Panel2.Controls.Add(this.buttonModifyGroups);
            this.splitContainer1.Panel2.Controls.Add(this.listGroups);
            this.splitContainer1.Panel2.Controls.Add(this.textboxID);
            this.splitContainer1.Panel2.Controls.Add(this.buttonDelete);
            this.splitContainer1.Panel2.Controls.Add(this.buttonReload);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAction);
            this.splitContainer1.Size = new System.Drawing.Size(330, 200);
            this.splitContainer1.SplitterDistance = 109;
            this.splitContainer1.TabIndex = 0;
            // 
            // labelDirty
            // 
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
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDelete.BackColor = System.Drawing.SystemColors.ControlLight;
            this.buttonDelete.Location = new System.Drawing.Point(85, 172);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(50, 23);
            this.buttonDelete.TabIndex = 11;
            this.buttonDelete.UseVisualStyleBackColor = false;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonReload
            // 
            this.buttonReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonReload.Location = new System.Drawing.Point(4, 173);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(75, 23);
            this.buttonReload.TabIndex = 6;
            this.buttonReload.Text = "Reload";
            this.buttonReload.UseVisualStyleBackColor = true;
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            // 
            // buttonAction
            // 
            this.buttonAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAction.Location = new System.Drawing.Point(141, 172);
            this.buttonAction.Name = "buttonAction";
            this.buttonAction.Size = new System.Drawing.Size(75, 23);
            this.buttonAction.TabIndex = 2;
            this.buttonAction.Text = "Save";
            this.buttonAction.UseVisualStyleBackColor = true;
            this.buttonAction.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // textboxID
            // 
            this.textboxID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxID.Enabled = false;
            this.textboxID.Location = new System.Drawing.Point(4, 12);
            this.textboxID.Name = "textboxID";
            this.textboxID.ReadOnly = true;
            this.textboxID.Size = new System.Drawing.Size(208, 20);
            this.textboxID.TabIndex = 14;
            // 
            // buttonModifyGroups
            // 
            this.buttonModifyGroups.Location = new System.Drawing.Point(175, 65);
            this.buttonModifyGroups.Name = "buttonModifyGroups";
            this.buttonModifyGroups.Size = new System.Drawing.Size(40, 23);
            this.buttonModifyGroups.TabIndex = 26;
            this.buttonModifyGroups.Text = "...";
            this.buttonModifyGroups.UseVisualStyleBackColor = true;
            this.buttonModifyGroups.Click += new System.EventHandler(this.buttonModifyGroups_Click);
            // 
            // listGroups
            // 
            this.listGroups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listGroups.FormattingEnabled = true;
            this.listGroups.Location = new System.Drawing.Point(4, 66);
            this.listGroups.Name = "listGroups";
            this.listGroups.Size = new System.Drawing.Size(165, 21);
            this.listGroups.TabIndex = 25;
            this.listGroups.SelectionChangeCommitted += new System.EventHandler(this.listGroups_SelectionChangeCommitted);
            // 
            // buttonModifyDevices
            // 
            this.buttonModifyDevices.Location = new System.Drawing.Point(175, 93);
            this.buttonModifyDevices.Name = "buttonModifyDevices";
            this.buttonModifyDevices.Size = new System.Drawing.Size(40, 23);
            this.buttonModifyDevices.TabIndex = 28;
            this.buttonModifyDevices.Text = "...";
            this.buttonModifyDevices.UseVisualStyleBackColor = true;
            this.buttonModifyDevices.Click += new System.EventHandler(this.buttonModifyDevices_Click);
            // 
            // listDevices
            // 
            this.listDevices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listDevices.FormattingEnabled = true;
            this.listDevices.Location = new System.Drawing.Point(4, 94);
            this.listDevices.Name = "listDevices";
            this.listDevices.Size = new System.Drawing.Size(165, 21);
            this.listDevices.TabIndex = 27;
            this.listDevices.SelectionChangeCommitted += new System.EventHandler(this.listDevices_SelectionChangeCommitted);
            // 
            // buttonModifyTypes
            // 
            this.buttonModifyTypes.Location = new System.Drawing.Point(175, 37);
            this.buttonModifyTypes.Name = "buttonModifyTypes";
            this.buttonModifyTypes.Size = new System.Drawing.Size(40, 23);
            this.buttonModifyTypes.TabIndex = 30;
            this.buttonModifyTypes.Text = "...";
            this.buttonModifyTypes.UseVisualStyleBackColor = true;
            this.buttonModifyTypes.Click += new System.EventHandler(this.buttonModifyTypes_Click);
            // 
            // listTypes
            // 
            this.listTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listTypes.FormattingEnabled = true;
            this.listTypes.Location = new System.Drawing.Point(4, 38);
            this.listTypes.Name = "listTypes";
            this.listTypes.Size = new System.Drawing.Size(165, 21);
            this.listTypes.TabIndex = 29;
            this.listTypes.SelectionChangeCommitted += new System.EventHandler(this.listTypes_SelectionChangeCommitted);
            // 
            // textboxComment
            // 
            this.textboxComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxComment.Location = new System.Drawing.Point(4, 148);
            this.textboxComment.Name = "textboxComment";
            this.textboxComment.Size = new System.Drawing.Size(208, 20);
            this.textboxComment.TabIndex = 31;
            this.textboxComment.Leave += new System.EventHandler(this.textboxComment_Leave);
            // 
            // numericValue
            // 
            this.numericValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericValue.Location = new System.Drawing.Point(4, 122);
            this.numericValue.Name = "numericValue";
            this.numericValue.Size = new System.Drawing.Size(211, 20);
            this.numericValue.TabIndex = 32;
            this.numericValue.ValueChanged += new System.EventHandler(this.numericValue_ValueChanged);
            this.numericValue.Click += new System.EventHandler(this.numericValue_Enter);
            this.numericValue.Enter += new System.EventHandler(this.numericValue_Enter);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(76, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 20);
            this.label1.TabIndex = 14;
            this.label1.Text = "ID:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(59, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 20);
            this.label2.TabIndex = 15;
            this.label2.Text = "Type:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(48, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 20);
            this.label3.TabIndex = 16;
            this.label3.Text = "Group:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(45, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 20);
            this.label4.TabIndex = 17;
            this.label4.Text = "Device:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(52, 122);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 20);
            this.label5.TabIndex = 18;
            this.label5.Text = "Value:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(24, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 20);
            this.label6.TabIndex = 19;
            this.label6.Text = "Comment:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FormAlarmEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 200);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormAlarmEditor";
            this.Text = "Alarm Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDeviceEditor_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
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
        private TextBox textboxID;
        private Button buttonModifyTypes;
        private ComboBox listTypes;
        private Button buttonModifyDevices;
        private ComboBox listDevices;
        private Button buttonModifyGroups;
        private ComboBox listGroups;
        private TextBox textboxComment;
        private NumericUpDown numericValue;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        #endregion
    }
}
