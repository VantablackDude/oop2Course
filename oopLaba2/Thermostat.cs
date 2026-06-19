using System;

namespace oopLaba2
{
    public class Thermostat : Device
    {
        private double _currentTemperature;
        private double _targetTemperature;
        private string _mode;
        private double _humidity;

        public Thermostat() : base()
        {
            _currentTemperature = 20.0;
            _targetTemperature = 22.0;
            _mode = "auto";
            _humidity = 50.0;
        }

        public Thermostat(string name, string manufacturer, double targetTemp)
            : base(name, manufacturer)
        {
            _currentTemperature = 20.0;
            _targetTemperature = targetTemp;
            _mode = "auto";
            _humidity = 50.0;
        }

        public Thermostat(string name, string manufacturer, double targetTemp, double currentTemp, string mode, double humidity)
            : base(name, manufacturer)
        {
            _currentTemperature = currentTemp;
            _targetTemperature = targetTemp;
            _mode = mode;
            _humidity = humidity;
        }

        public Thermostat(Thermostat other) : base(other)
        {
            _currentTemperature = other._currentTemperature;
            _targetTemperature = other._targetTemperature;
            _mode = other._mode;
            _humidity = other._humidity;
        }

        public double CurrentTemperature
        {
            get { return _currentTemperature; }
            set { _currentTemperature = value; }
        }

        public double TargetTemperature
        {
            get { return _targetTemperature; }
            set { _targetTemperature = value; }
        }

        public string Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public double Humidity
        {
            get { return _humidity; }
            set { _humidity = value; }
        }

        // ========== СОБСТВЕННЫЕ МЕТОДЫ (2) ==========

        public void SetTemperature(double temp)
        {
            _targetTemperature = temp;

            if (temp < 15)
            {
                Console.WriteLine("Температура слишком низкая! Включаем обогрев до 20°C");
                _targetTemperature = 20;
                _mode = "heat";
            }
            else if (temp > 30)
            {
                Console.WriteLine("Температура слишком высокая! Включаем охлаждение до 25°C");
                _targetTemperature = 25;
                _mode = "cool";
            }
            else
            {
                Console.WriteLine($"Целевая температура установлена: {_targetTemperature}°C");
            }
        }

        public string GetClimateInfo()
        {
            string warning = "";

            if (_humidity > 70)
                warning = "ВНИМАНИЕ: Влажность превышает норму! Рекомендуется включить осушитель.";
            else if (_humidity < 20)
                warning = "ВНИМАНИЕ: Воздух слишком сухой! Рекомендуется увлажнитель.";
            else if (_mode == "heat" && _currentTemperature < _targetTemperature - 2)
                warning = "Обогрев активен: температура растёт.";
            else if (_mode == "cool" && _currentTemperature > _targetTemperature + 2)
                warning = "Охлаждение активно: температура снижается.";
            else
                warning = "Микроклимат в норме.";

            return $"Температура: {_currentTemperature:F1}°C, Цель: {_targetTemperature}°C, Режим: {_mode}, Влажность: {_humidity:F0}%. {warning}";
        }

        // ========== ПЕРЕКРЫТЫЕ МЕТОДЫ (2) ==========

        public override void TurnOn()
        {
            base.TurnOn();
            _currentTemperature = _targetTemperature - 1;
            string modeText = _mode == "heat" ? "обогрев" : _mode == "cool" ? "охлаждение" : "авто";
            Console.WriteLine($"Термостат включен. Начинаем {modeText} до {_targetTemperature}°C");
        }

        public override string GetDetailedInfo()
        {
            string status = IsOn ? "Включен" : "Выключен";
            string modeText = _mode == "heat" ? "Обогрев" : _mode == "cool" ? "Охлаждение" : "Авто";
            return $"Термостат: {Name}, Производитель: {Manufacturer}, Статус: {status}, " +
                   $"Режим: {modeText}, Цель: {_targetTemperature}°C, Текущая: {_currentTemperature:F1}°C";
        }

        // ========== УНАСЛЕДОВАННЫЕ МЕТОДЫ (2) ==========
        // TurnOff() — из Device
        // GetStatus() — из Device

        public override string GetDeviceType() => "Thermostat";
        public override string GetShortDescription()
        {
            return $"Thermostat '{Name}': {_currentTemperature:F1}°C → {_targetTemperature}°C";
        }
    }
}