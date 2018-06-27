foreach (var value in db.${custom_listTypeTable}.OrderBy(v => v.${this.keyVarName}))
{
    this.${this.name}.Items.Add(value.${this.keyVarName});
    if (value.${custom_valueKey.variableName} == obj.${custom_objectKey.variableName})
        this.${this.name}.SelectedIndex = this.${this.name}.Items.Count - 1;
}
