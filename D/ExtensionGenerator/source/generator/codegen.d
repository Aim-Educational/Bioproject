/// This module contains all of the functions that are used to generate code.
module generator.codegen;
import scriptlike;
import generator.model, generator.config;
import std.typecons;

private
{
    alias IsInteger         = Flag!"isInteger";
    alias NeedsDesignerInit = Flag!"designerInit";
    alias IsReadOnly        = Flag!"readOnly";

    //////////////////////
    /// Template Files ///
    //////////////////////
    const TEMPLATE_MODEL_EXTENSION                           = import("templates/Model/generated.cs");
    const TEMPLATE_MODEL_CUSTOM_EXTENSION                    = import("templates/Model/custom.cs");
    const TEMPLATE_MODEL_CUSTOM_EXTENSION_IS_DELETABLE_QUERY = import("templates/Model/custom_isDeletable_query.cs");
    const TEMPLATE_SEARCHFORM_MAIN                           = import("templates/SearchForm/searchFormMain.cs");
    const TEMPLATE_SEARCHFORM_EDITOR_CASES                   = import("templates/SearchForm/searchFormEditorCases.cs");
    const TEMPLATE_EDITOR_CODE                               = import("templates/Editor/FormEditorCode.cs");
    const TEMPLATE_CONTROL_TEXTBOX_DESIGN                    = import("templates/Editor/Controls/ControlTextbox.cs");
    const TEMPLATE_CONTROL_TEXTBOX_CREATE_UPDATE             = import("templates/Editor/Controls/ControlTextbox_CreateUpdate.cs");
    const TEMPLATE_CONTROL_TEXTBOX_RELOAD                    = import("templates/Editor/Controls/ControlTextbox_Reload.cs");
    const TEMPLATE_CONTROL_TEXTBOX_EVENT_LEAVE               = import("templates/Editor/Controls/ControlTextbox_Leave.cs");
    const TEMPLATE_CONTROL_LABEL_DESIGN                      = import("templates/Editor/Controls/ControlLabel.cs");
    const TEMPLATE_CONTROL_NUMERIC_DESIGN                    = import("templates/Editor/Controls/ControlNumeric.cs");
    const TEMPLATE_CONTROL_NUMERIC_CREATE_UPDATE             = import("templates/Editor/Controls/ControlNumeric_CreateUpdate.cs");
    const TEMPLATE_CONTROL_NUMERIC_RELOAD                    = import("templates/Editor/Controls/ControlNumeric_Reload.cs");
    const TEMPLATE_CONTROL_NUMERIC_EVENT_ENTER               = import("templates/Editor/Controls/ControlNumeric_Enter.cs");
    const TEMPLATE_CONTROL_NUMERIC_EVENT_VALUECHANGED        = import("templates/Editor/Controls/ControlNumeric_ValueChanged.cs");
    const TEMPLATE_CONTROL_NUMERIC_CTOR_INIT                 = import("templates/Editor/Controls/ControlNumeric_CtorInit.cs");
    const TEMPLATE_CONTROL_OBJECTLIST_DESIGN                 = import("templates/Editor/Controls/ControlObjectDropdown.cs");
    const TEMPLATE_CONTROL_OBJECTLIST_CREATE_UPDATE          = import("templates/Editor/Controls/ControlObjectDropdown_CreateUpdate.cs");
    const TEMPLATE_CONTROL_OBJECTLIST_EVENT_SELECTIONCHANGED = import("templates/Editor/Controls/ControlObjectDropdown_SelectionChanged.cs");
    const TEMPLATE_CONTROL_OBJECTLIST_RELOAD_CREATEONLY      = import("templates/Editor/Controls/ControlObjectDropdown_Reload_CreateOnly.cs");
    const TEMPLATE_CONTROL_OBJECTLIST_RELOAD                 = import("templates/Editor/Controls/ControlObjectDropdown_Reload.cs");
    const TEMPLATE_CONTROL_SHOWLIST_BUTTON_DESIGN            = import("templates/Editor/Controls/ControlShowListButton.cs");
    const TEMPLATE_CONTROL_SHOWLIST_BUTTON_EVENT_CLICK       = import("templates/Editor/Controls/ControlShowListButton_Click.cs");
    const TEMPLATE_CONTROL_DATETIME_DESIGN                   = import("templates/Editor/Controls/ControlDateTime.cs");
    const TEMPLATE_CONTROL_DATETIME_CREATE_UPDATE            = import("templates/Editor/Controls/ControlDateTime_CreateUpdate.cs");
    const TEMPLATE_CONTROL_DATETIME_RELOAD                   = import("templates/Editor/Controls/ControlDateTime_Reload.cs");

    //////////////
    /// Config ///
    //////////////
    enum CONTROL_Y_PADDING  = 26;
    enum CONTROL_STARTING_Y = 12;
    // TODO: Add a config list of variable names to ignore for certain objects. (Only bother when needed)

    ////////////////
    /// Controls ///
    ////////////////
    abstract class Control 
    {
        string name;
        int yPos;
        const(Field) objectField;
        NeedsDesignerInit designerInit = NeedsDesignerInit.no;

        this(string name, const Field field)
        {
            this.name = name;
            this.objectField = field;
        }

        abstract string generateDesignCode();
        abstract string generateEventCode();
        abstract string generateCtorInit();
        abstract string generateReloadCode(const Model model, const TableObject object);
        abstract string generateCreateOnlyReloadCode(const Model model, const TableObject object);
        abstract string generateObjectCreateCode(const Model model);
        abstract string generateObjectUpdateCode(const Model model);
        abstract string winFormTypeName();
    }

    final class Textbox : Control
    {
        IsReadOnly readOnly;

        this(string name, const Field field, IsReadOnly readOnly)
        {
            this.readOnly = readOnly;
            super("textbox" ~ name.standardisedName.idup, field);
        }

        override string generateDesignCode()
        {
            return mixin(interp!TEMPLATE_CONTROL_TEXTBOX_DESIGN);
        }

        override string generateEventCode()
        {
            return mixin(interp!TEMPLATE_CONTROL_TEXTBOX_EVENT_LEAVE);
        }

        override string generateCtorInit() { return "";}

        override string generateReloadCode(const Model model, const TableObject object)
        {
            return mixin(interp!TEMPLATE_CONTROL_TEXTBOX_RELOAD);
        }

        override string generateCreateOnlyReloadCode(const Model model, const TableObject object) { return "";}

        override string generateObjectCreateCode(const Model model)
        {
            return mixin(interp!TEMPLATE_CONTROL_TEXTBOX_CREATE_UPDATE);
        }

        override string generateObjectUpdateCode(const Model model)
        {
            return this.generateObjectCreateCode(model);
        }

        override string winFormTypeName()
        {
            return "System.Windows.Forms.TextBox";
        }
    }

    final class Label : Control
    {
        string text;

        this(string name, const Field field, string text)
        {
            this.text = text;
            super("label" ~ name.standardisedName.idup, field);
        }

        override string generateDesignCode()
        {
            return mixin(interp!TEMPLATE_CONTROL_LABEL_DESIGN);
        }

        override string generateEventCode() { return "";}
        override string generateCtorInit() { return "";}
        override string generateReloadCode(const Model model, const TableObject object) { return "";}        
        override string generateCreateOnlyReloadCode(const Model model, const TableObject object) { return "";}
        override string generateObjectCreateCode(const Model model) { return "";}
        override string generateObjectUpdateCode(const Model model) { return "";}

        override string winFormTypeName()
        {
            return "System.Windows.Forms.Label";
        }
    }

    final class Numeric : Control
    {
        IsInteger isInteger;

        this(string name, const Field field, IsInteger isInteger)
        {
            this.isInteger = isInteger;
            super.designerInit = NeedsDesignerInit.yes;
            super("numeric" ~ name.standardisedName.idup, field);
        }

        override string generateDesignCode()
        {
            return mixin(interp!TEMPLATE_CONTROL_NUMERIC_DESIGN);
        }

        override string generateEventCode()
        {
            return mixin(interp!TEMPLATE_CONTROL_NUMERIC_EVENT_ENTER) ~ mixin(interp!TEMPLATE_CONTROL_NUMERIC_EVENT_VALUECHANGED);
        }

        override string generateCtorInit()
        {
            return mixin(interp!TEMPLATE_CONTROL_NUMERIC_CTOR_INIT);
        }

        override string generateReloadCode(const Model model, const TableObject object)
        {
            return mixin(interp!TEMPLATE_CONTROL_NUMERIC_RELOAD);
        }
        
        override string generateCreateOnlyReloadCode(const Model model, const TableObject object) { return "";}

        override string generateObjectCreateCode(const Model model)
        {
            return mixin(interp!TEMPLATE_CONTROL_NUMERIC_CREATE_UPDATE);
        }

        override string generateObjectUpdateCode(const Model model)
        {
            return this.generateObjectCreateCode(model);
        }

        override string winFormTypeName()
        {
            return "System.Windows.Forms.NumericUpDown";
        }
    }

    final class ObjectList : Control
    {
        string keyVarName;

        this(string name, const Field field, string keyVarName)
        {
            this.keyVarName = keyVarName;
            super("list" ~ name.standardisedName.idup, field);
        }

        override string generateDesignCode()
        {
            return mixin(interp!TEMPLATE_CONTROL_OBJECTLIST_DESIGN);
        }

        override string generateEventCode()
        {
            return mixin(interp!TEMPLATE_CONTROL_OBJECTLIST_EVENT_SELECTIONCHANGED);
        }

        override string generateCtorInit() { return "";}

        override string generateReloadCode(const Model model, const TableObject object)
        {
            auto custom_listTypeTable = model.context.getTableForType(this.objectField.typeName).variableName;
            auto valueObject = model.getObjectByType(this.objectField.typeName);

            Field custom_objectKey;
            foreach(dep; valueObject.dependants)
            {
                if(dep.dependant.className == object.className)
                    custom_objectKey = cast(Field)dep.dependantFK;
            }
            assert(custom_objectKey !is null);

            auto custom_valueKey = valueObject.getKey();
            return mixin(interp!TEMPLATE_CONTROL_OBJECTLIST_RELOAD);
        }
        
        override string generateCreateOnlyReloadCode(const Model model, const TableObject object)
        {
            auto custom_listTypeTable = model.context.getTableForType(this.objectField.typeName).variableName;
            return mixin(interp!TEMPLATE_CONTROL_OBJECTLIST_RELOAD_CREATEONLY);
        }

        override string generateObjectCreateCode(const Model model)
        {
            auto custom_listTypeTable = model.context.getTableForType(this.objectField.typeName).variableName;
            return mixin(interp!TEMPLATE_CONTROL_OBJECTLIST_CREATE_UPDATE);
        }

        override string generateObjectUpdateCode(const Model model)
        {
            return this.generateObjectCreateCode(model);
        }

        override string winFormTypeName()
        {
            return "System.Windows.Forms.ComboBox";
        }
    }

    final class ShowListButton : Control
    {
        this(string name, const Field field)
        {
            super("buttonShow" ~ name.standardisedName.idup, field);
        }

        override string generateDesignCode()
        {
            return mixin(interp!TEMPLATE_CONTROL_SHOWLIST_BUTTON_DESIGN);
        }

        override string generateEventCode()
        {
            return mixin(interp!TEMPLATE_CONTROL_SHOWLIST_BUTTON_EVENT_CLICK);
        }

        override string generateCtorInit() { return "";}
        override string generateReloadCode(const Model model, const TableObject object) { return "";}        
        override string generateCreateOnlyReloadCode(const Model model, const TableObject object) { return "";}
        override string generateObjectCreateCode(const Model model) { return "";}
        override string generateObjectUpdateCode(const Model model) { return "";}

        override string winFormTypeName()
        {
            return "System.Windows.Forms.Button";
        }
    }

    final class DateTime : Control
    {
        this(string name, const Field field)
        {
            super("datetime" ~ name.standardisedName.idup, field);
        }

        override string generateDesignCode()
        {
            return mixin(interp!TEMPLATE_CONTROL_DATETIME_DESIGN);
        }

        override string generateEventCode() { return "";}
        override string generateCtorInit() { return "";}

        override string generateReloadCode(const Model model, const TableObject object)
        {
            return mixin(interp!TEMPLATE_CONTROL_DATETIME_RELOAD);
        }
        
        override string generateCreateOnlyReloadCode(const Model model, const TableObject object) { return "";}

        override string generateObjectCreateCode(const Model model)
        {
            return mixin(interp!TEMPLATE_CONTROL_DATETIME_CREATE_UPDATE);
        }

        override string generateObjectUpdateCode(const Model model)
        {
            return this.generateObjectCreateCode(model);
        }

        override string winFormTypeName()
        {
            return "System.Windows.Forms.DateTimePicker";
        }
    }
}

void generateModelExtensions(Model model, Path outputDir)
{
    import std.format;
    import std.range;
    import std.algorithm;

	writefln("\n> Generating Model extensions, outputted to directory '%s'", outputDir);

	if(!outputDir.exists)
		mkdirRecurse(outputDir);

	foreach(object; model.objects)
	{
		// Find the corresponding DbSet for this object.
		DbSet dbSet = model.context.tables.filter!(s => s.typeName == object.className).front;

        // Create the code for the isDeletable function
        string custom_isDeletableCode;
        int queries = 0;
        foreach(dependant; object.dependants)
        {
            auto custom_isDeletable_queryName = format("query%s", queries);
            auto custom_isDeletable_queryDependantSet = model.context.tables.filter!(t => t.typeName == dependant.dependant.className).front;
            auto text = mixin(interp!TEMPLATE_MODEL_CUSTOM_EXTENSION_IS_DELETABLE_QUERY);
            custom_isDeletableCode ~= text ~ "\n\n";

            queries += 1;
        }

        // Generate the return statement (The random mass of spaces is so the output is formatted properly)
        if(queries == 0)
            custom_isDeletableCode ~= "            return true;";
        else
        {
            custom_isDeletableCode ~= format("            return %s;",
                                             iota(0, queries).map!(i => format("query%s.Count() == 0", i))
                                                             .joiner("\n                && ")
                                            );
        }
		
        // Generate an extension for this object.
		auto text = mixin(interp!TEMPLATE_MODEL_EXTENSION);
		writeFile(buildNormalizedPath(outputDir.raw, object.fileName), text);
	}
}

void generateCustomModelExtensions(const Model model, Path outputDir)
{
    writefln("\n> Generating model custom extension stubs, outputted to directory '%s'", outputDir);
    tryMkdirRecurse(outputDir);

    foreach(object; model.objects)
    {
        auto path = buildNormalizedPath(outputDir.raw, object.fileName);
        if(path.exists)
        {
            writeln("Skipping ", object.fileName);
            continue;
        }

        writeln("Generating ", object.fileName);
        auto text = mixin(interp!TEMPLATE_MODEL_CUSTOM_EXTENSION);
        writeFile(path, text);
    }
}

void generateEditorStubs(const Model model, Path outputDir)
{
    import std.algorithm;
    import std.format;

    writefln("\n> Generating Editor Form stubs, outputted to directory '%s'", outputDir);
    tryMkdirRecurse(outputDir);

    foreach(object; model.objects)
    {
        auto custom_fixedName     = object.className.standardisedName;
        auto custom_baseTypeTable = model.context.getTableForType(object.className).variableName;
        auto fileName             = format("Form%sEditor", custom_fixedName);

        // Then, create all of the controls it needs.
        // TODO: Move this into a function because it's gonna get _massive_
        writeln("\n>> Generating controls for ", fileName);

        struct ControlRow
        {
            Field field;
            string labelNameOverride; // If not null, then this name is used instead.
            int yPos;
            int priority; // Higher priority = Nearerst to the top of the form.
            Control[] controls;

            Label makeLabel() 
            { 
                auto obj = new Label(this.field.variableName.idup, 
                                     field,
                                     (this.labelNameOverride !is null) ? this.labelNameOverride 
                                                                       : this.field.variableName.standardisedName.idup);
                obj.yPos = this.yPos;
                return obj;
            }
        }

        ControlRow[] controls;
        foreach(field; object.fields)
        {
            auto fieldFQN         = format("%s.%s", object.className, field.variableName);
            auto objectQuery      = model.objects.filter!(f => f.className == field.typeName);
            auto row              = ControlRow(cast(Field)field);
            row.priority          = -1;
            row.labelNameOverride = appConfig.projUserInterface.labelTextOverrides.get(fieldFQN, null);

            if(appConfig.projUserInterface.variablesToIgnore.canFind(field.variableName))
            {
                writefln("\tIgnoring %s as it is listed in the variablesToIgnore list", fieldFQN);
            }
            else if(field.typeName == "string" || field.variableName == object.keyName)
            {
                writefln("\tCreating textbox for %s", fieldFQN);

                row.controls ~= new Textbox(field.variableName.idup, field, 
                                            field.variableName == object.keyName ? IsReadOnly.yes : IsReadOnly.no);

                if(field.variableName == object.keyName)
                {
                    row.labelNameOverride = "ID";
                    row.priority = 100; // The ID should be at the top
                }
            }
            else if(field.typeName == "int" || field.typeName == "float" || field.typeName == "double")
            {
                if(field.typeName == "int" && field.variableName.endsWith("_id"))
                {
                    writefln("\tIgnoring %s because it's a foriegn key", fieldFQN);
                    continue;
                }

                writefln("\tCreating numeric for %s", fieldFQN);
                row.controls ~= new Numeric(field.variableName.idup, field, 
                                            field.typeName == "int" ? IsInteger.yes : IsInteger.no);
            }
            else if(!objectQuery.empty)
            {
                writefln("\tCreating object list('%s') for %s", field.typeName, fieldFQN);

                auto listObject = objectQuery.front;
                assert(listObject.className == field.typeName);

                string bestKeyNameMatch;
                int bestMatchPriority;
                foreach(listField; listObject.fields)
                {
                    auto priority = appConfig.projUserInterface.objectListVariablePriority.get(listField.variableName, int.min);
                    if(priority > bestMatchPriority)
                    {
                        bestKeyNameMatch = listField.variableName;
                        bestMatchPriority = priority;
                    }
                }
                assert(bestMatchPriority != int.min);

                row.controls ~= new ObjectList(field.variableName.idup, field, bestKeyNameMatch);
                row.controls ~= new ShowListButton(field.variableName.idup, field);
                row.priority  = 50; // These should ideally be near the top, since they... look strange otherwise (there's probably some well known UI reason for why it looks odd)
            }
            else if(field.typeName == "DateTime")
            {
                writefln("\tCreating DateTimePicker for %s", fieldFQN);
                row.controls ~= new DateTime(field.variableName.idup, field);
            }
            else if(field.typeName.startsWith("ICollection<"))
            {
                writefln("\tIgnoring %s, as it is a list of dependants", fieldFQN);
            }
            else
            {
                writefln("\tWARNING: The type '%s' for variable '%s.%s' is being skipped, as there is no handler for it.",
                         field.typeName, object.className, field.variableName);
            }

            if(row.controls.length > 0)
                controls ~= row;
        }

        // Then, generate all of the controls and variables needed.
        char[] custom_control_events;
        char[] custom_control_initialisers;
        char[] custom_control_designs;
        char[] custom_control_variables;
        char[] custom_control_addToPanel1;
        char[] custom_control_addToPanel2;
        char[] custom_control_ctorInit;
        char[] custom_control_designerInitBegin;
        char[] custom_control_designerInitEnd;
        char[] custom_control_reload;
        char[] custom_control_reload_createOnly;
        char[] custom_control_createObjData;
        char[] custom_control_updateObjData;
        auto   custom_buttonYPad = (controls.length + 1) * CONTROL_Y_PADDING;
        void generateControlCode(Control control)
        {
            custom_control_events            ~= control.generateEventCode();
            custom_control_designs           ~= control.generateDesignCode();
            custom_control_ctorInit          ~= control.generateCtorInit();
            custom_control_reload            ~= control.generateReloadCode(model, object);
            custom_control_createObjData     ~= control.generateObjectCreateCode(model);
            custom_control_updateObjData     ~= control.generateObjectUpdateCode(model);
            custom_control_reload_createOnly ~= control.generateCreateOnlyReloadCode(model, object);
            custom_control_variables         ~= format("private %s %s;\n", control.winFormTypeName, control.name);
            custom_control_initialisers      ~= format("this.%s = new %s();\n", control.name, control.winFormTypeName);

            if(control.designerInit == NeedsDesignerInit.yes)
            {
                custom_control_designerInitBegin ~= format("((System.ComponentModel.ISupportInitialize)(this.%s)).BeginInit();\n", control.name);
                custom_control_designerInitEnd   ~= format("((System.ComponentModel.ISupportInitialize)(this.%s)).EndInit();\n", control.name);
            }

            // eww
            if(cast(Label)control !is null)
                custom_control_addToPanel1 ~= format("this.splitContainer1.Panel1.Controls.Add(%s);\n", control.name);
            else
                custom_control_addToPanel2 ~= format("this.splitContainer1.Panel2.Controls.Add(%s);\n", control.name);
        }

        foreach(i, row; controls.sort!("a.priority > b.priority", SwapStrategy.stable).enumerate)
        {
            row.yPos = cast(int)((CONTROL_Y_PADDING * i) + CONTROL_STARTING_Y);
            foreach(control; row.controls)
            {
                control.yPos = row.yPos;
                generateControlCode(control);
            }

            generateControlCode(row.makeLabel());
        }

        // Write the code out.
        auto codeFile = buildNormalizedPath(outputDir.raw, fileName ~ ".cs");
        if(!codeFile.exists)
        {
            writeln("> Generating ", fileName);
            writeFile(codeFile, mixin(interp!TEMPLATE_EDITOR_CODE));
        }
    }
}

void generateSearchExtensions(const Model model, Path outputFile)
{
    import std.uni : toUpperInPlace, toUpper;
    import std.array : array;
    import std.algorithm : splitter, canFind, map;

    writefln("\n> Generating Search Form extensions, outputted to file '%s'", outputFile);

    if(!outputFile.exists)
		mkdirRecurse(outputFile.dirName);

    // First, generate the cases
    char[] custom_EditorCaseStatements;
    foreach(object; model.objects)
    {
        char[] custom_FixedObjectName = object.className.standardisedName;

        // Then generate a case from the template
        custom_EditorCaseStatements ~= mixin(interp!TEMPLATE_SEARCHFORM_EDITOR_CASES);
    }

    // And then, generate the main file
    auto text = mixin(interp!TEMPLATE_SEARCHFORM_MAIN);
    writeFile(outputFile.raw, text);
}

char[] standardisedName(const char[] name)
{
    import std.uni : toUpperInPlace, toUpper;
    import std.array : array;
    import std.algorithm : splitter, canFind, map;

    char[] fixed;

    // Standardise the name. 'device_type' -> 'DeviceType'
    foreach(word; name.splitter('_'))
    {
        if(appConfig.wordsToCapitalise.canFind(word))
        {
            char[] copy = word.dup;
            copy.toUpperInPlace();
            fixed ~= copy;
        }
        else
        {
            char[] copy = word.dup;
            copy[0] = cast(char)copy[0].toUpper;
            fixed ~= copy;
        }
    }

    return fixed;
}