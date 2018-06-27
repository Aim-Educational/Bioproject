var selected${this.objectField.variableName.standardisedName} = this.${this.name}.Items[this.${this.name}.SelectedIndex] as string;
obj.${this.objectField.variableName} = db.${custom_listTypeTable}.Single(v => v.${this.keyVarName} == selected${this.objectField.variableName.standardisedName});
