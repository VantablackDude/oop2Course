package org.example.WebAPI.Controllers;

import java.util.*;

import org.example.AppDataAPI.DeviceService;
import org.example.AppDataAPI.SmartHomeRepository;
import org.example.Class.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.bind.annotation.*;

@RestController
@Transactional
@RequestMapping("/api/smarthome")
public class SmartHomeController {

    @Autowired
    private SmartHomeRepository smartHomeRepository;

    @Autowired
    private DeviceService deviceService;

    @GetMapping("/all")
    public ResponseEntity<List<SmartHome>> getAllHomes() {
        try {
            List<SmartHome> homes = smartHomeRepository.findAll();
            System.out.println("Возвращено домов: " + homes.size());
            return ResponseEntity.ok(homes);
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @GetMapping("/{id}")
    public ResponseEntity<SmartHome> getHomeById(@PathVariable Long id) {
        try {
            SmartHome home = smartHomeRepository.findById(id).orElse(null);
            if (home != null) {
                System.out.println("Возвращен дом: " + home.getName());
                return ResponseEntity.ok(home);
            } else {
                return ResponseEntity.notFound().build();
            }
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @PostMapping("/create")
    public ResponseEntity<SmartHome> createHome(@RequestParam String name) {
        try {
            SmartHome home = new SmartHome(name);
            smartHomeRepository.save(home);
            System.out.println("Создан дом: " + home.getName() + " с ID: " + home.getId());
            return ResponseEntity.status(HttpStatus.CREATED).body(home);
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @PostMapping("/create-full")
    public ResponseEntity<SmartHome> createHomeFull(@RequestBody Map<String, Object> data) {
        try {
            String name = (String) data.get("name");
            String address = (String) data.getOrDefault("address", "");
            SmartHome home = new SmartHome(name, address);
            smartHomeRepository.save(home);
            return ResponseEntity.status(HttpStatus.CREATED).body(home);
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @PutMapping("/{id}")
    public ResponseEntity<SmartHome> updateHome(@PathVariable Long id, @RequestParam String name) {
        try {
            SmartHome home = deviceService.updateSmartHome(id, name);
            if (home != null) {
                System.out.println("Обновлен дом с ID: " + id + ", новое имя: " + name);
                return ResponseEntity.ok(home);
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
            SmartHome home = smartHomeRepository.findById(id).orElse(null);
            if (home == null) {
                return ResponseEntity.notFound().build();
            }

            Map<String, Object> response = new HashMap<>();
            if (updates.containsKey("name")) {
                home.setName((String) updates.get("name"));
                response.put("name", updates.get("name"));
            }
            if (updates.containsKey("address")) {
                home.setAddress((String) updates.get("address"));
                response.put("address", updates.get("address"));
            }

            smartHomeRepository.save(home);
            response.put("status", "success");
            response.put("message", "Дом обновлен");
            return ResponseEntity.ok(response);
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> deleteHome(@PathVariable Long id) {
        try {
            SmartHome home = smartHomeRepository.findById(id).orElse(null);
            if (home != null) {
                smartHomeRepository.deleteById(id);
                System.out.println("Удален дом с ID: " + id);
                return ResponseEntity.noContent().build();
            } else {
                return ResponseEntity.notFound().build();
            }
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @GetMapping("/{homeId}/devices")
    public ResponseEntity<List<Map<String, Object>>> getHomeDevices(@PathVariable Long homeId) {
        try {
            SmartHome home = smartHomeRepository.findById(homeId).orElse(null);
            if (home != null) {
                List<Map<String, Object>> result = home.getDevicesWithDetails();
                System.out.println("Возвращено устройств из дома " + homeId + ": " + result.size());
                return ResponseEntity.ok(result);
            } else {
                return ResponseEntity.ok(new ArrayList<>());
            }
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @PostMapping("/{homeId}/add-device/{deviceId}")
    public ResponseEntity<Void> addDeviceToHome(@PathVariable Long homeId, @PathVariable Long deviceId) {
        try {
            deviceService.addExistingDeviceToHome(homeId, deviceId);
            System.out.println("Устройство " + deviceId + " добавлено в дом " + homeId);
            return ResponseEntity.ok().build();
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }

    @DeleteMapping("/{homeId}/remove-device/{deviceId}")
    public ResponseEntity<Void> removeDeviceFromHome(@PathVariable Long homeId, @PathVariable Long deviceId) {
        try {
            deviceService.removeDeviceFromHome(homeId, deviceId);
            System.out.println("Устройство " + deviceId + " удалено из дома " + homeId);
            return ResponseEntity.ok().build();
        } catch (Exception e) {
            e.printStackTrace();
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).build();
        }
    }
}
