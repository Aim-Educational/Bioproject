        private void ${this.name}_Leave(object sender, EventArgs e)
        {
            // The Convert.ToString is just in case the value we're comparing to is something like an int.
            if (this.${this.name}.Text != Convert.ToString(this._cached.${this.objectField.variableName}))
                this._isDirty = true;
        }
        