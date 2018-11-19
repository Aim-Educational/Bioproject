module generator;

private
{
    import std.file, std.path;
    import aim.efparser, aim.templater;
    import config;

    const TEMPLATE_SEARCH_PROVIDER      = import("templates/searchProvider.txt");
    const TEMPLATE_COLUMN_DEFINITION    = import("templates/column.txt");
    const TEMPLATE_LABEL_XAML           = import("templates/editor_label.xaml");
    const TEMPLATE_EDITOR_XAML          = import("templates/editor/editor.xaml");
    
    const TEMPLATE_TEXTBOX_XAML = import("templates/textbox/textbox.xaml");

    struct ControlInfo
    {
        string xaml;
    }
}

void generateFiles(Config config)
{
    auto model = Model.parseDirectory(config.modelFolder);

    generateProviders(model, config);
    generateEditorsXAML(model, config);
}

private void generateEditorsXAML(Model model, Config config)
{
    import std.format : format;
    import std.conv   : to;

    auto editorNamespace = getFinalNamespace(config, config.editorNamespace);
    auto outPath = namespaceToPath(editorNamespace);
    if(!outPath.exists)
        mkdirRecurse(outPath);

    foreach(object; model.objects)
    {
        // Create the XAML definitions.
        string rowDefs;
        string labels;
        string controls;
        foreach(i, field; object.fields)
        {
            if(shouldIgnoreField(model, object, field))
                continue;

            labels ~= Templater.resolveTemplate(
                [
                    "$GRID_ROW": i.to!string,
                    "$CONTENT": field.variableName
                ], 
                TEMPLATE_LABEL_XAML
            );

            rowDefs ~= getRowDef(model, field);
            controls ~= getControlInfo(model, object, field, i).xaml;
        }

        // Resolve the template.
        string[string] placeholders;
        placeholders["$EDITOR_NAMESPACE"]   = editorNamespace;
        placeholders["$EDITOR_NAME"]        = getEditorName(object);
        placeholders["$ROW_DEFS"]           = rowDefs;
        placeholders["$LABELS"]             = labels;
        placeholders["$CONTROLS"]           = controls;

        write(buildNormalizedPath(outPath, getEditorName(object)~".xaml"), Templater.resolveTemplate(placeholders, TEMPLATE_EDITOR_XAML));
    }
}

private void generateProviders(Model model, Config config)
{
    auto providerNamespace = getFinalNamespace(config, config.searchProviderNamespace);
    auto outPath = namespaceToPath(providerNamespace);
    if(!outPath.exists)
        mkdirRecurse(outPath);

    foreach(object; model.objects)
    {
        // Find the display name.
        string displayName;
        int largestPriority = -1;
        foreach(field; object.fields)
        {
            auto ptr = (field.variableName in config.displayNames);
            if(ptr is null)
                continue;

            if(*ptr > largestPriority)
                displayName = field.variableName;
        }
        if(displayName is null)
            displayName = object.keyName;

        // Create the column definitions.
        string definitions;
        foreach(field; config.columnVariables)
        {
            if(!object.hasField(field))
                continue;

            auto f = object.getFieldByName(field);
            definitions ~= Templater.resolveTemplate(
                [
                    "$HEADER": f.variableName,
                    "$VARIABLE": f.variableName,
                    "$TABLE_NAME": object.className
                ], 
                TEMPLATE_COLUMN_DEFINITION
            );
        }

        // Create the cache code.
        string cache;
        cacheObject(model, object, cache);

        // Resolve the template.
        string[string] placeholders;
        placeholders["$MODEL_NAMESPACE"]    = model.namespace;
        placeholders["$PROVIDER_NAMESPACE"] = providerNamespace;
        placeholders["$PROVIDER_NAME"]      = getProviderName(object);
        placeholders["$TABLE_NAME"]         = object.className;
        placeholders["$PRIMARY_KEY"]        = object.keyName;
        placeholders["$TABLE_DISPLAY_NAME"] = displayName;
        placeholders["$COLUMN_DEFINITIONS"] = definitions;
        placeholders["$CACHE_CODE"]         = cache;

        write(buildNormalizedPath(outPath, getProviderName(object)~".cs"), Templater.resolveTemplate(placeholders, TEMPLATE_SEARCH_PROVIDER));
    }
}

private string cacheObject(Model model, TableObject object, ref string code, int loopDepth = 0, string valName = "data", bool isFirst = true)
{
    import std.format : format;

    foreach(field; object.fields)
    {
        if(!model.isObjectType(field.typeName))
            continue;

        code ~= format("%s.%s.ToString();\n", valName, field.variableName);
    }

    if(object.dependants.length > 0)
    {
        foreach(dep; object.dependants)
        {
            code ~= format("foreach(var val%s in %s.%s){\n", loopDepth, valName, dep.dependantGetter.variableName);

            if(isFirst)
                cacheObject(model, dep.dependant, code, loopDepth+1, format("val%s", loopDepth), false);
            else
                code ~= "}\n";
        }
        code ~= "}\n";
    }

    return code;
}

private string getRowDef(Model model, Field field)
{
    import std.algorithm : startsWith;

    if(field.typeName.startsWith("ICollection<"))
        return "<RowDefinition Height=\"150\"/>\n";
    else
        return "<RowDefinition Height=\"30\"/>\n";
}

private ControlInfo getControlInfo(Model model, TableObject object, Field field, int row)
{
    import std.algorithm : filter;
    import std.conv      : to;

    ControlInfo info;
    string readOnly = (field.variableName == object.keyName) ? "true" : "false";

    // Statically known types.
    switch(field.typeName)
    {
        case "string":
            info.xaml = Templater.resolveTemplate(
                [
                    "$NAME": field.variableName,
                    "$GRID_ROW": row.to!string,
                    "$READ_ONLY": readOnly
                ],
                TEMPLATE_TEXTBOX_XAML
            );
            break;

        case "int":
        case "long":
        case "float":
        case "double":
            info.xaml = Templater.resolveTemplate(
                [
                    "$NAME": field.variableName,
                    "$GRID_ROW": row.to!string,
                    "$READ_ONLY": readOnly
                ],
                TEMPLATE_TEXTBOX_XAML
            );
            break;

        default: break;
    }

    // 'dynamic'ally known types (e.g. not built in).

    return info;
}

private bool shouldIgnoreField(Model model, TableObject object, Field field)
{
    import std.algorithm : endsWith, startsWith;

    return (field.variableName.endsWith("id") && field.variableName != object.keyName)
        || (field.typeName.startsWith("ICollection<"));
}

private string getFinalNamespace(Config config, string namespace)
{
    return config.defaultNamespace ~ "." ~ namespace;
}

private string namespaceToPath(string namespace)
{
    import std.string : replace;

    return namespace.replace(".", "/");
}

private string getProviderName(TableObject obj)
{
    return "SearchProvider"~obj.className;
}

private string getEditorName(TableObject obj)
{
    return "Editor"~obj.className;
}