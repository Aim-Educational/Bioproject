module generator;

private
{
    import std.file, std.path, std.exception;
    import aim.efparser, aim.templater;
    import config;

    const TEMPLATE_SEARCH_PROVIDER      = import("templates/searchProvider.cs");
    const TEMPLATE_COLUMN_DEFINITION    = import("templates/column.cs");
    const TEMPLATE_LABEL_XAML           = import("templates/editor_label.xaml");
    const TEMPLATE_EDITOR_XAML          = import("templates/editor/editor.xaml");
    const TEMPLATE_EDITOR_CODE          = import("templates/editor/editor.xaml.cs");
    const TEMPLATE_CSPROJ_XAML          = import("templates/csproj_entry_xaml.xml");
    const TEMPLATE_CSPROJ_CODE          = import("templates/csproj_entry_code.xml");
    const TEMPLATE_WINDOW_XAML          = import("templates/window.xaml");
    const TEMPLATE_WINDOW_CODE          = import("templates/window.xaml.cs");
    const TEMPLATE_WINDOW_BUTTON        = import("templates/windowbutton.cs");
    
    const TEMPLATE_TEXTBOX_XAML         = import("templates/textbox/textbox.xaml");
    const TEMPLATE_TEXTBOX_CREATE       = import("templates/textbox/create.cs");
    const TEMPLATE_TEXTBOX_LOAD         = import("templates/textbox/load.cs");
    const TEMPLATE_TEXTBOX_SAVE         = import("templates/textbox/save.cs");

    const TEMPLATE_CHECKBOX_XAML        = import("templates/checkbox/checkbox.xaml");
    const TEMPLATE_CHECKBOX_CREATE      = import("templates/checkbox/create.cs");
    const TEMPLATE_CHECKBOX_LOAD        = import("templates/checkbox/load.cs");
    const TEMPLATE_CHECKBOX_SAVE        = import("templates/checkbox/save.cs");

    const TEMPLATE_DATEPICKER_XAML      = import("templates/datepicker/datepicker.xaml");
    const TEMPLATE_DATEPICKER_CREATE    = import("templates/datepicker/create.cs");
    const TEMPLATE_DATEPICKER_LOAD      = import("templates/datepicker/load.cs");
    const TEMPLATE_DATEPICKER_SAVE      = import("templates/datepicker/save.cs");

    const TEMPLATE_SELECTOR_XAML        = import("templates/selector/selector.xaml");
    const TEMPLATE_SELECTOR_CTOR        = import("templates/selector/ctor.cs");
    const TEMPLATE_SELECTOR_CREATE      = import("templates/selector/create.cs");
    const TEMPLATE_SELECTOR_LOAD        = import("templates/selector/load.cs");
    const TEMPLATE_SELECTOR_SAVE        = import("templates/selector/save.cs");

    struct ControlInfo
    {
        string xaml;
        string onCtor;
        string onCreate;
        string onLoad;
        string onSave;
    }
}

void generateFiles(Config config)
{
    auto model = Model.parseDirectory(config.modelFolder);

    generateProviders(model, config);
    generateEditorsXAML(model, config);
    generateEditorsCode(model, config);
    generateWindow(model, config);
}

private void generateWindow(Model model, Config config)
{
    auto namespace = getFinalNamespace(config, config.interfaceWindowNamespace);
    auto outPath = namespaceToPath(namespace);
    if(!outPath.exists)
        mkdirRecurse(outPath);

    write(buildNormalizedPath(outPath, "InterfaceWindow.xaml"), Templater.resolveTemplate(["$WINDOW_NAMESPACE": namespace], TEMPLATE_WINDOW_XAML));
    string buttons;
    foreach(object; model.objects)
    {
        buttons ~= Templater.resolveTemplate(
            [
                "$DISPLAY_NAME": object.className,
                "$PROVIDER_NAME": getProviderName(object),
                "$EDITOR_NAME": getEditorName(object)
            ], 
            TEMPLATE_WINDOW_BUTTON
        );
    }

    auto placeholders = 
    [
        "$WINDOW_NAMESPACE":    namespace,
        "$CONTROL_NAMESPACE":   getFinalNamespace(config, config.editorNamespace),
        "$PROVIDER_NAMESPACE":  getFinalNamespace(config, config.searchProviderNamespace),
        "$BUTTONS":             buttons
    ];

    write(buildNormalizedPath(outPath, "InterfaceWindow.xaml.cs"), Templater.resolveTemplate(placeholders, TEMPLATE_WINDOW_CODE));

}

private void generateEditorsCode(Model model, Config config)
{
    auto editorNamespace = getFinalNamespace(config, config.editorNamespace);
    auto outPath = namespaceToPath(editorNamespace);
    if(!outPath.exists)
        mkdirRecurse(outPath);

    foreach(object; model.objects)
    {
        string onCtor;
        string onCreate;
        string onLoad;
        string onSave;

        int row = 0;
        foreach(field; object.fields)
        {
            if(shouldIgnoreField(config, model, object, field))
                continue;

            auto info = getControlInfo(config, model, object, field, row++);
            onCtor   ~= info.onCtor;
            onCreate ~= info.onCreate;
            onLoad   ~= info.onLoad;
            onSave   ~= info.onSave;
        }

        auto placeholders = 
        [
            "$MODEL_NAMESPACE":     model.namespace,
            "$PROVIDER_NAMESPACE":  getFinalNamespace(config, config.searchProviderNamespace),
            "$EDITOR_NAMESPACE":    getFinalNamespace(config, config.editorNamespace),
            "$EDITOR_NAME":         getEditorName(object),
            "$PK_NAME":             object.keyName,
            "$OBJECT_TYPE":         object.className,
            "$CONTEXT_NAME":        model.context.className,
            "$TABLE_NAME":          model.context.getTableForType(object.className).variableName,
            "$CTOR_CODE":           onCtor,
            "$CREATE_CODE":         onCreate,
            "$LOAD_CODE":           onLoad,
            "$SAVE_CODE":           onSave
        ];

        write(buildNormalizedPath(outPath, getEditorName(object)~".xaml.cs"), Templater.resolveTemplate(placeholders, TEMPLATE_EDITOR_CODE));
        writeXAMLEntry(config, outPath, buildNormalizedPath(outPath, getEditorName(object)~".xaml"));
    }
}

private void generateEditorsXAML(Model model, Config config)
{
    import std.format : format;
    import std.conv   : to;

    auto editorNamespace = getFinalNamespace(config, config.editorNamespace);
    auto outPath = namespaceToPath(editorNamespace);
    if(!outPath.exists)
        mkdirRecurse(outPath);

    string[] xamlFiles;

    foreach(object; model.objects)
    {
        // Create the XAML definitions.
        string rowDefs;
        string labels;
        string controls;
        int row = 0;
        foreach(i, field; object.fields)
        {
            if(shouldIgnoreField(config, model, object, field))
                continue;

            labels ~= Templater.resolveTemplate(
                [
                    "$GRID_ROW": row.to!string,
                    "$CONTENT": getLabelText(config, object, field)
                ], 
                TEMPLATE_LABEL_XAML
            );

            rowDefs ~= getRowDef(model, field);
            controls ~= getControlInfo(config, model, object, field, row++).xaml;
        }

        // Resolve the template.
        string[string] placeholders;
        placeholders["$EDITOR_NAMESPACE"]   = editorNamespace;
        placeholders["$EDITOR_NAME"]        = getEditorName(object);
        placeholders["$ROW_DEFS"]           = rowDefs;
        placeholders["$LABELS"]             = labels;
        placeholders["$CONTROLS"]           = controls;

        auto file = buildNormalizedPath(outPath, getEditorName(object)~".xaml");
        xamlFiles ~= file;
        write(file, Templater.resolveTemplate(placeholders, TEMPLATE_EDITOR_XAML));
    }

    foreach(file; xamlFiles)
        writeXAMLEntry(config, outPath, file);
}

private void generateProviders(Model model, Config config)
{
    auto providerNamespace = getFinalNamespace(config, config.searchProviderNamespace);
    auto outPath = namespaceToPath(providerNamespace);
    if(!outPath.exists)
        mkdirRecurse(outPath);

    string[] files;
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
        placeholders["$OBJECT_TYPE"]        = object.className;
        placeholders["$TABLE_NAME"]         = model.context.getTableForType(object.className).variableName;
        placeholders["$PRIMARY_KEY"]        = object.keyName;
        placeholders["$TABLE_DISPLAY_NAME"] = displayName;
        placeholders["$COLUMN_DEFINITIONS"] = definitions;
        placeholders["$CACHE_CODE"]         = cache;
        placeholders["$CONTEXTNAME"]        = model.context.className;

        files ~= buildNormalizedPath(outPath, getProviderName(object)~".cs");
        write(files[$-1], Templater.resolveTemplate(placeholders, TEMPLATE_SEARCH_PROVIDER));
    }

    writeCodeEntries(config, outPath, files);
}

private string cacheObject(Model model, TableObject object, ref string code, int loopDepth = 0, string valName = "data", bool isFirst = true)
{
    import std.format : format;

    foreach(field; object.fields)
    {
        if(!model.isObjectType(field.typeName))
            continue;

        code ~= format("if(%s.%s != null) %s.%s.ToString();\n", valName, field.variableName, valName, field.variableName);
    }

    foreach(dep; object.dependants)
    {
        code ~= format("foreach(var val%s in %s.%s){\n", loopDepth, valName, dep.dependantGetter.variableName);

        if(isFirst)
            cacheObject(model, dep.dependant, code, loopDepth+1, format("val%s", loopDepth), false);

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

private ControlInfo getControlInfo(Config config, Model model, TableObject object, Field field, int row)
{
    import std.algorithm : filter;
    import std.conv      : to;

    ControlInfo info;
    string readOnly = (field.variableName == object.keyName) ? "true" : "false";

    auto placeholders = 
    [
        // Some default ones
        "$NAME":          field.variableName,
        "$GRID_ROW":      row.to!string,
        "$GRID_COLUMN":   "2",
        "$READ_ONLY":     readOnly,
        "$OBJECT_TYPE":   field.typeName
    ];

    // Statically known types.
    switch(field.typeName)
    {
        case "string":
            placeholders["$CONVERT"] = "";
            info.xaml       = Templater.resolveTemplate(placeholders, TEMPLATE_TEXTBOX_XAML);
            info.onCreate   = Templater.resolveTemplate(placeholders, TEMPLATE_TEXTBOX_CREATE);
            info.onLoad     = Templater.resolveTemplate(placeholders, TEMPLATE_TEXTBOX_LOAD);
            info.onSave     = Templater.resolveTemplate(placeholders, TEMPLATE_TEXTBOX_SAVE);
            break;

        case "bool":
            info.xaml       = Templater.resolveTemplate(placeholders, TEMPLATE_CHECKBOX_XAML);
            info.onCreate   = Templater.resolveTemplate(placeholders, TEMPLATE_CHECKBOX_CREATE);
            info.onLoad     = Templater.resolveTemplate(placeholders, TEMPLATE_CHECKBOX_LOAD);
            info.onSave     = Templater.resolveTemplate(placeholders, TEMPLATE_CHECKBOX_SAVE);
            break;

        case "DateTime":
        case "DateTime?":
            info.xaml       = Templater.resolveTemplate(placeholders, TEMPLATE_DATEPICKER_XAML);
            info.onCreate   = Templater.resolveTemplate(placeholders, TEMPLATE_DATEPICKER_CREATE);
            info.onLoad     = Templater.resolveTemplate(placeholders, TEMPLATE_DATEPICKER_LOAD);
            info.onSave     = Templater.resolveTemplate(placeholders, TEMPLATE_DATEPICKER_SAVE);
            break;

        case "int":
        case "long":
        case "float":
        case "double":
        case "decimal":
            auto converters =
            [
                "int":     "Convert.ToInt32",
                "long":    "Convert.ToInt64",
                "float":   "Convert.ToSingle",
                "double":  "Convert.ToDouble",
                "decimal": "Convert.ToDecimal"
            ];

            placeholders["$CONVERT"] = converters[field.typeName];
            info.xaml       = Templater.resolveTemplate(placeholders, TEMPLATE_TEXTBOX_XAML);
            info.onCreate   = Templater.resolveTemplate(placeholders, TEMPLATE_TEXTBOX_CREATE);
            info.onLoad     = Templater.resolveTemplate(placeholders, TEMPLATE_TEXTBOX_LOAD);
            info.onSave     = Templater.resolveTemplate(placeholders, TEMPLATE_TEXTBOX_SAVE);
            break;

        default: break;
    }

    // 'dynamic'ally known types (e.g. not built in).
    if(model.isObjectType(field.typeName))
    {
        placeholders["$PROVIDER_NAME"] = getProviderName(model.getObjectByType(field.typeName)),
        info.xaml       = Templater.resolveTemplate(placeholders, TEMPLATE_SELECTOR_XAML);
        info.onCtor     = Templater.resolveTemplate(placeholders, TEMPLATE_SELECTOR_CTOR);
        info.onCreate   = Templater.resolveTemplate(placeholders, TEMPLATE_SELECTOR_CREATE);
        info.onLoad     = Templater.resolveTemplate(placeholders, TEMPLATE_SELECTOR_LOAD);
        info.onSave     = Templater.resolveTemplate(placeholders, TEMPLATE_SELECTOR_SAVE);
    }

    return info;
}

private bool shouldIgnoreField(Config config, Model model, TableObject object, Field field)
{
    import std.algorithm : endsWith, startsWith, canFind, any;
    import std.path      : globMatch;

    return (    config.ignoreVariables.any!(i => field.variableName.globMatch(i)) 
             && field.variableName != object.keyName
           )
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

private string getLabelText(Config config, TableObject object, Field field)
{
    import std.algorithm : splitter, map, joiner;
    import std.conv      : to;
    import std.path      : globMatch;
    import std.range     : split, array;
    import std.string    : capitalize;

    foreach(k, v; config.labelTextOverrides)
    {
        auto splitRange = k.split(":");
        enforce(splitRange.length == 2);

        auto objName   = splitRange[0];
        auto fieldName = splitRange[1];

        if(object.className.globMatch(objName) && field.variableName == fieldName)
            return v;
    }

    return field.variableName.splitter('_')
                             .map!capitalize
                             .joiner(" ")
                             .array // dstring
                             .to!string;
}

private void writeXAMLEntry(Config config, string outPath, string file)
{
    import std.string : detab;
    import std.path;
    import std.algorithm : canFind;

    auto csproj = readText(config.csproj);
    csproj.length -= "</Project>".length;
    csproj ~= "<ItemGroup>\n";

    auto code = Templater.resolveTemplate(["$XAMLFILE":file.baseName, "$XAMLFOLDER":file.dirName], TEMPLATE_CSPROJ_XAML);

    if(!csproj.detab.canFind(code.detab))
        csproj ~= code;
    csproj ~= "</ItemGroup>\n</Project>";

    write(config.csproj, csproj);
}

private void writeCodeEntries(Config config, string outPath, string[] files)
{
    import std.string : detab;

    auto csproj = readText(config.csproj);
    csproj.length -= "</Project>".length;
    
    csproj ~= "<ItemGroup>\n";
    foreach(file; files)
    {
        import std.algorithm : canFind;
        auto code = Templater.resolveTemplate(["$FILE": file], TEMPLATE_CSPROJ_CODE);

        if(!csproj.detab.canFind(code.detab))
            csproj ~= code;
    }
    csproj ~= "</ItemGroup>\n</Project>";

    write(config.csproj, csproj);
}