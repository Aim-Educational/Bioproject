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
    public partial class FormSupplierEditor : Form
    {
        private static supplier _defaultObject = new supplier();

        public int id { private set; get; }
        private supplier _cached { set; get; }

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
                    var obj = db.suppliers.SingleOrDefault(v => v.supplier_id == this.id);
                    if (obj != null)
                    {
                        this.textboxSupplierId.Text = Convert.ToString(obj.supplier_id);
this.textboxName.Text = obj.name;
this.numericVersion.Value = (decimal)obj.version;
this.textboxComment.Text = obj.comment;


                        this._cached  = obj;
                        this._isDirty = false;
                    }
                }
            }
        }
        
        public FormSupplierEditor(EnumEditorMode mode, int id = -1) // ID isn't always needed, e.g. Create
        {
            InitializeComponent();

            FormHelper.unlimitNumericBox(this.numericVersion, AllowDecimals.no);


            this.mode = mode;
            if (mode != EnumEditorMode.Create)
                this.id = id;
            else
                this._cached = FormSupplierEditor._defaultObject;

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
                var obj = db.suppliers.SingleOrDefault(v => v.supplier_id == this.id);

                if (!obj.isDeletable(db))
                {
                    MessageBox.Show("Cannot delete this record as other records are still linked to it.", 
                                    "Unable to delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this record?", "?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    return;

                db.suppliers.Remove(obj);
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
                var obj = db.suppliers.SingleOrDefault(v => v.supplier_id == this.id);

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
                var obj = new supplier();

                #error Fill out 'obj' with the new info.

                db.suppliers.Add(obj);
                db.SaveChanges();
                this._cached  = obj;
                this.id       = obj.supplier_id;
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

                private void textboxSupplierId_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxSupplierId.Text != Convert.ToString(this._cached.supplier_id))
                this._isDirty = true;
        }
                private void textboxName_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.textboxName.Text != Convert.ToString(this._cached.name))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSupplierEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelDirty = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.buttonAction = new System.Windows.Forms.Button();
            this.textboxSupplierId = new System.Windows.Forms.TextBox();
this.textboxName = new System.Windows.Forms.TextBox();
this.numericVersion = new System.Windows.Forms.NumericUpDown();
this.textboxComment = new System.Windows.Forms.TextBox();
this.labelSupplierId = new System.Windows.Forms.Label();
this.labelName = new System.Windows.Forms.Label();
this.labelVersion = new System.Windows.Forms.Label();
this.labelComment = new System.Windows.Forms.Label();

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
            this.splitContainer1.Panel1.Controls.Add(labelSupplierId);
this.splitContainer1.Panel1.Controls.Add(labelName);
this.splitContainer1.Panel1.Controls.Add(labelVersion);
this.splitContainer1.Panel1.Controls.Add(labelComment);

            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.buttonDelete);
            this.splitContainer1.Panel2.Controls.Add(this.buttonReload);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAction);
            this.splitContainer1.Panel2.Controls.Add(textboxSupplierId);
this.splitContainer1.Panel2.Controls.Add(textboxName);
this.splitContainer1.Panel2.Controls.Add(numericVersion);
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
            // textboxSupplierId
            // 
            this.textboxSupplierId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxSupplierId.Location = new System.Drawing.Point(4, 12);
            this.textboxSupplierId.Name = "textboxSupplierId";
            this.textboxSupplierId.Size = new System.Drawing.Size(208, 20);
            this.textboxSupplierId.TabIndex = 31;
            this.textboxSupplierId.Leave += new System.EventHandler(this.textboxSupplierId_Leave);
            this.textboxSupplierId.Enabled = false;
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
            // numericVersion
            // 
            this.numericVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericVersion.Location = new System.Drawing.Point(4, 64);
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
            this.textboxComment.Location = new System.Drawing.Point(4, 90);
            this.textboxComment.Name = "textboxComment";
            this.textboxComment.Size = new System.Drawing.Size(208, 20);
            this.textboxComment.TabIndex = 31;
            this.textboxComment.Leave += new System.EventHandler(this.textboxComment_Leave);
            this.textboxComment.Enabled = true;
                        // 
            // labelSupplierId
            // 
            this.labelSupplierId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSupplierId.AutoSize = true;
            this.labelSupplierId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSupplierId.Location = new System.Drawing.Point(0, 12);
            this.labelSupplierId.Name = "labelSupplierId";
            this.labelSupplierId.Size = new System.Drawing.Size(30, 20);
            this.labelSupplierId.TabIndex = 14;
            this.labelSupplierId.Text = "ID";
            this.labelSupplierId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // labelVersion
            // 
            this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVersion.AutoSize = true;
            this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersion.Location = new System.Drawing.Point(0, 64);
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
            this.labelComment.Location = new System.Drawing.Point(0, 90);
            this.labelComment.Name = "labelComment";
            this.labelComment.Size = new System.Drawing.Size(30, 20);
            this.labelComment.TabIndex = 14;
            this.labelComment.Text = "Comment";
            this.labelComment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            
            // 
            // FormSupplierEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 156);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormSupplierEditor";
            this.Text = "Supplier Editor";
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
        private System.Windows.Forms.TextBox textboxSupplierId;
private System.Windows.Forms.TextBox textboxName;
private System.Windows.Forms.NumericUpDown numericVersion;
private System.Windows.Forms.TextBox textboxComment;
private System.Windows.Forms.Label labelSupplierId;
private System.Windows.Forms.Label labelName;
private System.Windows.Forms.Label labelVersion;
private System.Windows.Forms.Label labelComment;

        #endregion
    }
}
