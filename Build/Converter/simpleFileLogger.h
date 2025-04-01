#ifndef __SIMPLEFILELOGGER__
#define __SIMPLEFILELOGGER__


#include<iostream>
#include<fstream>
#include <filesystem>

#include "sscharconverter.h"
#include "utils.h"

class Logger {
private:
    bool outputLogFileFlag;

    std::vector<std::string> textline;

    std::ofstream *ofs;

public:

    enum LogLevel {
        ERR = 1 << 1,
        WAR = 1 << 2,
        INF = 1 << 3
    };

    int mLogLevel;

private:
    static std::string OSLocaleString(const std::string &str) {
        return (isWindows())? spritestudio6::SsCharConverter::sjis_to_utf8(str) : str;
    }

public:
    Logger() : outputLogFileFlag(false), mLogLevel(LogLevel::ERR | LogLevel::WAR | LogLevel::INF), ofs(nullptr) {}

    virtual ~Logger() {
        LogFileFlush();
        if (ofs)
            ofs->close();
    }


    void outputLog(LogLevel level, const std::string &str) {

        switch (level) {
            case LogLevel::ERR:
                if (mLogLevel & LogLevel::ERR) {
                    //console
                    std::cerr << str << std::endl;

                    textline.push_back(OSLocaleString(str));

                }
                break;
            case LogLevel::WAR:
                if (mLogLevel & LogLevel::WAR) {
                    //console
                    std::cerr << str << std::endl;
                    textline.push_back(OSLocaleString(str));
                }
                break;
            case LogLevel::INF:
                //console
                if (mLogLevel & LogLevel::INF) {
                    std::cout << str << std::endl;
                    textline.push_back(OSLocaleString(str));
                }
                break;
        }
    }

    void LogFileFlush() {
        if (ofs == nullptr) return;

        for (const auto &item: textline) {
            auto out = item;
            *ofs << out << std::endl;
        }
        textline.clear();

    }

    void setOutputLogFile(const std::filesystem::path &filepath) {
        if (ofs) return;
        ofs = new std::ofstream(filepath);
    }

    void setOutputLogFileMode(bool flag) {
        outputLogFileFlag = flag;
    }

};

#endif

