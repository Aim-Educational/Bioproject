private void ${this.name}_CheckedChanged(object sender, EventArgs e)
{
    if (this._cached != null && this.${this.name}.Checked != this._cached.${this.objectField.variableName})
        this._isDirty = true;
}