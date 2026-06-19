package org.example.Class;

import jakarta.persistence.*;
import lombok.*;
import java.io.Serializable;
import java.util.Map;
import java.util.HashMap;

@Entity
@Table(name = "door_lock")
@PrimaryKeyJoinColumn(name = "device_id")
@Data
@EqualsAndHashCode(callSuper = true)
@NoArgsConstructor
@ToString(callSuper = true)
public class DoorLock extends Device implements Serializable {

    @Override
    public String toString() {
        return "DoorLock{" +
                "id=" + getId() +
                ", name='" + getName() + '\'' +
                ", manufacturer='" + getManufacturer() + '\'' +
                ", isOn=" + isOn() +
                ", isLocked=" + isLocked +
                ", accessCode='****'" +
                ", wrongAttempts=" + wrongAttempts +
                ", autoLock=" + autoLock +
                ", lastAccessTime='" + lastAccessTime + '\'' +
                '}';
    }

    @Column(name = "is_locked")
    private boolean isLocked;

    @Column(name = "access_code", length = 10)
    private String accessCode;

    @Column(name = "wrong_attempts")
    private int wrongAttempts;

    @Column(name = "auto_lock")
    private boolean autoLock;

    @Column(name = "last_access_time", length = 100)
    private String lastAccessTime;

    public DoorLock(String name, SmartHome smartHome) {
        super(name, smartHome);
        this.isLocked = true;
        this.accessCode = "1234";
        this.wrongAttempts = 0;
        this.autoLock = true;
        this.lastAccessTime = "Никогда";
        this.setManufacturer("Samsung");
    }

    public DoorLock(String name, String manufacturer, boolean isOn, boolean isLocked,
                    String accessCode, int wrongAttempts, boolean autoLock,
                    String lastAccessTime, SmartHome smartHome) {
        super(name, manufacturer, isOn, smartHome);
        this.isLocked = isLocked;
        this.accessCode = accessCode;
        this.wrongAttempts = wrongAttempts;
        this.autoLock = autoLock;
        this.lastAccessTime = lastAccessTime;
    }

    public boolean tryUnlock(String code) {
        if (wrongAttempts >= 3) {
            System.out.println("ВНИМАНИЕ: Превышено количество неверных попыток! Система временно заблокирована.");
            return false;
        }

        if (code.equals(accessCode)) {
            isLocked = false;
            wrongAttempts = 0;
            java.time.LocalTime now = java.time.LocalTime.now();
            lastAccessTime = now.format(java.time.format.DateTimeFormatter.ofPattern("HH:mm:ss"));
            System.out.println("Доступ разрешен. Время: " + lastAccessTime);
            return true;
        } else {
            wrongAttempts++;
            System.out.println("Неверный код. Попытка " + wrongAttempts + " из 3.");
            if (wrongAttempts >= 3) {
                System.out.println("КРИТИЧЕСКИЙ СБОЙ БЕЗОПАСНОСТИ: Замок заблокирован на 30 секунд!");
            }
            return false;
        }
    }

    public String getLockInfo() {
        String state = isLocked ? "Заперт" : "Открыт";
        String security = wrongAttempts >= 3 ? "ТРЕВОГА: Система заблокирована!" :
                          wrongAttempts >= 1 ? "ВНИМАНИЕ: " + wrongAttempts + " неверных попытки" :
                          "Безопасность в норме";

        return "Состояние: " + state + ", Попыток: " + wrongAttempts + ", Последний доступ: " + lastAccessTime + ". " + security;
    }

    @Override
    public void turnOn() {
        super.turnOn();
        if (autoLock && !isLocked) {
            isLocked = true;
            System.out.println("Автоблокировка: дверь заперта.");
        }
        System.out.println("Замок активен. Состояние: " + (isLocked ? "заперт" : "открыт"));
    }

    @Override
    public Map<String, Object> displayInfo() {
        Map<String, Object> result = super.displayInfo();
        result.put("type", "DoorLock");
        result.put("isLocked", isLocked);
        result.put("accessCode", "****");
        result.put("wrongAttempts", wrongAttempts);
        result.put("autoLock", autoLock);
        result.put("lastAccessTime", lastAccessTime);
        return result;
    }

    @Override
    public double calculatePowerConsumption() {
        return isOn() ? 3 : 0.1;
    }

    @Override
    public String getDeviceType() {
        return "DoorLock";
    }

    @Override
    public String getShortDescription() {
        return "DoorLock '" + getName() + "': " + (isLocked() ? "Заперт" : "Открыт");
    }

    @Override
    public Map<String, Object> getDetailedInfo() {
        Map<String, Object> info = new HashMap<>();
        info.put("deviceType", getDeviceType());
        info.put("name", getName());
        info.put("manufacturer", getManufacturer());
        info.put("isOn", isOn());
        info.put("isLocked", isLocked() ? "Заперт" : "Открыт");
        info.put("autoLock", isAutoLock() ? "Да" : "Нет");
        info.put("wrongAttempts", getWrongAttempts());
        info.put("lastAccessTime", getLastAccessTime());
        String alert = getWrongAttempts() >= 3 ? " !БЛОКИРОВКА!" : "";
        info.put("warning", alert);
        return info;
    }
}
