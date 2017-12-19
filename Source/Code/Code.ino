#include <WiFi.h>
#include <WiFiClient.h>

#include "SD.h"
#include "SPI.h"
#include "IniFile.h"
#include "version.h"
#include "ActivityLog.h"
#include "Constants.h"

#define SD_CARD_PIN 1
#define CONFIG_FILE "/config.ini"
#define ACTIVITY_LOG_FILE "/activity_log.csv"

#define NETWORK_SSID "www.goats_r_us.co.uk"
#define NETWORK_PASS "FreeGoats"
#define NETWORK_KEY_INDEX 0



enum HTTPRequestType
{
    GET,
    POST
};



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

struct Event
{
    // TODO: Finish
};

// General objects
Configuration config;
WiFiClient wifiServer;

// Arrays
Device devices[MAX_DEVICES];
int devices_index = 0; // Index of the latest empty slot in devices

String readStringFromIni(String section, String key, IniFile ini, char* buffer, int INI_BUFFER_SIZE)
{
    if(!ini.getValue(section.c_str(), key.c_str(), buffer, INI_BUFFER_SIZE))
    {
        Serial.println("Unable to find key"); // TODO: Bother looking up how to format a string
        // Error handling here
    }

    return String(buffer);
}

NetworkInfo readNetworkInfoFromIni(String section, IniFile ini, char* buffer)
{
    NetworkInfo info;
    info.IPAddress     = readStringFromIni(section, "IPAddress", ini, buffer, INI_BUFFER_SIZE);
    info.subnet        = readStringFromIni(section, "Subnet", ini, buffer, INI_BUFFER_SIZE);
    info.gateway       = readStringFromIni(section, "Gateway", ini, buffer, INI_BUFFER_SIZE);
    info.serverAddress = readStringFromIni(section, "ServerAddress", ini, buffer, INI_BUFFER_SIZE);
    info.serverPort    = readStringFromIni(section, "ServerPort", ini, buffer, INI_BUFFER_SIZE);

    return info;
}

BitFlags readFlagFromIni(String section, String key, IniFile ini, char* buffer, BitFlags flagValue)
{
    if(readStringFromIni(section, key, ini, buffer, INI_BUFFER_SIZE) == "1")
      return flagValue;
    else
      return 0;
}

void readConfiguration()
{  
    IniFile ini(CONFIG_FILE);
    if(!ini.open())
    {
        // Replace with Event
        Serial.println("Unable to open Config file");
        // Same question as above.
    }

    char buffer[INI_BUFFER_SIZE];
    if(!ini.validate(buffer, INI_BUFFER_SIZE))
    {
        Serial.println("Buffer size is too small");
        // Still need to handle errors
    }

    // General
    config.version = readStringFromIni("Version", "Version", ini, buffer, INI_BUFFER_SIZE);
    config.computerID = readStringFromIni("Configuration", "ComputerID", ini, buffer, INI_BUFFER_SIZE).toInt();
    //config.dateTimeLastPolled = readStringFromIni("Configuration", "DateTimeLastPolled", ini, buffer, INI_BUFFER_SIZE);
    // Commented out since I have no idea on how we store dates.
    config.heartbeatFrequencyMS = readStringFromIni("Configuration", "HeartbeatFrequencyMS", ini, buffer, INI_BUFFER_SIZE).toInt();

    // Server config
    config.primaryNetwork   = readNetworkInfoFromIni("PrimaryServer", ini, buffer);
    config.secondaryNetwork = readNetworkInfoFromIni("SecondaryServer", ini, buffer);

    // Flags
    config.flags |= readFlagFromIni("Configuration", "SerialLog", ini, buffer, BitFlags.LOGS_WRITE_TO_SERIAL);
    config.flags |= readFlagFromIni("Configuration", "PlayTune", ini, buffer, BitFlags.PLAY_TUNE);

    // Devices
    int numberOfDevices = readStringFromIni("Devices", "NumberOfDevices", ini, buffer, INI_BUFFER_SIZE).toInt();
    for(int i = 0; i < numberOfDevices; i++)
    {
        sprintf(buffer, "Device%i", i);
        String sectionName = String(buffer);

        Device device;
        device.deviceType = readStringFromIni(sectionName, "DeviceType", ini, buffer, INI_BUFFER_SIZE).toInt();
        device.inputNumber = readStringFromIni(sectionName, "InputNumber", ini, buffer, INI_BUFFER_SIZE).toInt();
        device.outputNumber = readStringFromIni(sectionName, "OutputNumber", ini, buffer, INI_BUFFER_SIZE).toInt();

        if(devices_index >= MAX_DEVICES)
        {
            Serial.println("Too many devices have been registered.");
            // Error code here, when it's eventually figured out
        }

        devices[devices_index++] = device;
    }
}

void setupWifi()
{
    Serial.println("Setting up WiFi");
  
    if(WiFi.status() == WL_NO_SHIELD)
    {
        Serial.println("ERROR: No WiFi shield is connected");
        // Error handling here
    }

    // DEBATE: Should we only try to connect X amount of times before failling?
    int status = WL_IDLE_STATUS;
    while(status != WL_CONNECTED)
    {
        Serial.print("Attempting to connect to: ");
        Serial.println(NETWORK_SSID);

        status = WiFi.begin(NETWORK_SSID, NETWORK_PASS);

        if(status != WL_CONNECTED)  
          delay(5000); // Try every 5 seconds
    }    
}

String wifiSendRequest(WiFiClient client, String url, String path, HTTPRequestType requestType)
{
    if(!client.connect(url, 80))
    {
        Serial.print("ERROR: Unable to connect to URL (Port 80): ");
        Serial.println(url);
        
    }
  
    char buffer[HTTP_BUFFER_SIZE];
    String requestString;
    switch(requestType)
    {
        case HTTPRequestType.GET:
          requestString = "GET";
          break;

        case HTTPRequestType.POST:
          requestString = "POST";
          break;

        default:
          Serial.println("ERROR: Unknown request type"); // TODO: Format the string to include the reqeustType's value.
          // TODO: Error handling here
          return null;
    }

    // I'm not too sure how to form a HTTP request, so correct me if I make a mistake.
    int length = sprintf(buffer, "%s %s HTTP/1.1\n", requestString, path);

    // TODO: Make some of this code into a function(s) (Going to be copy-pasting this for the mean time)
    if(length >= HTTP_BUFFER_SIZE)
    {
        Serial.println("ERROR: Not enough buffer size to complete request");
        // Error handling here.
        return null;
    }
}

void setup()
{
    Serial.begin(9600);

    // Disable the SD card, as apparently it can cause things to fail.
    pinMode(SD_CARD_PIN, OUTPUT);
    digitalWrite(SD_CARD_PIN, HIGH);

    SPI.begin();    
    if(!SD.begin(SD_CARD_PIN))
    {
        // REplace with Event
        Serial.println("SD.begin failed");
        // What should we do if it fails, the example code just forever prints out an error message.
    }

    readConfiguration();
    setupWifi();
}

void loop()
{
}
