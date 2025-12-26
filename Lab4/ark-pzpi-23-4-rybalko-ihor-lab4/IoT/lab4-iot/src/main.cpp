#include <Arduino.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>
#include <LiquidCrystal_I2C.h>

const char* ssid = "Wokwi-GUEST";
const char* password = "";

const String apiBaseUrl = "https://acrogynous-celinda-aerogenically.ngrok-free.dev/api";
const String serialNumber = "ESP32-KITCHEN-001";

#define BTN_PIN 14
#define BUZZER_PIN 25
LiquidCrystal_I2C lcd(0x27, 16, 2);

int currentOrderId = 1;
int currentStatus = 0; 
unsigned long startTime = 0;

void syncStatusWithServer(int orderId, int status) {
  if (WiFi.status() != WL_CONNECTED) return;

  HTTPClient http;
  String url = apiBaseUrl + "/" + String(orderId) + "/status?serialNumber=" + serialNumber;
  
  http.begin(url);
  
  http.addHeader("Content-Type", "application/json");
  http.addHeader("ngrok-skip-browser-warning", "true");

  JsonDocument doc;
  doc["Status"] = status;
  
  if (status == 4) {
    float duration = (millis() - startTime) / 1000.0;
    doc["PrepDuration"] = duration;
    Serial.printf(">>> Analytics: Order #%d cooked in %.2fs\n", orderId, duration);
  }

  String requestBody;
  serializeJson(doc, requestBody);

  int httpResponseCode = http.PATCH(requestBody);

  if (httpResponseCode == 200) {
    Serial.println(">>> Sync Success: HTTP 200");
    lcd.setCursor(0, 1);
    lcd.print("SYNC SUCCESS!   ");
    tone(BUZZER_PIN, 1500, 200);
  } else {
    Serial.printf(">>> Sync Error: %d\n", httpResponseCode);
    lcd.setCursor(0, 1);
    lcd.print("ERR CODE: "); lcd.print(httpResponseCode);
    tone(BUZZER_PIN, 500, 500);
  }

  http.end();
}

void setup() {
  Serial.begin(115200);
  pinMode(BTN_PIN, INPUT_PULLUP);
  pinMode(BUZZER_PIN, OUTPUT);

  lcd.init();
  lcd.backlight();
  lcd.print("Connecting WiFi");

  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  lcd.clear();
  lcd.print("KDS Node Online");
}

void loop() {
  if (digitalRead(BTN_PIN) == LOW) {
    delay(50); 
    
    currentStatus++;
    if (currentStatus > 4) {
      currentStatus = 1; 
      currentOrderId++;
    }

    if (currentStatus == 2) {
        startTime = millis();
    }

    lcd.clear();
    lcd.print("Update Order #"); lcd.print(currentOrderId);
    lcd.setCursor(0, 1);
    lcd.print("Status: "); lcd.print(currentStatus);

    syncStatusWithServer(currentOrderId, currentStatus);

    while (digitalRead(BTN_PIN) == LOW); 
    delay(200);
  }
}