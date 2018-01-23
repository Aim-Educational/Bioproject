#ifndef _CONSTANTS_H
#define _CONSTANTS_H

#include "util.h"

#define SD_CARD_PIN 1
#define CONFIG_FILE "/config.ini"
#define ACTIVITY_LOG_FILE "/activity_log.csv"

#define NETWORK_SSID "www.goats_r_us.co.uk"
#define NETWORK_PASS "FreeGoats"
#define NETWORK_KEY_INDEX 0

enum
{
    INI_BUFFER_SIZE = 128,
    HTTP_BUFFER_SIZE = 512,
    ACTIVITY_LOG_BUFFER_SIZE = 512,
    MAX_DEVICES = 8,
    MAX_BUILDSTEPS = 5,
    NTP_PACKET_SIZE = 48,
    UDP_PORT = 123
};

enum class BitFlags : int
{
    LOGS_WRITE_TO_SERIAL = 1 << 0,
    PLAY_TUNE            = 1 << 1
};
CREATE_ENUM_CLASS_OPERATOR_OVERLOADS(BitFlags)
#endif
