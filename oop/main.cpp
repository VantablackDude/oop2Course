#include <iostream>
#include <vector> 

#include "light.h"
#include "termostat.h"

using namespace std;

int main() {
    setlocale(LC_ALL, "Russian");

    Light* device1A = new Light("Гостиная", 80, "теплый", true);
    Light* device1B = new Light();
    Light* device1C = new Light(*device1A);
    Termostat* device2A = new Termostat("Спальня", 22, "авто", false);
    Termostat* device2B = new Termostat();
    Termostat* device2C = new Termostat(*device2A);


    vector<Device*> Devices;
    Devices.push_back(device1A);
    Devices.push_back(device1B);
    Devices.push_back(device1C);
    Devices.push_back(device2A);
    Devices.push_back(device2B);
    Devices.push_back(device2C);

    Devices[0]->showStatus();
    Devices[0]->ping();
    Devices[0]->turnOff();
    Devices[0]->turnOn();
    dynamic_cast<Light*>(Devices[0])->addLightUnit();
    dynamic_cast<Light*>(Devices[3])->changeMode(1);

    for (Device* device : Devices) {
        //device->ping();
        //device->showStatus();
         
        //Light* funcA = static_cast<Light*>(device);
        //funcA->changeMode();
        //funcA->addLightUnit();
        //Termostat* funcB = static_cast<Termostat*>(device);
        //funcB->tempUp();
        //funcB->tempDown();

        //if (Light* funcA = dynamic_cast<Light*>(device)) {
        //    funcA->changeMode();
        //    funcA->addLightUnit();
        //}
        //if (Termostat* funcB = static_cast<Termostat*>(device)) {
        //    funcB->tempUp();
        //    funcB->tempDown();
        //}
    }

    for (Device* device : Devices) {
        delete device;
    }
    return 0;
}
