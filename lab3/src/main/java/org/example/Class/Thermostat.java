package org.example.Class;

import jakarta.persistence.*;
import lombok.*;
import java.io.Serializable;
import java.util.Map;
import java.util.HashMap;

@Entity
@Table(name = "thermostat")
@PrimaryKeyJoinColumn(name = "device_id")
@Data
@EqualsAndHashCode(callSuper = true)
@NoArgsConstructor
@ToString(callSuper = true)
public class Thermostat extends Device implements Serializable {

    @Override
    public String toString() {
        return "Thermostat{" +
                "id=" + getId() +
                ", name='" + getName() + '\'' +
                ", manufacturer='" + getManufacturer() + '\'' +
                ", isOn=" + isOn() +
                ", currentTemperature=" + currentTemperature +
                ", targetTemperature=" + targetTemperature +
                ", mode='" + mode + '\'' +
                ", humidity=" + humidity +
                '}';
    }

    @Column(name = "current_temperature")
    private double currentTemperature;

    @Column(name = "target_temperature")
    private double targetTemperature;

    @Column(name = "mode", length = 20)
    private String mode;

    @Column(name = "humidity")
    private double humidity;

    public Thermostat(String name, SmartHome smartHome) {
        super(name, smartHome);
        this.currentTemperature = 20.0;
        this.targetTemperature = 22.0;
        this.mode = "auto";
        this.humidity = 50.0;
        this.setManufacturer("Nest");
    }

    public Thermostat(String name, String manufacturer, boolean isOn, double currentTemperature,
                      double targetTemperature, String mode, double humidity, SmartHome smartHome) {
        super(name, manufacturer, isOn, smartHome);
        this.currentTemperature = currentTemperature;
        this.targetTemperature = targetTemperature;
        this.mode = mode;
        this.humidity = humidity;
    }

    public void setTemperature(double temp) {
        targetTemperature = temp;

        if (temp < 15) {
            System.out.println("Температура слишком низкая! Включаем обогрев до 20°C");
            targetTemperature = 20;
            mode = "heat";
        } else if (temp > 30) {
            System.out.println("Температура слишком высокая! Включаем охлаждение до 25°C");
            targetTemperature = 25;
            mode = "cool";
        } else {
            System.out.println("Целевая температура установлена: " + targetTemperature + "°C");
        }
    }

    public String getClimateInfo() {
        String warning;

        if (humidity > 70) {
            warning = "ВНИМАНИЕ: Влажность превышает норму! Рекомендуется включить осушитель.";
        } else if (humidity < 20) {
            warning = "ВНИМАНИЕ: Воздух слишком сухой! Рекомендуется увлажнитель.";
        } else if ("heat".equals(mode) && currentTemperature < targetTemperature - 2) {
            warning = "Обогрев активен: температура растёт.";
        } else if ("cool".equals(mode) && currentTemperature > targetTemperature + 2) {
            warning = "Охлаждение активно: температура снижается.";
        } else {
            warning = "Микроклимат в норме.";
        }

        return "Температура: " + String.format("%.1f", currentTemperature) + "°C, Цель: " +
                targetTemperature + "°C, Режим: " + mode + ", Влажность: " +
                String.format("%.0f", humidity) + "%. " + warning;
    }

    @Override
    public void turnOn() {
        super.turnOn();
        currentTemperature = targetTemperature - 1;
        String modeText = "heat".equals(mode) ? "обогрев" : "cool".equals(mode) ? "охлаждение" : "авто";
        System.out.println("Термостат включен. Начинаем " + modeText + " до " + targetTemperature + "°C");
    }

    @Override
    public Map<String, Object> displayInfo() {
        Map<String, Object> result = super.displayInfo();
        result.put("type", "Thermostat");
        result.put("currentTemperature", currentTemperature);
        result.put("targetTemperature", targetTemperature);
        result.put("mode", mode);
        result.put("humidity", humidity);
        return result;
    }

    @Override
    public double calculatePowerConsumption() {
        if (!isOn()) return 0.5;
        if ("heat".equals(mode)) return 1500;
        if ("cool".equals(mode)) return 1200;
        return 50;
    }

    @Override
    public String getDeviceType() {
        return "Thermostat";
    }

    @Override
    public String getShortDescription() {
        return "Thermostat '" + getName() + "': " + String.format("%.1f", getCurrentTemperature()) +
                "°C → " + getTargetTemperature() + "°C";
    }

    @Override
    public Map<String, Object> getDetailedInfo() {
        Map<String, Object> info = new HashMap<>();
        info.put("deviceType", getDeviceType());
        info.put("name", getName());
        info.put("manufacturer", getManufacturer());
        info.put("isOn", isOn());
        info.put("currentTemperature", String.format("%.1f", getCurrentTemperature()) + "°C");
        info.put("targetTemperature", getTargetTemperature() + "°C");
        String modeText = "heat".equals(getMode()) ? "Обогрев" : "cool".equals(getMode()) ? "Охлаждение" : "Авто";
        info.put("mode", modeText);
        info.put("humidity", String.format("%.0f", getHumidity()) + "%");
        return info;
    }
}
