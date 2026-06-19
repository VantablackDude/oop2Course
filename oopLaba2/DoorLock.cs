using System;

namespace oopLaba2
{
    public class DoorLock : Device
    {
        private bool _isLocked;
        private string _accessCode;
        private int _wrongAttempts;
        private bool _autoLock;
        private string _lastAccessTime;

        public DoorLock() : base()
        {
            _isLocked = true;
            _accessCode = "1234";
            _wrongAttempts = 0;
            _autoLock = true;
            _lastAccessTime = "Никогда";
        }

        public DoorLock(string name, string manufacturer, string accessCode)
            : base(name, manufacturer)
        {
            _isLocked = true;
            _accessCode = accessCode;
            _wrongAttempts = 0;
            _autoLock = true;
            _lastAccessTime = "Никогда";
        }

        public DoorLock(string name, string manufacturer, string accessCode, bool autoLock, bool isLocked)
            : base(name, manufacturer)
        {
            _isLocked = isLocked;
            _accessCode = accessCode;
            _wrongAttempts = 0;
            _autoLock = autoLock;
            _lastAccessTime = "Никогда";
        }

        public DoorLock(DoorLock other) : base(other)
        {
            _isLocked = other._isLocked;
            _accessCode = other._accessCode;
            _wrongAttempts = other._wrongAttempts;
            _autoLock = other._autoLock;
            _lastAccessTime = other._lastAccessTime;
        }

        public bool IsLocked
        {
            get { return _isLocked; }
            set { _isLocked = value; }
        }

        public string AccessCode
        {
            get { return _accessCode; }
            set { _accessCode = value; }
        }

        public int WrongAttempts
        {
            get { return _wrongAttempts; }
            set { _wrongAttempts = value; }
        }

        public bool AutoLock
        {
            get { return _autoLock; }
            set { _autoLock = value; }
        }

        public string LastAccessTime
        {
            get { return _lastAccessTime; }
            set { _lastAccessTime = value; }
        }

        // ========== СОБСТВЕННЫЕ МЕТОДЫ (2) ==========

        public bool TryUnlock(string code)
        {
            if (_wrongAttempts >= 3)
            {
                Console.WriteLine("ВНИМАНИЕ: Превышено количество неверных попыток! Система временно заблокирована.");
                return false;
            }

            if (code == _accessCode)
            {
                _isLocked = false;
                _wrongAttempts = 0;
                _lastAccessTime = DateTime.Now.ToString("HH:mm:ss");
                Console.WriteLine($"Доступ разрешен. Время: {_lastAccessTime}");
                return true;
            }
            else
            {
                _wrongAttempts++;
                Console.WriteLine($"Неверный код. Попытка {_wrongAttempts} из 3.");
                if (_wrongAttempts >= 3)
                {
                    Console.WriteLine("КРИТИЧЕСКИЙ СБОЙ БЕЗОПАСНОСТИ: Замок заблокирован на 30 секунд!");
                }
                return false;
            }
        }

        public string GetLockInfo()
        {
            string state = _isLocked ? "Заперт" : "Открыт";
            string security = _wrongAttempts >= 3 ? "ТРЕВОГА: Система заблокирована!" :
                             _wrongAttempts >= 1 ? $"ВНИМАНИЕ: {_wrongAttempts} неверных попытки" :
                             "Безопасность в норме";

            return $"Состояние: {state}, Попыток: {_wrongAttempts}, Последний доступ: {_lastAccessTime}. {security}";
        }

        // ========== ПЕРЕКРЫТЫЕ МЕТОДЫ (2) ==========

        public override void TurnOn()
        {
            base.TurnOn();
            if (_autoLock && !_isLocked)
            {
                _isLocked = true;
                Console.WriteLine("Автоблокировка: дверь заперта.");
            }
            Console.WriteLine($"Замок активен. Состояние: {(_isLocked ? "заперт" : "открыт")}");
        }

        public override string GetDetailedInfo()
        {
            string state = _isLocked ? "Заперт" : "Открыт";
            string auto = _autoLock ? "Да" : "Нет";
            string alert = _wrongAttempts >= 3 ? " !БЛОКИРОВКА!" : "";
            return $"Замок: {Name}, Состояние: {state}{alert}, Автоблокировка: {auto}, Доступ: {_lastAccessTime}";
        }

        // ========== УНАСЛЕДОВАННЫЕ (2) ==========
        // TurnOff(), GetStatus()

        public override string GetDeviceType() => "DoorLock";
        public override string GetShortDescription()
        {
            return $"DoorLock '{Name}': {(_isLocked ? "Заперт" : "Открыт")}";
        }
    }
}