#ifndef _CONFIG_H
#define _CONFIG_H

#include "WString.h"
#include "Constants.h"
#include "IniFile.h"
#include "Globals.h"

struct NetworkInfo
{
    String IPAddress;
    String subnet;
    String gateway;
    String serverAddress;
    String serverPort;
};

struct Configuration
{
    String version;
    int computerID;
    //DateTime? dateTimeLastPolled;
    NetworkInfo primaryNetwork;
    NetworkInfo secondaryNetwork;
    int heartbeatFrequencyMS;
    BitFlags flags;
};

struct Device
{
    int deviceType;
    int inputNumber;
    int outputNumber;
};

extern void readConfiguration();
#endif
