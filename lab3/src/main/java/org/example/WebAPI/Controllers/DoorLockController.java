package org.example.WebAPI.Controllers;

import org.example.AppDataAPI.DeviceService;
import org.example.AppDataAPI.SmartHomeRepository;
import org.example.Class.*;
import jakarta.persistence.EntityManager;
import jakarta.persistence.PersistenceContext;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import java.util.List;
import java.util.Map;
import java.util.HashMap;

@RestController
@RequestMapping("/api/doorlock")
@Transactional
public class DoorLockController {

    @PersistenceContext
    private EntityManager entityManager;

    @Autowired
    private SmartHomeRepository smartHomeRepository;

    @Autowired
    private DeviceService deviceService;

    @GetMapping
    public ResponseEntity<List<Map<String, Object>>> getAllDoorLocks(@RequestParam Long homeId) {
        try {
            List<Map<String, Object>> locks = deviceService.getDoorLocksWithDetails(homeId);
            return ResponseEntity.ok(locks);
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @GetMapping("/{id}")
    public ResponseEntity<DoorLock> getDoorLockById(@PathVariable Long id) {
        try {
            Device device = entityManager.find(Device.class, id);
            if ("DoorLock".equals(device.getDeviceType())) {
                return ResponseEntity.ok((DoorLock) device);
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
            Device device = entityManager.find(Device.class, id);
            DoorLock doorLock = "DoorLock".equals(device.getDeviceType()) ? (DoorLock) device : null;
            if (doorLock == null) {
                return ResponseEntity.notFound().build();
            }

            Map<String, Object> response = new HashMap<>();

            if (updates.containsKey("name")) {
                doorLock.setName((String) updates.get("name"));
                response.put("name", updates.get("name"));
            }
            if (updates.containsKey("manufacturer")) {
                doorLock.setManufacturer((String) updates.get("manufacturer"));
                response.put("manufacturer", updates.get("manufacturer"));
            }
            if (updates.containsKey("isLocked")) {
                doorLock.setLocked((boolean) updates.get("isLocked"));
                response.put("isLocked", updates.get("isLocked"));
            }
            if (updates.containsKey("accessCode")) {
                doorLock.setAccessCode((String) updates.get("accessCode"));
                response.put("accessCode", "****");
            }
            if (updates.containsKey("autoLock")) {
                doorLock.setAutoLock((boolean) updates.get("autoLock"));
                response.put("autoLock", updates.get("autoLock"));
            }
            if (updates.containsKey("homeId")) {
                Long homeId = updates.get("homeId") != null ? ((Number) updates.get("homeId")).longValue() : null;
                SmartHome home = homeId != null ? smartHomeRepository.findById(homeId).orElse(null) : null;
                doorLock.setSmartHome(home);
                response.put("homeId", homeId);
            }

            deviceService.update(doorLock);
            response.put("status", "success");
            response.put("message", "Замок обновлен");
            return ResponseEntity.ok(response);

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
            String methodName = (String) methodInfoMap.get("methodName");

            Device device = entityManager.find(Device.class, id);
            DoorLock doorLock = "DoorLock".equals(device.getDeviceType()) ? (DoorLock) device : null;
            Map<String, Object> response = new HashMap<>();

            switch (methodName) {
                case "displayInfo":
                    response.put("status", "success");
                    response.put("data", doorLock.displayInfo());
                    break;
                case "turnOn":
                    doorLock.turnOn();
                    deviceService.update(doorLock);
                    response.put("status", "success");
                    response.put("message", "Замок включен");
                    break;
                case "turnOff":
                    doorLock.turnOff();
                    deviceService.update(doorLock);
                    response.put("status", "success");
                    response.put("message", "Замок выключен");
                    break;
                case "calculatePowerConsumption":
                    response.put("status", "success");
                    response.put("data", doorLock.calculatePowerConsumption());
                    break;
                case "tryUnlock":
                    String code = request.get("code") != null ? (String) request.get("code") : "";
                    boolean unlocked = doorLock.tryUnlock(code);
                    deviceService.update(doorLock);
                    response.put("status", "success");
                    response.put("data", unlocked);
                    response.put("message", unlocked ? "Дверь открыта" : "Не удалось открыть");
                    break;
                case "getLockInfo":
                    response.put("status", "success");
                    response.put("data", doorLock.getLockInfo());
                    break;
                case "getDeviceType":
                    response.put("status", "success");
                    response.put("data", doorLock.getDeviceType());
                    break;
                case "getDetailedInfo":
                    response.put("status", "success");
                    response.put("data", doorLock.getDetailedInfo());
                    break;
                default:
                    response.put("status", "error");
                    response.put("message", "Неизвестный метод: " + methodName);
                    return ResponseEntity.badRequest().body(response);
            }
            return ResponseEntity.ok(response);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                    .body(Map.of("status", "error", "message", e.getMessage()));
        }
    }

    @PostMapping("/default")
    public ResponseEntity<DoorLock> createDefaultDoorLock() {
        try {
            DoorLock doorLock = new DoorLock();
            deviceService.createDoorLock(doorLock);
            return ResponseEntity.status(HttpStatus.CREATED).body(doorLock);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @PostMapping("/create")
    public ResponseEntity<DoorLock> createDoorLock(@RequestBody Map<String, Object> data) {
        try {
            String name = (String) data.get("name");
            String accessCode = (String) data.getOrDefault("accessCode", "1234");
            Long homeId = data.get("homeId") != null ? ((Number) data.get("homeId")).longValue() : null;
            SmartHome home = homeId != null ? smartHomeRepository.findById(homeId).orElse(null) : null;
            DoorLock doorLock = new DoorLock(name, home);
            doorLock.setAccessCode(accessCode);
            deviceService.createDoorLock(doorLock);
            return ResponseEntity.status(HttpStatus.CREATED).body(doorLock);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @PostMapping("/create-full")
    public ResponseEntity<DoorLock> createDoorLockFull(@RequestBody Map<String, Object> data) {
        try {
            String name = (String) data.get("name");
            String manufacturer = (String) data.getOrDefault("manufacturer", "Samsung");
            boolean isOn = (boolean) data.getOrDefault("isOn", false);
            boolean isLocked = (boolean) data.getOrDefault("isLocked", true);
            String accessCode = (String) data.getOrDefault("accessCode", "1234");
            int wrongAttempts = data.get("wrongAttempts") != null ? ((Number) data.get("wrongAttempts")).intValue() : 0;
            boolean autoLock = (boolean) data.getOrDefault("autoLock", true);
            String lastAccessTime = (String) data.getOrDefault("lastAccessTime", "Никогда");
            Long homeId = data.get("homeId") != null ? ((Number) data.get("homeId")).longValue() : null;
            SmartHome home = homeId != null ? smartHomeRepository.findById(homeId).orElse(null) : null;
            DoorLock doorLock = new DoorLock(name, manufacturer, isOn, isLocked, accessCode, wrongAttempts, autoLock, lastAccessTime, home);
            deviceService.createDoorLock(doorLock);
            return ResponseEntity.status(HttpStatus.CREATED).body(doorLock);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> deleteDoorLock(@PathVariable Long id) {
        try {
            Device device = entityManager.find(Device.class, id);
            DoorLock doorLock = "DoorLock".equals(device.getDeviceType()) ? (DoorLock) device : null;
            if (doorLock != null) {
                deviceService.delete(id);
                return ResponseEntity.noContent().build();
            } else {
                return ResponseEntity.notFound().build();
            }
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }
}
