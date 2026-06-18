#include "device.h"
#include <iostream>
using namespace std;

Device::Device(string name) : name(name), isOn(false) {}

void Device::turnOn() {
    isOn = true;
    cout << name << " - Устройство включено!\n";
}

void Device::turnOff() {
    isOn = false;
    cout << name << " - Устройство выключено!\n";
}

void Device::showStatus() {
    cout << name << "<placeholder>";
}

void Device::ping() {
    cout << name << "<placeholder>";
}

Device::~Device() {}
