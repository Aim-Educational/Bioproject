        private void ${this.name}_ValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToDouble(this.${this.name}.Value) != this._cached.${this.objectField.variableName})
                this._isDirty = true;
        }
