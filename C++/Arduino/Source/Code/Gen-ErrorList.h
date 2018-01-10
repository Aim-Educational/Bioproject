#pragma once

enum class Severity
{
    Warning = 2,
    Severe  = 3
};

struct ErrorCode
{
    string code;
    string mneumonic;
    string narrative;
    Severity severity;
};

class ErrorList
{
public:
    static ErrorCode[] errorCodes;

    static ErrorCode getErrorByMneumonic(string& mneumonic);
};