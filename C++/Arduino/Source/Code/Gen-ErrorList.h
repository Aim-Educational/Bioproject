// File: Gen-ErrorList.h
// Platforms: Arduino
// Description: Holds the error definitions, and provides an interface to access them.

#pragma once

#include <avr/pgmspace.h>
#include "WString.h"

// Please see reference below
#define MAX_PARAMETERS 4
#define ERROR_COUNT 11

#if MAX_PARAMETERS < 0
	#error MAX_PARAMETERS must be positive
#endif

enum class Severity
{
    Warning = 2,
    Severe  = 3
};

struct ErrorCode
{
    String code;
    String mneumonic;
    String narrative;
	String additionalNarrative;
    Severity severity;

	ErrorCode(String code = String("-1"),
			  String mneumonic = String("UNDEFINED"), 
			  String narrative = String("NO NARRATIVE GIVEN"), 
			  Severity severity = Severity::Warning);

	void writeToSerial();
};

class ErrorList
{
private:
	static String _parameters[MAX_PARAMETERS];
	static unsigned char _parametersTail;

public:
    static ErrorCode errorCodes[];

	static bool pushParameter(String param);
	static void clearParameters();

    static ErrorCode getErrorByMneumonic(String mneumonic);
	static ErrorCode getErrorByMneumonic(String mneumonic, String additionalNarrative);
	//static ErrorCode getErrorByMneumonic(String& mneumonic, String parameterArray[]);
	//static ErrorCode getErrorByMneumonic(String& mneumonic, String& additionalNarrative, String parameterArray[]);
};

// Comments:
// We looked at various solutions for how we could store the array of parameters for an error, but due
// to the memory constraints of the Arduino, they're not suitable for use.
//
// For example, we could use malloc/realloc/free, but over time that will fragment the heap and could eventually cause
// the allocation to fail.
//
// There is also an issue of trying to keep track of the size of a dynamic array, which Arduino's C++ doesn't directly support
// an easy solution to do this.
//
// So to get around this, we're using a static array with a fixed size to store the parameters, which solves the issue
// of having to deal with memory allocations, but it will lead to more user-unfriendly/ugly code, and a limitiation on how
// many parameters can be used for an error.
//
// Another interesting issue is that Ardunio's String type uses dynamic allocation, so we may have to go over our usage
// of Strings to make sure we're not using large chunks of memory just by handling them.

// Versions:
// 1	16/01/2018	B.C		Added facilty for alternative narrative and parameter array in getErrorByMneumonic
// EOF