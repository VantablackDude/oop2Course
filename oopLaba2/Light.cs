using System;

namespace oopLaba2
{
    public class Light : Device
    {
        private int _brightness;
        private string _color;
        private bool _autoMode;
        private int _scheduledTime;

        public Light() : base()
        {
            _brightness = 50;
            _color = "white";
            _autoMode = false;
            _scheduledTime = -1;
        }

        public Light(string name, string manufacturer, int brightness)
            : base(name, manufacturer)
        {
            _brightness = brightness;
            _color = "white";
            _autoMode = false;
            _scheduledTime = -1;
        }

        public Light(string name, string manufacturer, int brightness, string color, bool autoMode, int scheduledTime)
            : base(name, manufacturer)
        {
            _brightness = brightness;
            _color = color;
            _autoMode = autoMode;
            _scheduledTime = scheduledTime;
        }

        public Light(Light other) : base(other)
        {
            _brightness = other._brightness;
            _color = other._color;
            _autoMode = other._autoMode;
            _scheduledTime = other._scheduledTime;
        }

        public int Brightness
        {
            get { return _brightness; }
            set { _brightness = value < 0 ? 0 : (value > 100 ? 100 : value); }
        }

        public string Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public bool AutoMode
        {
            get { return _autoMode; }
            set { _autoMode = value; }
        }

        public int ScheduledTime
        {
            get { return _scheduledTime; }
            set { _scheduledTime = value; }
        }

        // ========== СОБСТВЕННЫЕ МЕТОДЫ (2) ==========

        public void SetBrightness(int level)
        {
            _brightness = level < 0 ? 0 : (level > 100 ? 100 : level);

            if (_brightness > 80)
            {
                Console.WriteLine("ВНИМАНИЕ: Высокая яркость потребляет много энергии! Рекомендуется снизить до 60%.");
            }
            else if (_brightness < 10)
            {
                Console.WriteLine("Яркость минимальная. Включён ночной режим.");
            }
            else if (_brightness >= 40 && _brightness <= 60)
            {
                Console.WriteLine("Оптимальная яркость для работы. Энергопотребление в норме.");
            }
            else
            {
                Console.WriteLine($"Яркость установлена: {_brightness}%");
            }
        }

        public string GetLightInfo()
        {
            string info = $"Яркость: {_brightness}%, Цвет: {_color}";

            if (_autoMode && _scheduledTime >= 0)
            {
                int currentHour = DateTime.Now.Hour;
                if (currentHour >= _scheduledTime)
                {
                    info += ". Режим: авто. Вечернее освещение активно.";
                }
                else
                {
                    info += ". Режим: авто. Ожидание вечернего расписания.";
                }
            }
            else if (_autoMode)
            {
                info += ". Режим: авто. Расписание не задано.";
            }
            else
            {
                info += ". Режим: ручной.";
            }

            if (_brightness == 0)
                info += " СВЕТ ВЫКЛЮЧЕН.";

            return info;
        }

        // ========== ПЕРЕКРЫТЫЕ МЕТОДЫ (2) ==========

        public override void TurnOn()
        {
            base.TurnOn();
            if (_brightness < 20)
            {
                _brightness = 30;
                Console.WriteLine("Яркость автоматически увеличена до 30% для комфортного освещения.");
            }
            Console.WriteLine($"Свет включен. Яркость: {_brightness}%, Цвет: {_color}");
        }

        public override string GetDetailedInfo()
        {
            string status = IsOn ? "Включен" : "Выключен";
            string auto = _autoMode ? $", Авто в {_scheduledTime}:00" : "";
            return $"Свет: {Name}, Статус: {status}, Яркость: {_brightness}%, Цвет: {_color}{auto}";
        }

        // ========== УНАСЛЕДОВАННЫЕ (2) ==========
        // TurnOff(), GetStatus()

        public override string GetDeviceType() => "Light";
        public override string GetShortDescription()
        {
            return $"Light '{Name}': {_brightness}% ({_color})";
        }
    }
}