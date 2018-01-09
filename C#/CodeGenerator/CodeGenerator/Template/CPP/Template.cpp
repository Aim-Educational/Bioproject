#include "ToTemplate.h"

class ErrorList
{
    static extern ErrorCode[] errorCodes = 
    {
        <ErrorCodes>
        /*{
            code = "1",
            mneumonic = "Unknown",
            narrative = "An Unknown error has occurred.",
            severity = Severity::Warning;
        },
        {
            code = "1001",
            mneumonic = "FileNotFound",
            narrative = "File not found: %1",
            severity = Severity::Severe;
        }*/
    };

    static ErrorCode getErrorByMneumonic(string& mneumonic)
    {
        for(auto& code : ErrorList::errorCodes)
        {
            if(code.mneumonic == mneumonic)
                return code;
        }

        ErrorCode error = 
        {
            code = 9999,
            mneumonic = "ERROR MNEUMONIC NOT FOUND: " + mneumonic
        };

        return error;
    }
}