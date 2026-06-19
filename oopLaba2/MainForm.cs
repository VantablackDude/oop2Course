using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using oopLaba2;

namespace oopLaba2
{
    public class MainForm : Form
    {
        private DataGridView dgvDevices;
        private Panel panelDetails;
        private Panel panelButtons;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;
        private Button btnToggle;
        private Button btnMethods;
        private Button btnExit;
        private Label lblTitle;
        private Label lblDetails;
        private int _selectedDeviceId = -1;
        
        public MainForm()
        {
            InitializeComponent();
            LoadDevices();
        }

        private void InitializeComponent()
        {
            this.Text = "Умный дом - Управление устройствами";
            this.Size = new Size(900, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 245);

            // Заголовок
            lblTitle = new Label
            {
                Text = "Устройства умного дома",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 60, 100),
                Location = new Point(20, 15),
                Size = new Size(400, 40)
            };
            this.Controls.Add(lblTitle);

            // DataGridView для списка устройств
            dgvDevices = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(540, 450),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                RowHeadersVisible = false
            };
            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 40 });
            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Название", Width = 150 });
            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "Type", HeaderText = "Тип", Width = 100 });
            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "Manufacturer", HeaderText = "Производитель", Width = 120 });
            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Статус", Width = 80 });
            dgvDevices.SelectionChanged += DgvDevices_SelectionChanged;
            this.Controls.Add(dgvDevices);

            // Панель кнопок
            panelButtons = new Panel
            {
                Location = new Point(580, 60),
                Size = new Size(280, 50)
            };
            
            btnAdd = new Button
            {
                Text = "Добавить",
                Location = new Point(0, 0),
                Size = new Size(130, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 150, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;
            panelButtons.Controls.Add(btnAdd);

            btnEdit = new Button
            {
                Text = "Редактировать",
                Location = new Point(140, 0),
                Size = new Size(130, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 140, 0),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += BtnEdit_Click;
            panelButtons.Controls.Add(btnEdit);

            this.Controls.Add(panelButtons);

            // Вторая строка кнопок
            Panel panelButtons2 = new Panel
            {
                Location = new Point(580, 115),
                Size = new Size(280, 50)
            };

            btnDelete = new Button
            {
                Text = "Удалить",
                Location = new Point(0, 0),
                Size = new Size(130, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;
            panelButtons2.Controls.Add(btnDelete);

            btnRefresh = new Button
            {
                Text = "Обновить",
                Location = new Point(140, 0),
                Size = new Size(130, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(80, 130, 200),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;
            panelButtons2.Controls.Add(btnRefresh);

            this.Controls.Add(panelButtons2);

            // Третья строка - кнопка Включить/Выключить
            Panel panelButtons3 = new Panel
            {
                Location = new Point(580, 170),
                Size = new Size(280, 50)
            };

            btnToggle = new Button
            {
                Text = "Вкл/Выкл",
                Location = new Point(0, 0),
                Size = new Size(270, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 100, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
            btnToggle.FlatAppearance.BorderSize = 0;
            btnToggle.Click += BtnToggle_Click;
            panelButtons3.Controls.Add(btnToggle);

            this.Controls.Add(panelButtons3);

            // Четвертая строка - кнопка Методы
            Panel panelButtons4 = new Panel
            {
                Location = new Point(580, 225),
                Size = new Size(280, 50)
            };

            btnMethods = new Button
            {
                Text = "Методы класса",
                Location = new Point(0, 0),
                Size = new Size(270, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 130, 180),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
            btnMethods.FlatAppearance.BorderSize = 0;
            btnMethods.Click += BtnMethods_Click;
            panelButtons4.Controls.Add(btnMethods);

            this.Controls.Add(panelButtons4);

            // Панель деталей
            Panel panelDetailsContainer = new Panel
            {
                Location = new Point(580, 280),
                Size = new Size(280, 230),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblDetailsTitle = new Label
            {
                Text = "Детали устройства",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 60, 100),
                Location = new Point(10, 10),
                Size = new Size(260, 25)
            };
            panelDetailsContainer.Controls.Add(lblDetailsTitle);

            lblDetails = new Label
            {
                Text = "Выберите устройство\nиз списка",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Location = new Point(10, 40),
                Size = new Size(260, 230),
                AutoSize = false
            };
            panelDetailsContainer.Controls.Add(lblDetails);

            this.Controls.Add(panelDetailsContainer);

            // Кнопка выхода
            btnExit = new Button
            {
                Text = "Выход",
                Location = new Point(700, 520),
                Size = new Size(150, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(150, 50, 50),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += (s, e) => this.Close();
            this.Controls.Add(btnExit);
        }

        private void LoadDevices()
        {
            dgvDevices.Rows.Clear();
            
            if (!DatabaseHelper.IsInitialized)
            {
                DatabaseHelper.Initialize("smart_home.db");
            }

            var dt = DatabaseHelper.GetAllDevices();
            foreach (DataRow row in dt.Rows)
            {
                string status = Convert.ToInt32(row["IsOn"]) == 1 ? "Включено" : "Выключено";
                dgvDevices.Rows.Add(
                    row["Id"],
                    row["Name"],
                    row["DeviceType"],
                    row["Manufacturer"],
                    status
                );
            }
        }

        private void DgvDevices_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDevices.SelectedRows.Count > 0)
            {
                _selectedDeviceId = Convert.ToInt32(dgvDevices.SelectedRows[0].Cells["Id"].Value);
                ShowDeviceDetails(_selectedDeviceId);
            }
        }

        private void ShowDeviceDetails(int deviceId)
        {
            var deviceRow = DatabaseHelper.GetDeviceById(deviceId);
            if (deviceRow == null) return;

            string type = deviceRow["DeviceType"].ToString();
            string details = $"ID: {deviceRow["Id"]}\n";
            details += $"Название: {deviceRow["Name"]}\n";
            details += $"Тип: {type}\n";
            details += $"Производитель: {deviceRow["Manufacturer"]}\n";
            details += $"Статус: {(Convert.ToInt32(deviceRow["IsOn"]) == 1 ? "Включено" : "Выключено")}\n";
            details += $"Создано: {deviceRow["CreatedAt"]}\n\n";

            // Дополнительные данные в зависимости от типа
            switch (type)
            {
                case "Thermostat":
                    var therm = DatabaseHelper.GetThermostatByDeviceId(deviceId);
                    if (therm != null)
                    {
                        details += $"Текущая температура: {therm["CurrentTemperature"]}°C\n";
                        details += $"Целевая температура: {therm["TargetTemperature"]}°C\n";
                        details += $"Режим: {therm["Mode"]}\n";
                        details += $"Влажность: {therm["Humidity"]}%";
                    }
                    break;

                case "Light":
                    var light = DatabaseHelper.GetLightByDeviceId(deviceId);
                    if (light != null)
                    {
                        details += $"Яркость: {light["Brightness"]}%\n";
                        details += $"Цвет: {light["Color"]}\n";
                        details += $"Авторежим: {(Convert.ToInt32(light["AutoMode"]) == 1 ? "Да" : "Нет")}";
                    }
                    break;

                case "DoorLock":
                    var door = DatabaseHelper.GetDoorLockByDeviceId(deviceId);
                    if (door != null)
                    {
                        details += $"Состояние: {(Convert.ToInt32(door["IsLocked"]) == 1 ? "Заперт" : "Открыт")}\n";
                        details += $"Автоблокировка: {(Convert.ToInt32(door["AutoLock"]) == 1 ? "Да" : "Нет")}\n";
                        details += $"Неверных попыток: {door["WrongAttempts"]}";
                    }
                    break;

                case "Camera":
                    var cam = DatabaseHelper.GetCameraByDeviceId(deviceId);
                    if (cam != null)
                    {
                        details += $"Разрешение: {cam["Resolution"]}p\n";
                        details += $"Запись: {(Convert.ToInt32(cam["IsRecording"]) == 1 ? "Идет" : "Остановлена")}\n";
                        details += $"Детекция движения: {(Convert.ToInt32(cam["MotionDetection"]) == 1 ? "Включена" : "Выключена")}";
                    }
                    break;
            }

            lblDetails.Text = details;
        }

        private void BtnMethods_Click(object sender, EventArgs e)
        {
            if (_selectedDeviceId <= 0)
            {
                MessageBox.Show("Выберите устройство", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var form = new MethodCallForm(_selectedDeviceId);
            form.ShowDialog();
            ShowDeviceDetails(_selectedDeviceId);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var form = new DeviceEditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadDevices();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvDevices.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите устройство для редактирования", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int deviceId = Convert.ToInt32(dgvDevices.SelectedRows[0].Cells["Id"].Value);
            var form = new DeviceEditForm(deviceId);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadDevices();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvDevices.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите устройство для удаления", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int deviceId = Convert.ToInt32(dgvDevices.SelectedRows[0].Cells["Id"].Value);
            string name = dgvDevices.SelectedRows[0].Cells["Name"].Value.ToString();

            var result = MessageBox.Show($"Удалить устройство \"{name}\"?", "Подтверждение", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                DatabaseHelper.DeleteDevice(deviceId);
                LoadDevices();
                lblDetails.Text = "Выберите устройство\nиз списка";
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadDevices();
            MessageBox.Show("Список обновлен", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnToggle_Click(object sender, EventArgs e)
        {
            if (dgvDevices.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите устройство", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int deviceId = Convert.ToInt32(dgvDevices.SelectedRows[0].Cells["Id"].Value);
            var deviceRow = DatabaseHelper.GetDeviceById(deviceId);
            
            if (deviceRow == null)
            {
                MessageBox.Show("Устройство не найдено", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            bool currentState = Convert.ToInt32(deviceRow["IsOn"]) == 1;
            DatabaseHelper.SetDeviceState(deviceId, !currentState);
            
            LoadDevices();
            
            // Восстановить выбор
            foreach (DataGridViewRow row in dgvDevices.Rows)
            {
                if (Convert.ToInt32(row.Cells["Id"].Value) == deviceId)
                {
                    row.Selected = true;
                    break;
                }
            }
        }
    }
}