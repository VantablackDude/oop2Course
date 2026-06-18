#include "light.h"
#include <iostream>
using namespace std;

Light::Light(string name, int brightness, string color, bool isOn) : Device(name), brightness(brightness), color(color) {}
Light::Light():Device("Девайс0"), brightness(0), color("empty") {}
Light::Light(const Light& copy):Device(copy), brightness(copy.brightness), color(copy.color){}

void Light::ping() {
    cout << "Лампа " << name << " подключен к сети B." << endl;
}

void Light::showStatus() {
    cout << "Лампа: " << name << ", включено?: " << isOn
        << ", яркость: " << brightness << ", цвет: " << color << endl;
}


void Light::addLightUnit() {
    cout << "Добавлен новый источник света в систему" << endl;
}

void Light::changeMode(int colour) {
    color = colour;
    cout << "Режим работыт изменен: "<< color << endl;
}