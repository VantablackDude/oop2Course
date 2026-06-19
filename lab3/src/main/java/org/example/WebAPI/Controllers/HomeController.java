package org.example.WebAPI.Controllers;

import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.ResponseBody;
import java.util.Map;

@Controller
public class HomeController {

    @GetMapping("/")
    public String home() {
        return "redirect:/index.html";
    }

    @GetMapping("/api/info")
    @ResponseBody
    public Map<String, String> apiInfo() {
        return Map.of(
                "message", "Smart Home Management System API",
                "status", "running",
                "endpoints", "/api/device, /api/camera, /api/doorlock, /api/light, /api/thermostat, /api/smarthome"
        );
    }
}
