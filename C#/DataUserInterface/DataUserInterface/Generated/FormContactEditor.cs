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
    public partial class FormContactEditor : Form
    {
        private static contact _defaultObject = new contact();

        public int id { private set; get; }
        private contact _cached { set; get; }

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
                    foreach (var val in db.contact_type.OrderBy(v => v.description))
    this.listContactType.Items.Add(val.description);
    foreach (var val in db.suppliers.OrderBy(v => v.name))
    this.listSupplier.Items.Add(val.name);
    foreach (var val in db.users.OrderBy(v => v.comment))
    this.listUser.Items.Add(val.comment);
    
                }

                if (this.mode != EnumEditorMode.Create)
                {
                    var obj = db.contacts.SingleOrDefault(v => v.contact_id == this.id);
                    if (obj != null)
                    {
                        this.textboxContactId.Text = Convert.ToString(obj.contact_id);
this.numericVersion.Value = (decimal)obj.version;
this.textboxComment.Text = obj.comment;
foreach (var value in db.contact_type.OrderBy(v => v.description))
{
    this.listContactType.Items.Add(value.description);
    if (value.contact_type_id == obj.contact_type_id)
        this.listContactType.SelectedIndex = this.listContactType.Items.Count - 1;
}
foreach (var value in db.suppliers.OrderBy(v => v.name))
{
    this.listSupplier.Items.Add(value.name);
    if (value.supplier_id == obj.supplier_id)
        this.listSupplier.SelectedIndex = this.listSupplier.Items.Count - 1;
}
foreach (var value in db.users.OrderBy(v => v.comment))
{
    this.listUser.Items.Add(value.comment);
    if (value.user_id == obj.user_id)
        this.listUser.SelectedIndex = this.listUser.Items.Count - 1;
}


                        this._cached  = obj;
                        this._isDirty = false;
                    }
                }
            }
        }
        
        public FormContactEditor(EnumEditorMode mode, int id = -1) // ID isn't always needed, e.g. Create
        {
            InitializeComponent();

            FormHelper.unlimitNumericBox(this.numericVersion, AllowDecimals.no);


            this.mode = mode;
            if (mode != EnumEditorMode.Create)
                this.id = id;
            else
                this._cached = FormContactEditor._defaultObject;

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
                var obj = db.contacts.SingleOrDefault(v => v.contact_id == this.id);

                if (!obj.isDeletable(db))
                {
                    MessageBox.Show("Cannot delete this record as other records are still linked to it.", 
                                    "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this record?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    return;

                db.contacts.Remove(obj);
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
                var obj = db.contacts.SingleOrDefault(v => v.contact_id == this.id);

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
                var obj = new contact();

                #error Fill out 'obj' with the new info.

                db.contacts.Add(obj);
                db.SaveChanges();
                this._cached  = obj;
                this.id       = obj.contact_id;
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

                private void textboxContactId_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxContactId.Text != Convert.ToString(this._cached.contact_id))
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
                private void listContactType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listContactType.SelectedIndex;
            var value = this.listContactType.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || value != this._cached.contact_type.description)
                this._isDirty = true;
        }
                private void listSupplier_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listSupplier.SelectedIndex;
            var value = this.listSupplier.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || value != this._cached.supplier.name)
                this._isDirty = true;
        }
                private void listUser_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listUser.SelectedIndex;
            var value = this.listUser.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || value != this._cached.user.comment)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormContactEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelDirty = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.buttonAction = new System.Windows.Forms.Button();
            this.textboxContactId = new System.Windows.Forms.TextBox();
this.numericVersion = new System.Windows.Forms.NumericUpDown();
this.textboxComment = new System.Windows.Forms.TextBox();
this.listContactType = new System.Windows.Forms.ComboBox();
this.listSupplier = new System.Windows.Forms.ComboBox();
this.listUser = new System.Windows.Forms.ComboBox();
this.labelContactId = new System.Windows.Forms.Label();
this.labelVersion = new System.Windows.Forms.Label();
this.labelComment = new System.Windows.Forms.Label();
this.labelContactType = new System.Windows.Forms.Label();
this.labelSupplier = new System.Windows.Forms.Label();
this.labelUser = new System.Windows.Forms.Label();

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
            this.splitContainer1.Panel1.Controls.Add(labelContactId);
this.splitContainer1.Panel1.Controls.Add(labelVersion);
this.splitContainer1.Panel1.Controls.Add(labelComment);
this.splitContainer1.Panel1.Controls.Add(labelContactType);
this.splitContainer1.Panel1.Controls.Add(labelSupplier);
this.splitContainer1.Panel1.Controls.Add(labelUser);

            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.buttonDelete);
            this.splitContainer1.Panel2.Controls.Add(this.buttonReload);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAction);
            this.splitContainer1.Panel2.Controls.Add(textboxContactId);
this.splitContainer1.Panel2.Controls.Add(numericVersion);
this.splitContainer1.Panel2.Controls.Add(textboxComment);
this.splitContainer1.Panel2.Controls.Add(listContactType);
this.splitContainer1.Panel2.Controls.Add(listSupplier);
this.splitContainer1.Panel2.Controls.Add(listUser);

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
            // textboxContactId
            // 
            this.textboxContactId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxContactId.Location = new System.Drawing.Point(4, 12);
            this.textboxContactId.Name = "textboxContactId";
            this.textboxContactId.Size = new System.Drawing.Size(208, 20);
            this.textboxContactId.TabIndex = 31;
            this.textboxContactId.Leave += new System.EventHandler(this.textboxContactId_Leave);
            this.textboxContactId.Enabled = false;
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
            // listContactType
            // 
            this.listContactType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listContactType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listContactType.FormattingEnabled = true;
            this.listContactType.Location = new System.Drawing.Point(4, 90);
            this.listContactType.Name = "listContactType";
            this.listContactType.Size = new System.Drawing.Size(165, 21);
            this.listContactType.TabIndex = 25;
            this.listContactType.SelectionChangeCommitted += new System.EventHandler(this.listContactType_SelectionChangeCommitted);
                        // 
            // listSupplier
            // 
            this.listSupplier.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listSupplier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listSupplier.FormattingEnabled = true;
            this.listSupplier.Location = new System.Drawing.Point(4, 116);
            this.listSupplier.Name = "listSupplier";
            this.listSupplier.Size = new System.Drawing.Size(165, 21);
            this.listSupplier.TabIndex = 25;
            this.listSupplier.SelectionChangeCommitted += new System.EventHandler(this.listSupplier_SelectionChangeCommitted);
                        // 
            // listUser
            // 
            this.listUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listUser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listUser.FormattingEnabled = true;
            this.listUser.Location = new System.Drawing.Point(4, 142);
            this.listUser.Name = "listUser";
            this.listUser.Size = new System.Drawing.Size(165, 21);
            this.listUser.TabIndex = 25;
            this.listUser.SelectionChangeCommitted += new System.EventHandler(this.listUser_SelectionChangeCommitted);
                        // 
            // labelContactId
            // 
            this.labelContactId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelContactId.AutoSize = true;
            this.labelContactId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelContactId.Location = new System.Drawing.Point(0, 12);
            this.labelContactId.Name = "labelContactId";
            this.labelContactId.Size = new System.Drawing.Size(30, 20);
            this.labelContactId.TabIndex = 14;
            this.labelContactId.Text = "ID";
            this.labelContactId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // labelContactType
            // 
            this.labelContactType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelContactType.AutoSize = true;
            this.labelContactType.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelContactType.Location = new System.Drawing.Point(0, 90);
            this.labelContactType.Name = "labelContactType";
            this.labelContactType.Size = new System.Drawing.Size(30, 20);
            this.labelContactType.TabIndex = 14;
            this.labelContactType.Text = "ContactType";
            this.labelContactType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // labelSupplier
            // 
            this.labelSupplier.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSupplier.AutoSize = true;
            this.labelSupplier.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSupplier.Location = new System.Drawing.Point(0, 116);
            this.labelSupplier.Name = "labelSupplier";
            this.labelSupplier.Size = new System.Drawing.Size(30, 20);
            this.labelSupplier.TabIndex = 14;
            this.labelSupplier.Text = "Supplier";
            this.labelSupplier.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // labelUser
            // 
            this.labelUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUser.AutoSize = true;
            this.labelUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUser.Location = new System.Drawing.Point(0, 142);
            this.labelUser.Name = "labelUser";
            this.labelUser.Size = new System.Drawing.Size(30, 20);
            this.labelUser.TabIndex = 14;
            this.labelUser.Text = "User";
            this.labelUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            
            // 
            // FormContactEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 208);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormContactEditor";
            this.Text = "Contact Editor";
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
        private System.Windows.Forms.TextBox textboxContactId;
private System.Windows.Forms.NumericUpDown numericVersion;
private System.Windows.Forms.TextBox textboxComment;
private System.Windows.Forms.ComboBox listContactType;
private System.Windows.Forms.ComboBox listSupplier;
private System.Windows.Forms.ComboBox listUser;
private System.Windows.Forms.Label labelContactId;
private System.Windows.Forms.Label labelVersion;
private System.Windows.Forms.Label labelComment;
private System.Windows.Forms.Label labelContactType;
private System.Windows.Forms.Label labelSupplier;
private System.Windows.Forms.Label labelUser;

        #endregion
    }
}
