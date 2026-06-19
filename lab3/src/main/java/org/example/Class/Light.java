package org.example.Class;

import jakarta.persistence.*;
import lombok.*;
import java.io.Serializable;
import java.util.Map;
import java.util.HashMap;

@Entity
@Table(name = "light")
@PrimaryKeyJoinColumn(name = "device_id")
@Data
@EqualsAndHashCode(callSuper = true)
@NoArgsConstructor
@ToString(callSuper = true)
public class Light extends Device implements Serializable {

    @Override
    public String toString() {
        return "Light{" +
                "id=" + getId() +
                ", name='" + getName() + '\'' +
                ", manufacturer='" + getManufacturer() + '\'' +
                ", isOn=" + isOn() +
                ", brightness=" + brightness +
                ", color='" + color + '\'' +
                ", autoMode=" + autoMode +
                ", scheduledTime=" + scheduledTime +
                '}';
    }

    @Column(name = "brightness")
    private int brightness;

    @Column(name = "color", length = 50)
    private String color;

    @Column(name = "auto_mode")
    private boolean autoMode;

    @Column(name = "scheduled_time")
    private int scheduledTime;

    public Light(String name, SmartHome smartHome) {
        super(name, smartHome);
        this.brightness = 50;
        this.color = "white";
        this.autoMode = false;
        this.scheduledTime = -1;
        this.setManufacturer("Philips");
    }

    public Light(String name, String manufacturer, boolean isOn, int brightness,
                 String color, boolean autoMode, int scheduledTime, SmartHome smartHome) {
        super(name, manufacturer, isOn, smartHome);
        this.brightness = brightness;
        this.color = color;
        this.autoMode = autoMode;
        this.scheduledTime = scheduledTime;
    }

    public void setBrightness(int brightness) {
        this.brightness = brightness < 0 ? 0 : Math.min(brightness, 100);
    }

    public void setBrightnessWithMessage(int level) {
        brightness = level < 0 ? 0 : Math.min(level, 100);

        if (brightness > 80) {
            System.out.println("ВНИМАНИЕ: Высокая яркость потребляет много энергии! Рекомендуется снизить до 60%.");
        } else if (brightness < 10) {
            System.out.println("Яркость минимальная. Включён ночной режим.");
        } else if (brightness >= 40 && brightness <= 60) {
            System.out.println("Оптимальная яркость для работы. Энергопотребление в норме.");
        } else {
            System.out.println("Яркость установлена: " + brightness + "%");
        }
    }

    public String getLightInfo() {
        String info = "Яркость: " + brightness + "%, Цвет: " + color;

        if (autoMode && scheduledTime >= 0) {
            int currentHour = java.time.LocalTime.now().getHour();
            if (currentHour >= scheduledTime) {
                info += ". Режим: авто. Вечернее освещение активно.";
            } else {
                info += ". Режим: авто. Ожидание вечернего расписания.";
            }
        } else if (autoMode) {
            info += ". Режим: авто. Расписание не задано.";
        } else {
            info += ". Режим: ручной.";
        }

        if (brightness == 0) {
            info += " СВЕТ ВЫКЛЮЧЕН.";
        }

        return info;
    }

    @Override
    public void turnOn() {
        super.turnOn();
        if (brightness < 20) {
            brightness = 30;
            System.out.println("Яркость автоматически увеличена до 30% для комфортного освещения.");
        }
        System.out.println("Свет включен. Яркость: " + brightness + "%, Цвет: " + color);
    }

    @Override
    public Map<String, Object> displayInfo() {
        Map<String, Object> result = super.displayInfo();
        result.put("type", "Light");
        result.put("brightness", brightness);
        result.put("color", color);
        result.put("autoMode", autoMode);
        result.put("scheduledTime", scheduledTime);
        return result;
    }

    @Override
    public double calculatePowerConsumption() {
        if (!isOn()) return 0.1;
        return brightness * 0.5;
    }

    @Override
    public String getDeviceType() {
        return "Light";
    }

    @Override
    public String getShortDescription() {
        return "Light '" + getName() + "': " + getBrightness() + "% (" + getColor() + ")";
    }

    @Override
    public Map<String, Object> getDetailedInfo() {
        Map<String, Object> info = new HashMap<>();
        info.put("deviceType", getDeviceType());
        info.put("name", getName());
        info.put("manufacturer", getManufacturer());
        info.put("isOn", isOn());
        info.put("brightness", getBrightness() + "%");
        info.put("color", getColor());
        info.put("autoMode", isAutoMode() ? "Да" : "Нет");
        info.put("scheduledTime", scheduledTime >= 0 ? scheduledTime + ":00" : "Не задано");
        return info;
    }
}
