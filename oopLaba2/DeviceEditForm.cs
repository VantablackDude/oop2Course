using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using oopLaba2;

namespace oopLaba2
{
    /// <summary>
    /// Форма для добавления/редактирования устройства
    /// </summary>
    public class DeviceEditForm : Form
    {
        private int _deviceId = -1;
        private bool _isEdit = false;
        
        private ComboBox cmbType;
        private TextBox txtName;
        private TextBox txtManufacturer;
        private ComboBox cmbIsOn;
        
        // Thermostat fields
        private TextBox txtTargetTemp;
        private TextBox txtCurrentTemp;
        private ComboBox cmbMode;
        private TextBox txtHumidity;
        
        // Light fields
        private TextBox txtBrightness;
        private TextBox txtColor;
        private CheckBox chkAutoMode;
        
        // DoorLock fields
        private TextBox txtAccessCode;
        private CheckBox chkAutoLock;
        private CheckBox chkIsLocked;
        
        // Camera fields
        private ComboBox cmbResolution;
        private CheckBox chkRecording;
        private CheckBox chkMotionDetection;
        
        // Constructor selection
        private Panel panelConstructor;
        private RadioButton rbSimple;
        private RadioButton rbFull;
        
        // Labels for "extra" fields (to show/hide)
        private Label lblCurrentTemp;
        private Label lblMode;
        private Label lblHumidity;
        private Label lblColor;
        private Label lblAutoMode;
        private Label lblDoorAutoLock;
        private Label lblDoorIsLocked;
        private Label lblRecording;
        private Label lblMotion;
        
        private Panel panelThermostat;
        private Panel panelLight;
        private Panel panelDoorLock;
        private Panel panelCamera;
        
        private Button btnSave;
        private Button btnCancel;
        
        public DeviceEditForm() : this(-1) { }
        
        public DeviceEditForm(int deviceId)
        {
            _deviceId = deviceId;
            _isEdit = deviceId > 0;
            InitializeComponent();
            if (_isEdit) LoadDeviceData();
        }

        private void InitializeComponent()
        {
            this.Text = _isEdit ? "Редактирование устройства" : "Добавление устройства";
            this.Size = new Size(500, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 245);

            int yPos = 20;
            
            // Заголовок
            var lblTitle = new Label
            {
                Text = _isEdit ? "Редактирование устройства" : "Добавление нового устройства",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 60, 100),
                Location = new Point(20, yPos),
                Size = new Size(400, 30)
            };
            this.Controls.Add(lblTitle);
            yPos += 45;

            // Тип устройства
            var lblType = new Label { Text = "Тип устройства:", Location = new Point(20, yPos), Size = new Size(150, 20) };
            this.Controls.Add(lblType);
            
            cmbType = new ComboBox
            {
                Location = new Point(180, yPos - 3),
                Size = new Size(280, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = !_isEdit
            };
            cmbType.Items.AddRange(new string[] { "Thermostat", "Light", "DoorLock", "Camera" });
            cmbType.SelectedIndex = 0;
            cmbType.SelectedIndexChanged += CmbType_SelectedIndexChanged;
            this.Controls.Add(cmbType);
            yPos += 35;

            // Выбор конструктора (только при добавлении)
            panelConstructor = new Panel { Location = new Point(20, yPos), Size = new Size(440, 30), Visible = !_isEdit };
            
            var lblConstructor = new Label { Text = "Конструктор:", Location = new Point(0, 5), Size = new Size(100, 20) };
            panelConstructor.Controls.Add(lblConstructor);
            
            rbSimple = new RadioButton
            {
                Text = "Простой",
                Location = new Point(100, 3),
                Size = new Size(90, 25),
                
            };
            rbSimple.CheckedChanged += RbConstructor_CheckedChanged;
            panelConstructor.Controls.Add(rbSimple);
            
            rbFull = new RadioButton
            {
                Text = "Расширенный",
                Location = new Point(200, 3),
                Size = new Size(120, 25),
                Checked = true
            };
            rbFull.CheckedChanged += RbConstructor_CheckedChanged;
            panelConstructor.Controls.Add(rbFull);
            
            this.Controls.Add(panelConstructor);
            yPos += 35;

            // Название
            var lblName = new Label { Text = "Название:", Location = new Point(20, yPos), Size = new Size(150, 20) };
            this.Controls.Add(lblName);
            
            txtName = new TextBox { Location = new Point(180, yPos - 3), Size = new Size(280, 25) };
            this.Controls.Add(txtName);
            yPos += 35;

            // Производитель
            var lblManufacturer = new Label { Text = "Производитель:", Location = new Point(20, yPos), Size = new Size(150, 20) };
            this.Controls.Add(lblManufacturer);
            
            txtManufacturer = new TextBox { Location = new Point(180, yPos - 3), Size = new Size(280, 25) };
            this.Controls.Add(txtManufacturer);
            yPos += 35;

            // Состояние
            var lblIsOn = new Label { Text = "Состояние:", Location = new Point(20, yPos), Size = new Size(150, 20) };
            this.Controls.Add(lblIsOn);
            
            cmbIsOn = new ComboBox
            {
                Location = new Point(180, yPos - 3),
                Size = new Size(280, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbIsOn.Items.AddRange(new string[] { "Выключено", "Включено" });
            cmbIsOn.SelectedIndex = 0;
            this.Controls.Add(cmbIsOn);
            yPos += 35;

            // === Панель Thermostat ===
            panelThermostat = new Panel { Location = new Point(20, yPos), Size = new Size(440, 150), Visible = true };
            
            var lblTargetTemp = new Label { Text = "Целевая температура:", Location = new Point(0, 0), Size = new Size(180, 20) };
            panelThermostat.Controls.Add(lblTargetTemp);
            txtTargetTemp = new TextBox { Location = new Point(190, -3), Size = new Size(100, 25), Text = "22" };
            panelThermostat.Controls.Add(txtTargetTemp);
            
            lblCurrentTemp = new Label { Text = "Текущая температура:", Location = new Point(0, 35), Size = new Size(180, 20) };
            panelThermostat.Controls.Add(lblCurrentTemp);
            txtCurrentTemp = new TextBox { Location = new Point(190, 32), Size = new Size(100, 25), Text = "20" };
            panelThermostat.Controls.Add(txtCurrentTemp);
            
            lblMode = new Label { Text = "Режим:", Location = new Point(0, 70), Size = new Size(180, 20) };
            panelThermostat.Controls.Add(lblMode);
            cmbMode = new ComboBox { Location = new Point(190, 67), Size = new Size(100, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMode.Items.AddRange(new string[] { "heat", "cool", "auto" });
            cmbMode.SelectedIndex = 2;
            panelThermostat.Controls.Add(cmbMode);
            
            lblHumidity = new Label { Text = "Влажность (%):", Location = new Point(0, 105), Size = new Size(180, 20) };
            panelThermostat.Controls.Add(lblHumidity);
            txtHumidity = new TextBox { Location = new Point(190, 102), Size = new Size(100, 25), Text = "50" };
            panelThermostat.Controls.Add(txtHumidity);
            
            this.Controls.Add(panelThermostat);

            // === Панель Light ===
            panelLight = new Panel { Location = new Point(20, yPos), Size = new Size(440, 120), Visible = false };
            
            var lblBrightness = new Label { Text = "Яркость (0-100):", Location = new Point(0, 0), Size = new Size(180, 20) };
            panelLight.Controls.Add(lblBrightness);
            txtBrightness = new TextBox { Location = new Point(190, -3), Size = new Size(100, 25), Text = "50" };
            panelLight.Controls.Add(txtBrightness);
            
            lblColor = new Label { Text = "Цвет:", Location = new Point(0, 35), Size = new Size(180, 20) };
            panelLight.Controls.Add(lblColor);
            txtColor = new TextBox { Location = new Point(190, 32), Size = new Size(100, 25), Text = "white" };
            panelLight.Controls.Add(txtColor);
            
            lblAutoMode = new Label { Text = "Авторежим:", Location = new Point(0, 70), Size = new Size(180, 20) };
            panelLight.Controls.Add(lblAutoMode);
            chkAutoMode = new CheckBox { Location = new Point(190, 67), Size = new Size(100, 25) };
            panelLight.Controls.Add(chkAutoMode);
            
            this.Controls.Add(panelLight);

            // === Панель DoorLock ===
            panelDoorLock = new Panel { Location = new Point(20, yPos), Size = new Size(440, 120), Visible = false };
            
            var lblAccessCode = new Label { Text = "Код доступа:", Location = new Point(0, 0), Size = new Size(180, 20) };
            panelDoorLock.Controls.Add(lblAccessCode);
            txtAccessCode = new TextBox { Location = new Point(190, -3), Size = new Size(100, 25), Text = "1234" };
            panelDoorLock.Controls.Add(txtAccessCode);
            
            lblDoorIsLocked = new Label { Text = "Состояние:", Location = new Point(0, 35), Size = new Size(180, 20) };
            panelDoorLock.Controls.Add(lblDoorIsLocked);
            chkIsLocked = new CheckBox { Location = new Point(190, 32), Size = new Size(100, 25), Text = "Заперт", Checked = true };
            panelDoorLock.Controls.Add(chkIsLocked);
            
            lblDoorAutoLock = new Label { Text = "Автоблокировка:", Location = new Point(0, 70), Size = new Size(180, 20) };
            panelDoorLock.Controls.Add(lblDoorAutoLock);
            chkAutoLock = new CheckBox { Location = new Point(190, 67), Size = new Size(100, 25), Text = "Включена", Checked = true };
            panelDoorLock.Controls.Add(chkAutoLock);
            
            this.Controls.Add(panelDoorLock);

            // === Панель Camera ===
            panelCamera = new Panel { Location = new Point(20, yPos), Size = new Size(440, 120), Visible = false };
            
            var lblResolution = new Label { Text = "Разрешение:", Location = new Point(0, 0), Size = new Size(180, 20) };
            panelCamera.Controls.Add(lblResolution);
            cmbResolution = new ComboBox { Location = new Point(190, -3), Size = new Size(100, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbResolution.Items.AddRange(new string[] { "720", "1080", "2160", "3840" });
            cmbResolution.SelectedIndex = 1;
            panelCamera.Controls.Add(cmbResolution);
            
            lblRecording = new Label { Text = "Запись:", Location = new Point(0, 35), Size = new Size(180, 20) };
            panelCamera.Controls.Add(lblRecording);
            chkRecording = new CheckBox { Location = new Point(190, 32), Size = new Size(100, 25), Text = "Вести запись" };
            panelCamera.Controls.Add(chkRecording);
            
            lblMotion = new Label { Text = "Детекция движения:", Location = new Point(0, 70), Size = new Size(180, 20) };
            panelCamera.Controls.Add(lblMotion);
            chkMotionDetection = new CheckBox { Location = new Point(190, 67), Size = new Size(150, 25), Text = "Включена", Checked = true };
            panelCamera.Controls.Add(chkMotionDetection);
            
            this.Controls.Add(panelCamera);

            // Кнопки
            yPos = 400;
            
            btnSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(180, yPos),
                Size = new Size(120, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 150, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(310, yPos),
                Size = new Size(120, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(150, 50, 50),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);
        }

        private void CmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string type = cmbType.SelectedItem.ToString();
            
            panelThermostat.Visible = (type == "Thermostat");
            panelLight.Visible = (type == "Light");
            panelDoorLock.Visible = (type == "DoorLock");
            panelCamera.Visible = (type == "Camera");
            
            UpdateConstructorVisibility();
        }

        private void RbConstructor_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isEdit) UpdateConstructorVisibility();
        }

        private void UpdateConstructorVisibility()
        {
            bool isAdvanced = rbFull.Checked || _isEdit;
            string type = cmbType.SelectedItem.ToString();

            switch (type)
            {
                case "Thermostat":
                    lblCurrentTemp.Visible = isAdvanced;
                    txtCurrentTemp.Visible = isAdvanced;
                    lblMode.Visible = isAdvanced;
                    cmbMode.Visible = isAdvanced;
                    lblHumidity.Visible = isAdvanced;
                    txtHumidity.Visible = isAdvanced;
                    break;

                case "Light":
                    lblColor.Visible = isAdvanced;
                    txtColor.Visible = isAdvanced;
                    lblAutoMode.Visible = isAdvanced;
                    chkAutoMode.Visible = isAdvanced;
                    break;

                case "DoorLock":
                    lblDoorIsLocked.Visible = isAdvanced;
                    chkIsLocked.Visible = isAdvanced;
                    lblDoorAutoLock.Visible = isAdvanced;
                    chkAutoLock.Visible = isAdvanced;
                    break;

                case "Camera":
                    lblRecording.Visible = isAdvanced;
                    chkRecording.Visible = isAdvanced;
                    lblMotion.Visible = isAdvanced;
                    chkMotionDetection.Visible = isAdvanced;
                    break;
            }
        }

        private void LoadDeviceData()
        {
            var deviceRow = DatabaseHelper.GetDeviceById(_deviceId);
            if (deviceRow == null) return;

            string type = deviceRow["DeviceType"].ToString();
            cmbType.SelectedItem = type;
            txtName.Text = deviceRow["Name"].ToString();
            txtManufacturer.Text = deviceRow["Manufacturer"].ToString();
            cmbIsOn.SelectedIndex = Convert.ToInt32(deviceRow["IsOn"]);
            
            switch (type)
            {
                case "Thermostat":
                    var therm = DatabaseHelper.GetThermostatByDeviceId(_deviceId);
                    if (therm != null)
                    {
                        txtCurrentTemp.Text = therm["CurrentTemperature"].ToString();
                        txtTargetTemp.Text = therm["TargetTemperature"].ToString();
                        cmbMode.SelectedItem = therm["Mode"].ToString();
                        txtHumidity.Text = therm["Humidity"].ToString();
                    }
                    break;
                    
                case "Light":
                    var light = DatabaseHelper.GetLightByDeviceId(_deviceId);
                    if (light != null)
                    {
                        txtBrightness.Text = light["Brightness"].ToString();
                        txtColor.Text = light["Color"].ToString();
                        chkAutoMode.Checked = Convert.ToInt32(light["AutoMode"]) == 1;
                    }
                    break;
                    
                case "DoorLock":
                    var door = DatabaseHelper.GetDoorLockByDeviceId(_deviceId);
                    if (door != null)
                    {
                        txtAccessCode.Text = door["AccessCode"].ToString();
                        chkIsLocked.Checked = Convert.ToInt32(door["IsLocked"]) == 1;
                        chkAutoLock.Checked = Convert.ToInt32(door["AutoLock"]) == 1;
                    }
                    break;
                    
                case "Camera":
                    var cam = DatabaseHelper.GetCameraByDeviceId(_deviceId);
                    if (cam != null)
                    {
                        cmbResolution.SelectedItem = cam["Resolution"].ToString();
                        chkRecording.Checked = Convert.ToInt32(cam["IsRecording"]) == 1;
                        chkMotionDetection.Checked = Convert.ToInt32(cam["MotionDetection"]) == 1;
                    }
                    break;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }
            
            string type = cmbType.SelectedItem.ToString();
            string name = txtName.Text.Trim();
            string manufacturer = txtManufacturer.Text.Trim();
            bool isOn = cmbIsOn.SelectedIndex == 1;
            
            try
            {
                Device device = null;
                bool isAdvanced = rbFull.Checked || _isEdit;
                
                switch (type)
                {
                    case "Thermostat":
                        double targetTemp = Convert.ToDouble(txtTargetTemp.Text);
                        if (isAdvanced)
                        {
                            double currentTemp = Convert.ToDouble(txtCurrentTemp.Text);
                            double humidity = Convert.ToDouble(txtHumidity.Text);
                            device = new Thermostat(name, manufacturer, targetTemp, currentTemp, cmbMode.SelectedItem.ToString(), humidity);
                        }
                        else
                        {
                            device = new Thermostat(name, manufacturer, targetTemp);
                        }
                        break;
                        
                    case "Light":
                        int brightness = Convert.ToInt32(txtBrightness.Text);
                        if (isAdvanced)
                        {
                            device = new Light(name, manufacturer, brightness, txtColor.Text, chkAutoMode.Checked, -1);
                        }
                        else
                        {
                            device = new Light(name, manufacturer, brightness);
                        }
                        break;
                        
                    case "DoorLock":
                        if (isAdvanced)
                        {
                            device = new DoorLock(name, manufacturer, txtAccessCode.Text, chkAutoLock.Checked, chkIsLocked.Checked);
                        }
                        else
                        {
                            device = new DoorLock(name, manufacturer, txtAccessCode.Text);
                        }
                        break;
                        
                    case "Camera":
                        int resolution = Convert.ToInt32(cmbResolution.SelectedItem);
                        if (isAdvanced)
                        {
                            device = new Camera(name, manufacturer, resolution, chkRecording.Checked, chkMotionDetection.Checked, 0);
                        }
                        else
                        {
                            device = new Camera(name, manufacturer, resolution);
                        }
                        break;
                }
                
                device.IsOn = isOn;
                
                if (_isEdit)
                {
                    DatabaseHelper.UpdateDevice(device, _deviceId);
                    
                    // Update type-specific data
                    switch (type)
                    {
                        case "Thermostat":
                            var therm = (Thermostat)device;
                            var thermRow = DatabaseHelper.GetThermostatByDeviceId(_deviceId);
                            if (thermRow != null)
                            {
                                int id = Convert.ToInt32(thermRow["Id"]);
                                DatabaseHelper.UpdateThermostat(therm, id);
                            }
                            break;
                            
                        case "Light":
                            var light = (Light)device;
                            var lightRow = DatabaseHelper.GetLightByDeviceId(_deviceId);
                            if (lightRow != null)
                            {
                                int id = Convert.ToInt32(lightRow["Id"]);
                                DatabaseHelper.UpdateLight(light, id);
                            }
                            break;
                            
                        case "DoorLock":
                            var door = (DoorLock)device;
                            var doorRow = DatabaseHelper.GetDoorLockByDeviceId(_deviceId);
                            if (doorRow != null)
                            {
                                int id = Convert.ToInt32(doorRow["Id"]);
                                DatabaseHelper.UpdateDoorLock(door, id);
                            }
                            break;
                            
                        case "Camera":
                            var cam = (Camera)device;
                            var camRow = DatabaseHelper.GetCameraByDeviceId(_deviceId);
                            if (camRow != null)
                            {
                                int id = Convert.ToInt32(camRow["Id"]);
                                DatabaseHelper.UpdateCamera(cam, id);
                            }
                            break;
                    }
                }
                else
                {
                    int deviceId = DatabaseHelper.InsertDevice(device);
                    
                    switch (type)
                    {
                        case "Thermostat":
                            DatabaseHelper.InsertThermostat((Thermostat)device, deviceId);
                            break;
                        case "Light":
                            DatabaseHelper.InsertLight((Light)device, deviceId);
                            break;
                        case "DoorLock":
                            DatabaseHelper.InsertDoorLock((DoorLock)device, deviceId);
                            break;
                        case "Camera":
                            DatabaseHelper.InsertCamera((Camera)device, deviceId);
                            break;
                    }
                }
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}