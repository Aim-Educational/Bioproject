namespace RSSUserInterface
{
    partial class Form1
    {
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
            this.components = new System.ComponentModel.Container();
            this.timerDoProcess = new System.Windows.Forms.Timer(this.components);
            this.labelURL = new System.Windows.Forms.Label();
            this.textBoxURL = new System.Windows.Forms.TextBox();
            this.buttonProcessNow = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // timerDoProcess
            // 
            this.timerDoProcess.Enabled = true;
            this.timerDoProcess.Interval = 600000;
            this.timerDoProcess.Tick += new System.EventHandler(this.timerDoProcess_Tick);
            // 
            // labelURL
            // 
            this.labelURL.AutoSize = true;
            this.labelURL.Location = new System.Drawing.Point(12, 9);
            this.labelURL.Name = "labelURL";
            this.labelURL.Size = new System.Drawing.Size(32, 13);
            this.labelURL.TabIndex = 0;
            this.labelURL.Text = "URL:";
            // 
            // textBoxURL
            // 
            this.textBoxURL.Location = new System.Drawing.Point(50, 6);
            this.textBoxURL.Name = "textBoxURL";
            this.textBoxURL.Size = new System.Drawing.Size(222, 20);
            this.textBoxURL.TabIndex = 1;
            // 
            // buttonProcessNow
            // 
            this.buttonProcessNow.Location = new System.Drawing.Point(50, 45);
            this.buttonProcessNow.Name = "buttonProcessNow";
            this.buttonProcessNow.Size = new System.Drawing.Size(75, 23);
            this.buttonProcessNow.TabIndex = 2;
            this.buttonProcessNow.Text = "Do it now";
            this.buttonProcessNow.UseVisualStyleBackColor = true;
            this.buttonProcessNow.Click += new System.EventHandler(this.buttonProcessNow_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(197, 45);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(75, 23);
            this.buttonExit.TabIndex = 3;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 75);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonProcessNow);
            this.Controls.Add(this.textBoxURL);
            this.Controls.Add(this.labelURL);
            this.Name = "Form1";
            this.Text = "RSS";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timerDoProcess;
        private System.Windows.Forms.Label labelURL;
        private System.Windows.Forms.TextBox textBoxURL;
        private System.Windows.Forms.Button buttonProcessNow;
        private System.Windows.Forms.Button buttonExit;
    }
}

