#ifndef _GLOBALS_H
#define _GLOBALS_H

#include "WiFiClient.h"
#include "Config.h"

struct Configuration;
struct Device;

class Globals
{
public:
  // General objects
  static Configuration config;
  static WiFiClient wifiServer;
  // NOTE: Possibly make ActivityLog a static variable.
  
  // Arrays
  static Device devices[MAX_DEVICES];
  static int devices_index; // Index of the latest empty slot in devices

  static void setupDefaults()
  {
    devices_index = 0;
  }
};
#endif
