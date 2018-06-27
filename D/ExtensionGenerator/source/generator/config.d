module generator.config;

private
{
    import generator.serialise;
    import std.typecons : Nullable;

    const CONFIG_FILE = "config.sdl";
}

@Serialisable
struct Config
{
    mixin SerialisableInterface;

    ConfigDataManager projDataManager;
    ConfigDataUserInterface projUserInterface;
    Nullable!ConfigDebug debugOptions; // Notes: Will never be null, as 'loadConfig' will set it to a default value if it's not provided.
}

@Serialisable
struct ConfigDataManager
{
    mixin SerialisableInterface;

    string rootDir;
    string efModelDir;
    string generatorOutputDir;
}

@Serialisable
struct ConfigDataUserInterface
{
    mixin SerialisableInterface;

    string rootDir;
    string searchExtensionOutputDir;
    string searchExtensionFilename;
    string formOutputDir;
}

@Serialisable
struct ConfigDebug
{
    mixin SerialisableInterface;

    Nullable!bool echo;
    Nullable!bool dryRun;    
}

// The __gshared is more to make it obvious this is supposed to be used by other modules,
// rather than for it's multi-threaded reasons.
__gshared Config appConfig;

void loadConfig()
{
    import sdlang        : parseFile;
    import std.path      : isAbsolute;
    import std.exception : enforce;
    import std.format    : format;

    auto configSDL = parseFile(CONFIG_FILE);
    appConfig.updateFromSdlTag(configSDL);

    if(appConfig.debugOptions.isNull)
        appConfig.debugOptions = ConfigDebug.init;

    enforce(appConfig.projDataManager.rootDir.isAbsolute, 
            format("The root directory for the DataManager is not absolute. '%s'", appConfig.projDataManager.rootDir));

    enforce(appConfig.projUserInterface.rootDir.isAbsolute,
            format("The root directory for the DataUserInterface is not absolute. '%s'", appConfig.projUserInterface.rootDir));
}