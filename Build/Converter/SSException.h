#ifndef SSSDK_SSEXCEPTION_H
#define SSSDK_SSEXCEPTION_H

#include <stdexcept>

class SSException : public std::runtime_error {
public:
    SSException(const std::string &msg, int code) : std::runtime_error(msg), error_code(code) {}

    int getErrorCode() const { return error_code; }

private:
    int error_code;
};

#endif //SSSDK_SSEXCEPTION_H
