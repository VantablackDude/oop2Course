package org.example.AppDataAPI;

import org.example.Class.DoorLock;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface DoorLockRepository extends JpaRepository<DoorLock, Long> {
}
