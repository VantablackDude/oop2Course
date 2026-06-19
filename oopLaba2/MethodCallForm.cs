using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace oopLaba2
{
    public class MethodCallForm : Form
    {
        private int _deviceId;
        private ComboBox cmbMethods;
        private Button btnExecute;
        private Label lblDevice;
        private Label lblParam;
        private TextBox txtParam;
        private Label lblPrompt;

        public MethodCallForm(int deviceId)
        {
            _deviceId = deviceId;
            InitializeComponent();
            LoadMethods();
        }

        private void InitializeComponent()
        {
            this.Text = "Вызов методов устройства";
            this.Size = new Size(420, 260);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 245);

            var deviceRow = DatabaseHelper.GetDeviceById(_deviceId);
            string deviceName = deviceRow != null ? deviceRow["Name"].ToString() : "Unknown";

            lblDevice = new Label
            {
                Text = $"Устройство: {deviceName}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 60, 100),
                Location = new Point(15, 15),
                Size = new Size(380, 25)
            };
            this.Controls.Add(lblDevice);

            var lblSelect = new Label
            {
                Text = "Выберите метод:",
                Location = new Point(15, 50),
                Size = new Size(120, 20)
            };
            this.Controls.Add(lblSelect);

            cmbMethods = new ComboBox
            {
                Location = new Point(140, 48),
                Size = new Size(250, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbMethods.SelectedIndexChanged += CmbMethods_SelectedIndexChanged;
            this.Controls.Add(cmbMethods);

            lblPrompt = new Label
            {
                Location = new Point(15, 85),
                Size = new Size(120, 20),
                Visible = false
            };
            this.Controls.Add(lblPrompt);

            txtParam = new TextBox
            {
                Location = new Point(140, 83),
                Size = new Size(250, 25),
                Visible = false
            };
            this.Controls.Add(txtParam);

            btnExecute = new Button
            {
                Text = "Выполнить",
                Location = new Point(100, 130),
                Size = new Size(200, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 200),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11)
            };
            btnExecute.FlatAppearance.BorderSize = 0;
            btnExecute.Click += BtnExecute_Click;
            this.Controls.Add(btnExecute);

            var btnClose = new Button
            {
                Text = "Закрыть",
                Location = new Point(100, 180),
                Size = new Size(200, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(150, 50, 50),
                ForeColor = Color.White
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void LoadMethods()
        {
            var device = DatabaseHelper.CreateDeviceObject(_deviceId);
            if (device == null) return;

            string type = device.GetDeviceType();

            if (type == "Thermostat")
            {
                cmbMethods.Items.AddRange(new string[] {
                    "SetTemperature  ",
                    "GetClimateInfo  ",
                    "TurnOn  ",
                    "GetDetailedInfo  ",
                    "TurnOff  ",
                    "GetStatus  "
                });
            }
            else if (type == "Light")
            {
                cmbMethods.Items.AddRange(new string[] {
                    "SetBrightness  ",
                    "GetLightInfo  ",
                    "TurnOn  ",
                    "GetDetailedInfo  ",
                    "TurnOff  ",
                    "GetStatus  "
                });
            }
            else if (type == "DoorLock")
            {
                cmbMethods.Items.AddRange(new string[] {
                    "TryUnlock  ",
                    "GetLockInfo  ",
                    "TurnOn  ",
                    "GetDetailedInfo  ",
                    "TurnOff  ",
                    "GetStatus  "
                });
            }
            else if (type == "Camera")
            {
                cmbMethods.Items.AddRange(new string[] {
                    "StartRecording  ",
                    "RecordMotion  ",
                    "TurnOn  ",
                    "GetDetailedInfo  ",
                    "TurnOff  ",
                    "GetStatus  "
                });
            }

            if (cmbMethods.Items.Count > 0)
                cmbMethods.SelectedIndex = 0;
        }

        private void CmbMethods_SelectedIndexChanged(object sender, EventArgs e)
        {
            string method = cmbMethods.SelectedItem?.ToString() ?? "";
            bool needsParam = method.StartsWith("SetTemperature") ||
                              method.StartsWith("SetBrightness") ||
                              method.StartsWith("TryUnlock");

            lblPrompt.Visible = needsParam;
            txtParam.Visible = needsParam;
            txtParam.Text = "";

            if (method.StartsWith("SetTemperature"))
            {
                lblPrompt.Text = "Температура:";
                txtParam.Text = "22";
            }
            else if (method.StartsWith("SetBrightness"))
            {
                lblPrompt.Text = "Яркость (0-100):";
                txtParam.Text = "50";
            }
            else if (method.StartsWith("TryUnlock"))
            {
                lblPrompt.Text = "Код доступа:";
                txtParam.Text = "1234";
            }
        }

        private void BtnExecute_Click(object sender, EventArgs e)
        {
            var device = DatabaseHelper.CreateDeviceObject(_deviceId);
            if (device == null) return;

            string method = cmbMethods.SelectedItem?.ToString() ?? "";
            string result = "";

            try
            {
                switch (method)
                {
                    // === THERMOSTAT ===
                    case "SetTemperature  ":
                        double temp = Convert.ToDouble(txtParam.Text);
                        ((Thermostat)device).SetTemperature(temp);
                        result = $"Температура установлена: {((Thermostat)device).TargetTemperature}°C, режим: {((Thermostat)device).Mode}";
                        break;
                    case "GetClimateInfo  ":
                        result = ((Thermostat)device).GetClimateInfo();
                        break;
                    case "TurnOn  ":
                        device.TurnOn(); result = $"Устройство включено.";
                        break;
                    case "GetDetailedInfo  ":
                        result = device.GetDetailedInfo(); break;
                    case "TurnOff  ":
                        device.TurnOff(); result = $"Устройство выключено.";
                        break;
                    case "GetStatus  ":
                        result = device.GetStatus(); break;

                    // === LIGHT ===
                    case "SetBrightness  ":
                        int level = Convert.ToInt32(txtParam.Text);
                        ((Light)device).SetBrightness(level);
                        result = $"Яркость установлена: {((Light)device).Brightness}%";
                        break;
                    case "GetLightInfo  ":
                        result = ((Light)device).GetLightInfo(); break;
                    //case "GetDetailedInfo  ":
                    //    result = device.GetDetailedInfo(); break;

                    // === DOORLOCK ===
                    case "TryUnlock  ":
                        string code = txtParam.Text;
                        bool ok = ((DoorLock)device).TryUnlock(code);
                        result = ok ? $"Дверь открыта кодом: {code}" : $"Неверный код: {code}";
                        break;
                    case "GetLockInfo  ":
                        result = ((DoorLock)device).GetLockInfo(); break;

                    // === CAMERA ===
                    case "StartRecording  ":
                        result = ((Camera)device).StartRecording(); break;
                    case "RecordMotion  ":
                        result = ((Camera)device).RecordMotion(); break;

                    default:
                        result = "Метод не найден";
                        break;
                }

                // Для методов перекрытых/унаследованных, дублирующихся между классами
                if (method == "TurnOn  ") result = device.GetDeviceType() == "Thermostat" ? "Термостат включен." :
                                                              device.GetDeviceType() == "Light" ? "Свет включен." :
                                                              device.GetDeviceType() == "DoorLock" ? "Замок активирован." :
                                                              "Камера включена.";
                else if (method == "TurnOff  ") result = "Устройство выключено.";
                else if (method == "GetStatus  ") result = device.GetStatus();
                else if (method == "GetDetailedInfo  ") result = device.GetDetailedInfo();

                DatabaseHelper.SaveDeviceObject(device, _deviceId);
                MessageBox.Show(result, "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}