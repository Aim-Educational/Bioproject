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
    public partial class FormUserEditor : Form
    {
        private static user _defaultObject = new user();

        public int id { private set; get; }
        private user _cached { set; get; }

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
                    
                }

                if (this.mode != EnumEditorMode.Create)
                {
                    var obj = db.users.SingleOrDefault(v => v.user_id == this.id);
                    if (obj != null)
                    {
                        this.textboxUserId.Text = Convert.ToString(obj.user_id);
this.textboxForename.Text = obj.forename;
this.textboxSurname.Text = obj.surname;
this.textboxUsername.Text = obj.username;
this.textboxPassword.Text = obj.password;
this.textboxComment.Text = obj.comment;


                        this._cached  = obj;
                        this._isDirty = false;
                    }
                }
            }
        }
        
        public FormUserEditor(EnumEditorMode mode, int id = -1) // ID isn't always needed, e.g. Create
        {
            InitializeComponent();

            

            this.mode = mode;
            if (mode != EnumEditorMode.Create)
                this.id = id;
            else
                this._cached = FormUserEditor._defaultObject;

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
                var obj = db.users.SingleOrDefault(v => v.user_id == this.id);

                if (!obj.isDeletable(db))
                {
                    MessageBox.Show("Cannot delete this record as other records are still linked to it.", 
                                    "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this record?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    return;

                db.users.Remove(obj);
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
                var obj = db.users.SingleOrDefault(v => v.user_id == this.id);

                
obj.forename = this.textboxForename.Text;
obj.surname = this.textboxSurname.Text;
obj.username = this.textboxUsername.Text;
obj.password = this.textboxPassword.Text;
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
                var obj = new user();

                
obj.forename = this.textboxForename.Text;
obj.surname = this.textboxSurname.Text;
obj.username = this.textboxUsername.Text;
obj.password = this.textboxPassword.Text;
obj.comment = this.textboxComment.Text;


                db.users.Add(obj);
                db.SaveChanges();
                this._cached  = obj;
                this.id       = obj.user_id;
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

                private void textboxUserId_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxUserId.Text != Convert.ToString(this._cached.user_id))
                this._isDirty = true;
        }
                private void textboxForename_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxForename.Text != Convert.ToString(this._cached.forename))
                this._isDirty = true;
        }
                private void textboxSurname_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxSurname.Text != Convert.ToString(this._cached.surname))
                this._isDirty = true;
        }
                private void textboxUsername_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxUsername.Text != Convert.ToString(this._cached.username))
                this._isDirty = true;
        }
                private void textboxPassword_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxPassword.Text != Convert.ToString(this._cached.password))
                this._isDirty = true;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUserEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelDirty = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.buttonAction = new System.Windows.Forms.Button();
            this.textboxUserId = new System.Windows.Forms.TextBox();
this.labelUserId = new System.Windows.Forms.Label();
this.textboxForename = new System.Windows.Forms.TextBox();
this.labelForename = new System.Windows.Forms.Label();
this.textboxSurname = new System.Windows.Forms.TextBox();
this.labelSurname = new System.Windows.Forms.Label();
this.textboxUsername = new System.Windows.Forms.TextBox();
this.labelUsername = new System.Windows.Forms.Label();
this.textboxPassword = new System.Windows.Forms.TextBox();
this.labelPassword = new System.Windows.Forms.Label();
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
            this.splitContainer1.Panel1.Controls.Add(labelUserId);
this.splitContainer1.Panel1.Controls.Add(labelForename);
this.splitContainer1.Panel1.Controls.Add(labelSurname);
this.splitContainer1.Panel1.Controls.Add(labelUsername);
this.splitContainer1.Panel1.Controls.Add(labelPassword);
this.splitContainer1.Panel1.Controls.Add(labelComment);

            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.buttonDelete);
            this.splitContainer1.Panel2.Controls.Add(this.buttonReload);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAction);
            this.splitContainer1.Panel2.Controls.Add(textboxUserId);
this.splitContainer1.Panel2.Controls.Add(textboxForename);
this.splitContainer1.Panel2.Controls.Add(textboxSurname);
this.splitContainer1.Panel2.Controls.Add(textboxUsername);
this.splitContainer1.Panel2.Controls.Add(textboxPassword);
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
            // textboxUserId
            // 
            this.textboxUserId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxUserId.Location = new System.Drawing.Point(4, 12);
            this.textboxUserId.Name = "textboxUserId";
            this.textboxUserId.Size = new System.Drawing.Size(208, 20);
            this.textboxUserId.TabIndex = 0;
            this.textboxUserId.Leave += new System.EventHandler(this.textboxUserId_Leave);
            this.textboxUserId.Enabled = false;
                        // 
            // labelUserId
            // 
            this.labelUserId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUserId.AutoSize = true;
            this.labelUserId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUserId.Location = new System.Drawing.Point(0, 12);
            this.labelUserId.Name = "labelUserId";
            this.labelUserId.Size = new System.Drawing.Size(30, 20);
            this.labelUserId.TabIndex = 0;
            this.labelUserId.Text = "ID";
            this.labelUserId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxForename
            // 
            this.textboxForename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxForename.Location = new System.Drawing.Point(4, 38);
            this.textboxForename.Name = "textboxForename";
            this.textboxForename.Size = new System.Drawing.Size(208, 20);
            this.textboxForename.TabIndex = 1;
            this.textboxForename.Leave += new System.EventHandler(this.textboxForename_Leave);
            this.textboxForename.Enabled = true;
                        // 
            // labelForename
            // 
            this.labelForename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelForename.AutoSize = true;
            this.labelForename.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelForename.Location = new System.Drawing.Point(0, 38);
            this.labelForename.Name = "labelForename";
            this.labelForename.Size = new System.Drawing.Size(30, 20);
            this.labelForename.TabIndex = 0;
            this.labelForename.Text = "Forename";
            this.labelForename.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxSurname
            // 
            this.textboxSurname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxSurname.Location = new System.Drawing.Point(4, 64);
            this.textboxSurname.Name = "textboxSurname";
            this.textboxSurname.Size = new System.Drawing.Size(208, 20);
            this.textboxSurname.TabIndex = 2;
            this.textboxSurname.Leave += new System.EventHandler(this.textboxSurname_Leave);
            this.textboxSurname.Enabled = true;
                        // 
            // labelSurname
            // 
            this.labelSurname.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSurname.AutoSize = true;
            this.labelSurname.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSurname.Location = new System.Drawing.Point(0, 64);
            this.labelSurname.Name = "labelSurname";
            this.labelSurname.Size = new System.Drawing.Size(30, 20);
            this.labelSurname.TabIndex = 0;
            this.labelSurname.Text = "Surname";
            this.labelSurname.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxUsername
            // 
            this.textboxUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxUsername.Location = new System.Drawing.Point(4, 90);
            this.textboxUsername.Name = "textboxUsername";
            this.textboxUsername.Size = new System.Drawing.Size(208, 20);
            this.textboxUsername.TabIndex = 3;
            this.textboxUsername.Leave += new System.EventHandler(this.textboxUsername_Leave);
            this.textboxUsername.Enabled = true;
                        // 
            // labelUsername
            // 
            this.labelUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUsername.AutoSize = true;
            this.labelUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUsername.Location = new System.Drawing.Point(0, 90);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(30, 20);
            this.labelUsername.TabIndex = 0;
            this.labelUsername.Text = "Username";
            this.labelUsername.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxPassword
            // 
            this.textboxPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxPassword.Location = new System.Drawing.Point(4, 116);
            this.textboxPassword.Name = "textboxPassword";
            this.textboxPassword.Size = new System.Drawing.Size(208, 20);
            this.textboxPassword.TabIndex = 4;
            this.textboxPassword.Leave += new System.EventHandler(this.textboxPassword_Leave);
            this.textboxPassword.Enabled = true;
                        // 
            // labelPassword
            // 
            this.labelPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPassword.AutoSize = true;
            this.labelPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPassword.Location = new System.Drawing.Point(0, 116);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(30, 20);
            this.labelPassword.TabIndex = 0;
            this.labelPassword.Text = "Password";
            this.labelPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxComment
            // 
            this.textboxComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxComment.Location = new System.Drawing.Point(4, 142);
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
            this.labelComment.Location = new System.Drawing.Point(0, 142);
            this.labelComment.Name = "labelComment";
            this.labelComment.Size = new System.Drawing.Size(30, 20);
            this.labelComment.TabIndex = 0;
            this.labelComment.Text = "Comment";
            this.labelComment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            
            // 
            // FormUserEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 208);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormUserEditor";
            this.Text = "User Editor";
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
        private System.Windows.Forms.TextBox textboxUserId;
private System.Windows.Forms.Label labelUserId;
private System.Windows.Forms.TextBox textboxForename;
private System.Windows.Forms.Label labelForename;
private System.Windows.Forms.TextBox textboxSurname;
private System.Windows.Forms.Label labelSurname;
private System.Windows.Forms.TextBox textboxUsername;
private System.Windows.Forms.Label labelUsername;
private System.Windows.Forms.TextBox textboxPassword;
private System.Windows.Forms.Label labelPassword;
private System.Windows.Forms.TextBox textboxComment;
private System.Windows.Forms.Label labelComment;

        #endregion
    }
}
