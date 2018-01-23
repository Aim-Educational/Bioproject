#ifndef _HTTP_H
#define _HTTP_H

#include <WiFiClient.h>
#include <WiFi.h>

#include "WString.h"
#include "Constants.h"
#include "ActivityLog.h"

enum class HTTPRequestType
{
    GET,
    POST
};

extern void setupWifi();

extern String wifiSendRequest(WiFiClient client, String& url, String& path, HTTPRequestType requestType, String data = "");

#endif
