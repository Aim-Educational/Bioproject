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
                    foreach (var val in db.applications.OrderBy(v => v.name))
    this.listApplication.Items.Add(val.name);
    foreach (var val in db.message_type.OrderBy(v => v.description))
    this.listMessageType.Items.Add(val.description);
    
                }

                if (this.mode != EnumEditorMode.Create)
                {
                    var obj = db.application_log.SingleOrDefault(v => v.application_log_id == this.id);
                    if (obj != null)
                    {
                        this.textboxApplicationLogId.Text = Convert.ToString(obj.application_log_id);
foreach (var value in db.applications.OrderBy(v => v.name))
{
    this.listApplication.Items.Add(value.name);
    if (value.application_id == obj.application_id)
        this.listApplication.SelectedIndex = this.listApplication.Items.Count - 1;
}
foreach (var value in db.message_type.OrderBy(v => v.description))
{
    this.listMessageType.Items.Add(value.description);
    if (value.message_type_id == obj.message_type_id)
        this.listMessageType.SelectedIndex = this.listMessageType.Items.Count - 1;
}
this.textboxMessage.Text = obj.message;
this.datetimeDatetime.Value = obj.datetime;


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

                
var selectedApplication = this.listApplication.Items[this.listApplication.SelectedIndex] as string;
obj.application = db.applications.Single(v => v.name == selectedApplication);
var selectedMessageType = this.listMessageType.Items[this.listMessageType.SelectedIndex] as string;
obj.message_type = db.message_type.Single(v => v.description == selectedMessageType);
obj.message = this.textboxMessage.Text;
this.datetimeDatetime.Value = DateTime.Now;
obj.datetime = this.datetimeDatetime.Value;


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

                
var selectedApplication = this.listApplication.Items[this.listApplication.SelectedIndex] as string;
obj.application = db.applications.Single(v => v.name == selectedApplication);
var selectedMessageType = this.listMessageType.Items[this.listMessageType.SelectedIndex] as string;
obj.message_type = db.message_type.Single(v => v.description == selectedMessageType);
obj.message = this.textboxMessage.Text;
this.datetimeDatetime.Value = DateTime.Now;
obj.datetime = this.datetimeDatetime.Value;


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

                private void textboxApplicationLogId_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxApplicationLogId.Text != Convert.ToString(this._cached.application_log_id))
                this._isDirty = true;
        }
                private void listApplication_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listApplication.SelectedIndex;
            var value = this.listApplication.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || (this._cached.application != null && value != this._cached.application.name))
                this._isDirty = true;
        }
        private void buttonShowApplication_Click(object sender, EventArgs e)
{
    var form = new SearchForm(EnumSearchFormType.Application);
    form.MdiParent = this.MdiParent;
    form.Show();
}
        private void listMessageType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listMessageType.SelectedIndex;
            var value = this.listMessageType.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || (this._cached.message_type != null && value != this._cached.message_type.description))
                this._isDirty = true;
        }
        private void buttonShowMessageType_Click(object sender, EventArgs e)
{
    var form = new SearchForm(EnumSearchFormType.MessageType);
    form.MdiParent = this.MdiParent;
    form.Show();
}
        private void textboxMessage_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxMessage.Text != Convert.ToString(this._cached.message))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApplicationLogEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelDirty = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.buttonAction = new System.Windows.Forms.Button();
            this.textboxApplicationLogId = new System.Windows.Forms.TextBox();
this.labelApplicationLogId = new System.Windows.Forms.Label();
this.listApplication = new System.Windows.Forms.ComboBox();
this.buttonShowApplication = new System.Windows.Forms.Button();
this.labelApplication = new System.Windows.Forms.Label();
this.listMessageType = new System.Windows.Forms.ComboBox();
this.buttonShowMessageType = new System.Windows.Forms.Button();
this.labelMessageType = new System.Windows.Forms.Label();
this.textboxMessage = new System.Windows.Forms.TextBox();
this.labelMessage = new System.Windows.Forms.Label();
this.datetimeDatetime = new System.Windows.Forms.DateTimePicker();
this.labelDatetime = new System.Windows.Forms.Label();

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
            this.splitContainer1.Panel1.Controls.Add(this.labelDirty);
            this.splitContainer1.Panel1.Controls.Add(labelApplicationLogId);
this.splitContainer1.Panel1.Controls.Add(labelApplication);
this.splitContainer1.Panel1.Controls.Add(labelMessageType);
this.splitContainer1.Panel1.Controls.Add(labelMessage);
this.splitContainer1.Panel1.Controls.Add(labelDatetime);

            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.buttonDelete);
            this.splitContainer1.Panel2.Controls.Add(this.buttonReload);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAction);
            this.splitContainer1.Panel2.Controls.Add(textboxApplicationLogId);
this.splitContainer1.Panel2.Controls.Add(listApplication);
this.splitContainer1.Panel2.Controls.Add(buttonShowApplication);
this.splitContainer1.Panel2.Controls.Add(listMessageType);
this.splitContainer1.Panel2.Controls.Add(buttonShowMessageType);
this.splitContainer1.Panel2.Controls.Add(textboxMessage);
this.splitContainer1.Panel2.Controls.Add(datetimeDatetime);

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
            this.buttonDelete.Location = new System.Drawing.Point(85, 156);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Text = "[X]";
            this.buttonDelete.Size = new System.Drawing.Size(50, 23);
            this.buttonDelete.TabIndex = 11;
            this.buttonDelete.UseVisualStyleBackColor = false;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonReload
            // 
            this.buttonReload.Location = new System.Drawing.Point(4, 156);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(75, 23);
            this.buttonReload.TabIndex = 6;
            this.buttonReload.Text = "Reload";
            this.buttonReload.UseVisualStyleBackColor = true;
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            // 
            // buttonAction
            // 
            this.buttonAction.Location = new System.Drawing.Point(141, 156);
            this.buttonAction.Name = "buttonAction";
            this.buttonAction.Size = new System.Drawing.Size(75, 23);
            this.buttonAction.TabIndex = 2;
            this.buttonAction.Text = "Save";
            this.buttonAction.UseVisualStyleBackColor = true;
            this.buttonAction.Click += new System.EventHandler(this.buttonSave_Click);
                        // 
            // textboxApplicationLogId
            // 
            this.textboxApplicationLogId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxApplicationLogId.Location = new System.Drawing.Point(4, 12);
            this.textboxApplicationLogId.Name = "textboxApplicationLogId";
            this.textboxApplicationLogId.Size = new System.Drawing.Size(208, 20);
            this.textboxApplicationLogId.TabIndex = 31;
            this.textboxApplicationLogId.Leave += new System.EventHandler(this.textboxApplicationLogId_Leave);
            this.textboxApplicationLogId.Enabled = false;
                        // 
            // labelApplicationLogId
            // 
            this.labelApplicationLogId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelApplicationLogId.AutoSize = true;
            this.labelApplicationLogId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelApplicationLogId.Location = new System.Drawing.Point(0, 12);
            this.labelApplicationLogId.Name = "labelApplicationLogId";
            this.labelApplicationLogId.Size = new System.Drawing.Size(30, 20);
            this.labelApplicationLogId.TabIndex = 14;
            this.labelApplicationLogId.Text = "ID";
            this.labelApplicationLogId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // listApplication
            // 
            this.listApplication.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listApplication.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listApplication.FormattingEnabled = true;
            this.listApplication.Location = new System.Drawing.Point(4, 38);
            this.listApplication.Name = "listApplication";
            this.listApplication.Size = new System.Drawing.Size(165, 21);
            this.listApplication.TabIndex = 25;
            this.listApplication.SelectionChangeCommitted += new System.EventHandler(this.listApplication_SelectionChangeCommitted);
                        // 
            // buttonShowApplication
            // 
            this.buttonShowApplication.Location = new System.Drawing.Point(174, 38);
            this.buttonShowApplication.Name = "buttonShowApplication";
            this.buttonShowApplication.Size = new System.Drawing.Size(40, 23);
            this.buttonShowApplication.TabIndex = 10;
            this.buttonShowApplication.Text = "...";
            this.buttonShowApplication.Click += new System.EventHandler(this.buttonShowApplication_Click);
            // 
            // labelApplication
            // 
            this.labelApplication.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelApplication.AutoSize = true;
            this.labelApplication.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelApplication.Location = new System.Drawing.Point(0, 38);
            this.labelApplication.Name = "labelApplication";
            this.labelApplication.Size = new System.Drawing.Size(30, 20);
            this.labelApplication.TabIndex = 14;
            this.labelApplication.Text = "Application";
            this.labelApplication.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // listMessageType
            // 
            this.listMessageType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listMessageType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listMessageType.FormattingEnabled = true;
            this.listMessageType.Location = new System.Drawing.Point(4, 64);
            this.listMessageType.Name = "listMessageType";
            this.listMessageType.Size = new System.Drawing.Size(165, 21);
            this.listMessageType.TabIndex = 25;
            this.listMessageType.SelectionChangeCommitted += new System.EventHandler(this.listMessageType_SelectionChangeCommitted);
                        // 
            // buttonShowMessageType
            // 
            this.buttonShowMessageType.Location = new System.Drawing.Point(174, 64);
            this.buttonShowMessageType.Name = "buttonShowMessageType";
            this.buttonShowMessageType.Size = new System.Drawing.Size(40, 23);
            this.buttonShowMessageType.TabIndex = 10;
            this.buttonShowMessageType.Text = "...";
            this.buttonShowMessageType.Click += new System.EventHandler(this.buttonShowMessageType_Click);
            // 
            // labelMessageType
            // 
            this.labelMessageType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMessageType.AutoSize = true;
            this.labelMessageType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessageType.Location = new System.Drawing.Point(0, 64);
            this.labelMessageType.Name = "labelMessageType";
            this.labelMessageType.Size = new System.Drawing.Size(30, 20);
            this.labelMessageType.TabIndex = 14;
            this.labelMessageType.Text = "MessageType";
            this.labelMessageType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxMessage
            // 
            this.textboxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxMessage.Location = new System.Drawing.Point(4, 90);
            this.textboxMessage.Name = "textboxMessage";
            this.textboxMessage.Size = new System.Drawing.Size(208, 20);
            this.textboxMessage.TabIndex = 31;
            this.textboxMessage.Leave += new System.EventHandler(this.textboxMessage_Leave);
            this.textboxMessage.Enabled = true;
                        // 
            // labelMessage
            // 
            this.labelMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMessage.AutoSize = true;
            this.labelMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessage.Location = new System.Drawing.Point(0, 90);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(30, 20);
            this.labelMessage.TabIndex = 14;
            this.labelMessage.Text = "Message";
            this.labelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // datetimeDatetime
            // 
            this.datetimeDatetime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.datetimeDatetime.Enabled = false;
            this.datetimeDatetime.Location = new System.Drawing.Point(4, 116);
            this.datetimeDatetime.Name = "datetimeDatetime";
            this.datetimeDatetime.Size = new System.Drawing.Size(208, 20);
            this.datetimeDatetime.TabIndex = 34;
            // 
            // labelDatetime
            // 
            this.labelDatetime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDatetime.AutoSize = true;
            this.labelDatetime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDatetime.Location = new System.Drawing.Point(0, 116);
            this.labelDatetime.Name = "labelDatetime";
            this.labelDatetime.Size = new System.Drawing.Size(30, 20);
            this.labelDatetime.TabIndex = 14;
            this.labelDatetime.Text = "Datetime";
            this.labelDatetime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            
            // 
            // FormApplicationLogEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 182);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormApplicationLogEditor";
            this.Text = "ApplicationLog Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDeviceEditor_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonAction;
        private System.Windows.Forms.Label labelDirty;
        private System.Windows.Forms.Button buttonReload;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.TextBox textboxApplicationLogId;
private System.Windows.Forms.Label labelApplicationLogId;
private System.Windows.Forms.ComboBox listApplication;
private System.Windows.Forms.Button buttonShowApplication;
private System.Windows.Forms.Label labelApplication;
private System.Windows.Forms.ComboBox listMessageType;
private System.Windows.Forms.Button buttonShowMessageType;
private System.Windows.Forms.Label labelMessageType;
private System.Windows.Forms.TextBox textboxMessage;
private System.Windows.Forms.Label labelMessage;
private System.Windows.Forms.DateTimePicker datetimeDatetime;
private System.Windows.Forms.Label labelDatetime;

        #endregion
    }
}
