            // 
            // ${this.name}
            // 
            this.${this.name}.AutoSize = true;
            this.${this.name}.Location = new System.Drawing.Point(4, ${this.yPos + 2});
            this.${this.name}.Name = "${this.name}";
            this.${this.name}.Size = new System.Drawing.Size(53, 17);
            this.${this.name}.TabIndex = ${this.tabIndex};
            this.${this.name}.Text = "${this.name[`checkbox`.length..$]}";
            this.${this.name}.UseVisualStyleBackColor = true;
            this.${this.name}.CheckedChanged += new System.EventHandler(this.${this.name}_CheckedChanged);