#ifndef light_h
#define light_h

#include "device.h"

class Light : public Device {
private:
    int brightness;
    string color;

public:
    Light(string name, int brightness, string color, bool isOn);
    Light();
    Light(const Light& copy);

    void ping() override;
    void showStatus() override;
    void changeMode(int color);
    void addLightUnit();
};

#endif
