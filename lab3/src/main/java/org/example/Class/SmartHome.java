package org.example.Class;

import lombok.Data;
import lombok.NoArgsConstructor;
import lombok.AllArgsConstructor;
import lombok.ToString;
import com.fasterxml.jackson.annotation.JsonManagedReference;
import jakarta.persistence.*;
import java.io.Serializable;
import java.util.*;

@Entity
@Table(name = "smart_home")
@Data
@NoArgsConstructor
@AllArgsConstructor
public class SmartHome implements Serializable {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(name = "name", nullable = false)
    private String name = "Мой умный дом";

    @Column(name = "address", length = 200)
    private String address;

    @OneToMany(mappedBy = "smartHome", cascade = CascadeType.ALL, orphanRemoval = true, fetch = FetchType.EAGER)
    @ToString.Exclude
    @JsonManagedReference
    private List<Device> devices = new ArrayList<>();

    public SmartHome(String name) {
        this.name = name;
        System.out.println("Создан объект умного дома: " + name);
    }

    public SmartHome(String name, String address) {
        this.name = name;
        this.address = address;
        System.out.println("Создан объект умного дома: " + name + " по адресу: " + address);
    }

    public SmartHome(List<Device> devices, String name) {
        this.devices = devices;
        this.name = name;
        for (Device device : devices) {
            device.setSmartHome(this);
        }
        System.out.println("Создан объект умного дома: " + name);
    }

    public void addDevice(Device device) {
        devices.add(device);
        device.setSmartHome(this);
    }

    public void removeDeviceById(int index) {
        if (index >= 0 && index < devices.size()) {
            Device device = devices.get(index);
            device.setSmartHome(null);
            devices.remove(index);
        }
    }

    public void removeDevice(Device device) {
        device.setSmartHome(null);
        devices.remove(device);
    }

    public void removeDeviceById(Long id) {
        devices.removeIf(device -> device.getId().equals(id));
    }

    public List<Map<String, Object>> getDevicesWithDetails() {
        List<Map<String, Object>> result = new ArrayList<>();
        for (Device device : devices) {
            Map<String, Object> item = new HashMap<>();
            item.put("id", device.getId());
            item.put("name", device.getName());
            item.put("manufacturer", device.getManufacturer());
            item.put("isOn", device.isOn());

            switch (device.getDeviceType()) {
                case "Camera":
                    item.put("type", "camera");
                    item.put("resolution", device.getDetailedInfo().get("resolution"));
                    item.put("isRecording", device.getDetailedInfo().get("recording"));
                    item.put("motionDetection", device.getDetailedInfo().get("motionDetection"));
                    break;
                case "DoorLock":
                    item.put("type", "doorlock");
                    item.put("isLocked", device.getDetailedInfo().get("isLocked"));
                    item.put("autoLock", device.getDetailedInfo().get("autoLock"));
                    item.put("wrongAttempts", device.getDetailedInfo().get("wrongAttempts"));
                    break;
                case "Light":
                    item.put("type", "light");
                    item.put("brightness", device.getDetailedInfo().get("brightness"));
                    item.put("color", device.getDetailedInfo().get("color"));
                    break;
                case "Thermostat":
                    item.put("type", "thermostat");
                    item.put("currentTemperature", device.getDetailedInfo().get("currentTemperature"));
                    item.put("targetTemperature", device.getDetailedInfo().get("targetTemperature"));
                    item.put("mode", device.getDetailedInfo().get("mode"));
                    break;
            }
            result.add(item);
        }
        return result;
    }
}
