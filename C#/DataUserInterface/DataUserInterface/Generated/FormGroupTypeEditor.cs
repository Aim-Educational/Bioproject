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
    public partial class FormGroupTypeEditor : Form
    {
        private static group_type _defaultObject = new group_type();

        public int id { private set; get; }
        private group_type _cached { set; get; }

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
                    foreach (var val in db.group_type.OrderBy(v => v.name))
    this.listGroupType2.Items.Add(val.name);
    
                }

                if (this.mode != EnumEditorMode.Create)
                {
                    var obj = db.group_type.SingleOrDefault(v => v.group_type_id == this.id);
                    if (obj != null)
                    {
                        this.textboxGroupTypeId.Text = Convert.ToString(obj.group_type_id);
this.textboxName.Text = obj.name;
this.textboxDescription.Text = obj.description;
this.textboxComment.Text = obj.comment;
foreach (var value in db.group_type.OrderBy(v => v.name))
{
    this.listGroupType2.Items.Add(value.name);
    if (value.group_type_id == obj.parent_group_type_id)
        this.listGroupType2.SelectedIndex = this.listGroupType2.Items.Count - 1;
}


                        this._cached  = obj;
                        this._isDirty = false;
                    }
                }
            }
        }
        
        public FormGroupTypeEditor(EnumEditorMode mode, int id = -1) // ID isn't always needed, e.g. Create
        {
            InitializeComponent();

            

            this.mode = mode;
            if (mode != EnumEditorMode.Create)
                this.id = id;
            else
                this._cached = FormGroupTypeEditor._defaultObject;

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
                var obj = db.group_type.SingleOrDefault(v => v.group_type_id == this.id);

                if (!obj.isDeletable(db))
                {
                    MessageBox.Show("Cannot delete this record as other records are still linked to it.", 
                                    "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this record?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    return;

                db.group_type.Remove(obj);
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
                var obj = db.group_type.SingleOrDefault(v => v.group_type_id == this.id);

                
obj.name = this.textboxName.Text;
obj.description = this.textboxDescription.Text;
obj.comment = this.textboxComment.Text;
var selectedGroupType2 = this.listGroupType2.Items[this.listGroupType2.SelectedIndex] as string;
obj.group_type2 = db.group_type.Single(v => v.name == selectedGroupType2);


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
                var obj = new group_type();

                
obj.name = this.textboxName.Text;
obj.description = this.textboxDescription.Text;
obj.comment = this.textboxComment.Text;
var selectedGroupType2 = this.listGroupType2.Items[this.listGroupType2.SelectedIndex] as string;
obj.group_type2 = db.group_type.Single(v => v.name == selectedGroupType2);


                db.group_type.Add(obj);
                db.SaveChanges();
                this._cached  = obj;
                this.id       = obj.group_type_id;
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

                private void textboxGroupTypeId_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxGroupTypeId.Text != Convert.ToString(this._cached.group_type_id))
                this._isDirty = true;
        }
                private void textboxName_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxName.Text != Convert.ToString(this._cached.name))
                this._isDirty = true;
        }
                private void textboxDescription_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxDescription.Text != Convert.ToString(this._cached.description))
                this._isDirty = true;
        }
                private void textboxComment_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxComment.Text != Convert.ToString(this._cached.comment))
                this._isDirty = true;
        }
                private void listGroupType2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.listGroupType2.SelectedIndex;
            var value = this.listGroupType2.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || (this._cached.group_type2 != null && value != this._cached.group_type2.name))
                this._isDirty = true;
        }
        private void buttonShowGroupType2_Click(object sender, EventArgs e)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGroupTypeEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelDirty = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.buttonAction = new System.Windows.Forms.Button();
            this.textboxGroupTypeId = new System.Windows.Forms.TextBox();
this.labelGroupTypeId = new System.Windows.Forms.Label();
this.textboxName = new System.Windows.Forms.TextBox();
this.labelName = new System.Windows.Forms.Label();
this.textboxDescription = new System.Windows.Forms.TextBox();
this.labelDescription = new System.Windows.Forms.Label();
this.textboxComment = new System.Windows.Forms.TextBox();
this.labelComment = new System.Windows.Forms.Label();
this.listGroupType2 = new System.Windows.Forms.ComboBox();
this.buttonShowGroupType2 = new System.Windows.Forms.Button();
this.labelGroupType2 = new System.Windows.Forms.Label();

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
            this.splitContainer1.Panel1.Controls.Add(labelGroupTypeId);
this.splitContainer1.Panel1.Controls.Add(labelName);
this.splitContainer1.Panel1.Controls.Add(labelDescription);
this.splitContainer1.Panel1.Controls.Add(labelComment);
this.splitContainer1.Panel1.Controls.Add(labelGroupType2);

            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.buttonDelete);
            this.splitContainer1.Panel2.Controls.Add(this.buttonReload);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAction);
            this.splitContainer1.Panel2.Controls.Add(textboxGroupTypeId);
this.splitContainer1.Panel2.Controls.Add(textboxName);
this.splitContainer1.Panel2.Controls.Add(textboxDescription);
this.splitContainer1.Panel2.Controls.Add(textboxComment);
this.splitContainer1.Panel2.Controls.Add(listGroupType2);
this.splitContainer1.Panel2.Controls.Add(buttonShowGroupType2);

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
            // textboxGroupTypeId
            // 
            this.textboxGroupTypeId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxGroupTypeId.Location = new System.Drawing.Point(4, 12);
            this.textboxGroupTypeId.Name = "textboxGroupTypeId";
            this.textboxGroupTypeId.Size = new System.Drawing.Size(208, 20);
            this.textboxGroupTypeId.TabIndex = 31;
            this.textboxGroupTypeId.Leave += new System.EventHandler(this.textboxGroupTypeId_Leave);
            this.textboxGroupTypeId.Enabled = false;
                        // 
            // labelGroupTypeId
            // 
            this.labelGroupTypeId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelGroupTypeId.AutoSize = true;
            this.labelGroupTypeId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGroupTypeId.Location = new System.Drawing.Point(0, 12);
            this.labelGroupTypeId.Name = "labelGroupTypeId";
            this.labelGroupTypeId.Size = new System.Drawing.Size(30, 20);
            this.labelGroupTypeId.TabIndex = 14;
            this.labelGroupTypeId.Text = "ID";
            this.labelGroupTypeId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxName
            // 
            this.textboxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxName.Location = new System.Drawing.Point(4, 38);
            this.textboxName.Name = "textboxName";
            this.textboxName.Size = new System.Drawing.Size(208, 20);
            this.textboxName.TabIndex = 31;
            this.textboxName.Leave += new System.EventHandler(this.textboxName_Leave);
            this.textboxName.Enabled = true;
                        // 
            // labelName
            // 
            this.labelName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelName.AutoSize = true;
            this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName.Location = new System.Drawing.Point(0, 38);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(30, 20);
            this.labelName.TabIndex = 14;
            this.labelName.Text = "Name";
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxDescription
            // 
            this.textboxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxDescription.Location = new System.Drawing.Point(4, 64);
            this.textboxDescription.Name = "textboxDescription";
            this.textboxDescription.Size = new System.Drawing.Size(208, 20);
            this.textboxDescription.TabIndex = 31;
            this.textboxDescription.Leave += new System.EventHandler(this.textboxDescription_Leave);
            this.textboxDescription.Enabled = true;
                        // 
            // labelDescription
            // 
            this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDescription.AutoSize = true;
            this.labelDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDescription.Location = new System.Drawing.Point(0, 64);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(30, 20);
            this.labelDescription.TabIndex = 14;
            this.labelDescription.Text = "Description";
            this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // textboxComment
            // 
            this.textboxComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxComment.Location = new System.Drawing.Point(4, 90);
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
            this.labelComment.Location = new System.Drawing.Point(0, 90);
            this.labelComment.Name = "labelComment";
            this.labelComment.Size = new System.Drawing.Size(30, 20);
            this.labelComment.TabIndex = 14;
            this.labelComment.Text = "Comment";
            this.labelComment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                        // 
            // listGroupType2
            // 
            this.listGroupType2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listGroupType2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listGroupType2.FormattingEnabled = true;
            this.listGroupType2.Location = new System.Drawing.Point(4, 116);
            this.listGroupType2.Name = "listGroupType2";
            this.listGroupType2.Size = new System.Drawing.Size(165, 21);
            this.listGroupType2.TabIndex = 25;
            this.listGroupType2.SelectionChangeCommitted += new System.EventHandler(this.listGroupType2_SelectionChangeCommitted);
                        // 
            // buttonShowGroupType2
            // 
            this.buttonShowGroupType2.Location = new System.Drawing.Point(174, 116);
            this.buttonShowGroupType2.Name = "buttonShowGroupType2";
            this.buttonShowGroupType2.Size = new System.Drawing.Size(40, 23);
            this.buttonShowGroupType2.TabIndex = 10;
            this.buttonShowGroupType2.Text = "...";
            this.buttonShowGroupType2.Click += new System.EventHandler(this.buttonShowGroupType2_Click);
            // 
            // labelGroupType2
            // 
            this.labelGroupType2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelGroupType2.AutoSize = true;
            this.labelGroupType2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGroupType2.Location = new System.Drawing.Point(0, 116);
            this.labelGroupType2.Name = "labelGroupType2";
            this.labelGroupType2.Size = new System.Drawing.Size(30, 20);
            this.labelGroupType2.TabIndex = 14;
            this.labelGroupType2.Text = "GroupType2";
            this.labelGroupType2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            
            // 
            // FormGroupTypeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 182);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormGroupTypeEditor";
            this.Text = "GroupType Editor";
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
        private System.Windows.Forms.TextBox textboxGroupTypeId;
private System.Windows.Forms.Label labelGroupTypeId;
private System.Windows.Forms.TextBox textboxName;
private System.Windows.Forms.Label labelName;
private System.Windows.Forms.TextBox textboxDescription;
private System.Windows.Forms.Label labelDescription;
private System.Windows.Forms.TextBox textboxComment;
private System.Windows.Forms.Label labelComment;
private System.Windows.Forms.ComboBox listGroupType2;
private System.Windows.Forms.Button buttonShowGroupType2;
private System.Windows.Forms.Label labelGroupType2;

        #endregion
    }
}
