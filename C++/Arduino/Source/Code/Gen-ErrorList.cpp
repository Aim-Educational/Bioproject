#include "ToTemplate.h"

class ErrorList
{
    static extern ErrorCode[] errorCodes = 
    {
{
  code = "1",
  mneumonic = "Unknown",
  narrative = "Unknown",
  severity = "Severity::Warning"
},
{
  code = "1001",
  mneumonic = "FileNotFound",
  narrative = "FileNotFound",
  severity = "Severity::Severe"
},
{
  code = "1002",
  mneumonic = "DiskNotFound",
  narrative = "DiskNotFound",
  severity = "Severity::Severe"
},
{
  code = "1003",
  mneumonic = "FileAlreadyOpen",
  narrative = "FileAlreadyOpen",
  severity = "Severity::Severe"
},
{
  code = "2000",
  mneumonic = "InternetNotAvaliable",
  narrative = "InternetNotAvaliable",
  severity = "Severity::Severe"
},
{
  code = "2001",
  mneumonic = "InternetGatewayNotFound",
  narrative = "InternetGatewayNotFound",
  severity = "Severity::Severe"
},
{
  code = "2002",
  mneumonic = "UnableToConnectToServer",
  narrative = "UnableToConnectToServer",
  severity = "Severity::Severe"
},
{
  code = "3000",
  mneumonic = "UnableToConnectToSensor",
  narrative = "UnableToConnectToSensor",
  severity = "Severity::Severe"
},
{
  code = "3001",
  mneumonic = "SensorOutOfRange",
  narrative = "SensorOutOfRange",
  severity = "Severity::Severe"
},
{
  code = "1004",
  mneumonic = "DiskFull",
  narrative = "DiskFull",
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