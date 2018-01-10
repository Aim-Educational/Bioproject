#include "ToTemplate.h"

class ErrorList
{
    static extern ErrorCode[] errorCodes = 
    {
<ErrorCodes>
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