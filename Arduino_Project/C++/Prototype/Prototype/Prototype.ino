#include <SimpleDHT.h>

// sr = Shift register
const int srLatchPin = 3;
const int srClockPin = 2;
const int srDataPin  = 4;
byte srCurrentData   = 0; // Updated to reflect the current data of the shift registers. Do not change as there is no point.

// Constants
const unsigned int PEN_COUNT = 2;
const unsigned int DHT_SAMPLE_RATE_MS = 2000;
const unsigned int NO_DHT_SENSOR = -999;
const unsigned int DHT_FAILED_READING = -1000;

// Other
SimpleDHT11 dht;
int tempOffset;  // For debugging

class Pen
{
public:
  int redLEDIndex;
  int greenLEDIndex;
  int blueLEDIndex;
  int temperaturePin;

  Pen(int redLED, int greenLED, int blueLED, int temp) 
  : redLEDIndex(redLED),
    greenLEDIndex(greenLED),
    blueLEDIndex(blueLED),
    temperaturePin(temp)
  {}

  byte getRedLEDMask()
  {
    return 1 << redLEDIndex;
  }

  byte getGreenLEDMask()
  {
    return 1 << greenLEDIndex;
  }

  byte getBlueLEDMask()
  {
    return 1 << blueLEDIndex;
  }

  byte getLEDMask()
  {
    return (this->getRedLEDMask() | this->getGreenLEDMask() | this->getBlueLEDMask());
  }

  int getTemperature()
  {
    if(this->temperaturePin < 0)
      return NO_DHT_SENSOR;

    byte temperature = 0;
    if(dht.read(this->temperaturePin, &temperature, nullptr, nullptr))
      return DHT_FAILED_READING;

    return temperature;
  }
};

/// All pens
Pen pens[PEN_COUNT] = {Pen(0, 1, 2, 8), Pen(3, 4, 5, -1)};


/// Updates the Shift Register's data.
///
/// Params:
///   data = The new data for the Shift Register.
void updateSR(byte data)
{
  digitalWrite(srLatchPin, LOW);
  shiftOut(srDataPin, srClockPin, MSBFIRST, data);
  digitalWrite(srLatchPin, HIGH);

  srCurrentData = data;
}

void setup() 
{
  Serial.begin(9600);
  
  // Setup pins
  pinMode(srLatchPin, OUTPUT);
  pinMode(srClockPin, OUTPUT);
  pinMode(srDataPin,  OUTPUT);

  // Reset shift register
  updateSR(0);
  tempOffset = 0;
}

void loop() 
{
  auto value = Serial.read();
  if(value == '-')
    tempOffset -= 1;
  else if(value == '+')
    tempOffset += 1;

  Serial.print("Temperature Offset: ");
  Serial.println(tempOffset);
  
  int i = 0;
  for(i = 0; i < PEN_COUNT; i++)
  {
    auto pen = pens[i];
    auto temp = pen.getTemperature();

    Serial.print("Pen #");
    Serial.print(i);
    Serial.print(": ");

    if(temp == NO_DHT_SENSOR)
    {
      Serial.println("No DHT sensor has been specified");
      continue;
    }
    else if(temp == DHT_FAILED_READING)
    {
      Serial.println("Reading failed.");
      continue;
    }

    temp += tempOffset;
    Serial.print(temp);
    Serial.println(" C");

    // Values are random, just for testing
    const int lower = 14;
    const int upper = 16;
    if(temp < lower)
      updateSR(pen.getBlueLEDMask());
    else if(temp >= lower && temp <= upper)
      updateSR(pen.getGreenLEDMask());
    else // > upper
      updateSR(pen.getRedLEDMask());
  }
  delay(DHT_SAMPLE_RATE_MS);
}
