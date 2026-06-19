package org.example.WebAPI.Controllers;

import org.example.AppDataAPI.DeviceService;
import org.example.AppDataAPI.DeviceRepository;
import org.example.AppDataAPI.SmartHomeRepository;
import org.example.Class.*;
import jakarta.persistence.EntityManager;
import jakarta.persistence.PersistenceContext;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import java.lang.reflect.Method;
import java.util.List;
import java.util.Map;
import java.util.HashMap;

@RestController
@RequestMapping("/api/device")
@Transactional
public class DeviceController {

    @PersistenceContext
    private EntityManager entityManager;

    @Autowired
    private DeviceRepository deviceRepository;

    @Autowired
    private SmartHomeRepository smartHomeRepository;

    @Autowired
    private DeviceService deviceService;

    @GetMapping
    public ResponseEntity<List<Map<String, Object>>> getAll() {
        try {
            List<Map<String, Object>> devices = deviceService.getAllDevicesWithHome();
            return ResponseEntity.ok(devices);
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @GetMapping("/{id}")
    public ResponseEntity<Device> getById(@PathVariable Long id) {
        try {
            Device existing = deviceRepository.getReferenceById(id);
            if (existing != null) {
                return ResponseEntity.ok(existing);
            } else {
                return ResponseEntity.notFound().build();
            }
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @PostMapping
    public ResponseEntity<Device> create(@RequestBody Device device) {
        try {
            deviceService.create(device);
            return ResponseEntity.status(HttpStatus.CREATED).body(device);
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @PutMapping("/{id}")
    public ResponseEntity<Device> update(@PathVariable Long id, @RequestBody Device device) {
        try {
            Device existing = entityManager.find(Device.class, id);
            if (existing != null) {
                device.setId(id);
                deviceService.update(device);
                return ResponseEntity.ok(device);
            } else {
                return ResponseEntity.notFound().build();
            }
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @PatchMapping("/{id}")
    public ResponseEntity<Map<String, Object>> patchUpdate(@PathVariable Long id, @RequestBody Map<String, Object> updates) {
        try {
            Device device = deviceRepository.getReferenceById(id);
            if (device == null) {
                return ResponseEntity.notFound().build();
            }

            Map<String, Object> response = new HashMap<>();

            if (updates.containsKey("name")) {
                String newName = (String) updates.get("name");
                device.setName(newName);
                response.put("name", newName);
            }
            if (updates.containsKey("manufacturer")) {
                String newManufacturer = (String) updates.get("manufacturer");
                device.setManufacturer(newManufacturer);
                response.put("manufacturer", newManufacturer);
            }
            if (updates.containsKey("isOn")) {
                boolean newIsOn = (boolean) updates.get("isOn");
                device.setOn(newIsOn);
                response.put("isOn", newIsOn);
            }
            if (updates.containsKey("homeId")) {
                Long homeId = updates.get("homeId") != null ? ((Number) updates.get("homeId")).longValue() : null;
                SmartHome home = homeId != null ? smartHomeRepository.findById(homeId).orElse(null) : null;
                device.setSmartHome(home);
                response.put("homeId", homeId);
            }

            deviceService.update(device);
            response.put("status", "success");
            response.put("message", "Устройство обновлено");
            return ResponseEntity.ok(response);

        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> delete(@PathVariable Long id) {
        try {
            Device existing = deviceRepository.getReferenceById(id);
            if (existing != null) {
                deviceService.delete(id);
                return ResponseEntity.noContent().build();
            } else {
                return ResponseEntity.notFound().build();
            }
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @PostMapping("/call_method")
    public ResponseEntity<Map<String, Object>> callMethod(@RequestBody Map<String, Object> request) {
        try {
            Long id = ((Number) request.get("id")).longValue();
            Map<String, Object> methodInfoMap = (Map<String, Object>) request.get("methodInfo");
            if (methodInfoMap == null) {
                throw new IllegalArgumentException("MethodInfo not found");
            }
            String methodName = (String) methodInfoMap.get("methodName");

            Device device = deviceRepository.getReferenceById(id);
            if (device == null) {
                return ResponseEntity.notFound().build();
            }

            Map<String, Object> response = new HashMap<>();

            switch (methodName) {
                case "displayInfo":
                    response.put("status", "success");
                    response.put("data", device.displayInfo());
                    break;
                case "calculatePowerConsumption":
                    response.put("status", "success");
                    response.put("data", device.calculatePowerConsumption());
                    break;
                case "turnOn":
                    device.turnOn();
                    deviceService.update(device);
                    response.put("status", "success");
                    response.put("message", "Устройство " + (device.isOn() ? "включено" : "выключено"));
                    response.put("isOn", device.isOn());
                    break;
                case "turnOff":
                    device.turnOff();
                    deviceService.update(device);
                    response.put("status", "success");
                    response.put("message", "Устройство выключено");
                    response.put("isOn", device.isOn());
                    break;
                case "getDeviceType":
                    response.put("status", "success");
                    response.put("data", device.getDeviceType());
                    break;
                case "getShortDescription":
                    response.put("status", "success");
                    response.put("data", device.getShortDescription());
                    break;
                case "getDetailedInfo":
                    response.put("status", "success");
                    response.put("data", device.getDetailedInfo());
                    break;
                default:
                    try {
                        Method method = device.getClass().getMethod(methodName);
                        Object result = method.invoke(device);
                        response.put("status", "success");
                        if (result != null) response.put("data", result);
                    } catch (NoSuchMethodException e) {
                        response.put("status", "error");
                        response.put("message", "Неизвестный метод: " + methodName);
                        return ResponseEntity.badRequest().body(response);
                    }
                    break;
            }
            return ResponseEntity.ok(response);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                    .body(Map.of("status", "error", "message", e.getMessage()));
        }
    }
}
