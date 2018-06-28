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

    @Ignore
    string[] wordsToCapitalise;
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

    @Ignore
    int[string] objectListVariablePriority;

    @Ignore
    string[] variablesToIgnore;

    @Ignore
    string[string] labelTextOverrides;
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

    // Read in the @Ignored stuff
    auto tag = configSDL.expectTag("projUserInterface").expectTag("objectListVariablePriority");
    foreach(subTag; tag.tags)
    {
        enforce(subTag.values.length == 2, "Expected 2 values"); // TODO: Better error message.
        appConfig.projUserInterface.objectListVariablePriority[subTag.values[0].get!string] = subTag.values[1].get!int;
    }

    tag = configSDL.expectTag("wordsToCapitalise");
    foreach(subTag; tag.tags)
        appConfig.wordsToCapitalise ~= subTag.expectValue!string();

    tag = configSDL.expectTag("projUserInterface").expectTag("variablesToIgnore");
    foreach(subTag; tag.tags)
        appConfig.projUserInterface.variablesToIgnore ~= subTag.expectValue!string();

    tag = configSDL.expectTag("projUserInterface").expectTag("labelTextOverrides");
    foreach(subTag; tag.tags)
    {
        enforce(subTag.values.length == 2, "Expected 2 values"); // TODO: Better error message.
        appConfig.projUserInterface.labelTextOverrides[subTag.values[0].get!string] = subTag.values[1].get!string;
    }

    // Validation
    enforce(appConfig.projDataManager.rootDir.isAbsolute, 
            format("The root directory for the DataManager is not absolute. '%s'", appConfig.projDataManager.rootDir));

    enforce(appConfig.projUserInterface.rootDir.isAbsolute,
            format("The root directory for the DataUserInterface is not absolute. '%s'", appConfig.projUserInterface.rootDir));
}