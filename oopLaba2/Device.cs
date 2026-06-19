using System;

namespace oopLaba2
{
    /// <summary>
    /// Базовый абстрактный класс для всех устройств умного дома
    /// </summary>
    public abstract class Device
    {
        // Собственные поля базового класса
        private string _name;
        private string _manufacturer;
        private bool _isOn;
        private DateTime _createdAt;

        // Конструктор по умолчанию
        protected Device()
        {
            _name = "fdsafawe";
            _manufacturer = "fdaswefda";
            _isOn = false;
            _createdAt = DateTime.Now;
        }

        // Конструктор с параметрами
        protected Device(string name, string manufacturer)
        {
            _name = name;
            _manufacturer = manufacturer;
            _isOn = false;
            _createdAt = DateTime.Now;
        }

        // Конструктор копирования
        protected Device(Device other)
        {
            _name = other._name;
            _manufacturer = other._manufacturer;
            _isOn = other._isOn;
            _createdAt = other._createdAt;
        }

        // Геттеры/Сеттеры для доступа к полям
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Manufacturer
        {
            get { return _manufacturer; }
            set { _manufacturer = value; }
        }

        public bool IsOn
        {
            get { return _isOn; }
            set { _isOn = value; }
        }

        public DateTime CreatedAt
        {
            get { return _createdAt; }
            set { _createdAt = value; }
        }

        // Собственные методы базового класса
        public virtual void TurnOn()
        {
            _isOn = true;
            Console.WriteLine($"Устройство {_name} включено");
        }

        public virtual void TurnOff()
        {
            _isOn = false;
            Console.WriteLine($"Устройство {_name} выключено");
        }

        public virtual string GetStatus()
        {
            return $"Устройство: {_name}, Производитель: {_manufacturer}, Состояние: {(_isOn ? "Включено" : "Выключено")}, Создано: {_createdAt}";
        }

        // Перекрытые методы - виртуальные для полиморфизма
        public virtual string GetDeviceType()
        {
            return "Device";
        }

        public virtual string GetDetailedInfo()
        {
            return GetStatus();
        }

        // Абстрактный метод для обязательной реализации в дочерних классах
        public abstract string GetShortDescription();
    }
}