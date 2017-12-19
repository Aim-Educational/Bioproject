#include "ActivityLog.h"

ActivityLog::ActivityLog(String fileName)
{
    this->fileName = fileName;
}

void ActivityLog::writeEntry(ActivityLogEntry entry)
{
    char buffer[ACTIVITY_LOG_BUFFER_SIZE];
    sprintf(buffer, "%s,%s,%s,%s", 
                    //dateTimeToString(entry.dateTime),  // Figure out whenever
                    entry.severity, 
                    entry.deviceID, 
                    entry.activityCode, 
                    entry.narrative.c_str());

    File logFile = SD.open(fileName, FILE_WRITE);
    if(logFile)
    {
        logFile.println(buffer);
        logFile.close();
    }
    else
    {
        Serial.println("[Error 1001] Unable to open activity log"); // eventually add fileName
    }

    if((config.flags & BitFlags.LOGS_WRITE_TO_SERIAL) > 0 || entry.severity == ActivityLogSeverity.Severe)
        Serial.println(buffer);
}

bool ActivityLog::isFileOpen()
{
    return _isFileOpen;
}
