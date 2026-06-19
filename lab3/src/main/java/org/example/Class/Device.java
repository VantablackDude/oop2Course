package org.example.Class;

import jakarta.persistence.*;
import lombok.*;
import com.fasterxml.jackson.annotation.JsonBackReference;
import java.io.Serializable;
import java.time.LocalDateTime;
import java.util.Map;
import java.util.HashMap;

@Entity
@Table(name = "device")
@Inheritance(strategy = InheritanceType.JOINED)
@Data
@NoArgsConstructor
@AllArgsConstructor
@EqualsAndHashCode(onlyExplicitlyIncluded = true)
public abstract class Device implements Serializable {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @EqualsAndHashCode.Include
    private Long id;

    @Column(name = "name", nullable = false, length = 100)
    private String name;

    @Column(name = "manufacturer", length = 100)
    private String manufacturer;

    @Column(name = "is_on")
    private boolean isOn;

    @Column(name = "created_at")
    private LocalDateTime createdAt;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "home_id")
    @JsonBackReference
    @ToString.Exclude
    private SmartHome smartHome;

    public Device(String name, SmartHome smartHome) {
        this.name = name;
        this.manufacturer = "Unknown";
        this.isOn = false;
        this.createdAt = LocalDateTime.now();
        this.smartHome = smartHome;
    }

    public Device(String name, String manufacturer, boolean isOn, SmartHome smartHome) {
        this.name = name;
        this.manufacturer = manufacturer;
        this.isOn = isOn;
        this.createdAt = LocalDateTime.now();
        this.smartHome = smartHome;
    }

    public void turnOn() {
        this.isOn = true;
        System.out.println("Устройство " + name + " включено");
    }

    public void turnOff() {
        this.isOn = false;
        System.out.println("Устройство " + name + " выключено");
    }

    public Map<String, Object> displayInfo() {
        Map<String, Object> data = new HashMap<>();
        data.put("type", "Устройство");
        data.put("id", id);
        data.put("name", name);
        data.put("manufacturer", manufacturer);
        data.put("isOn", isOn);
        data.put("status", isOn ? "Включено" : "Выключено");
        data.put("createdAt", createdAt != null ? createdAt.toString() : "-");
        return data;
    }

    public double calculatePowerConsumption() {
        return 0;
    }

    public abstract String getDeviceType();

    public abstract String getShortDescription();

    public abstract Map<String, Object> getDetailedInfo();
}
