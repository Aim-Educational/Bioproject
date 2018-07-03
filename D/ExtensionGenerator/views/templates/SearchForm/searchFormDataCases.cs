case EnumSearchFormType.${custom_FixedObjectName}:
    var ${custom_FixedObjectName}Query = from value in db.${custom_objectTableName}
                orderby value.${custom_objectKeyField.variableName}
                select value;

    this.list.Columns.Add("ID");
    this.list.Columns.Add("${custom_objectKeyField.variableName.standardisedName}");
    foreach (var value in alQuery)
    {
        var item = new ListViewItem(
            new string[]
            {
                Convert.ToString(value.${object.getKey().variableName}),
                value.custom_objectKeyField.variableName
            }
        );
        item.Tag = value.${object.getKey().variableName};
        this.list.Items.Add(item);
    }

    this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
    break;