#include <WiFi.h>
#include <Wire.h>                 // Must include Wire library for I2C
#include "SparkFun_MMA8452Q.h"    // Click here to get the library: http://librarymanager/All#SparkFun_MMA8452Q

const char* ssid     = "SeriesOfTubes";
const char* password = "michaelhan4president";

const char* host = "192.168.0.147";
const int port = 8080;

MMA8452Q accel;

WiFiClient client;

void setup() {
  Serial.begin(115200);
  delay(10);
  Serial.print("Connecting to ");
  Serial.println(ssid);

  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
      delay(500);
      Serial.print(".");
  }

  Serial.println("");
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());

  Wire.begin();
  if (accel.begin() == false) {
    Serial.println("Not Connected. Please check connections and read the hookup guide.");
    while (1);
  }
  accel.setupTap(0x08, 0x08, 0x08);
}

void loop() {
  if (!client.connected()) {
    Serial.print("Connecting to ");
    Serial.println(host);
    IPAddress ip;
    ip.fromString(host);
    if (!client.connect(ip, port)) {
      Serial.println("Connection failed.");
      delay(1000);
      return;
    }
    Serial.println("Connected!");
  } 

  if (accel.available()) {
    if (accel.readTap() > 0) {
      Serial.println("Tap");
      client.write(1);
    }
  }

}
