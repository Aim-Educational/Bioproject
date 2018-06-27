foreach (var val in db.${custom_listTypeTable}.OrderBy(v => v.${this.keyVarName}))
    this.${this.name}.Items.Add(val.${this.keyVarName});
    