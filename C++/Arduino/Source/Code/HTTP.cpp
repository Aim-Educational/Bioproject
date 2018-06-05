#include "HTTP.h"

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

//    Globals::udp.begin(UDP_PORT);
}

String wifiSendRequest(WiFiClient client, String& url, String& path, HTTPRequestType requestType, String data)
{
    if(!client.connect(url.c_str(), 80))
    {
        Serial.print("ERROR: Unable to connect to URL (Port 80): ");
        Serial.println(url);
        return "";
    }
    
    String requestString;
    switch(requestType)
    {
        case HTTPRequestType::GET:
          requestString = "GET";
          break;

        case HTTPRequestType::POST:
          requestString = "POST";
          break;

        default:
          Serial.println("ERROR: Unknown request type"); // TODO: Format the string to include the reqeustType's value.
          // TODO: Error handling here
          return "";
    }

    // GET/SET /example HTTP/1.1
    client.print(requestString);
    client.print(" ");
    client.print(path);
    client.println(" HTTP/1.1");

    // Host: url.com
    client.print("Host: ");
    client.println(url);

    // Content-Type
    client.println("Content-Type: text/plain");

    // Content-Length: 0
    client.print("Content-Length: ");
    client.println(String(data.length()));

    // The content
    client.println("");
    client.println(data);

    // End with a new line
    client.println("");

    // Read in the response.
    char buffer[256]; // TODO: Add a #define in Globals.h for this size
    int index = 0;
    while(client.connected() && client.available())
    {
        if(index >= 256 - 1) // Need to leave one character for the null terminator
        {
          // TODO: Make a BufferTooSmall (or whatever we'll name it) error.
          return "";
        }

        buffer[index++] = client.read();
    }
    buffer[index] = '\0';

    client.stop();
    return String(buffer);
}

// send an NTP request to the time server at the given address
unsigned long sendNTPpacket(IPAddress& address) {
  // set all bytes in the buffer to 0
  char packetBuffer[NTP_PACKET_SIZE];
  memset(packetBuffer, 0, NTP_PACKET_SIZE);
  
  // Initialize values needed to form NTP request
  // (see URL above for details on the packets)
  packetBuffer[0] = 0b11100011;   // LI, Version, Mode
  packetBuffer[1] = 0;     // Stratum, or type of clock
  packetBuffer[2] = 6;     // Polling Interval
  packetBuffer[3] = 0xEC;  // Peer Clock Precision
  // 8 bytes of zero for Root Delay & Root Dispersion
  packetBuffer[12]  = 49;
  packetBuffer[13]  = 0x4E;
  packetBuffer[14]  = 49;
  packetBuffer[15]  = 52;

  //Serial.println("3");

  // all NTP fields have been given values, now
  // you can send a packet requesting a timestamp:
 // Globals::udp.beginPacket(address, 123); //NTP requests are to port 123
 // Globals::udp.write(packetBuffer, NTP_PACKET_SIZE);
 // Globals::udp.endPacket();
}

UTCTime parseNTPPacket()
{
  //if (Globals::udp.available())
  {
    char packetBuffer[NTP_PACKET_SIZE];
    
    // We've received a packet, read the data from it
    //Globals::udp.read(packetBuffer, NTP_PACKET_SIZE); // read the packet into the buffer

    //the timestamp starts at byte 40 of the received packet and is four bytes,
    // or two words, long. First, esxtract the two words:

    unsigned long highWord = word(packetBuffer[40], packetBuffer[41]);
    unsigned long lowWord = word(packetBuffer[42], packetBuffer[43]);
    
    // combine the four bytes (two words) into a long integer
    // this is NTP time (seconds since Jan 1 1900):
    unsigned long secsSince1900 = highWord << 16 | lowWord;
    
    // Unix time starts on Jan 1 1970. In seconds, that's 2208988800:
    const unsigned long seventyYears = 2208988800UL;
    
    // subtract seventy years:
    unsigned long epoch = secsSince1900 - seventyYears;

    UTCTime time;
    time.hours = (epoch % 86400L) / 3600;
    time.minutes = (epoch % 3600) / 60;
    time.seconds = (epoch % 60);

    return time;
  }

  return UTCTime();
}
