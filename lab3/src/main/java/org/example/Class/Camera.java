package org.example.Class;

import jakarta.persistence.*;
import lombok.*;
import java.io.Serializable;
import java.util.Map;
import java.util.HashMap;

@Entity
@Table(name = "camera")
@PrimaryKeyJoinColumn(name = "device_id")
@Data
@EqualsAndHashCode(callSuper = true)
@NoArgsConstructor
@ToString(callSuper = true)
public class Camera extends Device implements Serializable {

    @Override
    public String toString() {
        return "Camera{" +
                "id=" + getId() +
                ", name='" + getName() + '\'' +
                ", manufacturer='" + getManufacturer() + '\'' +
                ", isOn=" + isOn() +
                ", resolution=" + resolution +
                ", isRecording=" + isRecording +
                ", motionDetection=" + motionDetection +
                ", storageUsed=" + storageUsed +
                ", lastMotionTime='" + lastMotionTime + '\'' +
                '}';
    }

    @Column(name = "resolution")
    private int resolution;

    @Column(name = "is_recording")
    private boolean isRecording;

    @Column(name = "motion_detection")
    private boolean motionDetection;

    @Column(name = "storage_used")
    private int storageUsed;

    @Column(name = "last_motion_time", length = 100)
    private String lastMotionTime;

    public Camera(String name, SmartHome smartHome) {
        super(name, smartHome);
        this.resolution = 1080;
        this.isRecording = false;
        this.motionDetection = true;
        this.storageUsed = 0;
        this.lastMotionTime = "Никогда";
        this.setManufacturer("HikVision");
    }

    public Camera(String name, String manufacturer, boolean isOn, int resolution,
                  boolean isRecording, boolean motionDetection, int storageUsed,
                  String lastMotionTime, SmartHome smartHome) {
        super(name, manufacturer, isOn, smartHome);
        this.resolution = resolution;
        this.isRecording = isRecording;
        this.motionDetection = motionDetection;
        this.storageUsed = storageUsed;
        this.lastMotionTime = lastMotionTime;
    }

    public String startRecording() {
        if (storageUsed >= 95) {
            return "ОШИБКА: Недостаточно места на накопителе! Освободите память.";
        }

        isRecording = true;

        String msg;
        if (resolution >= 2160) {
            storageUsed += 10;
            msg = "Запись 4K запущена. Расход памяти высокий (" + storageUsed + "%).";
        } else if (resolution >= 1080) {
            storageUsed += 5;
            msg = "Запись Full HD запущена. Расход памяти средний (" + storageUsed + "%).";
        } else {
            storageUsed += 2;
            msg = "Запись HD запущена. Расход памяти низкий (" + storageUsed + "%).";
        }

        return msg;
    }

    public String recordMotion() {
        java.time.LocalTime now = java.time.LocalTime.now();
        lastMotionTime = now.format(java.time.format.DateTimeFormatter.ofPattern("HH:mm:ss"));

        if (!motionDetection) {
            return "Движение зафиксировано в " + lastMotionTime + ", но детекция движения отключена.";
        }

        if (!isRecording) {
            return "ВНИМАНИЕ: Движение обнаружено в " + lastMotionTime + "! Запись не ведётся. Включите запись.";
        }

        storageUsed += 1;
        return "ДВИЖЕНИЕ ОБНАРУЖЕНО в " + lastMotionTime + ". Фрагмент сохранён. Память: " + storageUsed + "%.";
    }

    @Override
    public void turnOn() {
        super.turnOn();
        String resText = resolution >= 2160 ? "4K" : resolution >= 1080 ? "Full HD" : "HD";
        System.out.println("Камера " + resText + " готова к работе. Детекция движения: " + (motionDetection ? "вкл" : "выкл"));
    }

    @Override
    public Map<String, Object> displayInfo() {
        Map<String, Object> result = super.displayInfo();
        result.put("type", "Camera");
        result.put("resolution", resolution);
        result.put("isRecording", isRecording);
        result.put("motionDetection", motionDetection);
        result.put("storageUsed", storageUsed);
        result.put("lastMotionTime", lastMotionTime);
        return result;
    }

    @Override
    public double calculatePowerConsumption() {
        double base = isOn() ? 10 : 0.5;
        double recordingBonus = isRecording ? 5 : 0;
        double resolutionBonus = resolution >= 2160 ? 8 : resolution >= 1080 ? 3 : 1;
        return base + recordingBonus + resolutionBonus;
    }

    @Override
    public String getDeviceType() {
        return "Camera";
    }

    @Override
    public String getShortDescription() {
        return "Camera '" + getName() + "': " + resolution + "p " + (isRecording() ? "запись" : "ожидание");
    }

    @Override
    public Map<String, Object> getDetailedInfo() {
        Map<String, Object> info = new HashMap<>();
        info.put("deviceType", getDeviceType());
        info.put("name", getName());
        info.put("manufacturer", getManufacturer());
        info.put("isOn", isOn());
        info.put("resolution", resolution);
        info.put("resolutionText", resolution >= 2160 ? "4K" : resolution >= 1080 ? "Full HD" : "HD");
        info.put("recording", isRecording ? "Записывает" : "Ожидание");
        info.put("motionDetection", motionDetection ? "Вкл" : "Выкл");
        info.put("storageUsed", storageUsed + "%");
        info.put("lastMotionTime", lastMotionTime);
        String storageWarn = storageUsed >= 90 ? " !ОСТАЛОСЬ МАЛО МЕСТА!" : "";
        info.put("warning", storageWarn);
        return info;
    }
}
