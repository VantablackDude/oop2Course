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
@RequestMapping("/api/thermostat")
@Transactional
public class ThermostatController {

    @PersistenceContext
    private EntityManager entityManager;

    @Autowired
    private SmartHomeRepository smartHomeRepository;

    @Autowired
    private DeviceService deviceService;

    @GetMapping
    public ResponseEntity<List<Map<String, Object>>> getAllThermostats(@RequestParam Long homeId) {
        try {
            List<Map<String, Object>> thermostats = deviceService.getThermostatsWithDetails(homeId);
            return ResponseEntity.ok(thermostats);
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @GetMapping("/{id}")
    public ResponseEntity<Thermostat> getThermostatById(@PathVariable Long id) {
        try {
            Device device = entityManager.find(Device.class, id);
            if ("Thermostat".equals(device.getDeviceType())) {
                return ResponseEntity.ok((Thermostat) device);
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
            Thermostat thermostat = "Thermostat".equals(device.getDeviceType()) ? (Thermostat) device : null;
            if (thermostat == null) {
                return ResponseEntity.notFound().build();
            }

            Map<String, Object> response = new HashMap<>();

            if (updates.containsKey("name")) {
                thermostat.setName((String) updates.get("name"));
                response.put("name", updates.get("name"));
            }
            if (updates.containsKey("manufacturer")) {
                thermostat.setManufacturer((String) updates.get("manufacturer"));
                response.put("manufacturer", updates.get("manufacturer"));
            }
            if (updates.containsKey("targetTemperature")) {
                thermostat.setTargetTemperature(((Number) updates.get("targetTemperature")).doubleValue());
                response.put("targetTemperature", updates.get("targetTemperature"));
            }
            if (updates.containsKey("mode")) {
                thermostat.setMode((String) updates.get("mode"));
                response.put("mode", updates.get("mode"));
            }
            if (updates.containsKey("homeId")) {
                Long homeId = updates.get("homeId") != null ? ((Number) updates.get("homeId")).longValue() : null;
                SmartHome home = homeId != null ? smartHomeRepository.findById(homeId).orElse(null) : null;
                thermostat.setSmartHome(home);
                response.put("homeId", homeId);
            }

            deviceService.update(thermostat);
            response.put("status", "success");
            response.put("message", "Термостат обновлен");
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
            Thermostat thermostat = "Thermostat".equals(device.getDeviceType()) ? (Thermostat) device : null;
            Map<String, Object> response = new HashMap<>();

            switch (methodName) {
                case "displayInfo":
                    response.put("status", "success");
                    response.put("data", thermostat.displayInfo());
                    break;
                case "turnOn":
                    thermostat.turnOn();
                    deviceService.update(thermostat);
                    response.put("status", "success");
                    response.put("message", "Термостат включен");
                    break;
                case "turnOff":
                    thermostat.turnOff();
                    deviceService.update(thermostat);
                    response.put("status", "success");
                    response.put("message", "Термостат выключен");
                    break;
                case "calculatePowerConsumption":
                    response.put("status", "success");
                    response.put("data", thermostat.calculatePowerConsumption());
                    break;
                case "setTemperature":
                    double temp = request.get("temperature") != null ? ((Number) request.get("temperature")).doubleValue() : 22.0;
                    thermostat.setTemperature(temp);
                    deviceService.update(thermostat);
                    response.put("status", "success");
                    response.put("message", "Температура установлена: " + thermostat.getTargetTemperature() + "°C");
                    break;
                case "getClimateInfo":
                    response.put("status", "success");
                    response.put("data", thermostat.getClimateInfo());
                    break;
                case "getDeviceType":
                    response.put("status", "success");
                    response.put("data", thermostat.getDeviceType());
                    break;
                case "getShortDescription":
                    response.put("status", "success");
                    response.put("data", thermostat.getShortDescription());
                    break;
                case "getDetailedInfo":
                    response.put("status", "success");
                    response.put("data", thermostat.getDetailedInfo());
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
    public ResponseEntity<Thermostat> createDefaultThermostat() {
        try {
            Thermostat thermostat = new Thermostat();
            deviceService.createThermostat(thermostat);
            return ResponseEntity.status(HttpStatus.CREATED).body(thermostat);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @PostMapping("/create")
    public ResponseEntity<Thermostat> createThermostat(@RequestBody Map<String, Object> data) {
        try {
            String name = (String) data.get("name");
            double targetTemp = data.get("targetTemperature") != null ? ((Number) data.get("targetTemperature")).doubleValue() : 22.0;
            Long homeId = data.get("homeId") != null ? ((Number) data.get("homeId")).longValue() : null;
            SmartHome home = homeId != null ? smartHomeRepository.findById(homeId).orElse(null) : null;
            Thermostat thermostat = new Thermostat(name, home);
            thermostat.setTargetTemperature(targetTemp);
            deviceService.createThermostat(thermostat);
            return ResponseEntity.status(HttpStatus.CREATED).body(thermostat);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @PostMapping("/create-full")
    public ResponseEntity<Thermostat> createThermostatFull(@RequestBody Map<String, Object> data) {
        try {
            String name = (String) data.get("name");
            String manufacturer = (String) data.getOrDefault("manufacturer", "Nest");
            boolean isOn = (boolean) data.getOrDefault("isOn", false);
            double currentTemp = data.get("currentTemperature") != null ? ((Number) data.get("currentTemperature")).doubleValue() : 20.0;
            double targetTemp = ((Number) data.get("targetTemperature")).doubleValue();
            String mode = (String) data.getOrDefault("mode", "auto");
            double humidity = data.get("humidity") != null ? ((Number) data.get("humidity")).doubleValue() : 50.0;
            Long homeId = data.get("homeId") != null ? ((Number) data.get("homeId")).longValue() : null;
            SmartHome home = homeId != null ? smartHomeRepository.findById(homeId).orElse(null) : null;
            Thermostat thermostat = new Thermostat(name, manufacturer, isOn, currentTemp, targetTemp, mode, humidity, home);
            deviceService.createThermostat(thermostat);
            return ResponseEntity.status(HttpStatus.CREATED).body(thermostat);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> deleteThermostat(@PathVariable Long id) {
        try {
            Device device = entityManager.find(Device.class, id);
            Thermostat thermostat = "Thermostat".equals(device.getDeviceType()) ? (Thermostat) device : null;
            if (thermostat != null) {
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
