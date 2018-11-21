module config;

private
{
    import std.exception : enforce;
    import std.typecons : Flag;
    import sdlang;
}

const CONFIG_TEMPLATE = import("config.template.sdl");
const CONFIG_NAME     = "dbinterfacegen.sdl";

alias ConfigGenerated = Flag!"confGenerated";

struct Config
{
    string      csproj;
    string      modelFolder;
    string      defaultNamespace;
    string      searchProviderNamespace;
    string      editorNamespace;
    string      interfaceWindowNamespace;
    int[string] displayNames;
    string[]    columnVariables;
    string[]    ignoreVariables;
}

ConfigGenerated generateConfig()
{
    import std.file : exists, write;

    if(CONFIG_NAME.exists)
        return ConfigGenerated.no;

    write(CONFIG_NAME, CONFIG_TEMPLATE);
    return ConfigGenerated.yes;
}

Config readConfig()
{
    import std.file : exists;

    enforce(CONFIG_NAME.exists, "No config file could be found.");
    
    Config config;
    auto sdl = parseFile(CONFIG_NAME);
    enforce(sdl.getTagValue!string("REMOVE_ME", null) == null, 
            "Please remove the 'REMOVE_ME' value from the config before continuing.");

    config.csproj                   = sdl.expectTagValue!string("csproj");
    config.modelFolder              = sdl.expectTagValue!string("modelFolder");
    config.defaultNamespace         = sdl.expectTagValue!string("defaultNamespace");
    config.searchProviderNamespace  = sdl.expectTagValue!string("searchProviderNamespace");
    config.editorNamespace          = sdl.expectTagValue!string("editorNamespace");
    config.interfaceWindowNamespace = sdl.expectTagValue!string("interfaceWindowNamespace");
    config.displayNames             = sdl.asAssocArray!(string, int)("displayNames");
    config.columnVariables          = sdl.asArray!string("columnVariables");
    config.ignoreVariables          = sdl.asArray!string("ignoreVariables");

    enforce(config.modelFolder.exists, "The modelFolder at "~config.modelFolder~" doesn't exist.");

    return config;
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