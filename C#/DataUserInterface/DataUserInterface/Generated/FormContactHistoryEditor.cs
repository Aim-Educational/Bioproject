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
    public partial class FormContactHistoryEditor : Form
    {
        private static contact_history _defaultObject = new contact_history();

        public int id { private set; get; }
        private contact_history _cached { set; get; }

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
                    foreach (var val in db.contacts.OrderBy(v => v.comment))
    this.listContact.Items.Add(val.comment);
    foreach (var val in db.history_event.OrderBy(v => v.description))
    this.listHistoryEvent.Items.Add(val.description);
    
                }

                if (this.mode != EnumEditorMode.Create)
                {
                    var obj = db.contact_history.SingleOrDefault(v => v.contact_history_id == this.id);
                    if (obj != null)
                    {
                        this.textboxContactHistoryId.Text = Convert.ToString(obj.contact_history_id);
this.numericVersion.Value = (decimal)obj.version;
this.textboxComment.Text = obj.comment;
foreach (var value in db.contacts.OrderBy(v => v.comment))
{
    this.listContact.Items.Add(value.comment);
    if (value.contact_id == obj.contact_id)
        this.listContact.SelectedIndex = this.listContact.Items.Count - 1;
}
foreach (var value in db.history_event.OrderBy(v => v.description))
{
    this.listHistoryEvent.Items.Add(value.description);
    if (value.history_event_id == obj.history_event_id)
        this.listHistoryEvent.SelectedIndex = this.listHistoryEvent.Items.Count - 1;
}


                        this._cached  = obj;
                        this._isDirty = false;
                    }
                }
            }
        }
        
        public FormContactHistoryEditor(EnumEditorMode mode, int id = -1) // ID isn't always needed, e.g. Create
        {
            InitializeComponent();

            FormHelper.unlimitNumericBox(this.numericVersion, AllowDecimals.no);


            this.mode = mode;
            if (mode != EnumEditorMode.Create)
                this.id = id;
            else
                this._cached = FormContactHistoryEditor._defaultObject;

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
                var obj = db.contact_history.SingleOrDefault(v => v.contact_history_id == this.id);

                if (!obj.isDeletable(db))
                {
                    MessageBox.Show("Cannot delete this record as other records are still linked to it.", 
                                    "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this record?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    return;

                db.contact_history.Remove(obj);
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
                var obj = db.contact_history.SingleOrDefault(v => v.contact_history_id == this.id);

                #error Edit 'obj' with the new info to upload to the database.

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
                var obj = new contact_history();

                #error Fill out 'obj' with the new info.

                db.contact_history.Add(obj);
                db.SaveChanges();
                this._cached  = obj;
                this.id       = obj.contact_history_id;
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

                private void textboxContactHistoryId_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxContactHistoryId.Text != Convert.ToString(this._cached.contact_history_id))
                this._isDirty = true;
        }
                private void numericVersion_Enter(object sender, EventArgs e)
        {
            FormHelper.selectAllText(this.numericVersion);
        }
        private void numericVersion_ValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToDouble(this.numericVersion.Value) != this._cached.version)
                this._isDirty = true;
        }
        private void textboxComment_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxComment.Text != Convert.ToString(this._cached.comment))
                this._isDirty = true;
        }
                private void listContact_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listContact.SelectedIndex;
            var value = this.listContact.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || value != this._cached.contact.comment)
                this._isDirty = true;
        }
                private void listHistoryEvent_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listHistoryEvent.SelectedIndex;
            var value = this.listHistoryEvent.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || value != this._cached.history_event.description)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormContactHistoryEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelDirty = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.buttonAction = new System.Windows.Forms.Button();
            this.textboxContactHistoryId = new System.Windows.Forms.TextBox();
this.numericVersion = new System.Windows.Forms.NumericUpDown();
this.textboxComment = new System.Windows.Forms.TextBox();
this.listContact = new System.Windows.Forms.ComboBox();
this.listHistoryEvent = new System.Windows.Forms.ComboBox();
this.labelContactHistoryId = new System.Windows.Forms.Label();
this.labelVersion = new System.Windows.Forms.Label();
this.labelComment = new System.Windows.Forms.Label();
this.labelContact = new System.Windows.Forms.Label();
this.labelHistoryEvent = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericVersion)).BeginInit();

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
            this.splitContainer1.Panel1.Controls.Add(labelContactHistoryId);
this.splitContainer1.Panel1.Controls.Add(labelVersion);
this.splitContainer1.Panel1.Controls.Add(labelComment);
this.splitContainer1.Panel1.Controls.Add(labelContact);
this.splitContainer1.Panel1.Controls.Add(labelHistoryEvent);

            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.buttonDelete);
            this.splitContainer1.Panel2.Controls.Add(this.buttonReload);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAction);
            this.splitContainer1.Panel2.Controls.Add(textboxContactHistoryId);
this.splitContainer1.Panel2.Controls.Add(numericVersion);
this.splitContainer1.Panel2.Controls.Add(textboxComment);
this.splitContainer1.Panel2.Controls.Add(listContact);
this.splitContainer1.Panel2.Controls.Add(listHistoryEvent);

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
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDelete.BackColor = System.Drawing.SystemColors.ControlLight;
            this.buttonDelete.Image = ((System.Drawing.Image)(resources.GetObject("buttonDelete.Image")));
            this.buttonDelete.Location = new System.Drawing.Point(85, 313);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(50, 23);
            this.buttonDelete.TabIndex = 11;
            this.buttonDelete.UseVisualStyleBackColor = false;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonReload
            // 
            this.buttonReload.Location = new System.Drawing.Point(4, 314);
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
            this.buttonAction.Location = new System.Drawing.Point(141, 313);
            this.buttonAction.Name = "buttonAction";
            this.buttonAction.Size = new System.Drawing.Size(75, 23);
            this.buttonAction.TabIndex = 2;
            this.buttonAction.Text = "Save";
            this.buttonAction.UseVisualStyleBackColor = true;
            this.buttonAction.Click += new System.EventHandler(this.buttonSave_Click);
                        // 
            // textboxContactHistoryId
            // 
            this.textboxContactHistoryId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxContactHistoryId.Location = new System.Drawing.Point(4, 12);
            this.textboxContactHistoryId.Name = "textboxContactHistoryId";
            this.textboxContactHistoryId.Size = new System.Drawing.Size(208, 20);
            this.textboxContactHistoryId.TabIndex = 31;
            this.textboxContactHistoryId.Leave += new System.EventHandler(this.textboxContactHistoryId_Leave);
            this.textboxContactHistoryId.Enabled = false;
                        // 
            // numericVersion
            // 
            this.numericVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericVersion.Location = new System.Drawing.Point(4, 38);
            this.numericVersion.Name = "numericVersion";
            this.numericVersion.Size = new System.Drawing.Size(211, 20);
            this.numericVersion.TabIndex = 32;
            this.numericVersion.ValueChanged += new System.EventHandler(this.numericVersion_ValueChanged);
            this.numericVersion.Click += new System.EventHandler(this.numericVersion_Enter);
            this.numericVersion.Enter += new System.EventHandler(this.numericVersion_Enter);
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
            // listContact
            // 
            this.listContact.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listContact.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listContact.FormattingEnabled = true;
            this.listContact.Location = new System.Drawing.Point(4, 90);
            this.listContact.Name = "listContact";
            this.listContact.Size = new System.Drawing.Size(165, 21);
            this.listContact.TabIndex = 25;
            this.listContact.SelectionChangeCommitted += new System.EventHandler(this.listContact_SelectionChangeCommitted);
                        // 
            // listHistoryEvent
            // 
            this.listHistoryEvent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listHistoryEvent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listHistoryEvent.FormattingEnabled = true;
            this.listHistoryEvent.Location = new System.Drawing.Point(4, 116);
            this.listHistoryEvent.Name = "listHistoryEvent";
            this.listHistoryEvent.Size = new System.Drawing.Size(165, 21);
            this.listHistoryEvent.TabIndex = 25;
            this.listHistoryEvent.SelectionChangeCommitted += new System.EventHandler(this.listHistoryEvent_SelectionChangeCommitted);
                        // 
            // labelContactHistoryId
            // 
            this.labelContactHistoryId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelContactHistoryId.AutoSize = true;
            this.labelContactHistoryId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelContactHistoryId.Location = new System.Drawing.Point(0, 12);
            this.labelContactHistoryId.Name = "labelContactHistoryId";
            this.labelContactHistoryId.Size = new System.Drawing.Size(30, 20);
            this.labelContactHistoryId.TabIndex = 14;
            this.labelContactHistoryId.Text = "ID";
            this.labelContactHistoryId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // labelVersion
            // 
            this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVersion.AutoSize = true;
            this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersion.Location = new System.Drawing.Point(0, 38);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(30, 20);
            this.labelVersion.TabIndex = 14;
            this.labelVersion.Text = "Version";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // labelContact
            // 
            this.labelContact.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelContact.AutoSize = true;
            this.labelContact.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelContact.Location = new System.Drawing.Point(0, 90);
            this.labelContact.Name = "labelContact";
            this.labelContact.Size = new System.Drawing.Size(30, 20);
            this.labelContact.TabIndex = 14;
            this.labelContact.Text = "Contact";
            this.labelContact.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // labelHistoryEvent
            // 
            this.labelHistoryEvent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelHistoryEvent.AutoSize = true;
            this.labelHistoryEvent.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHistoryEvent.Location = new System.Drawing.Point(0, 116);
            this.labelHistoryEvent.Name = "labelHistoryEvent";
            this.labelHistoryEvent.Size = new System.Drawing.Size(30, 20);
            this.labelHistoryEvent.TabIndex = 14;
            this.labelHistoryEvent.Text = "HistoryEvent";
            this.labelHistoryEvent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            
            // 
            // FormContactHistoryEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 182);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormContactHistoryEditor";
            this.Text = "ContactHistory Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDeviceEditor_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericVersion)).EndInit();

            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonAction;
        private System.Windows.Forms.Label labelDirty;
        private System.Windows.Forms.Button buttonReload;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.TextBox textboxContactHistoryId;
private System.Windows.Forms.NumericUpDown numericVersion;
private System.Windows.Forms.TextBox textboxComment;
private System.Windows.Forms.ComboBox listContact;
private System.Windows.Forms.ComboBox listHistoryEvent;
private System.Windows.Forms.Label labelContactHistoryId;
private System.Windows.Forms.Label labelVersion;
private System.Windows.Forms.Label labelComment;
private System.Windows.Forms.Label labelContact;
private System.Windows.Forms.Label labelHistoryEvent;

        #endregion
    }
}
