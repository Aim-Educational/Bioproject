private
{
	import std.stdio, std.getopt;
	import config, generator;
}

void main(string[] args)
{
	bool shouldGenerate;
	auto helpInfo = args.getopt(
		"generate|g", "Instructs the program to generate a default config file (if one doesn't exist)", &shouldGenerate
	);

	if(helpInfo.helpWanted)
	{
		defaultGetoptPrinter("", helpInfo.options);
		return;
	}

	if(shouldGenerate)
	{
		pcall(() => generateConfig());
		return;
	}

	Config conf;
	pcall(() => conf = readConfig());
	pcall(() => generateFiles(conf));
}

void pcall(F)(F func)
{
	version(release)
	{
		try func();
		catch(Exception ex)
		{
			writefln("ERROR: %s", ex.msg);
			return;
		}
	}
	else
	{
		func();
	}
}