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
    public partial class FormRSSConfigurationEditor : Form
    {
        private static rss_configuration _defaultObject = new rss_configuration();

        public int id { private set; get; }
        private rss_configuration _cached { set; get; }

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
    foreach (var val in db.update_period.OrderBy(v => v.description))
    this.listUpdatePeriod.Items.Add(val.description);
    
                }

                if (this.mode != EnumEditorMode.Create)
                {
                    var obj = db.rss_configuration.SingleOrDefault(v => v.rss_configuration_id == this.id);
                    if (obj != null)
                    {
                        this.textboxRSSConfigurationId.Text = Convert.ToString(obj.rss_configuration_id);
foreach (var value in db.devices.OrderBy(v => v.name))
{
    this.listDevice.Items.Add(value.name);
    if (value.device_id == obj.device_id)
        this.listDevice.SelectedIndex = this.listDevice.Items.Count - 1;
}
foreach (var value in db.update_period.OrderBy(v => v.description))
{
    this.listUpdatePeriod.Items.Add(value.description);
    if (value.update_period_id == obj.update_period_id)
        this.listUpdatePeriod.SelectedIndex = this.listUpdatePeriod.Items.Count - 1;
}
this.textboxDescription.Text = obj.description;
this.datetimeLastUpdate.Value = obj.last_update;
this.numericUpdateFrequency.Value = (decimal)obj.update_frequency;
this.textboxRSSURL.Text = obj.rss_url;


                        this._cached  = obj;
                        this._isDirty = false;
                    }
                }
            }
        }
        
        public FormRSSConfigurationEditor(EnumEditorMode mode, int id = -1) // ID isn't always needed, e.g. Create
        {
            InitializeComponent();

            FormHelper.unlimitNumericBox(this.numericUpdateFrequency, AllowDecimals.yes);


            this.mode = mode;
            if (mode != EnumEditorMode.Create)
                this.id = id;
            else
                this._cached = FormRSSConfigurationEditor._defaultObject;

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
                var obj = db.rss_configuration.SingleOrDefault(v => v.rss_configuration_id == this.id);

                if (!obj.isDeletable(db))
                {
                    MessageBox.Show("Cannot delete this record as other records are still linked to it.", 
                                    "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this record?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    return;

                db.rss_configuration.Remove(obj);
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
                var obj = db.rss_configuration.SingleOrDefault(v => v.rss_configuration_id == this.id);

                
var selectedDevice = this.listDevice.Items[this.listDevice.SelectedIndex] as string;
obj.device = db.devices.Single(v => v.name == selectedDevice);
var selectedUpdatePeriod = this.listUpdatePeriod.Items[this.listUpdatePeriod.SelectedIndex] as string;
obj.update_period = db.update_period.Single(v => v.description == selectedUpdatePeriod);
obj.description = this.textboxDescription.Text;
this.datetimeLastUpdate.Value = DateTime.Now;
obj.last_update = this.datetimeLastUpdate.Value;
obj.update_frequency = (double)this.numericUpdateFrequency.Value;
obj.rss_url = this.textboxRSSURL.Text;


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
                var obj = new rss_configuration();

                
var selectedDevice = this.listDevice.Items[this.listDevice.SelectedIndex] as string;
obj.device = db.devices.Single(v => v.name == selectedDevice);
var selectedUpdatePeriod = this.listUpdatePeriod.Items[this.listUpdatePeriod.SelectedIndex] as string;
obj.update_period = db.update_period.Single(v => v.description == selectedUpdatePeriod);
obj.description = this.textboxDescription.Text;
this.datetimeLastUpdate.Value = DateTime.Now;
obj.last_update = this.datetimeLastUpdate.Value;
obj.update_frequency = (double)this.numericUpdateFrequency.Value;
obj.rss_url = this.textboxRSSURL.Text;


                db.rss_configuration.Add(obj);
                db.SaveChanges();
                this._cached  = obj;
                this.id       = obj.rss_configuration_id;
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

                private void textboxRSSConfigurationId_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxRSSConfigurationId.Text != Convert.ToString(this._cached.rss_configuration_id))
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
        private void listUpdatePeriod_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listUpdatePeriod.SelectedIndex;
            var value = this.listUpdatePeriod.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || (this._cached.update_period != null && value != this._cached.update_period.description))
                this._isDirty = true;
        }
        private void buttonShowUpdatePeriod_Click(object sender, EventArgs e)
{
    var form = new SearchForm(EnumSearchFormType.UpdatePeriod);
    form.MdiParent = this.MdiParent;
    form.Show();
}
        private void textboxDescription_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxDescription.Text != Convert.ToString(this._cached.description))
                this._isDirty = true;
        }
                private void numericUpdateFrequency_Enter(object sender, EventArgs e)
        {
            FormHelper.selectAllText(this.numericUpdateFrequency);
        }
        private void numericUpdateFrequency_ValueChanged(object sender, EventArgs e)
        {
            if(this._cached == null)
                return;

            if (Convert.ToDouble(this.numericUpdateFrequency.Value) != this._cached.update_frequency)
                this._isDirty = true;
        }
        private void textboxRSSURL_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxRSSURL.Text != Convert.ToString(this._cached.rss_url))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRSSConfigurationEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelDirty = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.buttonAction = new System.Windows.Forms.Button();
            this.textboxRSSConfigurationId = new System.Windows.Forms.TextBox();
this.labelRSSConfigurationId = new System.Windows.Forms.Label();
this.listDevice = new System.Windows.Forms.ComboBox();
this.buttonShowDevice = new System.Windows.Forms.Button();
this.labelDevice = new System.Windows.Forms.Label();
this.listUpdatePeriod = new System.Windows.Forms.ComboBox();
this.buttonShowUpdatePeriod = new System.Windows.Forms.Button();
this.labelUpdatePeriod = new System.Windows.Forms.Label();
this.textboxDescription = new System.Windows.Forms.TextBox();
this.labelDescription = new System.Windows.Forms.Label();
this.datetimeLastUpdate = new System.Windows.Forms.DateTimePicker();
this.labelLastUpdate = new System.Windows.Forms.Label();
this.numericUpdateFrequency = new System.Windows.Forms.NumericUpDown();
this.labelUpdateFrequency = new System.Windows.Forms.Label();
this.textboxRSSURL = new System.Windows.Forms.TextBox();
this.labelRSSURL = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpdateFrequency)).BeginInit();

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
            this.splitContainer1.Panel1.Controls.Add(labelRSSConfigurationId);
this.splitContainer1.Panel1.Controls.Add(labelDevice);
this.splitContainer1.Panel1.Controls.Add(labelUpdatePeriod);
this.splitContainer1.Panel1.Controls.Add(labelDescription);
this.splitContainer1.Panel1.Controls.Add(labelLastUpdate);
this.splitContainer1.Panel1.Controls.Add(labelUpdateFrequency);
this.splitContainer1.Panel1.Controls.Add(labelRSSURL);

            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.buttonDelete);
            this.splitContainer1.Panel2.Controls.Add(this.buttonReload);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAction);
            this.splitContainer1.Panel2.Controls.Add(textboxRSSConfigurationId);
this.splitContainer1.Panel2.Controls.Add(listDevice);
this.splitContainer1.Panel2.Controls.Add(buttonShowDevice);
this.splitContainer1.Panel2.Controls.Add(listUpdatePeriod);
this.splitContainer1.Panel2.Controls.Add(buttonShowUpdatePeriod);
this.splitContainer1.Panel2.Controls.Add(textboxDescription);
this.splitContainer1.Panel2.Controls.Add(datetimeLastUpdate);
this.splitContainer1.Panel2.Controls.Add(numericUpdateFrequency);
this.splitContainer1.Panel2.Controls.Add(textboxRSSURL);

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
            // textboxRSSConfigurationId
            // 
            this.textboxRSSConfigurationId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxRSSConfigurationId.Location = new System.Drawing.Point(4, 12);
            this.textboxRSSConfigurationId.Name = "textboxRSSConfigurationId";
            this.textboxRSSConfigurationId.Size = new System.Drawing.Size(208, 20);
            this.textboxRSSConfigurationId.TabIndex = 0;
            this.textboxRSSConfigurationId.Leave += new System.EventHandler(this.textboxRSSConfigurationId_Leave);
            this.textboxRSSConfigurationId.Enabled = false;
                        // 
            // labelRSSConfigurationId
            // 
            this.labelRSSConfigurationId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRSSConfigurationId.AutoSize = true;
            this.labelRSSConfigurationId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRSSConfigurationId.Location = new System.Drawing.Point(0, 12);
            this.labelRSSConfigurationId.Name = "labelRSSConfigurationId";
            this.labelRSSConfigurationId.Size = new System.Drawing.Size(30, 20);
            this.labelRSSConfigurationId.TabIndex = 0;
            this.labelRSSConfigurationId.Text = "ID";
            this.labelRSSConfigurationId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // listUpdatePeriod
            // 
            this.listUpdatePeriod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listUpdatePeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listUpdatePeriod.FormattingEnabled = true;
            this.listUpdatePeriod.Location = new System.Drawing.Point(4, 64);
            this.listUpdatePeriod.Name = "listUpdatePeriod";
            this.listUpdatePeriod.Size = new System.Drawing.Size(165, 21);
            this.listUpdatePeriod.TabIndex = 3;
            this.listUpdatePeriod.SelectionChangeCommitted += new System.EventHandler(this.listUpdatePeriod_SelectionChangeCommitted);
                        // 
            // buttonShowUpdatePeriod
            // 
            this.buttonShowUpdatePeriod.Location = new System.Drawing.Point(174, 64);
            this.buttonShowUpdatePeriod.Name = "buttonShowUpdatePeriod";
            this.buttonShowUpdatePeriod.Size = new System.Drawing.Size(40, 23);
            this.buttonShowUpdatePeriod.TabIndex = 4;
            this.buttonShowUpdatePeriod.Text = "...";
            this.buttonShowUpdatePeriod.Click += new System.EventHandler(this.buttonShowUpdatePeriod_Click);
            // 
            // labelUpdatePeriod
            // 
            this.labelUpdatePeriod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUpdatePeriod.AutoSize = true;
            this.labelUpdatePeriod.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUpdatePeriod.Location = new System.Drawing.Point(0, 64);
            this.labelUpdatePeriod.Name = "labelUpdatePeriod";
            this.labelUpdatePeriod.Size = new System.Drawing.Size(30, 20);
            this.labelUpdatePeriod.TabIndex = 0;
            this.labelUpdatePeriod.Text = "UpdatePeriod";
            this.labelUpdatePeriod.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxDescription
            // 
            this.textboxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxDescription.Location = new System.Drawing.Point(4, 90);
            this.textboxDescription.Name = "textboxDescription";
            this.textboxDescription.Size = new System.Drawing.Size(208, 20);
            this.textboxDescription.TabIndex = 5;
            this.textboxDescription.Leave += new System.EventHandler(this.textboxDescription_Leave);
            this.textboxDescription.Enabled = true;
                        // 
            // labelDescription
            // 
            this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDescription.AutoSize = true;
            this.labelDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDescription.Location = new System.Drawing.Point(0, 90);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(30, 20);
            this.labelDescription.TabIndex = 0;
            this.labelDescription.Text = "Description";
            this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // datetimeLastUpdate
            // 
            this.datetimeLastUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.datetimeLastUpdate.Enabled = false;
            this.datetimeLastUpdate.Location = new System.Drawing.Point(4, 116);
            this.datetimeLastUpdate.Name = "datetimeLastUpdate";
            this.datetimeLastUpdate.Size = new System.Drawing.Size(208, 20);
            this.datetimeLastUpdate.TabIndex = 6;
            // 
            // labelLastUpdate
            // 
            this.labelLastUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelLastUpdate.AutoSize = true;
            this.labelLastUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLastUpdate.Location = new System.Drawing.Point(0, 116);
            this.labelLastUpdate.Name = "labelLastUpdate";
            this.labelLastUpdate.Size = new System.Drawing.Size(30, 20);
            this.labelLastUpdate.TabIndex = 0;
            this.labelLastUpdate.Text = "LastUpdate";
            this.labelLastUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // numericUpdateFrequency
            // 
            this.numericUpdateFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpdateFrequency.Location = new System.Drawing.Point(4, 142);
            this.numericUpdateFrequency.Name = "numericUpdateFrequency";
            this.numericUpdateFrequency.Size = new System.Drawing.Size(211, 20);
            this.numericUpdateFrequency.TabIndex = 7;
            this.numericUpdateFrequency.ValueChanged += new System.EventHandler(this.numericUpdateFrequency_ValueChanged);
            this.numericUpdateFrequency.Click += new System.EventHandler(this.numericUpdateFrequency_Enter);
            this.numericUpdateFrequency.Enter += new System.EventHandler(this.numericUpdateFrequency_Enter);
                        // 
            // labelUpdateFrequency
            // 
            this.labelUpdateFrequency.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUpdateFrequency.AutoSize = true;
            this.labelUpdateFrequency.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUpdateFrequency.Location = new System.Drawing.Point(0, 142);
            this.labelUpdateFrequency.Name = "labelUpdateFrequency";
            this.labelUpdateFrequency.Size = new System.Drawing.Size(30, 20);
            this.labelUpdateFrequency.TabIndex = 0;
            this.labelUpdateFrequency.Text = "UpdateFrequency";
            this.labelUpdateFrequency.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxRSSURL
            // 
            this.textboxRSSURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxRSSURL.Location = new System.Drawing.Point(4, 168);
            this.textboxRSSURL.Name = "textboxRSSURL";
            this.textboxRSSURL.Size = new System.Drawing.Size(208, 20);
            this.textboxRSSURL.TabIndex = 8;
            this.textboxRSSURL.Leave += new System.EventHandler(this.textboxRSSURL_Leave);
            this.textboxRSSURL.Enabled = true;
                        // 
            // labelRSSURL
            // 
            this.labelRSSURL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRSSURL.AutoSize = true;
            this.labelRSSURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRSSURL.Location = new System.Drawing.Point(0, 168);
            this.labelRSSURL.Name = "labelRSSURL";
            this.labelRSSURL.Size = new System.Drawing.Size(30, 20);
            this.labelRSSURL.TabIndex = 0;
            this.labelRSSURL.Text = "RSSURL";
            this.labelRSSURL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            
            // 
            // FormRSSConfigurationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 234);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormRSSConfigurationEditor";
            this.Text = "RSSConfiguration Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDeviceEditor_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpdateFrequency)).EndInit();

            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonAction;
        private System.Windows.Forms.Label labelDirty;
        private System.Windows.Forms.Button buttonReload;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.TextBox textboxRSSConfigurationId;
private System.Windows.Forms.Label labelRSSConfigurationId;
private System.Windows.Forms.ComboBox listDevice;
private System.Windows.Forms.Button buttonShowDevice;
private System.Windows.Forms.Label labelDevice;
private System.Windows.Forms.ComboBox listUpdatePeriod;
private System.Windows.Forms.Button buttonShowUpdatePeriod;
private System.Windows.Forms.Label labelUpdatePeriod;
private System.Windows.Forms.TextBox textboxDescription;
private System.Windows.Forms.Label labelDescription;
private System.Windows.Forms.DateTimePicker datetimeLastUpdate;
private System.Windows.Forms.Label labelLastUpdate;
private System.Windows.Forms.NumericUpDown numericUpdateFrequency;
private System.Windows.Forms.Label labelUpdateFrequency;
private System.Windows.Forms.TextBox textboxRSSURL;
private System.Windows.Forms.Label labelRSSURL;

        #endregion
    }
}
