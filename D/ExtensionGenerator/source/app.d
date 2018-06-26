import std.stdio;
import scriptlike;
import generator.model, generator.codegen;

/+
Next steps:
	- Modify the Model parser, so it can read in the variables for all of the TableObjects.
			- Use this information to automatically generate all of the editor forms.

	- Use std.getopt to make a proper command line interface for this program.
		- Alternativley/also use a config file for certain settings?
			- The config file could store the input and output directories, as well
			  as certain settings such as which words have to be made fully upper-case.

	- When/if needed, add the ability to parse all of the editor forms (or just generate the forms first,
	  and keep around whatever information is needed from the generation) so that the code generation
	  for the SearchForm can be more generalised, and so we can perform certain kind of validation.
	  	- Maybe also parse the EnumSearchFormType enum as well, to make sure all object names have a
		  corresponding enum value (after their names are standardised).
+/

void main()
{
	debug scriptlikeEcho = true;
	try
	{
		// Hard coded paths for testing
		auto model = parseModelDirectory(Path(`C:\Users\user\Desktop\Arduino_Project\C#\DataManager\DataManager\Generated\Model`));		
		validateModel(model);
		generateModelExtensions(model, Path(`C:\Users\user\Desktop\Arduino_Project\C#\DataManager\DataManager\Generated\ModelExtension\Generated`));
		generateCustomModelExtensions(model, Path(`C:\Users\user\Desktop\Arduino_Project\C#\DataManager\DataManager\Generated\ModelExtension\Custom`));
		generateSearchExtensions(model, Path(`C:\Users\user\Desktop\Arduino_Project\C#\DataUserInterface\DataUserInterface\Generated`));
		generateEditorStubs(model, Path(`C:\Users\user\Desktop\Arduino_Project\C#\DataUserInterface\DataUserInterface\Forms\Generated`));
		//writeln(model);
	}
	catch(Exception ex)
	{
		// In debug mode, rethrow the exception so we can get a stack trace.
		writeln("Something went wrong: ", ex.msg);
		debug throw ex;
	}
}