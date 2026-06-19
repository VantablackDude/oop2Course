package org.example.AppDataAPI;

import org.example.Class.Thermostat;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface ThermostatRepository extends JpaRepository<Thermostat, Long> {
}
