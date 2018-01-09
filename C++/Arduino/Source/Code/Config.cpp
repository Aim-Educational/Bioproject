#include "Config.h"

String readStringFromIni(String section, String key, IniFile ini, char* buffer)
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
    info.IPAddress     = readStringFromIni(section, "IPAddress", ini, buffer);
    info.subnet        = readStringFromIni(section, "Subnet", ini, buffer);
    info.gateway       = readStringFromIni(section, "Gateway", ini, buffer);
    info.serverAddress = readStringFromIni(section, "ServerAddress", ini, buffer);
    info.serverPort    = readStringFromIni(section, "ServerPort", ini, buffer);

    return info;
}

BitFlags readFlagFromIni(String section, String key, IniFile ini, char* buffer, BitFlags flagValue)
{
    if(readStringFromIni(section, key, ini, buffer) == "1")
      return flagValue;
    else
      return (BitFlags)0;
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
    Globals::config.version = readStringFromIni("Version", "Version", ini, buffer);
    Globals::config.computerID = readStringFromIni("Configuration", "ComputerID", ini, buffer).toInt();
    //config.dateTimeLastPolled = readStringFromIni("Configuration", "DateTimeLastPolled", ini, buffer, INI_BUFFER_SIZE);
    // Commented out since I have no idea on how we store dates.
    Globals::config.heartbeatFrequencyMS = readStringFromIni("Configuration", "HeartbeatFrequencyMS", ini, buffer).toInt();

    // Server config
    Globals::config.primaryNetwork   = readNetworkInfoFromIni("PrimaryServer", ini, buffer);
    Globals::config.secondaryNetwork = readNetworkInfoFromIni("SecondaryServer", ini, buffer);

    // Flags
    Globals::config.flags |= readFlagFromIni("Configuration", "SerialLog", ini, buffer, BitFlags::LOGS_WRITE_TO_SERIAL);
    Globals::config.flags |= readFlagFromIni("Configuration", "PlayTune", ini, buffer, BitFlags::PLAY_TUNE);

    // Devices
    int numberOfDevices = readStringFromIni("Devices", "NumberOfDevices", ini, buffer).toInt();
    for(int i = 0; i < numberOfDevices; i++)
    {
        sprintf(buffer, "Device%i", i);
        String sectionName = String(buffer);

        Device device;
        device.deviceType = readStringFromIni(sectionName, "DeviceType", ini, buffer).toInt();
        device.inputNumber = readStringFromIni(sectionName, "InputNumber", ini, buffer).toInt();
        device.outputNumber = readStringFromIni(sectionName, "OutputNumber", ini, buffer).toInt();

        if(Globals::devices_index >= MAX_DEVICES)
        {
            Serial.println("Too many devices have been registered.");
            // Error code here, when it's eventually figured out
        }

        Globals::devices[Globals::devices_index++] = device;
    }
}
