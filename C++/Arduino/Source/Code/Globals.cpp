#include "Globals.h"

// General objects
static Configuration Globals::config;
static WiFiClient Globals::wifiServer;
static WiFiUDP Globals::udp;

// Arrays
static Device Globals::devices[MAX_DEVICES];
static int Globals::devices_index; // Index of the latest empty slot in devices
