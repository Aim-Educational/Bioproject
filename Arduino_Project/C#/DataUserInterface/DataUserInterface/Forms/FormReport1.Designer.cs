namespace DataUserInterface.Forms
{
    partial class FormReport1
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "0",
            "Test"}, -1);
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.tabView = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.listDevices = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.chartIndividual = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.dropdownChartType = new System.Windows.Forms.ComboBox();
            this.chartStacked = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabView.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartIndividual)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartStacked)).BeginInit();
            this.SuspendLayout();
            // 
            // tabView
            // 
            this.tabView.Controls.Add(this.tabPage1);
            this.tabView.Controls.Add(this.tabPage2);
            this.tabView.Controls.Add(this.tabPage3);
            this.tabView.Location = new System.Drawing.Point(1, 0);
            this.tabView.Margin = new System.Windows.Forms.Padding(0);
            this.tabView.Name = "tabView";
            this.tabView.SelectedIndex = 0;
            this.tabView.Size = new System.Drawing.Size(478, 413);
            this.tabView.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.listDevices);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(470, 387);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Devices";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 35);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(26, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "v";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(26, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "^";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listDevices
            // 
            this.listDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listDevices.CheckBoxes = true;
            this.listDevices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listDevices.FullRowSelect = true;
            this.listDevices.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            listViewItem1.StateImageIndex = 0;
            this.listDevices.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.listDevices.Location = new System.Drawing.Point(38, 0);
            this.listDevices.MultiSelect = false;
            this.listDevices.Name = "listDevices";
            this.listDevices.Size = new System.Drawing.Size(249, 381);
            this.listDevices.TabIndex = 0;
            this.listDevices.UseCompatibleStateImageBehavior = false;
            this.listDevices.View = System.Windows.Forms.View.Details;
            this.listDevices.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listDevices_ItemChecked);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ID";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.chartIndividual);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(470, 387);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Chart[Individual]";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // chartIndividual
            // 
            legend1.Name = "Legend1";
            this.chartIndividual.Legends.Add(legend1);
            this.chartIndividual.Location = new System.Drawing.Point(0, 0);
            this.chartIndividual.Name = "chartIndividual";
            this.chartIndividual.Size = new System.Drawing.Size(470, 387);
            this.chartIndividual.TabIndex = 0;
            this.chartIndividual.Text = "chart1";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.dropdownChartType);
            this.tabPage3.Controls.Add(this.chartStacked);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(470, 387);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Chart[Stacked]";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Chart Type:";
            // 
            // dropdownChartType
            // 
            this.dropdownChartType.FormattingEnabled = true;
            this.dropdownChartType.Location = new System.Drawing.Point(75, 3);
            this.dropdownChartType.Name = "dropdownChartType";
            this.dropdownChartType.Size = new System.Drawing.Size(121, 21);
            this.dropdownChartType.TabIndex = 2;
            this.dropdownChartType.SelectionChangeCommitted += new System.EventHandler(this.dropdownChartType_SelectionChangeCommitted);
            // 
            // chartStacked
            // 
            this.chartStacked.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "Default";
            this.chartStacked.ChartAreas.Add(chartArea1);
            legend2.Name = "Legend1";
            this.chartStacked.Legends.Add(legend2);
            this.chartStacked.Location = new System.Drawing.Point(3, 30);
            this.chartStacked.Name = "chartStacked";
            this.chartStacked.Size = new System.Drawing.Size(464, 333);
            this.chartStacked.TabIndex = 1;
            this.chartStacked.Text = "chart1";
            // 
            // FormReport1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 410);
            this.Controls.Add(this.tabView);
            this.Name = "FormReport1";
            this.Text = "FormReport1";
            this.Load += new System.EventHandler(this.FormReport1_Load);
            this.Resize += new System.EventHandler(this.FormReport1_Resize);
            this.tabView.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartIndividual)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartStacked)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabView;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView listDevices;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartIndividual;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartStacked;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox dropdownChartType;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}