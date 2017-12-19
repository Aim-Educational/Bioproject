// Just an idea on what it could look like/work
class ProtocolSocket
{
    alias Result = string;?
    alias Result = Json;?
    alias Result = XML;?

    ProtocolSocket(Socket connection);

    Result sendVersionNumber(int version);
    Result sendHello();
    Result getErrorLog();
    // etc.

    // Example result from a valid 'getErrorLog'
    /*
    {
        "status": "ok",
        "errorLog": 
        [
            "[timestamp here] Failed to connect to server at xxx.xxx.x.xxx",
            "[timestamp here] Sensor reported unusual value '-2,132,420,203'"
        ]
    }
    */

    // Example of a timeout
    /*
    {
        "status": "timeout"
    }
    */

    // Example of some kind of error (timeout could be included as an error, instead of it's own status like above)
    /*
    {
        "status": "failed",
        "reason": "Not connected to the internet"
    }
    */
}

// Maybe instead of returning the JSON/XML/whatever response, we instead return it into some kind of class.
// e.g. 'getErrorLog' returns an 'ErrorLog' class, populated by the data from the JSON/XML/whatever response.

// It could look a little like this
class Result<T>
{
    // Some standard stuff between all responses
    ResultType type; // ok, time, failed, etc.
    string errorReason; // If the function failed, then this message is set to the reason why.

    // The data specific to the function.
    T data;
}

class ErrorLog
{
    string[] errors;
}

// Then some getErrorLog might look like
Result<ErrorLog> getErrorLog();