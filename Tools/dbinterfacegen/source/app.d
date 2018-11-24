private
{
    import std.stdio, std.getopt;
    import config, generator;
}

void main(string[] args)
{
    bool shouldGenerate;
    bool shouldPrint;
    auto helpInfo = args.getopt(
        "generate|g", 	 "Instructs the program to generate a default config file (if one doesn't exist)", &shouldGenerate,
        "print-model|p", "Prints the parsed version of the model out",									  &shouldPrint
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
    pcall(() => generateFiles(conf, shouldPrint));
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