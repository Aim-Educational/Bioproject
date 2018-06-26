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
        private static action_level _defaultObject = new action_level();

        public int id { private set; get; }
        private action_level _cached { set; get; }

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
                    foreach (var type in db.action_type.OrderBy(a => a.description))
                        this.listTypes.Items.Add(type.description);

                    foreach (var dev in db.devices.OrderBy(d => d.name))
                        this.listDevices.Items.Add(dev.name);
                }

                if (this.mode != EnumEditorMode.Create)
                {
                    var obj = db.action_level.SingleOrDefault(v => v.action_level_id == this.id);
                    if (obj != null)
                    {
                        this.textboxID.Text = Convert.ToString(obj.action_level_id);
                        this.numericValue.Value = (decimal)obj.value;
                        this.textComment.Text = obj.comment;

                        foreach (var dev in db.devices.OrderBy(d => d.name))
                        {
                            this.listDevices.Items.Add(dev.name);
                            if (dev.device_id == obj.device_id)
                                this.listDevices.SelectedIndex = this.listDevices.Items.Count - 1;
                        }

                        foreach (var type in db.action_type.OrderBy(a => a.description))
                        {
                            this.listTypes.Items.Add(type.description);
                            if (type.action_type_id == obj.action_type_id)
                                this.listTypes.SelectedIndex = this.listTypes.Items.Count - 1;
                        }

                        this._cached  = obj;
                        this._isDirty = false;
                    }
                }
            }
        }
        
        public FormActionLevelEditor(EnumEditorMode mode, int id = -1) // ID isn't always needed, e.g. Create
        {
            InitializeComponent();

            FormHelper.unlimitNumericBox(this.numericValue);

            this.mode = mode;
            if (mode != EnumEditorMode.Create)
                this.id = id;
            else
                this._cached = FormActionLevelEditor._defaultObject;

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
                var obj = db.action_level.SingleOrDefault(v => v.action_level_id == this.id);

                if (!obj.isDeletable(db))
                {
                    MessageBox.Show("Cannot delete this record as other records are still linked to it.", 
                                    "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this record?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    return;

                db.action_level.Remove(obj);
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
                var obj = db.action_level.SingleOrDefault(v => v.action_level_id == this.id);

                obj.value = (double)this.numericValue.Value;
                obj.comment = this.textComment.Text;

                var selectedType = this.listTypes.Items[this.listTypes.SelectedIndex] as string;
                obj.action_type = db.action_type.Single(t => t.description == selectedType);

                var selectedDevice = this.listDevices.Items[this.listDevices.SelectedIndex] as string;
                obj.device = db.devices.Single(d => d.name == selectedDevice);

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
                var obj = new action_level();

                obj.value = (double)this.numericValue.Value;
                obj.comment = this.textComment.Text;

                var selectedType = this.listTypes.Items[this.listTypes.SelectedIndex] as string;
                obj.action_type = db.action_type.Single(t => t.description == selectedType);

                var selectedDevice = this.listDevices.Items[this.listDevices.SelectedIndex] as string;
                obj.device = db.devices.Single(d => d.name == selectedDevice);

                db.action_level.Add(obj);
                db.SaveChanges();
                this._cached  = obj;
                this.id       = obj.action_level_id;
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

        private void textComment_Leave(object sender, EventArgs e)
        {
            if (textComment.Text != this._cached.comment)
                this._isDirty = true;
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

        private void listTypes_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listTypes.SelectedIndex;
            var value = this.listTypes.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || value != this._cached.action_type.description)
                this._isDirty = true;
        }

        private void listDevices_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listDevices.SelectedIndex;
            var value = this.listDevices.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || value != this._cached.action_type.description)
                this._isDirty = true;
        }

        private void buttonModifyDevices_Click(object sender, EventArgs e)
        {
            var form = new SearchForm(EnumSearchFormType.Device);
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        private void buttonModifyTypes_Click(object sender, EventArgs e)
        {
            var form = new SearchForm(EnumSearchFormType.ActionType);
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelDirty = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textComment = new System.Windows.Forms.TextBox();
            this.numericValue = new System.Windows.Forms.NumericUpDown();
            this.buttonModifyTypes = new System.Windows.Forms.Button();
            this.listTypes = new System.Windows.Forms.ComboBox();
            this.buttonModifyDevices = new System.Windows.Forms.Button();
            this.listDevices = new System.Windows.Forms.ComboBox();
            this.textboxID = new System.Windows.Forms.TextBox();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.buttonAction = new System.Windows.Forms.Button();
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
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.labelDirty);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.textComment);
            this.splitContainer1.Panel2.Controls.Add(this.numericValue);
            this.splitContainer1.Panel2.Controls.Add(this.buttonModifyTypes);
            this.splitContainer1.Panel2.Controls.Add(this.listTypes);
            this.splitContainer1.Panel2.Controls.Add(this.buttonModifyDevices);
            this.splitContainer1.Panel2.Controls.Add(this.listDevices);
            this.splitContainer1.Panel2.Controls.Add(this.textboxID);
            this.splitContainer1.Panel2.Controls.Add(this.buttonDelete);
            this.splitContainer1.Panel2.Controls.Add(this.buttonReload);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAction);
            this.splitContainer1.Size = new System.Drawing.Size(330, 173);
            this.splitContainer1.SplitterDistance = 109;
            this.splitContainer1.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(24, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 20);
            this.label5.TabIndex = 16;
            this.label5.Text = "Comment:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(52, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 20);
            this.label3.TabIndex = 15;
            this.label3.Text = "Value:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(59, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 20);
            this.label2.TabIndex = 14;
            this.label2.Text = "Type:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelDirty
            // 
            this.labelDirty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelDirty.AutoSize = true;
            this.labelDirty.Location = new System.Drawing.Point(3, -178);
            this.labelDirty.Name = "labelDirty";
            this.labelDirty.Size = new System.Drawing.Size(50, 13);
            this.labelDirty.TabIndex = 3;
            this.labelDirty.Text = "Changed";
            this.labelDirty.Visible = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(76, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 20);
            this.label1.TabIndex = 13;
            this.label1.Text = "ID:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(45, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 20);
            this.label4.TabIndex = 13;
            this.label4.Text = "Device:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textComment
            // 
            this.textComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textComment.Location = new System.Drawing.Point(3, 120);
            this.textComment.Name = "textComment";
            this.textComment.Size = new System.Drawing.Size(211, 20);
            this.textComment.TabIndex = 19;
            this.textComment.Leave += new System.EventHandler(this.textComment_Leave);
            // 
            // numericValue
            // 
            this.numericValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericValue.Location = new System.Drawing.Point(3, 94);
            this.numericValue.Name = "numericValue";
            this.numericValue.Size = new System.Drawing.Size(211, 20);
            this.numericValue.TabIndex = 18;
            this.numericValue.ValueChanged += new System.EventHandler(this.numericValue_ValueChanged);
            this.numericValue.Click += new System.EventHandler(this.numericValue_Enter);
            this.numericValue.Enter += new System.EventHandler(this.numericValue_Enter);
            // 
            // buttonModifyTypes
            // 
            this.buttonModifyTypes.Location = new System.Drawing.Point(174, 65);
            this.buttonModifyTypes.Name = "buttonModifyTypes";
            this.buttonModifyTypes.Size = new System.Drawing.Size(40, 23);
            this.buttonModifyTypes.TabIndex = 17;
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
            this.listTypes.Location = new System.Drawing.Point(3, 67);
            this.listTypes.Name = "listTypes";
            this.listTypes.Size = new System.Drawing.Size(165, 21);
            this.listTypes.TabIndex = 16;
            this.listTypes.SelectionChangeCommitted += new System.EventHandler(this.listTypes_SelectionChangeCommitted);
            // 
            // buttonModifyDevices
            // 
            this.buttonModifyDevices.Location = new System.Drawing.Point(174, 38);
            this.buttonModifyDevices.Name = "buttonModifyDevices";
            this.buttonModifyDevices.Size = new System.Drawing.Size(40, 23);
            this.buttonModifyDevices.TabIndex = 15;
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
            this.listDevices.Location = new System.Drawing.Point(3, 40);
            this.listDevices.Name = "listDevices";
            this.listDevices.Size = new System.Drawing.Size(165, 21);
            this.listDevices.TabIndex = 14;
            this.listDevices.SelectionChangeCommitted += new System.EventHandler(this.listDevices_SelectionChangeCommitted);
            // 
            // textboxID
            // 
            this.textboxID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxID.Enabled = false;
            this.textboxID.Location = new System.Drawing.Point(3, 12);
            this.textboxID.Name = "textboxID";
            this.textboxID.ReadOnly = true;
            this.textboxID.Size = new System.Drawing.Size(211, 20);
            this.textboxID.TabIndex = 12;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDelete.BackColor = System.Drawing.SystemColors.ControlLight;
            this.buttonDelete.Location = new System.Drawing.Point(85, 146);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(50, 23);
            this.buttonDelete.TabIndex = 11;
            this.buttonDelete.UseVisualStyleBackColor = false;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonReload
            // 
            this.buttonReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonReload.Location = new System.Drawing.Point(4, 146);
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
            this.buttonAction.Location = new System.Drawing.Point(142, 146);
            this.buttonAction.Name = "buttonAction";
            this.buttonAction.Size = new System.Drawing.Size(75, 23);
            this.buttonAction.TabIndex = 2;
            this.buttonAction.Text = "Save";
            this.buttonAction.UseVisualStyleBackColor = true;
            this.buttonAction.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // FormActionLevelEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 173);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormActionLevelEditor";
            this.Text = "Action Level Editor";
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
        private Label label1;
        private TextBox textboxID;
        private Label label4;
        private Button buttonModifyDevices;
        private ComboBox listDevices;
        private Label label2;
        private Button buttonModifyTypes;
        private ComboBox listTypes;
        private Label label3;
        private NumericUpDown numericValue;
        private Label label5;
        private TextBox textComment;
        #endregion
    }
}
