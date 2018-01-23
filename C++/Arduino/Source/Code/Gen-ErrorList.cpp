#include "Gen-ErrorList.h"

static String ErrorList::_parameters[MAX_PARAMETERS];
static unsigned char ErrorList::_parametersTail;

ErrorCode::ErrorCode(String code, String mneumonic, String narrative, Severity severity)
{
	this->code = code;
	this->mneumonic = mneumonic;
	this->narrative = narrative;
	this->severity = severity;
}

static ErrorCode ErrorList::errorCodes[ERROR_COUNT];

void temp()
{
ErrorList::errorCodes[0] = 
ErrorCode(
  F("1"),
  F("Unknown"),
  F("An unknown error has occured."),
  Severity::Warning
);
ErrorList::errorCodes[1] = 
ErrorCode(
  F("1001"),
  F("FileNotFound"),
  F("Unable to find file/directory: %1"),
  Severity::Severe
);
ErrorList::errorCodes[2] = 
ErrorCode(
  F("1002"),
  F("DiskNotFound"),
  F("Unable to find disk: %1"),
  Severity::Severe
);
ErrorList::errorCodes[3] = 
ErrorCode(
  F("1003"),
  F("FileAlreadyOpen"),
  F("Unable to open file because it is already open: %1"),
  Severity::Severe
);
ErrorList::errorCodes[4] = 
ErrorCode(
  F("2000"),
  F("InternetNotAvaliable"),
  F("No connection to the internet is avaliable."),
  Severity::Severe
);
ErrorList::errorCodes[5] = 
ErrorCode(
  F("2001"),
  F("InternetGatewayNotFound"),
  F("No connection to the gateway is avaliable."),
  Severity::Severe
);
ErrorList::errorCodes[6] = 
ErrorCode(
  F("2002"),
  F("UnableToConnectToServer"),
  F("Unable to connect to server at: %1"),
  Severity::Severe
);
ErrorList::errorCodes[7] = 
ErrorCode(
  F("3000"),
  F("UnableToConnectToSensor"),
  F("Unable to connec to a sensor: %1"),
  Severity::Severe
);
ErrorList::errorCodes[8] = 
ErrorCode(
  F("3001"),
  F("SensorOutOfRange"),
  F("A sensor's value is outside of a valid range: %1"),
  Severity::Severe
);
ErrorList::errorCodes[9] = 
ErrorCode(
  F("1004"),
  F("DiskFull"),
  F("The targeted disk is full: %1"),
  Severity::Severe
);
ErrorList::errorCodes[10] = 
ErrorCode(
  F("3002"),
  F("UnableToOpenSDCard"),
  F("Unable to open the SD card."),
  Severity::Severe
);
}

static bool ErrorList::pushParameter(String param)
{
	if(ErrorList::_parametersTail == MAX_PARAMETERS)
	{
		// Not enough room.
		return false;
	}

	ErrorList::_parameters[ErrorList::_parametersTail++] = param;
	return true;
}

static void ErrorList::clearParameters()
{
	for(int i = 0; i < MAX_PARAMETERS; i++)
		ErrorList::_parameters[i] = "";

	ErrorList::_parametersTail = 0;
}

static ErrorCode ErrorList::getErrorByMneumonic(String mneumonic)
{
    for(int i = 0; i < ERROR_COUNT; i++)
    {
		auto& code = ErrorList::errorCodes[i];
        if(code.mneumonic == mneumonic)
            return code;
    }

    ErrorCode error(String("9999"), String("ERROR MNEUMONIC NOT FOUND: ") + mneumonic);

    return error;
}

static ErrorCode ErrorList::getErrorByMneumonic(String mneumonic, String additionalNarrative)
{
	auto error = ErrorList::getErrorByMneumonic(mneumonic);
	error.additionalNarrative = additionalNarrative;

	return error;
}

/*
static ErrorCode getErrorByMneumonic(String mneumonic, string parameterArray[])
{
	auto error = ErrorList::getErrorByMneumonic(mneumonic);
	error.parameterArray = parameterArray;

	return error;
}

static ErrorCode getErrorByMneumonic(String mneumonic, String additionalNarrative, string parameterArray[])
{
	auto error = ErrorList::getErrorByMneumonic(mneumonic);
	error.additionalNarrative = additionalNarrative;
	error.parameterArray = parameterArray;

	return error;
}*/

void ErrorCode::writeToSerial()
{
	//Serial.print();
}
