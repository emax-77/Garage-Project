#include <Arduino.h>
#include <WiFi.h>
#include <WebServer.h>
#include "config.h"

namespace
{
const int RELAY_PIN = 5;
const int RELAY_ACTIVE_LEVEL = HIGH;
const int RELAY_INACTIVE_LEVEL = LOW;
const unsigned long RELAY_PULSE_MS = 500;

WebServer server(80);

void handleOpen()
{
  digitalWrite(RELAY_PIN, RELAY_ACTIVE_LEVEL);
  delay(RELAY_PULSE_MS);
  digitalWrite(RELAY_PIN, RELAY_INACTIVE_LEVEL);

  server.send(200, "text/plain", "OK");
}

void handleNotFound()
{
  server.send(404, "text/plain", "Not Found");
}

void connectWiFi()
{
  WiFi.mode(WIFI_STA);

  if (USE_STATIC_IP)
  {
    IPAddress ip;
    IPAddress gateway;
    IPAddress subnet;

    ip.fromString(STATIC_IP);
    gateway.fromString(STATIC_GATEWAY);
    subnet.fromString(STATIC_SUBNET);

    WiFi.config(ip, gateway, subnet);
  }

  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);

  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
  }
}
}

void setup()
{
  pinMode(RELAY_PIN, OUTPUT);
  digitalWrite(RELAY_PIN, RELAY_INACTIVE_LEVEL);

  connectWiFi();

  server.on("/open", HTTP_POST, handleOpen);
  server.onNotFound(handleNotFound);
  server.begin();
}

void loop()
{
  server.handleClient();
}
