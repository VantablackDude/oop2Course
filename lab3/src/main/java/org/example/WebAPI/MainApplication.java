package org.example.WebAPI;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.autoconfigure.domain.EntityScan;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.data.jpa.repository.config.EnableJpaRepositories;
import org.springframework.boot.CommandLineRunner;
import org.springframework.context.annotation.Bean;
import java.awt.Desktop;
import java.net.URI;

@SpringBootApplication
@ComponentScan(basePackages = {"org.example.WebAPI", "org.example.AppDataAPI", "org.example.Class"})
@EntityScan(basePackages = {"org.example.Class"})
@EnableJpaRepositories(basePackages = {"org.example.AppDataAPI"})
public class MainApplication {

    public static void main(String[] args) {
        SpringApplication.run(MainApplication.class, args);
    }

    @Bean
    public CommandLineRunner openBrowser() {
        return args -> {
            Thread.sleep(3000);

            String url = "http://localhost:8080";

            try {
                if (Desktop.isDesktopSupported()) {
                    Desktop.getDesktop().browse(new URI(url));
                    System.out.println("Браузер открыт по адресу: " + url);
                } else {
                    System.out.println("Desktop не поддерживается. Откройте вручную: " + url);
                }
            } catch (Exception e) {
                System.out.println("Откройте вручную: " + url);
            }
        };
    }
}
