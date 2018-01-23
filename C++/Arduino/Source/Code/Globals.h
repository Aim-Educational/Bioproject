#ifndef _GLOBALS_H
#define _GLOBALS_H

#include "WiFiClient.h"
#include "WiFiUdp.h"
#include "Config.h"
#include "UTCTime.h"

struct Configuration;
struct Device;

class Globals
{
public:
  // General objects
  static Configuration config;
  static UTCTime localTime;
  static WiFiClient wifiServer;
  static WiFiUDP udp;
  // NOTE: Possibly make ActivityLog a static variable.
  
  // Arrays
  static Device devices[MAX_DEVICES];
  static int devices_index; // Index of the latest empty slot in devices

  static String buildSteps[MAX_BUILDSTEPS];
  static int buildSteps_index;

  static void setupDefaults()
  {
    devices_index = 0;
  }
};
#endif
