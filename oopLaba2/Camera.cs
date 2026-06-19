using System;

namespace oopLaba2
{
    public class Camera : Device
    {
        private int _resolution;
        private bool _isRecording;
        private bool _motionDetection;
        private int _storageUsed;
        private string _lastMotionTime;

        public Camera() : base()
        {
            _resolution = 1080;
            _isRecording = false;
            _motionDetection = true;
            _storageUsed = 0;
            _lastMotionTime = "Никогда";
        }

        public Camera(string name, string manufacturer, int resolution)
            : base(name, manufacturer)
        {
            _resolution = resolution;
            _isRecording = false;
            _motionDetection = true;
            _storageUsed = 0;
            _lastMotionTime = "Никогда";
        }

        public Camera(string name, string manufacturer, int resolution, bool recording, bool motionDetect, int storageUsed)
            : base(name, manufacturer)
        {
            _resolution = resolution;
            _isRecording = recording;
            _motionDetection = motionDetect;
            _storageUsed = storageUsed;
            _lastMotionTime = "Никогда";
        }

        public Camera(Camera other) : base(other)
        {
            _resolution = other._resolution;
            _isRecording = other._isRecording;
            _motionDetection = other._motionDetection;
            _storageUsed = other._storageUsed;
            _lastMotionTime = other._lastMotionTime;
        }

        public int Resolution
        {
            get { return _resolution; }
            set { _resolution = value; }
        }

        public bool IsRecording
        {
            get { return _isRecording; }
            set { _isRecording = value; }
        }

        public bool MotionDetection
        {
            get { return _motionDetection; }
            set { _motionDetection = value; }
        }

        public int StorageUsed
        {
            get { return _storageUsed; }
            set { _storageUsed = value; }
        }

        public string LastMotionTime
        {
            get { return _lastMotionTime; }
            set { _lastMotionTime = value; }
        }

        // ========== СОБСТВЕННЫЕ МЕТОДЫ (2) ==========

        public string StartRecording()
        {
            if (_storageUsed >= 95)
            {
                return "ОШИБКА: Недостаточно места на накопителе! Освободите память.";
            }

            _isRecording = true;

            string msg;
            if (_resolution >= 2160)
            {
                _storageUsed += 10;
                msg = $"Запись 4K запущена. Расход памяти высокий ({_storageUsed}%).";
            }
            else if (_resolution >= 1080)
            {
                _storageUsed += 5;
                msg = $"Запись Full HD запущена. Расход памяти средний ({_storageUsed}%).";
            }
            else
            {
                _storageUsed += 2;
                msg = $"Запись HD запущена. Расход памяти низкий ({_storageUsed}%).";
            }

            return msg;
        }

        public string RecordMotion()
        {
            _lastMotionTime = DateTime.Now.ToString("HH:mm:ss");

            if (!_motionDetection)
            {
                return $"Движение зафиксировано в {_lastMotionTime}, но детекция движения отключена.";
            }

            if (!_isRecording)
            {
                return $"ВНИМАНИЕ: Движение обнаружено в {_lastMotionTime}! Запись не ведётся. Включите запись.";
            }

            _storageUsed += 1;
            return $"ДВИЖЕНИЕ ОБНАРУЖЕНО в {_lastMotionTime}. Фрагмент сохранён. Память: {_storageUsed}%.";
        }

        // ========== ПЕРЕКРЫТЫЕ МЕТОДЫ (2) ==========

        public override void TurnOn()
        {
            base.TurnOn();
            string resText = _resolution >= 2160 ? "4K" : _resolution >= 1080 ? "Full HD" : "HD";
            Console.WriteLine($"Камера {resText} готова к работе. Детекция движения: {(_motionDetection ? "вкл" : "выкл")}");
        }

        public override string GetDetailedInfo()
        {
            string rec = _isRecording ? "Записывает" : "Ожидание";
            string motion = _motionDetection ? "Вкл" : "Выкл";
            string resText = _resolution >= 2160 ? "4K" : _resolution >= 1080 ? "Full HD" : "HD";
            string storageWarn = _storageUsed >= 90 ? " !ОСТАЛОСЬ МАЛО МЕСТА!" : "";
            return $"Камера: {Name}, Режим: {rec}, Разрешение: {resText} ({_resolution}p), " +
                   $"Детекция: {motion}, Память: {_storageUsed}%{storageWarn}";
        }

        // ========== УНАСЛЕДОВАННЫЕ (2) ==========
        // TurnOff(), GetStatus()

        public override string GetDeviceType() => "Camera";
        public override string GetShortDescription()
        {
            return $"Camera '{Name}': {_resolution}p {(_isRecording ? "запись" : "ожидание")}";
        }
    }
}