#ifndef device_h
#define device_h

#include <string>
using namespace std;

class Device {
protected:
    string name;
    bool isOn;

public:
    Device(string name);

    virtual void turnOn();
    virtual void turnOff();
    virtual void showStatus() = 0;
    virtual void ping() = 0;

    ~Device();
};

#endif
