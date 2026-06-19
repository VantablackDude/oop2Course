using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace oopLaba2
{
    /// <summary>
    /// Класс для работы с базой данных SQLite
    /// </summary>
    public class DatabaseHelper
    {
        private static string _connectionString;
        private static bool _initialized = false;

        /// <summary>
        /// Инициализация базы данных
        /// </summary>
        public static void Initialize(string dbPath = "smart_home.db")
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbPath);
            _connectionString = $"Data Source={fullPath};Version=3;";
            _initialized = true;
            CreateTables();
            Console.WriteLine($"База данных инициализирована: {fullPath}");
        }

        /// <summary>
        /// Получить строку подключения
        /// </summary>
        public static string ConnectionString
        {
            get { return _connectionString; }
        }

        /// <summary>
        /// Проверить инициализацию
        /// </summary>
        public static bool IsInitialized
        {
            get { return _initialized; }
        }

        /// <summary>
        /// Создание всех таблиц
        /// </summary>
        private static void CreateTables()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();

                // Таблица Device - общие данные
                string createDevices = @"
                    CREATE TABLE IF NOT EXISTS Devices (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Manufacturer TEXT NOT NULL,
                        IsOn INTEGER NOT NULL DEFAULT 0,
                        DeviceType TEXT NOT NULL,
                        CreatedAt TEXT NOT NULL
                    )";

                // Таблица Thermostats
                string createThermostats = @"
                    CREATE TABLE IF NOT EXISTS Thermostats (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        DeviceId INTEGER NOT NULL,
                        CurrentTemperature REAL NOT NULL DEFAULT 20.0,
                        TargetTemperature REAL NOT NULL DEFAULT 22.0,
                        Mode TEXT NOT NULL DEFAULT 'auto',
                        Humidity REAL NOT NULL DEFAULT 50.0,
                        FOREIGN KEY (DeviceId) REFERENCES Devices(Id) ON DELETE CASCADE
                    )";

                // Таблица Lights
                string createLights = @"
                    CREATE TABLE IF NOT EXISTS Lights (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        DeviceId INTEGER NOT NULL,
                        Brightness INTEGER NOT NULL DEFAULT 50,
                        Color TEXT NOT NULL DEFAULT 'white',
                        AutoMode INTEGER NOT NULL DEFAULT 0,
                        ScheduledTime INTEGER NOT NULL DEFAULT -1,
                        FOREIGN KEY (DeviceId) REFERENCES Devices(Id) ON DELETE CASCADE
                    )";

                // Таблица DoorLocks
                string createDoorLocks = @"
                    CREATE TABLE IF NOT EXISTS DoorLocks (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        DeviceId INTEGER NOT NULL,
                        IsLocked INTEGER NOT NULL DEFAULT 1,
                        AccessCode TEXT NOT NULL DEFAULT '1234',
                        WrongAttempts INTEGER NOT NULL DEFAULT 0,
                        AutoLock INTEGER NOT NULL DEFAULT 1,
                        LastAccessTime TEXT,
                        FOREIGN KEY (DeviceId) REFERENCES Devices(Id) ON DELETE CASCADE
                    )";

                // Таблица Cameras
                string createCameras = @"
                    CREATE TABLE IF NOT EXISTS Cameras (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        DeviceId INTEGER NOT NULL,
                        Resolution INTEGER NOT NULL DEFAULT 1080,
                        IsRecording INTEGER NOT NULL DEFAULT 0,
                        MotionDetection INTEGER NOT NULL DEFAULT 1,
                        StorageUsed INTEGER NOT NULL DEFAULT 0,
                        LastMotionTime TEXT,
                        FOREIGN KEY (DeviceId) REFERENCES Devices(Id) ON DELETE CASCADE
                    )";

                using (var cmd = new SQLiteCommand(createDevices, conn))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SQLiteCommand(createThermostats, conn))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SQLiteCommand(createLights, conn))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SQLiteCommand(createDoorLocks, conn))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SQLiteCommand(createCameras, conn))
                    cmd.ExecuteNonQuery();
            }
        }

        #region Device CRUD

        /// <summary>
        /// Добавить устройство в БД
        /// </summary>
        public static int InsertDevice(Device device)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Devices (Name, Manufacturer, IsOn, DeviceType, CreatedAt) 
                             VALUES (@Name, @Manufacturer, @IsOn, @DeviceType, @CreatedAt);
                             SELECT last_insert_rowid();";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", device.Name);
                    cmd.Parameters.AddWithValue("@Manufacturer", device.Manufacturer);
                    cmd.Parameters.AddWithValue("@IsOn", device.IsOn ? 1 : 0);
                    cmd.Parameters.AddWithValue("@DeviceType", device.GetDeviceType());
                    cmd.Parameters.AddWithValue("@CreatedAt", device.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                    
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Обновить устройство
        /// </summary>
        public static void UpdateDevice(Device device, int id)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"UPDATE Devices SET Name = @Name, Manufacturer = @Manufacturer, 
                             IsOn = @IsOn WHERE Id = @Id";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Name", device.Name);
                    cmd.Parameters.AddWithValue("@Manufacturer", device.Manufacturer);
                    cmd.Parameters.AddWithValue("@IsOn", device.IsOn ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Удалить устройство и связанные данные
        /// </summary>
        public static void DeleteDevice(int id)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                
                // Сначала получаем тип устройства
                string typeSql = "SELECT DeviceType FROM Devices WHERE Id = @Id";
                string deviceType = null;
                
                using (var cmd = new SQLiteCommand(typeSql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                        deviceType = result.ToString();
                }
                
                // Удаляем из связанной таблицы
                if (deviceType != null)
                {
                    string deleteRelated = "";
                    switch (deviceType)
                    {
                        case "Thermostat":
                            deleteRelated = "DELETE FROM Thermostats WHERE DeviceId = @Id";
                            break;
                        case "Light":
                            deleteRelated = "DELETE FROM Lights WHERE DeviceId = @Id";
                            break;
                        case "DoorLock":
                            deleteRelated = "DELETE FROM DoorLocks WHERE DeviceId = @Id";
                            break;
                        case "Camera":
                            deleteRelated = "DELETE FROM Cameras WHERE DeviceId = @Id";
                            break;
                    }
                    
                    if (!string.IsNullOrEmpty(deleteRelated))
                    {
                        using (var cmd = new SQLiteCommand(deleteRelated, conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                
                // Удаляем из Devices
                using (var cmd = new SQLiteCommand("DELETE FROM Devices WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
                
                // Сбрасываем AUTOINCREMENT если таблица пуста
                CheckAndResetAutoIncrement(conn);
            }
        }
        
        /// <summary>
        /// Проверить и сбросить AUTOINCREMENT если нужно
        /// </summary>
        private static void CheckAndResetAutoIncrement(SQLiteConnection conn)
        {
            using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Devices", conn))
            {
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count == 0)
                {
                    using (var resetCmd = new SQLiteCommand("DELETE FROM sqlite_sequence WHERE name = 'Devices'", conn))
                        resetCmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Получить все устройства
        /// </summary>
        public static DataTable GetAllDevices()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Devices ORDER BY Id";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var adapter = new SQLiteDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Получить устройство по ID
        /// </summary>
        public static Dictionary<string, object> GetDeviceById(int id)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Devices WHERE Id = @Id";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var result = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result[reader.GetName(i)] = reader.GetValue(i);
                            }
                            return result;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Включить/выключить устройство
        /// </summary>
        public static void SetDeviceState(int id, bool isOn)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "UPDATE Devices SET IsOn = @IsOn WHERE Id = @Id";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@IsOn", isOn ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Thermostat CRUD

        /// <summary>
        /// Добавить термостат
        /// </summary>
        public static void InsertThermostat(Thermostat thermostat, int deviceId)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Thermostats 
                             (DeviceId, CurrentTemperature, TargetTemperature, Mode, Humidity) 
                             VALUES (@DeviceId, @CurrentTemperature, @TargetTemperature, @Mode, @Humidity)";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DeviceId", deviceId);
                    cmd.Parameters.AddWithValue("@CurrentTemperature", thermostat.CurrentTemperature);
                    cmd.Parameters.AddWithValue("@TargetTemperature", thermostat.TargetTemperature);
                    cmd.Parameters.AddWithValue("@Mode", thermostat.Mode);
                    cmd.Parameters.AddWithValue("@Humidity", thermostat.Humidity);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Обновить термостат
        /// </summary>
        public static void UpdateThermostat(Thermostat thermostat, int id)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"UPDATE Thermostats SET 
                             CurrentTemperature = @CurrentTemperature, 
                             TargetTemperature = @TargetTemperature, 
                             Mode = @Mode, 
                             Humidity = @Humidity 
                             WHERE Id = @Id";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@CurrentTemperature", thermostat.CurrentTemperature);
                    cmd.Parameters.AddWithValue("@TargetTemperature", thermostat.TargetTemperature);
                    cmd.Parameters.AddWithValue("@Mode", thermostat.Mode);
                    cmd.Parameters.AddWithValue("@Humidity", thermostat.Humidity);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Получить данные термостата по DeviceId
        /// </summary>
        public static Dictionary<string, object> GetThermostatByDeviceId(int deviceId)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Thermostats WHERE DeviceId = @DeviceId";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DeviceId", deviceId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var result = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result[reader.GetName(i)] = reader.GetValue(i);
                            }
                            return result;
                        }
                    }
                }
            }
            return null;
        }

        #endregion

        #region Light CRUD

        /// <summary>
        /// Добавить свет
        /// </summary>
        public static void InsertLight(Light light, int deviceId)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Lights 
                             (DeviceId, Brightness, Color, AutoMode, ScheduledTime) 
                             VALUES (@DeviceId, @Brightness, @Color, @AutoMode, @ScheduledTime)";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DeviceId", deviceId);
                    cmd.Parameters.AddWithValue("@Brightness", light.Brightness);
                    cmd.Parameters.AddWithValue("@Color", light.Color);
                    cmd.Parameters.AddWithValue("@AutoMode", light.AutoMode ? 1 : 0);
                    cmd.Parameters.AddWithValue("@ScheduledTime", light.ScheduledTime);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Обновить свет
        /// </summary>
        public static void UpdateLight(Light light, int id)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"UPDATE Lights SET 
                             Brightness = @Brightness, 
                             Color = @Color, 
                             AutoMode = @AutoMode, 
                             ScheduledTime = @ScheduledTime 
                             WHERE Id = @Id";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Brightness", light.Brightness);
                    cmd.Parameters.AddWithValue("@Color", light.Color);
                    cmd.Parameters.AddWithValue("@AutoMode", light.AutoMode ? 1 : 0);
                    cmd.Parameters.AddWithValue("@ScheduledTime", light.ScheduledTime);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Получить данные света по DeviceId
        /// </summary>
        public static Dictionary<string, object> GetLightByDeviceId(int deviceId)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Lights WHERE DeviceId = @DeviceId";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DeviceId", deviceId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var result = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result[reader.GetName(i)] = reader.GetValue(i);
                            }
                            return result;
                        }
                    }
                }
            }
            return null;
        }

        #endregion

        #region DoorLock CRUD

        /// <summary>
        /// Добавить замок
        /// </summary>
        public static void InsertDoorLock(DoorLock doorLock, int deviceId)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO DoorLocks 
                             (DeviceId, IsLocked, AccessCode, WrongAttempts, AutoLock, LastAccessTime) 
                             VALUES (@DeviceId, @IsLocked, @AccessCode, @WrongAttempts, @AutoLock, @LastAccessTime)";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DeviceId", deviceId);
                    cmd.Parameters.AddWithValue("@IsLocked", doorLock.IsLocked ? 1 : 0);
                    cmd.Parameters.AddWithValue("@AccessCode", doorLock.AccessCode);
                    cmd.Parameters.AddWithValue("@WrongAttempts", doorLock.WrongAttempts);
                    cmd.Parameters.AddWithValue("@AutoLock", doorLock.AutoLock ? 1 : 0);
                    cmd.Parameters.AddWithValue("@LastAccessTime", doorLock.LastAccessTime);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Обновить замок
        /// </summary>
        public static void UpdateDoorLock(DoorLock doorLock, int id)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"UPDATE DoorLocks SET 
                             IsLocked = @IsLocked, 
                             AccessCode = @AccessCode, 
                             WrongAttempts = @WrongAttempts, 
                             AutoLock = @AutoLock, 
                             LastAccessTime = @LastAccessTime 
                             WHERE Id = @Id";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@IsLocked", doorLock.IsLocked ? 1 : 0);
                    cmd.Parameters.AddWithValue("@AccessCode", doorLock.AccessCode);
                    cmd.Parameters.AddWithValue("@WrongAttempts", doorLock.WrongAttempts);
                    cmd.Parameters.AddWithValue("@AutoLock", doorLock.AutoLock ? 1 : 0);
                    cmd.Parameters.AddWithValue("@LastAccessTime", doorLock.LastAccessTime);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Получить данные замка по DeviceId
        /// </summary>
        public static Dictionary<string, object> GetDoorLockByDeviceId(int deviceId)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM DoorLocks WHERE DeviceId = @DeviceId";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DeviceId", deviceId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var result = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result[reader.GetName(i)] = reader.GetValue(i);
                            }
                            return result;
                        }
                    }
                }
            }
            return null;
        }

        #endregion

        #region Camera CRUD

        /// <summary>
        /// Добавить камеру
        /// </summary>
        public static void InsertCamera(Camera camera, int deviceId)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Cameras 
                             (DeviceId, Resolution, IsRecording, MotionDetection, StorageUsed, LastMotionTime) 
                             VALUES (@DeviceId, @Resolution, @IsRecording, @MotionDetection, @StorageUsed, @LastMotionTime)";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DeviceId", deviceId);
                    cmd.Parameters.AddWithValue("@Resolution", camera.Resolution);
                    cmd.Parameters.AddWithValue("@IsRecording", camera.IsRecording ? 1 : 0);
                    cmd.Parameters.AddWithValue("@MotionDetection", camera.MotionDetection ? 1 : 0);
                    cmd.Parameters.AddWithValue("@StorageUsed", camera.StorageUsed);
                    cmd.Parameters.AddWithValue("@LastMotionTime", camera.LastMotionTime);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Обновить камеру
        /// </summary>
        public static void UpdateCamera(Camera camera, int id)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"UPDATE Cameras SET 
                             Resolution = @Resolution, 
                             IsRecording = @IsRecording, 
                             MotionDetection = @MotionDetection, 
                             StorageUsed = @StorageUsed, 
                             LastMotionTime = @LastMotionTime 
                             WHERE Id = @Id";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Resolution", camera.Resolution);
                    cmd.Parameters.AddWithValue("@IsRecording", camera.IsRecording ? 1 : 0);
                    cmd.Parameters.AddWithValue("@MotionDetection", camera.MotionDetection ? 1 : 0);
                    cmd.Parameters.AddWithValue("@StorageUsed", camera.StorageUsed);
                    cmd.Parameters.AddWithValue("@LastMotionTime", camera.LastMotionTime);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Получить данные камеры по DeviceId
        /// </summary>
        public static Dictionary<string, object> GetCameraByDeviceId(int deviceId)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Cameras WHERE DeviceId = @DeviceId";
                
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@DeviceId", deviceId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var result = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result[reader.GetName(i)] = reader.GetValue(i);
                            }
                            return result;
                        }
                    }
                }
            }
            return null;
        }

        #endregion

        #region Utility

        /// <summary>
        /// Очистить все данные
        /// </summary>
        public static void ClearAllData()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                // Удаляем данные из всех таблиц
                using (var cmd = new SQLiteCommand("DELETE FROM Cameras; DELETE FROM DoorLocks; DELETE FROM Lights; DELETE FROM Thermostats; DELETE FROM Devices;", conn))
                    cmd.ExecuteNonQuery();
                
                // Сбрасываем AUTOINCREMENT
                using (var cmd = new SQLiteCommand("DELETE FROM sqlite_sequence WHERE name IN ('Cameras', 'DoorLocks', 'Lights', 'Thermostats', 'Devices');", conn))
                    cmd.ExecuteNonQuery();
            }
        }
        
        /// <summary>
        /// Сбросить ID устройств
        /// </summary>
        public static void ResetAutoIncrement()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM sqlite_sequence WHERE name IN ('Cameras', 'DoorLocks', 'Lights', 'Thermostats', 'Devices');", conn))
                    cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Получить количество устройств
        /// </summary>
        public static int GetDeviceCount()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Devices", conn))
                    return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Создать объект класса из данных БД
        /// </summary>
        public static Device CreateDeviceObject(int deviceId)
        {
            var deviceRow = GetDeviceById(deviceId);
            if (deviceRow == null) return null;

            string type = deviceRow["DeviceType"].ToString();
            string name = deviceRow["Name"].ToString();
            string manufacturer = deviceRow["Manufacturer"].ToString();
            bool isOn = Convert.ToInt32(deviceRow["IsOn"]) == 1;

            Device device = null;

            switch (type)
            {
                case "Thermostat":
                    var thermData = GetThermostatByDeviceId(deviceId);
                    if (thermData != null)
                    {
                        device = new Thermostat(
                            name,
                            manufacturer,
                            Convert.ToDouble(thermData["TargetTemperature"]),
                            Convert.ToDouble(thermData["CurrentTemperature"]),
                            thermData["Mode"].ToString(),
                            Convert.ToDouble(thermData["Humidity"])
                        );
                    }
                    break;

                case "Light":
                    var lightData = GetLightByDeviceId(deviceId);
                    if (lightData != null)
                    {
                        device = new Light(
                            name,
                            manufacturer,
                            Convert.ToInt32(lightData["Brightness"]),
                            lightData["Color"].ToString(),
                            Convert.ToInt32(lightData["AutoMode"]) == 1,
                            Convert.ToInt32(lightData["ScheduledTime"])
                        );
                    }
                    break;

                case "DoorLock":
                    var doorData = GetDoorLockByDeviceId(deviceId);
                    if (doorData != null)
                    {
                        device = new DoorLock(
                            name,
                            manufacturer,
                            doorData["AccessCode"].ToString(),
                            Convert.ToInt32(doorData["AutoLock"]) == 1,
                            Convert.ToInt32(doorData["IsLocked"]) == 1
                        );
                    }
                    break;

                case "Camera":
                    var camData = GetCameraByDeviceId(deviceId);
                    if (camData != null)
                    {
                        device = new Camera(
                            name,
                            manufacturer,
                            Convert.ToInt32(camData["Resolution"]),
                            Convert.ToInt32(camData["IsRecording"]) == 1,
                            Convert.ToInt32(camData["MotionDetection"]) == 1,
                            Convert.ToInt32(camData["StorageUsed"])
                        );
                    }
                    break;
            }

            if (device != null)
                device.IsOn = isOn;

            return device;
        }

        /// <summary>
        /// Сохранить объект обратно в БД
        /// </summary>
        public static void SaveDeviceObject(Device device, int deviceId)
        {
            UpdateDevice(device, deviceId);

            switch (device.GetDeviceType())
            {
                case "Thermostat":
                    var thermRow = GetThermostatByDeviceId(deviceId);
                    if (thermRow != null)
                        UpdateThermostat((Thermostat)device, Convert.ToInt32(thermRow["Id"]));
                    break;

                case "Light":
                    var lightRow = GetLightByDeviceId(deviceId);
                    if (lightRow != null)
                        UpdateLight((Light)device, Convert.ToInt32(lightRow["Id"]));
                    break;

                case "DoorLock":
                    var doorRow = GetDoorLockByDeviceId(deviceId);
                    if (doorRow != null)
                        UpdateDoorLock((DoorLock)device, Convert.ToInt32(doorRow["Id"]));
                    break;

                case "Camera":
                    var camRow = GetCameraByDeviceId(deviceId);
                    if (camRow != null)
                        UpdateCamera((Camera)device, Convert.ToInt32(camRow["Id"]));
                    break;
            }
        }

        #endregion
    }
}