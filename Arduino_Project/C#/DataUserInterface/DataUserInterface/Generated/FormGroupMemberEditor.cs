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
    public partial class FormGroupMemberEditor : Form
    {
        private static group_member _defaultObject = new group_member();

        public int id { private set; get; }
        private group_member _cached { set; get; }

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
    foreach (var val in db.group_type.OrderBy(v => v.name))
    this.listGroupType.Items.Add(val.name);
    
                }

                if (this.mode != EnumEditorMode.Create)
                {
                    var obj = db.group_member.SingleOrDefault(v => v.group_member_id == this.id);
                    if (obj != null)
                    {
                        this.textboxGroupMemberId.Text = Convert.ToString(obj.group_member_id);
foreach (var value in db.contacts.OrderBy(v => v.comment))
{
    this.listContact.Items.Add(value.comment);
    if (value.contact_id == obj.contact_id)
        this.listContact.SelectedIndex = this.listContact.Items.Count - 1;
}
foreach (var value in db.group_type.OrderBy(v => v.name))
{
    this.listGroupType.Items.Add(value.name);
    if (value.group_type_id == obj.group_type_id)
        this.listGroupType.SelectedIndex = this.listGroupType.Items.Count - 1;
}
this.textboxComment.Text = obj.comment;


                        this._cached  = obj;
                        this._isDirty = false;
                    }
                }
            }
        }
        
        public FormGroupMemberEditor(EnumEditorMode mode, int id = -1) // ID isn't always needed, e.g. Create
        {
            InitializeComponent();

            

            this.mode = mode;
            if (mode != EnumEditorMode.Create)
                this.id = id;
            else
                this._cached = FormGroupMemberEditor._defaultObject;

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
                var obj = db.group_member.SingleOrDefault(v => v.group_member_id == this.id);

                if (!obj.isDeletable(db))
                {
                    MessageBox.Show("Cannot delete this record as other records are still linked to it.", 
                                    "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this record?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    return;

                db.group_member.Remove(obj);
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
                var obj = db.group_member.SingleOrDefault(v => v.group_member_id == this.id);

                
var selectedContact = this.listContact.Items[this.listContact.SelectedIndex] as string;
obj.contact = db.contacts.Single(v => v.comment == selectedContact);
var selectedGroupType = this.listGroupType.Items[this.listGroupType.SelectedIndex] as string;
obj.group_type = db.group_type.Single(v => v.name == selectedGroupType);
obj.comment = this.textboxComment.Text;


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
                var obj = new group_member();

                
var selectedContact = this.listContact.Items[this.listContact.SelectedIndex] as string;
obj.contact = db.contacts.Single(v => v.comment == selectedContact);
var selectedGroupType = this.listGroupType.Items[this.listGroupType.SelectedIndex] as string;
obj.group_type = db.group_type.Single(v => v.name == selectedGroupType);
obj.comment = this.textboxComment.Text;


                db.group_member.Add(obj);
                db.SaveChanges();
                this._cached  = obj;
                this.id       = obj.group_member_id;
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

                private void textboxGroupMemberId_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxGroupMemberId.Text != Convert.ToString(this._cached.group_member_id))
                this._isDirty = true;
        }
                private void listContact_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listContact.SelectedIndex;
            var value = this.listContact.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || (this._cached.contact != null && value != this._cached.contact.comment))
                this._isDirty = true;
        }
        private void buttonShowContact_Click(object sender, EventArgs e)
{
    var form = new SearchForm(EnumSearchFormType.Contact);
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
        private void textboxComment_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxComment.Text != Convert.ToString(this._cached.comment))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGroupMemberEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelDirty = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.buttonAction = new System.Windows.Forms.Button();
            this.textboxGroupMemberId = new System.Windows.Forms.TextBox();
this.labelGroupMemberId = new System.Windows.Forms.Label();
this.listContact = new System.Windows.Forms.ComboBox();
this.buttonShowContact = new System.Windows.Forms.Button();
this.labelContact = new System.Windows.Forms.Label();
this.listGroupType = new System.Windows.Forms.ComboBox();
this.buttonShowGroupType = new System.Windows.Forms.Button();
this.labelGroupType = new System.Windows.Forms.Label();
this.textboxComment = new System.Windows.Forms.TextBox();
this.labelComment = new System.Windows.Forms.Label();

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
            this.splitContainer1.Panel1.Controls.Add(labelGroupMemberId);
this.splitContainer1.Panel1.Controls.Add(labelContact);
this.splitContainer1.Panel1.Controls.Add(labelGroupType);
this.splitContainer1.Panel1.Controls.Add(labelComment);

            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.buttonDelete);
            this.splitContainer1.Panel2.Controls.Add(this.buttonReload);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAction);
            this.splitContainer1.Panel2.Controls.Add(textboxGroupMemberId);
this.splitContainer1.Panel2.Controls.Add(listContact);
this.splitContainer1.Panel2.Controls.Add(buttonShowContact);
this.splitContainer1.Panel2.Controls.Add(listGroupType);
this.splitContainer1.Panel2.Controls.Add(buttonShowGroupType);
this.splitContainer1.Panel2.Controls.Add(textboxComment);

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
            this.buttonDelete.Location = new System.Drawing.Point(85, 130);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Text = "[X]";
            this.buttonDelete.Size = new System.Drawing.Size(50, 23);
            this.buttonDelete.TabIndex = 11;
            this.buttonDelete.UseVisualStyleBackColor = false;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonReload
            // 
            this.buttonReload.Location = new System.Drawing.Point(4, 130);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(75, 23);
            this.buttonReload.TabIndex = 6;
            this.buttonReload.Text = "Reload";
            this.buttonReload.UseVisualStyleBackColor = true;
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            // 
            // buttonAction
            // 
            this.buttonAction.Location = new System.Drawing.Point(141, 130);
            this.buttonAction.Name = "buttonAction";
            this.buttonAction.Size = new System.Drawing.Size(75, 23);
            this.buttonAction.TabIndex = 2;
            this.buttonAction.Text = "Save";
            this.buttonAction.UseVisualStyleBackColor = true;
            this.buttonAction.Click += new System.EventHandler(this.buttonSave_Click);
                        // 
            // textboxGroupMemberId
            // 
            this.textboxGroupMemberId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxGroupMemberId.Location = new System.Drawing.Point(4, 12);
            this.textboxGroupMemberId.Name = "textboxGroupMemberId";
            this.textboxGroupMemberId.Size = new System.Drawing.Size(208, 20);
            this.textboxGroupMemberId.TabIndex = 0;
            this.textboxGroupMemberId.Leave += new System.EventHandler(this.textboxGroupMemberId_Leave);
            this.textboxGroupMemberId.Enabled = false;
                        // 
            // labelGroupMemberId
            // 
            this.labelGroupMemberId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelGroupMemberId.AutoSize = true;
            this.labelGroupMemberId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGroupMemberId.Location = new System.Drawing.Point(0, 12);
            this.labelGroupMemberId.Name = "labelGroupMemberId";
            this.labelGroupMemberId.Size = new System.Drawing.Size(30, 20);
            this.labelGroupMemberId.TabIndex = 0;
            this.labelGroupMemberId.Text = "ID";
            this.labelGroupMemberId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // listContact
            // 
            this.listContact.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listContact.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listContact.FormattingEnabled = true;
            this.listContact.Location = new System.Drawing.Point(4, 38);
            this.listContact.Name = "listContact";
            this.listContact.Size = new System.Drawing.Size(165, 21);
            this.listContact.TabIndex = 1;
            this.listContact.SelectionChangeCommitted += new System.EventHandler(this.listContact_SelectionChangeCommitted);
                        // 
            // buttonShowContact
            // 
            this.buttonShowContact.Location = new System.Drawing.Point(174, 38);
            this.buttonShowContact.Name = "buttonShowContact";
            this.buttonShowContact.Size = new System.Drawing.Size(40, 23);
            this.buttonShowContact.TabIndex = 2;
            this.buttonShowContact.Text = "...";
            this.buttonShowContact.Click += new System.EventHandler(this.buttonShowContact_Click);
            // 
            // labelContact
            // 
            this.labelContact.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelContact.AutoSize = true;
            this.labelContact.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelContact.Location = new System.Drawing.Point(0, 38);
            this.labelContact.Name = "labelContact";
            this.labelContact.Size = new System.Drawing.Size(30, 20);
            this.labelContact.TabIndex = 0;
            this.labelContact.Text = "Contact";
            this.labelContact.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // listGroupType
            // 
            this.listGroupType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listGroupType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listGroupType.FormattingEnabled = true;
            this.listGroupType.Location = new System.Drawing.Point(4, 64);
            this.listGroupType.Name = "listGroupType";
            this.listGroupType.Size = new System.Drawing.Size(165, 21);
            this.listGroupType.TabIndex = 3;
            this.listGroupType.SelectionChangeCommitted += new System.EventHandler(this.listGroupType_SelectionChangeCommitted);
                        // 
            // buttonShowGroupType
            // 
            this.buttonShowGroupType.Location = new System.Drawing.Point(174, 64);
            this.buttonShowGroupType.Name = "buttonShowGroupType";
            this.buttonShowGroupType.Size = new System.Drawing.Size(40, 23);
            this.buttonShowGroupType.TabIndex = 4;
            this.buttonShowGroupType.Text = "...";
            this.buttonShowGroupType.Click += new System.EventHandler(this.buttonShowGroupType_Click);
            // 
            // labelGroupType
            // 
            this.labelGroupType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelGroupType.AutoSize = true;
            this.labelGroupType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGroupType.Location = new System.Drawing.Point(0, 64);
            this.labelGroupType.Name = "labelGroupType";
            this.labelGroupType.Size = new System.Drawing.Size(30, 20);
            this.labelGroupType.TabIndex = 0;
            this.labelGroupType.Text = "GroupType";
            this.labelGroupType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxComment
            // 
            this.textboxComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxComment.Location = new System.Drawing.Point(4, 90);
            this.textboxComment.Name = "textboxComment";
            this.textboxComment.Size = new System.Drawing.Size(208, 20);
            this.textboxComment.TabIndex = 5;
            this.textboxComment.Leave += new System.EventHandler(this.textboxComment_Leave);
            this.textboxComment.Enabled = true;
                        // 
            // labelComment
            // 
            this.labelComment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelComment.AutoSize = true;
            this.labelComment.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelComment.Location = new System.Drawing.Point(0, 90);
            this.labelComment.Name = "labelComment";
            this.labelComment.Size = new System.Drawing.Size(30, 20);
            this.labelComment.TabIndex = 0;
            this.labelComment.Text = "Comment";
            this.labelComment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            
            // 
            // FormGroupMemberEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 156);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormGroupMemberEditor";
            this.Text = "GroupMember Editor";
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
        private System.Windows.Forms.TextBox textboxGroupMemberId;
private System.Windows.Forms.Label labelGroupMemberId;
private System.Windows.Forms.ComboBox listContact;
private System.Windows.Forms.Button buttonShowContact;
private System.Windows.Forms.Label labelContact;
private System.Windows.Forms.ComboBox listGroupType;
private System.Windows.Forms.Button buttonShowGroupType;
private System.Windows.Forms.Label labelGroupType;
private System.Windows.Forms.TextBox textboxComment;
private System.Windows.Forms.Label labelComment;

        #endregion
    }
}