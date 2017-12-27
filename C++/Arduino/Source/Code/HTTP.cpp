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

String wifiSendRequest(WiFiClient client, String& url, String& path, HTTPRequestType requestType)
{
    if(!client.connect(url.c_str(), 80))
    {
        Serial.print("ERROR: Unable to connect to URL (Port 80): ");
        Serial.println(url);
        
    }
  
    char buffer[HTTP_BUFFER_SIZE];
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

    // I'm not too sure how to form a HTTP request, so correct me if I make a mistake.
    int length = sprintf(buffer, "%s %s HTTP/1.1\n", requestString.c_str(), path.c_str());

    // TODO: Make some of this code into a function(s) (Going to be copy-pasting this for the mean time)
    if(length >= HTTP_BUFFER_SIZE)
    {
        Serial.println("ERROR: Not enough buffer size to complete request");
        // Error handling here.
        return "";
    }
}
