#include "<FileName>.h"

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
<ErrorCodes>
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
	Serial.print();
}