/// This module contains all of the functions that are used to generate code.
module generator.codegen;
import scriptlike;
import generator.model;

private
{
    //////////////////////
    /// Template Files ///
    //////////////////////
    const TEMPLATE_MODEL_EXTENSION = cast(string)import("templates/Model/generated.cs");
    const TEMPLATE_MODEL_CUSTOM_EXTENSION = cast(string)import("templates/Model/custom.cs");
    const TEMPLATE_MODEL_CUSTOM_EXTENSION_IS_DELETABLE_QUERY = cast(string)import("templates/Model/custom_isDeletable_query.cs");
    const TEMPLATE_SEARCHFORM_MAIN = cast(string)import("templates/SearchForm/searchFormMain.cs");
    const TEMPLATE_SEARCHFORM_EDITOR_CASES = cast(string)import("templates/SearchForm/searchFormEditorCases.cs");
    const TEMPLATE_EDITOR_CODE = cast(string)import("templates/Editor/FormEditorCode.cs");
    const TEMPLATE_EDITOR_DESIGNER_CODE = cast(string)import("templates/Editor/FormEditorDesigner.cs");

    //////////////
    /// Config ///
    //////////////
    const SEARCH_FORM_EXTENSION_FILENAME = "SearchForm.cs";
    const WORDS_TO_CAPITALISE = ["bbc", "rss"];
}

void generateModelExtensions(Model model, Path outputDir)
{
    import std.format;
    import std.range;
    import std.algorithm;

	writefln("Generating model extensions, outputted to directory '%s'", outputDir);

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
    writefln("Generating model custom extensions, outputted to directory '%s'", outputDir);

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

void generateEditorStubs(const Model model, Path csprojDir, Path outputDir)
{
    import std.algorithm;
    import std.format;

    writefln("Generating Editor Form stubs, outputted to directory '%s'", outputDir);
    tryMkdirRecurse(outputDir);

    auto relativeDir = relativePath(outputDir, csprojDir);
    char[] csproj = "Include the following XML into your .csproj file, replacing any old generated ones.\n\n".dup;

    foreach(object; model.objects)
    {
        auto custom_fixedName     = object.className.standardisedName;
        auto custom_baseTypeTable = model.context.tables.filter!(t => t.typeName == object.className).front.variableName;

        auto fileName     = format("Form%sEditor", custom_fixedName);
        auto codeFile     = buildNormalizedPath(outputDir.raw, fileName ~ ".cs");
        auto designerFile = buildNormalizedPath(outputDir.raw, fileName ~ ".Designer.cs");

        csproj ~= format(
`<Compile Include="%s.cs">
    <SubType>Form</SubType>
</Compile>
<Compile Include="%s.Designer.cs">
    <DependentUpon>%s.cs</DependentUpon>
</Compile>`,

        buildPath(relativeDir.raw, fileName),
        buildPath(relativeDir.raw, fileName),
        buildPath(relativeDir.raw, fileName)
        );

        writeln("Generating ", fileName);
        if(!codeFile.exists)
            writeFile(codeFile, mixin(interp!TEMPLATE_EDITOR_CODE));

        if(!designerFile.exists)
            writeFile(designerFile, mixin(interp!TEMPLATE_EDITOR_DESIGNER_CODE));
    }

    writeln("==================================================================");
    writeln("== Please review the contents of 'ADD_TO_CSPROJ.txt', which was ==");
    writeln("== generated next to the .csproj file                           ==");
    writeln("==================================================================");
    writeFile(buildNormalizedPath(csprojDir.raw, "ADD_TO_CSPROJ.txt"), csproj);
}

void generateSearchExtensions(const Model model, Path outputDir)
{
    import std.uni : toUpperInPlace, toUpper;
    import std.array : array;
    import std.algorithm : splitter, canFind, map;

    writefln("Generating Search Form extensions, outputted to directory '%s'", outputDir);

    if(!outputDir.exists)
		mkdirRecurse(outputDir);

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
    writeFile(buildNormalizedPath(outputDir.raw, SEARCH_FORM_EXTENSION_FILENAME), text);
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
        if(WORDS_TO_CAPITALISE.canFind(word))
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