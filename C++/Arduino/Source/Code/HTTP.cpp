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
