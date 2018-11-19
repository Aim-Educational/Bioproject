///
module aim.templater.core;

private
{
    import std.exception : enforce;
}

private struct TemplateConfig
{
    string[] placeholders;
}

/// A class containing static functions for templating.
static class Templater
{
    /++
     + Resolves a template, and returns the resulting string.
     +
     + Template_Format:
     +  A template starts with various config options about the template, followed by a `$FINISH_CONFIG` tag, followed by the actual template text.
     +
     +  An example of a template would be
     +
     +  ```
     +  $PLACEHOLDERS
     +      $NAME  The person's name
     +      $AGE   The person's age
     +  $END
     +  $FINISH_CONFIG
     +  Hi, my name is $NAME and I'm $AGE years old.
     +  ```
     +
     + Placeholders:
     +  Placeholders are defined using the `$PLACEHOLDER` config tag, and are ended with the `$END` tag.
     +
     +  Between the two tags are line-seperated names starting with a '$', these are placeholder names.
     +  Anything after the first space after the name is discarded, and can be used for comments.
     +
     +  The values of placeholders are defined by the `placeholders` parameter, where the name of each placeholder is used
     +  as the key.
     +
     + Params:
     +  placeholders = The values for all of the placeholders.
     +  data         = The template's data.
     +
     + Returns:
     +  The resolved string.
     + ++/
    static string resolveTemplate(string[string] placeholders, string data)
    {
        import std.array : replace;

        const config = Templater.parseConfig(data, data);

        foreach(placeholder; config.placeholders)
        {
            enforce((placeholder in placeholders) !is null, 
                    "The placeholder '"~placeholder~"' hasn't been given a value.");
            
            data = data.replace(placeholder, placeholders[placeholder]);
        }

        return data;
    }

    private static TemplateConfig parseConfig(string data, out string remaining)
    {
        // Don't worry, I hate myself for this function as well.
        import std.ascii     : isWhite;
        import std.algorithm : all, splitter, countUntil;
        import std.string    : strip;

        enum Stage
        {
            None,
            Placeholders
        }

        size_t start;
        size_t end;
        TemplateConfig config;
        Stage stage;

        while(true)
        {
            if(end >= data.length)
                throw new Exception("No $FINISH_CONFIG was found.");
            
            if(data[end] == '\n')
            {
                auto str = data[start..end].strip;
                start = end + 1;

                if(str == "" || str.all!isWhite)
                {
                    end++;
                    continue;
                }

                final switch(stage)
                {
                    case Stage.None:
                        if(str == "$PLACEHOLDERS")
                            stage = Stage.Placeholders;
                        else if(str == "$FINISH_CONFIG")
                        {
                            remaining = data[start..$];
                            return config;
                        }
                        break;

                    case Stage.Placeholders:
                        if(str == "$END")
                        {
                            stage = Stage.None;
                            break;
                        }

                        auto index = str.countUntil(' ');
                        config.placeholders ~= str[0..(index == -1) ? $ : index];

                        enforce(config.placeholders[$-1][0] == '$', 
                                "'"~config.placeholders[$-1]~"' is an invalid placeholder name as it does not start with a '$'");
                        break;
                }
            }

            end++;
        }
    }
}
///
unittest
{
    auto templ = `
    $PLACEHOLDERS
        $NAME  The person's name
        $AGE   The person's age
    $END
    $FINISH_CONFIG
    Hi, my name is $NAME and I'm $AGE years old.`;

    auto data = 
    [
        "$NAME": "Bob",
        "$AGE": "69"
    ];
    assert(Templater.resolveTemplate(data, templ) == "    Hi, my name is Bob and I'm 69 years old.");
}