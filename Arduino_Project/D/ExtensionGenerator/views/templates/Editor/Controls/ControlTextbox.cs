            // 
            // ${this.name}
            // 
            this.${this.name}.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.${this.name}.Location = new System.Drawing.Point(4, ${this.yPos});
            this.${this.name}.Name = "${this.name}";
            this.${this.name}.Size = new System.Drawing.Size(208, 20);
            this.${this.name}.TabIndex = ${this.tabIndex};
            this.${this.name}.Leave += new System.EventHandler(this.${this.name}_Leave);
            this.${this.name}.Enabled = ${this.readOnly ? "false" : "true"};
            