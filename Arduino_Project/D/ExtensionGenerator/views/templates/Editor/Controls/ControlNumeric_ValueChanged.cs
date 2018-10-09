        private void ${this.name}_ValueChanged(object sender, EventArgs e)
        {
            if(this._cached == null)
                return;

            if (Convert.${this.objectField.typeName == "decimal" ? "ToDecimal" : "ToDouble"}(this.${this.name}.Value) != this._cached.${this.objectField.variableName})
                this._isDirty = true;
        }
