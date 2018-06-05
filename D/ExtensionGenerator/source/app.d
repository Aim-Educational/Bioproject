import std.stdio, std.regex, std.path, std.file, std.experimental.logger, std.exception,
       std.format, std.algorithm;

/// Contains information about a DbSet from EF.
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
	string  className;
	DbSet[] tables;
}

struct TableObject
{
	string className;
	string keyName;
	string fileName;
}

struct Model
{
	string          namespace; // Determined by the file holding the DbContext
	DatabaseContext context;
	TableObject[]   objects;
}

void main()
{
	try
	{
		// Hard coded paths for testing
		auto model = parseDirectory(`C:\Users\user\Desktop\Arduino_Project\C#\DataManager\DataManager\Model`);
		validateModel(model);
		generateExtensions(model, `C:\Users\user\Desktop\Arduino_Project\C#\DataManager\DataManager\ModelExtension`);
	}
	catch(Exception ex)
	{
		// In debug mode, rethrow the exception so we can get a stack trace.
		writeln("Something went wrong: ", ex.msg);
		debug throw ex;
	}
}

Model parseDirectory(string dirPath)
{
	writefln("Looking through directory for files '%s'", dirPath);
	enforce(dirPath.exists, "The path '%s' doesn't exist.".format(dirPath));
	enforce(dirPath.isDir, "The path '%s' doesn't point to a directory".format(dirPath));

	// Only has one match, which is the name of the DbContext class.
	auto dbModelNameRegex = regex(`public\spartial\sclass\s([a-zA-Z_]+)\s:\sDbContext`);

	// Contains two matches, [1] is the class name of the DbSet, [2] is the variable name of the DbSet
	auto dbModelDbSetRegex = regex(`public\svirtual\sDbSet<([a-zA-Z_]+)>\s([a-zA-Z_]+)\s`);

	// Only has one match, which is the namespace
	auto dbModelNamespaceRegex = regex(`namespace\s([a-zA-Z\._]+)\s`);

	// Only has one match, which is the name of the table object class.
	auto dbObjectNameRegex = regex(`public\spartial\sclass\s([a-zA-Z_]+)\s`);

	// Only has one match, which is the name of the object's key variable.
	auto dbObjectKeyNameRegex = regex(`\[Key\]\s*\[?[^\]]+\]?\s*public\sint\s([a-zA-Z_0-1]+)\s\{\sget;\sset;\s\}`);

	Model model;
	foreach(DirEntry entry; dirEntries(dirPath, SpanMode.breadth))
	{
		if(entry.isDir || entry.name.extension != ".cs")
			continue;

		writefln("Processing %s...", entry.name.baseName);
		auto content = readText(entry.name);

		// Check if it's the DbContext file
		auto matches = matchFirst(content, dbModelNameRegex);
		if(matches.length > 1)
		{
			// Reminder: [0] is always the fully matched string.
			//           The actual capture groups start at [1]
			model.context.className = matches[1];

			// Figure out which namespace this is in.
			matches = matchFirst(content, dbModelNamespaceRegex);
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

			continue;
		}

		// Then, check if it's a 'public partial' class, which is probably one of the table objects.
		matches = matchFirst(content, dbObjectNameRegex);
		if(matches.length > 1)
		{
			TableObject object;
			object.className = matches[1];
			object.fileName  = entry.name.baseName;

			// Look for what the key is called
			matches = matchFirst(content, dbObjectKeyNameRegex);
			enforce(matches.length == 2, "Couldn't find the Key field for object '%s'".format(object.className));
			object.keyName = matches[1];

			model.objects ~= object;

			// QUESTION: Should the generator also check for 'version', 'is_active', 'comment', and 'timestamp'?
			// This'd make sure that all of our objects have them.
		}
	}

	return model;
}

void validateModel(Model model)
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

void generateExtensions(Model model, string outputDir)
{
	writefln("Generating model extensions, outputted to directory '%s'", outputDir);

	if(!outputDir.exists)
		mkdirRecurse(outputDir);

	import std.array : Appender;
	import scriptlike : interp, _interp_text;

	foreach(object; model.objects)
	{
		// Find the corresponding DbSet for this object.
		DbSet dbSet = model.context.tables.filter!(s => s.typeName == object.className).front;
			
		auto text = mixin(interp!`/// Generated by ExtensionGenerator
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ${model.namespace}
{
    public partial class ${object.className} : IDataModel
    {
        public bool isOutOfDate(${model.context.className} db)
        {
            var obj = db.${dbSet.variableName}.SingleOrDefault(d => d.${object.keyName} == this.${object.keyName});

            var dbTimestamp = BitConverter.ToInt64(obj.timestamp, 0);
            var localTimestamp = BitConverter.ToInt64(this.timestamp, 0);

            return (dbTimestamp > localTimestamp);
        }

        public bool isValidForUpdate(IncrementVersion shouldIncrement = IncrementVersion.no)
        {
            using (var db = new ${model.context.className}())
            {
                var obj = db.${dbSet.variableName}.SingleOrDefault(d => d.${object.keyName} == this.${object.keyName});
                
                if (this.isOutOfDate(db) && obj.version <= this.version)
                {
                    if(shouldIncrement == IncrementVersion.yes)
                        this.version += 1;

                    return true;
                }

                return false;
            }
        }
    }
}`);

		std.file.write(buildNormalizedPath(outputDir, object.fileName), text);
	}
}