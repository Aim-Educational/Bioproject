enum
{
    INI_BUFFER_SIZE = 128,
    HTTP_BUFFER_SIZE = 512,
    ACTIVITY_LOG_BUFFER_SIZE = 512,
    MAX_DEVICES = 8
};

enum BitFlags : int
{
    LOGS_WRITE_TO_SERIAL = 1 << 0,
    PLAY_TUNE            = 1 << 1
};
