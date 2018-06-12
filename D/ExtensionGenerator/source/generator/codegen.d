/// This module contains all of the functions that are used to generate code.
module generator.codegen;
import scriptlike;
import generator.model;

private
{
    //////////////////////
    /// Template Files ///
    //////////////////////
    const TEMPLATE_MODEL_EXTENSION = cast(string)import("templates/modelExtension.cs");
    const TEMPLATE_SEARCHFORM_MAIN = cast(string)import("templates/SearchForm/searchFormMain.cs");
    const TEMPLATE_SEARCHFORM_EDITOR_CASES = cast(string)import("templates/SearchForm/searchFormEditorCases.cs");

    //////////////
    /// Config ///
    //////////////
    const SEARCH_FORM_EXTENSION_FILENAME = "SearchForm.cs";
    const WORDS_TO_CAPITALISE = ["bbc", "rss"];
}

void generateExtensions(Model model, Path outputDir)
{
	writefln("Generating model extensions, outputted to directory '%s'", outputDir);

	if(!outputDir.exists)
		mkdirRecurse(outputDir);

	foreach(object; model.objects)
	{
		// Find the corresponding DbSet for this object.
		DbSet dbSet = model.context.tables.filter!(s => s.typeName == object.className).front;
		
        // Generate an extension for this object.
		auto text = mixin(interp!TEMPLATE_MODEL_EXTENSION);
		writeFile(buildNormalizedPath(outputDir.raw, object.fileName), text);
	}
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
        char[] custom_FixedObjectName;

        // Standardise the name. 'device_type' -> 'DeviceType'
        foreach(word; object.className.splitter('_'))
        {
            if(WORDS_TO_CAPITALISE.canFind(word))
            {
                char[] name = word.dup;
                name.toUpperInPlace();
                custom_FixedObjectName ~= name;
            }
            else
            {
                char[] name = word.dup;
                name[0] = cast(char)name[0].toUpper;
                custom_FixedObjectName ~= name;
            }
        }

        // Then generate a case from the template
        custom_EditorCaseStatements ~= mixin(interp!TEMPLATE_SEARCHFORM_EDITOR_CASES);
    }

    // And then, generate the main file
    auto text = mixin(interp!TEMPLATE_SEARCHFORM_MAIN);
    writeFile(buildNormalizedPath(outputDir.raw, SEARCH_FORM_EXTENSION_FILENAME), text);
}