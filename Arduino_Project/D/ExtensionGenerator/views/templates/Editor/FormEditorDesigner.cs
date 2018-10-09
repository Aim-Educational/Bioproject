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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form${custom_fixedName}Editor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelDirty = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonReload = new System.Windows.Forms.Button();
            this.buttonAction = new System.Windows.Forms.Button();
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
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.buttonDelete);
            this.splitContainer1.Panel2.Controls.Add(this.buttonReload);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAction);
            this.splitContainer1.Size = new System.Drawing.Size(330, 341);
            this.splitContainer1.SplitterDistance = 109;
            this.splitContainer1.TabIndex = 2000000;
            // 
            // labelDirty
            // 
            this.labelDirty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelDirty.AutoSize = true;
            this.labelDirty.Location = new System.Drawing.Point(3, 9);
            this.labelDirty.Name = "labelDirty";
            this.labelDirty.Size = new System.Drawing.Size(50, 13);
            this.labelDirty.TabIndex = 2000000;
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
            this.buttonDelete.TabIndex = 1999999;
            this.buttonDelete.UseVisualStyleBackColor = false;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonReload
            // 
            this.buttonReload.Location = new System.Drawing.Point(4, 314);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(75, 23);
            this.buttonReload.TabIndex = 1999998;
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
            this.buttonAction.TabIndex = 1999997;
            this.buttonAction.Text = "Save";
            this.buttonAction.UseVisualStyleBackColor = true;
            this.buttonAction.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // FormDeviceEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 341);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form${custom_fixedName}Editor";
            this.Text = "Device Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDeviceEditor_FormClosing);
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
#endregion