#ifndef __SENSORS_H
#define __SENSORS_H

#include <Time>
#include "util.h"

#define CASE_FAILURE -10000

enum class SensorType : int
{
  UNKNOWN,
  DHT11
};
CREATE_ENUM_CLASS_OPERATOR_OVERLOADS(SensorType)

class InputControl
{
private:
  time_t _nextPollTime;
  
public:
  int id;
  SensorType type;
  float lastValue;
  time_t lastDateTime;
  int pollFrequencySecs;
  int portNumber;
  bool hasFailed;

  float getValue()
  {
    auto value = getValueBySensorType();

    if(value != CASE_FAILURE)
    {
      this->lastValue = value;
      this->lastDateTime = now();
      this->_nextPollTime = this->lastDateTime + this->pollFrequencySecs;
      this->hasFailed = false;
    }
    else
      this->hasFailed = true;
  }

  float getValueBySensorType()
  {
    switch(this->type)
    {
      case SensorType::DHT11:
        SimpleDHT11 dht;
        byte temperature = 0;
        if(dht.read(this->portNumber, &temperature, nullptr, nullptr))
          return CASE_FAILURE;

        this->lastValue = temperature;
        return this->lastValue;

      default:
        return CASE_FAILURE; // TODO: Figure out what NaN is.
    }
  }
};

#endif
