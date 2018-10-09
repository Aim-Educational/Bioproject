        private void ${this.name}_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.${this.name}.SelectedIndex;
            var value = this.${this.name}.Items[index] as string;

            if (this.mode == EnumEditorMode.Create || (this._cached.${this.objectField.variableName} != null && value != this._cached.${this.objectField.variableName}.${this.keyVarName}))
                this._isDirty = true;
        }
        