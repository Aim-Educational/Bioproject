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
    public partial class FormApplicationLogEditor : Form
    {
        private static application_log _defaultObject = new application_log();

        public int id { private set; get; }
        private application_log _cached { set; get; }

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
                    foreach (var val in db.message_type.OrderBy(v => v.description))
                        this.listTypes.Items.Add(val.description);

                    foreach (var val in db.applications.OrderBy(v => v.name))
                        this.listApps.Items.Add(val.name);
                }

                if (this.mode != EnumEditorMode.Create)
                {
                    var obj = db.application_log.SingleOrDefault(v => v.application_log_id == this.id);
                    if (obj != null)
                    {
                        this.textboxID.Text = Convert.ToString(obj.application_id);
                        this.textboxMessage.Text = obj.message;
                        this.datetime.Value = obj.datetime;

                        foreach (var value in db.message_type.OrderBy(v => v.description))
                        {
                            this.listTypes.Items.Add(value.description);
                            if (value.message_type_id == obj.message_type_id)
                                this.listTypes.SelectedIndex = this.listTypes.Items.Count - 1;
                        }

                        foreach (var value in db.applications.OrderBy(v => v.name))
                        {
                            this.listApps.Items.Add(value.name);
                            if (value.application_id == obj.application_id)
                                this.listApps.SelectedIndex = this.listApps.Items.Count - 1;
                        }

                        this._cached  = obj;
                        this._isDirty = false;
                    }
                }
            }
        }
        
        public FormApplicationLogEditor(EnumEditorMode mode, int id = -1) // ID isn't always needed, e.g. Create
        {
            InitializeComponent();

            this.mode = mode;
            if (mode != EnumEditorMode.Create)
                this.id = id;
            else
                this._cached = FormApplicationLogEditor._defaultObject;

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
                var obj = db.application_log.SingleOrDefault(v => v.application_log_id == this.id);

                if (!obj.isDeletable(db))
                {
                    MessageBox.Show("Cannot delete this record as other records are still linked to it.", 
                                    "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this record?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    return;

                db.application_log.Remove(obj);
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
                var obj = db.application_log.SingleOrDefault(v => v.application_log_id == this.id);

                this.datetime.Value = DateTime.Now;

                obj.datetime = this.datetime.Value;
                obj.message = this.textboxMessage.Text;

                var selectedType = this.listTypes.Items[this.listTypes.SelectedIndex] as string;
                obj.message_type = db.message_type.Single(v => v.description == selectedType);

                var selectedApps = this.listApps.Items[this.listApps.SelectedIndex] as string;
                obj.application = db.applications.Single(v => v.name == selectedType);

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
                var obj = new application_log();

                obj.datetime = this.datetime.Value;
                obj.message = this.textboxMessage.Text;

                var selectedType = this.listTypes.Items[this.listTypes.SelectedIndex] as string;
                obj.message_type = db.message_type.Single(v => v.description == selectedType);

                var selectedApps = this.listApps.Items[this.listApps.SelectedIndex] as string;
                obj.application = db.applications.Single(v => v.name == selectedApps);

                db.application_log.Add(obj);
                db.SaveChanges();
                this._cached  = obj;
                this.id       = obj.application_log_id;
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

            if (this.mode == EnumEditorMode.Create || value != this._cached.message_type.description)
                this._isDirty = true;
        }

        private void listApps_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listApps.SelectedIndex;
            var value = this.listApps.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || value != this._cached.application.name)
                this._isDirty = true;
        }

        private void datetime_ValueChanged(object sender, EventArgs e)
        {
            if (this.mode == EnumEditorMode.Create || this.datetime.Value != this._cached.datetime)
                this._isDirty = true;
        }

        private void textboxMessage_Leave(object sender, EventArgs e)
        {
            if (this.mode == EnumEditorMode.Create || this.textboxMessage.Text != this._cached.message)
                this._isDirty = true;
        }

        private void buttonModifyTypes_Click(object sender, EventArgs e)
        {
            var form = new SearchForm(EnumSearchFormType.MessageType);
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        private void buttonModifyApps_Click(object sender, EventArgs e)
        {
            var form = new SearchForm(EnumSearchFormType.Application);
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
            this.labelDirty = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.buttonAction = new System.Windows.Forms.Button();
            this.datetime = new System.Windows.Forms.DateTimePicker();
            this.textboxID = new System.Windows.Forms.TextBox();
            this.buttonModifyTypes = new System.Windows.Forms.Button();
            this.listTypes = new System.Windows.Forms.ComboBox();
            this.textboxMessage = new System.Windows.Forms.TextBox();
            this.buttonModifyApps = new System.Windows.Forms.Button();
            this.listApps = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.labelDirty);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.buttonModifyApps);
            this.splitContainer1.Panel2.Controls.Add(this.listApps);
            this.splitContainer1.Panel2.Controls.Add(this.textboxMessage);
            this.splitContainer1.Panel2.Controls.Add(this.buttonModifyTypes);
            this.splitContainer1.Panel2.Controls.Add(this.listTypes);
            this.splitContainer1.Panel2.Controls.Add(this.textboxID);
            this.splitContainer1.Panel2.Controls.Add(this.datetime);
            this.splitContainer1.Panel2.Controls.Add(this.buttonDelete);
            this.splitContainer1.Panel2.Controls.Add(this.buttonReload);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAction);
            this.splitContainer1.Size = new System.Drawing.Size(330, 173);
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
            this.buttonDelete.Location = new System.Drawing.Point(82, 146);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(50, 23);
            this.buttonDelete.TabIndex = 11;
            this.buttonDelete.UseVisualStyleBackColor = false;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonReload
            // 
            this.buttonReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonReload.Location = new System.Drawing.Point(1, 147);
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
            this.buttonAction.Location = new System.Drawing.Point(138, 146);
            this.buttonAction.Name = "buttonAction";
            this.buttonAction.Size = new System.Drawing.Size(75, 23);
            this.buttonAction.TabIndex = 2;
            this.buttonAction.Text = "Save";
            this.buttonAction.UseVisualStyleBackColor = true;
            this.buttonAction.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // datetime
            // 
            this.datetime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.datetime.Enabled = false;
            this.datetime.Location = new System.Drawing.Point(1, 92);
            this.datetime.Name = "datetime";
            this.datetime.Size = new System.Drawing.Size(208, 20);
            this.datetime.TabIndex = 12;
            this.datetime.ValueChanged += new System.EventHandler(this.datetime_ValueChanged);
            // 
            // textboxID
            // 
            this.textboxID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxID.Enabled = false;
            this.textboxID.Location = new System.Drawing.Point(1, 12);
            this.textboxID.Name = "textboxID";
            this.textboxID.ReadOnly = true;
            this.textboxID.Size = new System.Drawing.Size(208, 20);
            this.textboxID.TabIndex = 15;
            // 
            // buttonModifyTypes
            // 
            this.buttonModifyTypes.Location = new System.Drawing.Point(172, 37);
            this.buttonModifyTypes.Name = "buttonModifyTypes";
            this.buttonModifyTypes.Size = new System.Drawing.Size(40, 23);
            this.buttonModifyTypes.TabIndex = 32;
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
            this.listTypes.Location = new System.Drawing.Point(1, 38);
            this.listTypes.Name = "listTypes";
            this.listTypes.Size = new System.Drawing.Size(165, 21);
            this.listTypes.TabIndex = 31;
            this.listTypes.SelectionChangeCommitted += new System.EventHandler(this.listTypes_SelectionChangeCommitted);
            // 
            // textboxMessage
            // 
            this.textboxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxMessage.Location = new System.Drawing.Point(1, 118);
            this.textboxMessage.Name = "textboxMessage";
            this.textboxMessage.Size = new System.Drawing.Size(208, 20);
            this.textboxMessage.TabIndex = 33;
            this.textboxMessage.Leave += new System.EventHandler(this.textboxMessage_Leave);
            // 
            // buttonModifyApps
            // 
            this.buttonModifyApps.Location = new System.Drawing.Point(172, 64);
            this.buttonModifyApps.Name = "buttonModifyApps";
            this.buttonModifyApps.Size = new System.Drawing.Size(40, 23);
            this.buttonModifyApps.TabIndex = 35;
            this.buttonModifyApps.Text = "...";
            this.buttonModifyApps.UseVisualStyleBackColor = true;
            this.buttonModifyApps.Click += new System.EventHandler(this.buttonModifyApps_Click);
            // 
            // listApps
            // 
            this.listApps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listApps.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listApps.FormattingEnabled = true;
            this.listApps.Location = new System.Drawing.Point(1, 65);
            this.listApps.Name = "listApps";
            this.listApps.Size = new System.Drawing.Size(165, 21);
            this.listApps.TabIndex = 34;
            this.listApps.SelectionChangeCommitted += new System.EventHandler(this.listApps_SelectionChangeCommitted);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(72, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 20);
            this.label2.TabIndex = 16;
            this.label2.Text = "ID:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(21, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 20);
            this.label1.TabIndex = 17;
            this.label1.Text = "Msg Type:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(11, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 20);
            this.label3.TabIndex = 18;
            this.label3.Text = "Application:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(24, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 20);
            this.label4.TabIndex = 19;
            this.label4.Text = "Datetime:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(24, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 20);
            this.label5.TabIndex = 20;
            this.label5.Text = "Message:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FormApplicationLogEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 173);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormApplicationLogEditor";
            this.Text = "Application Log Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDeviceEditor_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonAction;
        private System.Windows.Forms.Label labelDirty;
        private System.Windows.Forms.Button buttonReload;
        private System.Windows.Forms.Button buttonDelete;
        private DateTimePicker datetime;
        private TextBox textboxID;
        private Button buttonModifyTypes;
        private ComboBox listTypes;
        private TextBox textboxMessage;
        private Button buttonModifyApps;
        private ComboBox listApps;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label1;
        private Label label2;
        #endregion
    }
}
