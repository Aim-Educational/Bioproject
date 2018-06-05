#include <DS1307RTC.h>
#include <TimeLib.h>
//#include <WiFi.h>
//#include <WiFiClient.h>

#include "SD.h"
#include "SPI.h"
#include "IniFile.h"
#include "version.h"
#include "ActivityLog.h"
#include "Constants.h"
#include "Globals.h"
//#include "HTTP.h"
//#include "Gen-ErrorList.h"

struct Event
{
    // TODO: Finish
};

void setup()
{
  Globals::setupDefaults();
  Serial.begin(9600);

  setSyncProvider(RTC.get);

  // Disable the SD card, as apparently it can cause things to fail.
  pinMode(SD_CARD_PIN, OUTPUT);
  digitalWrite(SD_CARD_PIN, HIGH);

  SPI.begin();    
  if(!SD.begin(SD_CARD_PIN))
  {
//      auto error = ErrorList::getErrorByMneumonic("UnableToOpenSDCard");
      // What do we do with this object now?
  }

  readConfiguration();
  //setupWifi();

  //IPAddress timeServer;
  //timeServer.fromString(Globals::config.timeServer.IPAddress);
  //sendNTPPacket(timeServer);

  //while(!Globals::udp.parsePacket()); // Should I do this?
  //Globals::localTime = parseNTPPacket();
}

void loop()
{
}
