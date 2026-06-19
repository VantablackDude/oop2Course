package org.example.AppDataAPI;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import jakarta.persistence.EntityManager;
import jakarta.persistence.PersistenceContext;
import java.util.List;
import java.util.ArrayList;
import java.util.Map;
import java.util.HashMap;
import org.example.Class.*;

@Service
@Transactional
public class DeviceService {

    @PersistenceContext
    private EntityManager entityManager;

    @Autowired
    private SmartHomeRepository smartHomeRepository;

    @Autowired
    private DeviceRepository deviceRepository;

    public void update(Device device) {
        entityManager.merge(device);
        entityManager.flush();
    }

    public SmartHome updateSmartHome(Long id, String name) {
        SmartHome home = smartHomeRepository.findById(id).orElse(null);
        if (home != null) {
            home.setName(name);
            return smartHomeRepository.save(home);
        }
        return null;
    }

    public void addExistingDeviceToHome(Long homeId, Long deviceId) {
        SmartHome home = smartHomeRepository.findById(homeId).orElse(null);
        Device device = deviceRepository.findById(deviceId).orElse(null);

        if (home != null && device != null) {
            home.addDevice(device);
            smartHomeRepository.save(home);
        }
    }

    public void removeDeviceFromHome(Long homeId, Long deviceId) {
        SmartHome home = smartHomeRepository.findById(homeId).orElse(null);
        if (home != null) {
            home.removeDeviceById(deviceId);
            smartHomeRepository.save(home);
        }
    }

    public void delete(Long id) {
        Device device = entityManager.find(Device.class, id);
        if (device != null) {
            entityManager.remove(device);
            entityManager.flush();
        }
    }

    public void create(Device device) {
        if (device.getId() != null && device.getId() == 0) {
            device.setId(null);
        }
        entityManager.persist(device);
        entityManager.flush();
    }

    public void createCamera(Camera camera) {
        System.out.println("=== Creating Camera ===");
        System.out.println("Name: " + camera.getName());
        System.out.println("Resolution: " + camera.getResolution());
        if (camera.getId() != null && camera.getId() == 0) {
            camera.setId(null);
        }
        entityManager.persist(camera);
        entityManager.flush();
        System.out.println("Camera created with ID: " + camera.getId());
    }

    public void createDoorLock(DoorLock doorLock) {
        System.out.println("=== Creating DoorLock ===");
        System.out.println("Name: " + doorLock.getName());
        if (doorLock.getId() != null && doorLock.getId() == 0) {
            doorLock.setId(null);
        }
        entityManager.persist(doorLock);
        entityManager.flush();
        System.out.println("DoorLock created with ID: " + doorLock.getId());
    }

    public void createLight(Light light) {
        System.out.println("=== Creating Light ===");
        System.out.println("Name: " + light.getName());
        if (light.getId() != null && light.getId() == 0) {
            light.setId(null);
        }
        entityManager.persist(light);
        entityManager.flush();
        System.out.println("Light created with ID: " + light.getId());
    }

    public void createThermostat(Thermostat thermostat) {
        System.out.println("=== Creating Thermostat ===");
        System.out.println("Name: " + thermostat.getName());
        if (thermostat.getId() != null && thermostat.getId() == 0) {
            thermostat.setId(null);
        }
        entityManager.persist(thermostat);
        entityManager.flush();
        System.out.println("Thermostat created with ID: " + thermostat.getId());
    }

    public List<Map<String, Object>> getCamerasWithDetails(Long homeId) {
        SmartHome home = smartHomeRepository.findById(homeId).orElse(null);
        if (home == null) return List.of();
        List<Map<String, Object>> result = new ArrayList<>();
        for (Device device : home.getDevices()) {
            if (device instanceof Camera) {
                result.add(device.displayInfo());
            }
        }
        return result;
    }

    public List<Map<String, Object>> getDoorLocksWithDetails(Long homeId) {
        SmartHome home = smartHomeRepository.findById(homeId).orElse(null);
        if (home == null) return List.of();
        List<Map<String, Object>> result = new ArrayList<>();
        for (Device device : home.getDevices()) {
            if (device instanceof DoorLock) {
                result.add(device.displayInfo());
            }
        }
        return result;
    }

    public List<Map<String, Object>> getLightsWithDetails(Long homeId) {
        SmartHome home = smartHomeRepository.findById(homeId).orElse(null);
        if (home == null) return List.of();
        List<Map<String, Object>> result = new ArrayList<>();
        for (Device device : home.getDevices()) {
            if (device instanceof Light) {
                result.add(device.displayInfo());
            }
        }
        return result;
    }

    public List<Map<String, Object>> getThermostatsWithDetails(Long homeId) {
        SmartHome home = smartHomeRepository.findById(homeId).orElse(null);
        if (home == null) return List.of();
        List<Map<String, Object>> result = new ArrayList<>();
        for (Device device : home.getDevices()) {
            if (device instanceof Thermostat) {
                result.add(device.displayInfo());
            }
        }
        return result;
    }

    public List<Map<String, Object>> getAllDevicesWithHome() {
        List<Device> devices = deviceRepository.findAll();
        List<Map<String, Object>> result = new ArrayList<>();
        for (Device device : devices) {
            Map<String, Object> item = device.displayInfo();
            if (device.getSmartHome() != null) {
                item.put("homeName", device.getSmartHome().getName());
                item.put("homeId", device.getSmartHome().getId());
            }
            result.add(item);
        }
        return result;
    }

}

