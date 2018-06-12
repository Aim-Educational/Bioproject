/// This module contains the data structures representing an EF Model,
/// functions to parse a Model, and functions to validate a Model.
module generator.model;

import std.stdio, std.regex, std.experimental.logger, std.exception,
       std.format, std.algorithm;

import scriptlike;

/// Contains information about a DbSet from EF.
///
/// For example, the variable `DbSet<device_type> devices`
/// would correspond with `DbSet("device_type", "devices")`
struct DbSet
{
	/// The name of the type the set stores
	string typeName;

	/// The name of the variable the set is stored as, inside of the DbContext.
	string variableName;
}

/// Contains the information about the DbContext class for the data model.
struct DatabaseContext
{
    /// The name of the DbContext class.
	string  className;

    /// All of the tables that the context manages.
	DbSet[] tables;
}

/// Contains information about an object in a table(DbSet).
struct TableObject
{
    /// The name of the class.
	string className;

    /// The name of the variable that stores the object's primary key.
	string keyName;

    /// The name of the file the object is stored in.
	string fileName;
}

/// Contains information about the entire model.
struct Model
{
    /// The namespace that all of the files in the model use.
	string          namespace; // Determined by the file holding the DbContext

    /// The main DbContext for the model.
	DatabaseContext context;

    /// The various objects that make up the model's data.
	TableObject[]   objects;
}

/++
 + Parses the directory created by EF, and gathers information about the model.
 +
 + Params:
 +  dirPath = The path to the directory to parse.
 +
 + Returns:
 +  The parsed `Model`
 + ++/
Model parseModelDirectory(Path dirPath)
{
	writefln("Looking through directory for files '%s'", dirPath);
	enforce(dirPath.exists, "The path '%s' doesn't exist.".format(dirPath));
	enforce(dirPath.isDir, "The path '%s' doesn't point to a directory".format(dirPath));

	// Only has one match, which is the name of the DbContext class.
	auto dbModelNameRegex = regex(`public\spartial\sclass\s([a-zA-Z_]+)\s:\sDbContext`);

	// Only has one match, which is the name of the table object class.
	auto dbObjectNameRegex = regex(`public\spartial\sclass\s([a-zA-Z_]+)\s`);

	Model model;
	foreach(entry; dirEntries(dirPath, SpanMode.breadth))
	{
		if(entry.isDir || entry.extension != ".cs")
			continue;

		writefln("Processing %s...", entry.baseName);
		auto content = readText(entry);

		// Check if it's the DbContext file
		auto matches = matchFirst(content, dbModelNameRegex);
		if(matches.length > 1)
		{
            // Reminder: [0] is always the fully matched string.
            //           The actual capture groups start at [1]
            parseDbContext(model, content, matches[1]);
			continue;
		}

		// Then, check if it's a 'public partial' class, which is probably one of the table objects.
		matches = matchFirst(content, dbObjectNameRegex);
		if(matches.length > 1)
		{
			parseObjectFile(model, content, matches[1], entry.name);
		}
	}

	return model;
}

private void parseDbContext(ref Model model, string content, string className)
{
    // Contains two matches, [1] is the class name of the DbSet, [2] is the variable name of the DbSet
	auto dbModelDbSetRegex = regex(`public\svirtual\sDbSet<([a-zA-Z_]+)>\s([a-zA-Z_]+)\s`);

    // Only has one match, which is the namespace
	auto dbModelNamespaceRegex = regex(`namespace\s([a-zA-Z\._]+)\s`);

    model.context.className = className;

    // Figure out which namespace this is in.
    auto matches = matchFirst(content, dbModelNamespaceRegex);
    enforce(matches.length == 2, "Could not determine the namespace for the model.");
    model.namespace = matches[1];

    // Look for all of the DbSets in the context, which represent tables.
    auto tableMatches = matchAll(content, dbModelDbSetRegex);
    foreach(match; tableMatches)
    {
        assert(match.length == 3);

        DbSet set;
        set.typeName = match[1];
        set.variableName = match[2];

        model.context.tables ~= set;
    }
}

private void parseObjectFile(ref Model model, string content, string className, string filePath)
{
    // This function is called multiple times, so ctRegex is being used instead of the normal regex.

    // Only has one match, which is the name of the object's key variable.
	auto dbObjectKeyNameRegex = ctRegex!(`\[Key\]\s*\[?[^\]]+\]?\s*public\sint\s([a-zA-Z_0-1]+)\s\{\sget;\sset;\s\}`);

    TableObject object;
    object.className = className;
    object.fileName  = filePath.baseName;

    // Look for what the key is called
    auto matches = matchFirst(content, dbObjectKeyNameRegex);
    enforce(matches.length == 2, "Couldn't find the Key field for object '%s'".format(object.className));
    object.keyName = matches[1];

    model.objects ~= object;

    // QUESTION: Should the generator also check for 'version', 'is_active', 'comment', and 'timestamp'?
    // This'd make sure that all of our objects have them.
}

void validateModel(const Model model)
{
	writeln("Validating model");
	// #1, make sure that for each table object, there's a corresponding DbSet inside the DbContext.
	foreach(object; model.objects)
	{
		bool found = false;
		foreach(set; model.context.tables)
		{
			if(set.typeName == object.className)
			{
				found = true;
				break;
			}
		}

		enforce(found, format("The DbContext '%s' has no DbSet for the table object '%s",
							  model.context.className, object.className));
	}
}