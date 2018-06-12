import std.stdio;
import scriptlike;
import generator.model, generator.codegen;

void main()
{
	debug scriptlikeEcho = true;
	try
	{
		// Hard coded paths for testing
		auto model = parseModelDirectory(Path(`C:\Users\user\Desktop\Arduino_Project\C#\DataManager\DataManager\Model`));
		validateModel(model);
		generateExtensions(model, Path(`C:\Users\user\Desktop\Arduino_Project\C#\DataManager\DataManager\Generated\ModelExtension`));
		generateSearchExtensions(model, Path(`C:\Users\user\Desktop\Arduino_Project\C#\DataUserInterface\DataUserInterface\Generated`));
	}
	catch(Exception ex)
	{
		// In debug mode, rethrow the exception so we can get a stack trace.
		writeln("Something went wrong: ", ex.msg);
		debug throw ex;
	}
}