#include "ToTemplate.h"

class ErrorList
{
    static extern ErrorCode[] errorCodes = 
    {
{
  code = "1",
  mneumonic = "Unknown",
  narrative = "An unknown error has occured.",
  severity = "Severity::Warning"
},
{
  code = "1001",
  mneumonic = "FileNotFound",
  narrative = "Unable to find file/directory: %1",
  severity = "Severity::Severe"
},
{
  code = "1002",
  mneumonic = "DiskNotFound",
  narrative = "Unable to find disk: %1",
  severity = "Severity::Severe"
},
{
  code = "1003",
  mneumonic = "FileAlreadyOpen",
  narrative = "Unable to open file because it is already open: %1",
  severity = "Severity::Severe"
},
{
  code = "2000",
  mneumonic = "InternetNotAvaliable",
  narrative = "No connection to the internet is avaliable.",
  severity = "Severity::Severe"
},
{
  code = "2001",
  mneumonic = "InternetGatewayNotFound",
  narrative = "No connection to the gateway is avaliable.",
  severity = "Severity::Severe"
},
{
  code = "1004",
  mneumonic = "DiskFull",
  narrative = "The targeted disk is full: %1",
  severity = "Severity::Severe"
}
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