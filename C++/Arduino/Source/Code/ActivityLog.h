#include <WString.h>
#include <stdlib.h>
#include <HardwareSerial.h>
#include "Constants.h"

enum ActivityLogSeverity : int
{
    Warning = 1,
    Severe = 10
};

struct ActivityLogEntry
{
    //DateTime? dateTime;
    ActivityLogSeverity severity;
    int deviceID;
    int activityCode;
    String narrative;
};

class ActivityLog
{
public:
    ActivityLog(String fileName);
    void writeEntry(ActivityLogEntry entry);
    bool isFileOpen();

private:
    String fileName;
    bool _isFileOpen;
};
