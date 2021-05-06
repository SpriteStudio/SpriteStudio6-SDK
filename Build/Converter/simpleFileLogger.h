#ifndef __SIMPLEFILELOGGER__
#define __SIMPLEFILELOGGER__


#include<iostream>
#include<fstream>

class Logger
{
private:
	bool outputLogFileFlag;

	std::vector<std::string> textline;

	std::ofstream* ofs;

public:

	enum LogLevel
	{
		ERR = 1 << 1,
		WAR = 1 << 2 ,
		INF = 1 << 3
	};

	int mLogLevel;

public:
	Logger() : outputLogFileFlag(false) , mLogLevel( LogLevel::ERR | LogLevel::WAR | LogLevel::INF ) , ofs(0){}

	virtual ~Logger()
	{
		LogFileFlush();
		if (ofs)
			ofs->close();
	}


	void outputLog(LogLevel level , std::string str )
	{
		
		switch (level)
		{
		case LogLevel::ERR:
			if (mLogLevel & LogLevel::ERR)
			{
				//console
				std::cerr << str << std::endl;
				textline.push_back(str);
//				*ofs << str << std::endl;
			}
			break;
		case LogLevel::WAR:
			if (mLogLevel & LogLevel::WAR)
			{
				//console
				std::cerr << str << std::endl;
				textline.push_back(str);
//				*ofs << str << std::endl;
			}
			break;
		case LogLevel::INF:
			//console
			if (mLogLevel & LogLevel::INF)
			{
				std::cout << str << std::endl;
				textline.push_back(str);
//				*ofs << str << std::endl;
			}
			break;
		}
	}

	void LogFileFlush()
	{
		if (ofs == 0) return; 

		for (const auto item : textline)
		{
			std::string out = item;
			*ofs << out << std::endl;
		}
	}

	void setOutputLogFile(std::wstring filepath)
	{
		if (ofs) return;
		ofs = new std::ofstream(filepath);
	}

	void setOutputLogFileMode(bool flag)
	{
		outputLogFileFlag = flag;
	}

};

#endif

