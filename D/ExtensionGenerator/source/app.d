import std.stdio;
import scriptlike;
import generator.model, generator.codegen, generator.config;

void main()
{
	try
	{
		loadConfig();

		scriptlikeEcho   = appConfig.debugOptions.echo.get(false);
		scriptlikeDryRun = appConfig.debugOptions.dryRun.get(false);

		auto model = parseModelDirectory(Path(buildPath(appConfig.projDataManager.rootDir, appConfig.projDataManager.efModelDir)));		
		validateModel(model);
		generateModelExtensions(model, Path(buildPath(appConfig.projDataManager.rootDir, appConfig.projDataManager.generatorOutputDir, "ModelExtension/Generated")));
		generateCustomModelExtensions(model, Path(buildPath(appConfig.projDataManager.rootDir, appConfig.projDataManager.generatorOutputDir, "ModelExtension/Custom")));
		generateSearchExtensions(model, Path(buildPath(appConfig.projUserInterface.rootDir, appConfig.projUserInterface.searchExtensionOutputDir, appConfig.projUserInterface.searchExtensionFilename)));
		generateEditorForms(model, Path(buildPath(appConfig.projUserInterface.rootDir, appConfig.projUserInterface.formOutputDir)));
	}
	catch(Exception ex)
	{
		// In debug mode, rethrow the exception so we can get a stack trace.
		writeln("Something went wrong: ", ex.msg);
		debug throw ex;
	}
}