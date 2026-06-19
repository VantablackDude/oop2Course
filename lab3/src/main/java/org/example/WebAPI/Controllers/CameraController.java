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
@RequestMapping("/api/camera")
@Transactional
public class CameraController {

    @PersistenceContext
    private EntityManager entityManager;

    @Autowired
    private SmartHomeRepository smartHomeRepository;

    @Autowired
    private DeviceService deviceService;

    @GetMapping
    public ResponseEntity<List<Map<String, Object>>> getAllCameras(@RequestParam Long homeId) {
        try {
            List<Map<String, Object>> cameras = deviceService.getCamerasWithDetails(homeId);
            return ResponseEntity.ok(cameras);
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @GetMapping("/{id}")
    public ResponseEntity<Camera> getCameraById(@PathVariable Long id) {
        try {
            Device device = entityManager.find(Device.class, id);
            if ("Camera".equals(device.getDeviceType())) {
                return ResponseEntity.ok((Camera) device);
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
            Camera camera = "Camera".equals(device.getDeviceType()) ? (Camera) device : null;
            if (camera == null) {
                return ResponseEntity.notFound().build();
            }

            Map<String, Object> response = new HashMap<>();

            if (updates.containsKey("name")) {
                camera.setName((String) updates.get("name"));
                response.put("name", updates.get("name"));
            }
            if (updates.containsKey("manufacturer")) {
                camera.setManufacturer((String) updates.get("manufacturer"));
                response.put("manufacturer", updates.get("manufacturer"));
            }
            if (updates.containsKey("resolution")) {
                camera.setResolution(((Number) updates.get("resolution")).intValue());
                response.put("resolution", updates.get("resolution"));
            }
            if (updates.containsKey("isRecording")) {
                camera.setRecording((boolean) updates.get("isRecording"));
                response.put("isRecording", updates.get("isRecording"));
            }
            if (updates.containsKey("motionDetection")) {
                camera.setMotionDetection((boolean) updates.get("motionDetection"));
                response.put("motionDetection", updates.get("motionDetection"));
            }
            if (updates.containsKey("homeId")) {
                Long homeId = updates.get("homeId") != null ? ((Number) updates.get("homeId")).longValue() : null;
                SmartHome home = homeId != null ? smartHomeRepository.findById(homeId).orElse(null) : null;
                camera.setSmartHome(home);
                response.put("homeId", homeId);
            }

            deviceService.update(camera);
            response.put("status", "success");
            response.put("message", "Камера обновлена");
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
            Camera camera = "Camera".equals(device.getDeviceType()) ? (Camera) device : null;
            Map<String, Object> response = new HashMap<>();

            switch (methodName) {
                case "displayInfo":
                    response.put("status", "success");
                    response.put("data", camera.displayInfo());
                    break;
                case "turnOn":
                    camera.turnOn();
                    deviceService.update(camera);
                    response.put("status", "success");
                    response.put("message", "Камера включена");
                    break;
                case "turnOff":
                    camera.turnOff();
                    deviceService.update(camera);
                    response.put("status", "success");
                    response.put("message", "Камера выключена");
                    break;
                case "calculatePowerConsumption":
                    response.put("status", "success");
                    response.put("data", camera.calculatePowerConsumption());
                    break;
                case "startRecording":
                    String recordMsg = camera.startRecording();
                    deviceService.update(camera);
                    response.put("status", "success");
                    response.put("message", recordMsg);
                    break;
                case "recordMotion":
                    String motionMsg = camera.recordMotion();
                    deviceService.update(camera);
                    response.put("status", "success");
                    response.put("message", motionMsg);
                    break;
                case "getDeviceType":
                    response.put("status", "success");
                    response.put("data", camera.getDeviceType());
                    break;
                case "getShortDescription":
                    response.put("status", "success");
                    response.put("data", camera.getShortDescription());
                    break;
                case "getDetailedInfo":
                    response.put("status", "success");
                    response.put("data", camera.getDetailedInfo());
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
    public ResponseEntity<Camera> createDefaultCamera() {
        try {
            Camera camera = new Camera();
            deviceService.createCamera(camera);
            return ResponseEntity.status(HttpStatus.CREATED).body(camera);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @PostMapping("/create")
    public ResponseEntity<Camera> createCamera(@RequestBody Map<String, Object> data) {
        try {
            String name = (String) data.get("name");
            int resolution = ((Number) data.get("resolution")).intValue();
            Long homeId = data.get("homeId") != null ? ((Number) data.get("homeId")).longValue() : null;
            SmartHome home = homeId != null ? smartHomeRepository.findById(homeId).orElse(null) : null;
            Camera camera = new Camera(name, home);
            camera.setResolution(resolution);
            deviceService.createCamera(camera);
            return ResponseEntity.status(HttpStatus.CREATED).body(camera);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @PostMapping("/create-full")
    public ResponseEntity<Camera> createCameraFull(@RequestBody Map<String, Object> data) {
        try {
            String name = (String) data.get("name");
            String manufacturer = (String) data.getOrDefault("manufacturer", "HikVision");
            boolean isOn = (boolean) data.getOrDefault("isOn", false);
            int resolution = ((Number) data.get("resolution")).intValue();
            boolean isRecording = (boolean) data.getOrDefault("isRecording", false);
            boolean motionDetection = (boolean) data.getOrDefault("motionDetection", true);
            int storageUsed = data.get("storageUsed") != null ? ((Number) data.get("storageUsed")).intValue() : 0;
            String lastMotionTime = (String) data.getOrDefault("lastMotionTime", "Никогда");
            Long homeId = data.get("homeId") != null ? ((Number) data.get("homeId")).longValue() : null;
            SmartHome home = homeId != null ? smartHomeRepository.findById(homeId).orElse(null) : null;
            Camera camera = new Camera(name, manufacturer, isOn, resolution, isRecording, motionDetection, storageUsed, lastMotionTime, home);
            deviceService.createCamera(camera);
            return ResponseEntity.status(HttpStatus.CREATED).body(camera);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> deleteCamera(@PathVariable Long id) {
        try {
            Device device = entityManager.find(Device.class, id);
            Camera camera = "Camera".equals(device.getDeviceType()) ? (Camera) device : null;
            if (camera != null) {
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
