this.${this.name}.Text = ${this.objectField.typeName != "string" ? "Convert.ToString(obj.%s)".format(this.objectField.variableName) : "obj." ~ this.objectField.variableName};
