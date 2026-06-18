#ifndef termostat_h
#define termostat_h

#include "device.h"

class Termostat : public Device {
private:
    int temperature;
    string mode;

public:
    Termostat(string name, int temperature, string mode, bool isOn);
    Termostat();
    Termostat(const Termostat& copy);

    void showStatus() override;
    void ping() override;
    void tempUp();
    void tempDown();
};

#endif
