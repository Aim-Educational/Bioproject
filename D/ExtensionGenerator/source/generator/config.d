module generator.config;

private
{
    import std.typecons : Nullable;
    import std.exception : enforce;
    import sdlang;
    import generator.serialise;

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

    @Ignore
    string[] mandatoryVariables;
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
    import std.path   : isAbsolute;
    import std.format : format;

    // Load the Config
    auto configSDL = parseFile(CONFIG_FILE);
    appConfig.updateFromSdlTag(configSDL);

    // Setup defaults for stuff that weren't loaded in
    if(appConfig.debugOptions.isNull)
        appConfig.debugOptions = ConfigDebug.init;

    // Read in the @Ignored stuff
    appConfig.projUserInterface.objectListVariablePriority = configSDL.asAssocArray!(string, int)("projUserInterface", "objectListVariablePriority");
    appConfig.projUserInterface.labelTextOverrides         = configSDL.asAssocArray!(string, string)("projUserInterface", "labelTextOverrides");
    appConfig.wordsToCapitalise                            = configSDL.asArray!string("wordsToCapitalise");
    appConfig.projUserInterface.variablesToIgnore          = configSDL.asArray!string("projUserInterface", "variablesToIgnore");
    appConfig.projDataManager.mandatoryVariables           = configSDL.asArray!string("projDataManager", "mandatoryVariables");

    // Validation
    enforce(appConfig.projDataManager.rootDir.isAbsolute, 
            format("The root directory for the DataManager is not absolute. '%s'", appConfig.projDataManager.rootDir));

    enforce(appConfig.projUserInterface.rootDir.isAbsolute,
            format("The root directory for the DataUserInterface is not absolute. '%s'", appConfig.projUserInterface.rootDir));
}

private T[] asArray(T, S...)(Tag root, S names)
{
    // Get the tag.
    auto tag = root.getTagEasy(names);

    // Then read in it's data
    T[] data;
    foreach(subTag; tag.tags)
        data ~= subTag.expectValue!T();

    return data;
}

private V[K] asAssocArray(K, V, S...)(Tag root, S names)
{
    // Get the tag.
    auto tag = root.getTagEasy(names);

    // Then read in it's data
    V[K] data;
    foreach(subTag; tag.tags)
    {
        enforce(subTag.values.length == 2, "Expected 2 values"); // TODO: Better error message.
        data[subTag.values[0].get!K] = subTag.values[1].get!V;
    }

    return data;
}

private Tag getTagEasy(S...)(Tag root, S names)
if(S.length >= 1)
{
    auto tag = root.expectTag(names[0]);
    static if(S.length > 1)
    {
        foreach(name; names[1..$])
            tag = tag.expectTag(name);
    }

    return tag;
}