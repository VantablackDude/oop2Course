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
@RequestMapping("/api/light")
@Transactional
public class LightController {

    @PersistenceContext
    private EntityManager entityManager;

    @Autowired
    private SmartHomeRepository smartHomeRepository;

    @Autowired
    private DeviceService deviceService;

    @GetMapping
    public ResponseEntity<List<Map<String, Object>>> getAllLights(@RequestParam Long homeId) {
        try {
            List<Map<String, Object>> lights = deviceService.getLightsWithDetails(homeId);
            return ResponseEntity.ok(lights);
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @GetMapping("/{id}")
    public ResponseEntity<Light> getLightById(@PathVariable Long id) {
        try {
            Device device = entityManager.find(Device.class, id);
            if ("Light".equals(device.getDeviceType())) {
                return ResponseEntity.ok((Light) device);
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
            Light light = "Light".equals(device.getDeviceType()) ? (Light) device : null;
            if (light == null) {
                return ResponseEntity.notFound().build();
            }

            Map<String, Object> response = new HashMap<>();

            if (updates.containsKey("name")) {
                light.setName((String) updates.get("name"));
                response.put("name", updates.get("name"));
            }
            if (updates.containsKey("manufacturer")) {
                light.setManufacturer((String) updates.get("manufacturer"));
                response.put("manufacturer", updates.get("manufacturer"));
            }
            if (updates.containsKey("brightness")) {
                int brightness = ((Number) updates.get("brightness")).intValue();
                light.setBrightness(brightness);
                response.put("brightness", brightness);
            }
            if (updates.containsKey("color")) {
                light.setColor((String) updates.get("color"));
                response.put("color", updates.get("color"));
            }
            if (updates.containsKey("autoMode")) {
                light.setAutoMode((boolean) updates.get("autoMode"));
                response.put("autoMode", updates.get("autoMode"));
            }
            if (updates.containsKey("homeId")) {
                Long homeId = updates.get("homeId") != null ? ((Number) updates.get("homeId")).longValue() : null;
                SmartHome home = homeId != null ? smartHomeRepository.findById(homeId).orElse(null) : null;
                light.setSmartHome(home);
                response.put("homeId", homeId);
            }

            deviceService.update(light);
            response.put("status", "success");
            response.put("message", "Свет обновлен");
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
            Light light = "Light".equals(device.getDeviceType()) ? (Light) device : null;
            Map<String, Object> response = new HashMap<>();

            switch (methodName) {
                case "displayInfo":
                    response.put("status", "success");
                    response.put("data", light.displayInfo());
                    break;
                case "turnOn":
                    light.turnOn();
                    deviceService.update(light);
                    response.put("status", "success");
                    response.put("message", "Свет включен");
                    break;
                case "turnOff":
                    light.turnOff();
                    deviceService.update(light);
                    response.put("status", "success");
                    response.put("message", "Свет выключен");
                    break;
                case "calculatePowerConsumption":
                    response.put("status", "success");
                    response.put("data", light.calculatePowerConsumption());
                    break;
                case "setBrightness":
                    int level = request.get("level") != null ? ((Number) request.get("level")).intValue() : 50;
                    light.setBrightnessWithMessage(level);
                    deviceService.update(light);
                    response.put("status", "success");
                    response.put("message", "Яркость установлена: " + level + "%");
                    break;
                case "getLightInfo":
                    response.put("status", "success");
                    response.put("data", light.getLightInfo());
                    break;
                case "getDeviceType":
                    response.put("status", "success");
                    response.put("data", light.getDeviceType());
                    break;
                case "getShortDescription":
                    response.put("status", "success");
                    response.put("data", light.getShortDescription());
                    break;
                case "getDetailedInfo":
                    response.put("status", "success");
                    response.put("data", light.getDetailedInfo());
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
    public ResponseEntity<Light> createDefaultLight() {
        try {
            Light light = new Light();
            deviceService.createLight(light);
            return ResponseEntity.status(HttpStatus.CREATED).body(light);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @PostMapping("/create")
    public ResponseEntity<Light> createLight(@RequestBody Map<String, Object> data) {
        try {
            String name = (String) data.get("name");
            int brightness = data.get("brightness") != null ? ((Number) data.get("brightness")).intValue() : 50;
            Long homeId = data.get("homeId") != null ? ((Number) data.get("homeId")).longValue() : null;
            SmartHome home = homeId != null ? smartHomeRepository.findById(homeId).orElse(null) : null;
            Light light = new Light(name, home);
            light.setBrightness(brightness);
            deviceService.createLight(light);
            return ResponseEntity.status(HttpStatus.CREATED).body(light);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @PostMapping("/create-full")
    public ResponseEntity<Light> createLightFull(@RequestBody Map<String, Object> data) {
        try {
            String name = (String) data.get("name");
            String manufacturer = (String) data.getOrDefault("manufacturer", "Philips");
            boolean isOn = (boolean) data.getOrDefault("isOn", false);
            int brightness = ((Number) data.get("brightness")).intValue();
            String color = (String) data.getOrDefault("color", "white");
            boolean autoMode = (boolean) data.getOrDefault("autoMode", false);
            int scheduledTime = data.get("scheduledTime") != null ? ((Number) data.get("scheduledTime")).intValue() : -1;
            Long homeId = data.get("homeId") != null ? ((Number) data.get("homeId")).longValue() : null;
            SmartHome home = homeId != null ? smartHomeRepository.findById(homeId).orElse(null) : null;
            Light light = new Light(name, manufacturer, isOn, brightness, color, autoMode, scheduledTime, home);
            deviceService.createLight(light);
            return ResponseEntity.status(HttpStatus.CREATED).body(light);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> deleteLight(@PathVariable Long id) {
        try {
            Device device = entityManager.find(Device.class, id);
            Light light = "Light".equals(device.getDeviceType()) ? (Light) device : null;
            if (light != null) {
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
