import std.stdio;
import scriptlike;
import generator.model, generator.codegen, generator.config;

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
	try
	{
		loadConfig();

		scriptlikeEcho   = appConfig.debugOptions.echo.get(false);
		scriptlikeDryRun = appConfig.debugOptions.dryRun.get(false);

		auto model = parseModelDirectory(Path(buildPath(appConfig.projDataManager.rootDir, appConfig.projDataManager.efModelDir)));		
		validateModel(model);
		generateModelExtensions(model, Path(buildPath(appConfig.projDataManager.rootDir, appConfig.projDataManager.generatorOutputDir, "/ModelExtension/Generated")));
		generateCustomModelExtensions(model, Path(buildPath(appConfig.projDataManager.rootDir, appConfig.projDataManager.generatorOutputDir, "/ModelExtension/Custom")));
		generateSearchExtensions(model, Path(buildPath(appConfig.projUserInterface.rootDir, appConfig.projUserInterface.searchExtensionOutputDir, appConfig.projUserInterface.searchExtensionFilename)));
		generateEditorStubs(model, Path(buildPath(appConfig.projUserInterface.rootDir, appConfig.projUserInterface.formOutputDir)));
	}
	catch(Exception ex)
	{
		// In debug mode, rethrow the exception so we can get a stack trace.
		writeln("Something went wrong: ", ex.msg);
		debug throw ex;
	}
}