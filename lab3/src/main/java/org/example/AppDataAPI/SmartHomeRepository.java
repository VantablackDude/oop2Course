package org.example.AppDataAPI;

import org.example.Class.SmartHome;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import java.util.List;

@Repository
public interface SmartHomeRepository extends JpaRepository<SmartHome, Long> {
}
