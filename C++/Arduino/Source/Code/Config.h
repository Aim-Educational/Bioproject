#ifndef _CONFIG_H
#define _CONFIG_H

#include "WString.h"
#include "Constants.h"
#include "IniFile.h"
#include "Globals.h"
#include "UTCTime.h"

struct NetworkInfo
{
    String IPAddress;
    String subnet;
    String gateway;
    String serverAddress;
    String serverPort;
};

struct TimeServer
{
  String IPAddress;
  String offset;
};

struct Configuration
{
    String version;
    int computerID;
    //DateTime? dateTimeLastPolled;
    NetworkInfo primaryNetwork;
    NetworkInfo secondaryNetwork;
    TimeServer timeServer;
    int heartbeatFrequencyMS;
    String localIPAddress;
    BitFlags flags;
};

struct Device
{
    int deviceType;
    int inputNumber;
    int outputNumber;
    int pollFrequencySeconds;
    UTCTime timeLastPolled;
};

extern void readConfiguration();
#endif
