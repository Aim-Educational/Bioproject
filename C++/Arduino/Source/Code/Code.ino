#include <WiFi.h>
#include <WiFiClient.h>

#include "SD.h"
#include "SPI.h"
#include "IniFile.h"
#include "version.h"
#include "ActivityLog.h"
#include "Constants.h"
#include "Globals.h"
#include "HTTP.h"

struct Event
{
    // TODO: Finish
};

void setup()
{
  Globals::setupDefaults();
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
