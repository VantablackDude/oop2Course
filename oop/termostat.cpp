#include "termostat.h"
#include <iostream>
using namespace std;

Termostat::Termostat(string name, int temperature, string mode, bool isOn):Device(name), temperature(temperature), mode(mode) {}
Termostat::Termostat() :Device("Девайс0"), temperature(0), mode("default") {}
Termostat::Termostat(const Termostat& copy) :Device(copy), temperature(copy.temperature), mode(copy.mode) {}

void Termostat::ping() {
    cout << "Термостат " << name << " подключен к сети A." << endl;
}
void Termostat::showStatus() {
    cout << "Термостат: " << name << ", включено?: " << isOn
        << ", температура: " << temperature << ", режим: " << mode << endl;
}


void Termostat::tempUp() {
    cout << "Температура повышена" << endl;
    temperature = 20;
}

void Termostat::tempDown() {
    cout << "Температура понижена" << endl;
    temperature = 10;
}
