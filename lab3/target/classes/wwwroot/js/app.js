const API_URL = 'http://localhost:8080';
const API_BASE = `${API_URL}/api`;

let homes = [];
let selectedHomeId = null;
let notificationTimeout = null;

const cameraConstructors = [
    {
        name: 'Конструктор 1: Базовый',
        description: 'Camera(name, resolution) + дом',
        fields: [
            { name: 'name', label: 'Название', type: 'text', required: true },
            { name: 'resolution', label: 'Разрешение (пикс)', type: 'number', required: true, min: 480 }
        ],
        endpoint: '/create'
    },
    {
        name: 'Конструктор 2: Полный',
        description: 'Все поля камеры',
        fields: [
            { name: 'name', label: 'Название', type: 'text', required: true },
            { name: 'manufacturer', label: 'Производитель', type: 'text', required: false, defaultVal: 'HikVision' },
            { name: 'isOn', label: 'Включена', type: 'checkbox' },
            { name: 'resolution', label: 'Разрешение (пикс)', type: 'number', required: true, min: 480 },
            { name: 'isRecording', label: 'Запись', type: 'checkbox' },
            { name: 'motionDetection', label: 'Детекция движения', type: 'checkbox' },
            { name: 'storageUsed', label: 'Память (%)', type: 'number', required: false, min: 0, max: 100, defaultVal: '0' }
        ],
        endpoint: '/create-full'
    }
];

const doorlockConstructors = [
    {
        name: 'Конструктор 1: Базовый',
        description: 'DoorLock(name, accessCode) + дом',
        fields: [
            { name: 'name', label: 'Название', type: 'text', required: true },
            { name: 'accessCode', label: 'Код доступа', type: 'text', required: false, defaultVal: '1234' }
        ],
        endpoint: '/create'
    },
    {
        name: 'Конструктор 2: Полный',
        description: 'Все поля замка',
        fields: [
            { name: 'name', label: 'Название', type: 'text', required: true },
            { name: 'manufacturer', label: 'Производитель', type: 'text', required: false, defaultVal: 'Samsung' },
            { name: 'isOn', label: 'Включен', type: 'checkbox' },
            { name: 'isLocked', label: 'Заперт', type: 'checkbox' },
            { name: 'accessCode', label: 'Код доступа', type: 'text', required: false, defaultVal: '1234' },
            { name: 'wrongAttempts', label: 'Неверных попыток', type: 'number', required: false, min: 0, defaultVal: '0' },
            { name: 'autoLock', label: 'Автоблокировка', type: 'checkbox' }
        ],
        endpoint: '/create-full'
    }
];

const lightConstructors = [
    {
        name: 'Конструктор 1: Базовый',
        description: 'Light(name, brightness) + дом',
        fields: [
            { name: 'name', label: 'Название', type: 'text', required: true },
            { name: 'brightness', label: 'Яркость (%)', type: 'number', required: true, min: 0, max: 100 }
        ],
        endpoint: '/create'
    },
    {
        name: 'Конструктор 2: Полный',
        description: 'Все поля лампы',
        fields: [
            { name: 'name', label: 'Название', type: 'text', required: true },
            { name: 'manufacturer', label: 'Производитель', type: 'text', required: false, defaultVal: 'Philips' },
            { name: 'isOn', label: 'Включена', type: 'checkbox' },
            { name: 'brightness', label: 'Яркость (%)', type: 'number', required: true, min: 0, max: 100 },
            { name: 'color', label: 'Цвет', type: 'text', required: false, defaultVal: 'white' },
            { name: 'autoMode', label: 'Авторежим', type: 'checkbox' }
        ],
        endpoint: '/create-full'
    }
];

const thermostatConstructors = [
    {
        name: 'Конструктор 1: Базовый',
        description: 'Thermostat(name, targetTemperature) + дом',
        fields: [
            { name: 'name', label: 'Название', type: 'text', required: true },
            { name: 'targetTemperature', label: 'Целевая T (°C)', type: 'number', required: true, min: 5, max: 40 }
        ],
        endpoint: '/create'
    },
    {
        name: 'Конструктор 2: Полный',
        description: 'Все поля термостата',
        fields: [
            { name: 'name', label: 'Название', type: 'text', required: true },
            { name: 'manufacturer', label: 'Производитель', type: 'text', required: false, defaultVal: 'Nest' },
            { name: 'isOn', label: 'Включен', type: 'checkbox' },
            { name: 'currentTemperature', label: 'Текущая T (°C)', type: 'number', required: false, min: -10, max: 50, defaultVal: '20' },
            { name: 'targetTemperature', label: 'Целевая T (°C)', type: 'number', required: true, min: 5, max: 40 },
            { name: 'mode', label: 'Режим', type: 'text', required: false, defaultVal: 'auto' },
            { name: 'humidity', label: 'Влажность (%)', type: 'number', required: false, min: 0, max: 100, defaultVal: '50' }
        ],
        endpoint: '/create-full'
    }
];

document.addEventListener('DOMContentLoaded', () => {
    initializeApp();
});

async function initializeApp() {
    document.querySelectorAll('.nav-tab').forEach(tab => {
        tab.addEventListener('click', () => switchTab(tab.dataset.tab));
    });
    document.getElementById('refreshBtn')?.addEventListener('click', () => loadAllData());
    document.getElementById('createHomeBtn')?.addEventListener('click', () => createHome());
    await loadAllData();
    renderAllConstructors();
}

function switchTab(tabId) {
    document.querySelectorAll('.nav-tab').forEach(tab => {
        tab.classList.toggle('active', tab.dataset.tab === tabId);
    });
    document.querySelectorAll('.tab-content').forEach(content => {
        content.classList.toggle('active', content.id === `${tabId}Tab`);
    });
}

async function loadAllData() {
    showLoading(true);
    try {
        await Promise.all([
            loadHomes(),
            loadDevices(),
            selectedHomeId ? loadCameras() : Promise.resolve(),
            selectedHomeId ? loadDoorLocks() : Promise.resolve(),
            selectedHomeId ? loadLights() : Promise.resolve(),
            selectedHomeId ? loadThermostats() : Promise.resolve()
        ]);
    } catch (error) {
        console.error('Error loading data:', error);
        showError('Ошибка загрузки данных');
    } finally {
        showLoading(false);
    }
}

// ============ SMARTHOMES ============
async function loadHomes() {
    try {
        const response = await fetch(`${API_BASE}/smarthome/all`);
        if (!response.ok) throw new Error('Failed to fetch homes');
        homes = await response.json();
        displayHomes();
        document.getElementById('homeCount').textContent = homes.length;
        updateHomeSelector();
        renderAllConstructors();
    } catch (error) {
        console.error('Error loading homes:', error);
    }
}

function updateHomeSelector() {
    const sel = document.getElementById('homeSelector');
    if (!sel) return;
    const currentVal = sel.value;
    sel.innerHTML = '<option value="">— Выберите дом —</option>' +
        homes.map(h => `<option value="${h.id}">${escapeHtml(h.name)} (ID: ${h.id})</option>`).join('');
    if (currentVal && homes.some(h => h.id == currentVal)) {
        sel.value = currentVal;
    }
}

function setHomeContext(homeId) {
    document.getElementById('homeSelector').value = homeId;
    onHomeSelected(String(homeId));
}

function onHomeSelected(homeIdStr) {
    selectedHomeId = homeIdStr ? parseInt(homeIdStr) : null;
    const hint = document.getElementById('homeContextHint');
    if (selectedHomeId) {
        const home = homes.find(h => h.id === selectedHomeId);
        hint.textContent = `Показаны устройства дома: ${home ? escapeHtml(home.name) : 'ID ' + selectedHomeId}`;
    } else {
        hint.textContent = 'Устройства будут показаны только для выбранного дома';
    }
    loadCameras();
    loadDoorLocks();
    loadLights();
    loadThermostats();
    document.querySelectorAll('.home-select').forEach(sel => {
        if (sel.options.length > 0) sel.value = homeIdStr || '';
    });
}

function displayHomes() {
    const grid = document.getElementById('homesGrid');
    if (!grid) return;

    if (!homes || homes.length === 0) {
        grid.innerHTML = '<div class="empty-state">Нет созданных домов</div>';
        return;
    }

    grid.innerHTML = homes.map(home => {
        const deviceCount = home.devices ? home.devices.length : 0;
        return `
        <div class="card">
            <h4>🏠 ${escapeHtml(home.name || 'Без названия')}</h4>
            <p>ID: <strong>${home.id}</strong> | Устройств: <strong>${deviceCount}</strong></p>
            ${home.address ? `<p>📍 ${escapeHtml(home.address)}</p>` : ''}
            <div class="card-buttons">
                <button class="btn btn-sm btn-info" onclick="viewHomeDevices(${home.id}, '${escapeHtml(home.name || '')}'); setHomeContext(${home.id})">📋 Устройства</button>
                <button class="btn btn-sm btn-warning" onclick="editHome(${home.id}, '${escapeHtml(home.name || '')}')">✏️</button>
                <button class="btn btn-sm btn-danger" onclick="deleteHome(${home.id})">🗑️</button>
            </div>
        </div>
        `;
    }).join('');
}

async function createHome() {
    const nameInput = document.getElementById('homeNameInput');
    const addressInput = document.getElementById('homeAddressInput');
    const name = nameInput.value.trim();
    if (!name) {
        showNotification('Введите название дома', 'error');
        return;
    }
    const address = addressInput.value.trim();

    showLoading(true);
    try {
        let response;
        if (address) {
            response = await fetch(`${API_BASE}/smarthome/create-full`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ name, address })
            });
        } else {
            response = await fetch(`${API_BASE}/smarthome/create?name=${encodeURIComponent(name)}`, { method: 'POST' });
        }
        if (!response.ok) throw new Error('Failed to create home');
        showNotification(`Дом "${name}" создан`, 'success');
        nameInput.value = '';
        addressInput.value = '';
        await loadHomes();
    } catch (error) {
        showNotification('Ошибка создания дома', 'error');
    } finally {
        showLoading(false);
    }
}

async function editHome(id, currentName) {
    const newName = prompt('Новое название дома:', currentName);
    if (!newName || newName === currentName) return;
    showLoading(true);
    try {
        const response = await fetch(`${API_BASE}/smarthome/${id}?name=${encodeURIComponent(newName)}`, { method: 'PUT' });
        if (!response.ok) throw new Error('Failed to update home');
        showNotification('Дом обновлен', 'success');
        await loadHomes();
    } catch (error) {
        showNotification('Ошибка обновления', 'error');
    } finally {
        showLoading(false);
    }
}

async function deleteHome(id) {
    if (!confirm('Удалить дом? Все устройства в нем останутся без дома.')) return;
    showLoading(true);
    try {
        const response = await fetch(`${API_BASE}/smarthome/${id}`, { method: 'DELETE' });
        if (!response.ok) throw new Error('Failed to delete home');
        showNotification('Дом удален', 'success');
        await loadAllData();
    } catch (error) {
        showNotification('Ошибка удаления', 'error');
    } finally {
        showLoading(false);
    }
}

function viewHomeDevices(homeId, homeName) {
    showContentModal(`📋 Устройства в доме: ${homeName}`, async (bodyEl) => {
        try {
            const response = await fetch(`${API_BASE}/smarthome/${homeId}/devices`);
            if (!response.ok) throw new Error('Failed');
            const devices = await response.json();
            if (!devices || devices.length === 0) {
                bodyEl.innerHTML = '<p style="text-align:center;padding:40px;color:#999;">В этом доме нет устройств</p>';
                return;
            }
            let html = `<div class="table-container"><table class="data-table"><thead><tr>
                <th>ID</th><th>Тип</th><th>Название</th><th>Производитель</th><th>Статус</th><th>Характеристики</th><th>Действия</th>
            </tr></thead><tbody>`;
            devices.forEach(d => {
                let typeIcon = '📡';
                let specs = '-';
                if (d.type === 'camera') {
                    typeIcon = '📷';
                    specs = `Разр: ${d.resolution || '-'}p, Запись: ${d.isRecording ? '✅' : '❌'}`;
                } else if (d.type === 'doorlock') {
                    typeIcon = '🔒';
                    specs = `Замок: ${d.isLocked ? '🔒 Заперт' : '🔓 Открыт'}, Попыток: ${d.wrongAttempts || 0}`;
                } else if (d.type === 'light') {
                    typeIcon = '💡';
                    specs = `Ярк: ${d.brightness || 0}%, Цвет: ${d.color || 'white'}`;
                } else if (d.type === 'thermostat') {
                    typeIcon = '🌡️';
                    specs = `T: ${d.currentTemperature || '-'}°C → ${d.targetTemperature || '-'}°C, Режим: ${d.mode || 'auto'}`;
                }
                html += `<tr>
                    <td>${d.id}</td>
                    <td>${typeIcon}</td>
                    <td>${escapeHtml(d.name || '-')}</td>
                    <td>${escapeHtml(d.manufacturer || '-')}</td>
                    <td>${d.isOn ? '✅ Вкл' : '❌ Выкл'}</td>
                    <td>${specs}</td>
                    <td><button class="delete-btn" onclick="removeDeviceFromHome(${homeId}, ${d.id})">🗑️ Убрать</button></td>
                </tr>`;
            });
            html += '</tbody></table></div>';
            bodyEl.innerHTML = html;
        } catch (e) {
            bodyEl.innerHTML = '<p style="color:red;">Ошибка загрузки устройств</p>';
        }
    });
}

async function removeDeviceFromHome(homeId, deviceId) {
    if (!confirm('Убрать устройство из дома?')) return;
    try {
        const response = await fetch(`${API_BASE}/smarthome/${homeId}/remove-device/${deviceId}`, { method: 'DELETE' });
        if (!response.ok) throw new Error('Failed');
        showNotification('Устройство убрано из дома', 'success');
        closeModal();
        await loadAllData();
    } catch (error) {
        showNotification('Ошибка', 'error');
    }
}

// ============ DEVICES ============
async function loadDevices() {
    try {
        const response = await fetch(`${API_BASE}/device`);
        if (!response.ok) throw new Error('Failed to fetch devices');
        const devices = await response.json();
        displayDevices(devices);
        document.getElementById('deviceCount').textContent = devices.length;
    } catch (error) {
        console.error('Error loading devices:', error);
    }
}

function displayDevices(devices) {
    const grid = document.getElementById('deviceCardsGrid');
    if (!grid) return;
    if (!devices || devices.length === 0) {
        grid.innerHTML = '<div class="empty-devices">📡 Нет устройств</div>';
        return;
    }
    grid.innerHTML = devices.map(d => {
        const type = d.type || 'Device';
        const typeLower = type.toLowerCase();
        let typeIcon = '📡';
        let typeClass = 'device';
        if (typeLower === 'camera') { typeClass = 'camera'; typeIcon = '📷'; }
        else if (typeLower === 'doorlock') { typeClass = 'doorlock'; typeIcon = '🔒'; }
        else if (typeLower === 'light') { typeClass = 'light'; typeIcon = '💡'; }
        else if (typeLower === 'thermostat') { typeClass = 'thermostat'; typeIcon = '🌡️'; }

        const specTags = buildDeviceSpecTags(d, typeLower);

        return `
        <div class="device-card device-card--${typeClass}">
            <div class="device-card__topbar">
                <span class="device-icon">${typeIcon}</span>
                <span>${type}</span>
                <span class="device-card__status-dot device-card__status-dot--${d.isOn ? 'on' : 'off'}"></span>
            </div>
            <div class="device-card__body">
                <div class="device-card__name">${escapeHtml(d.name || 'Без названия')}</div>
                <div class="device-card__manufacturer">${escapeHtml(d.manufacturer || 'Производитель не указан')}</div>
                <div class="device-card__specs">${specTags}</div>
                <div class="device-card__home">🏠 ${d.homeName ? escapeHtml(d.homeName) : '— без дома'}</div>
            </div>
            <div class="device-card__actions">
                <button class="method-btn btn-sm" onclick="callDeviceMethod(${d.id}, 'displayInfo')">📄 Инфо</button>
                <button class="method-btn btn-sm" onclick="callDeviceMethod(${d.id}, 'turnOn')">🔌 Вкл</button>
                <button class="method-btn btn-sm" onclick="callDeviceMethod(${d.id}, 'turnOff')">🔌 Выкл</button>
                <button class="method-btn btn-sm" onclick="callDeviceMethod(${d.id}, 'calculatePowerConsumption')">⚡ Энергия</button>
                <button class="method-btn btn-sm" onclick="callDeviceMethod(${d.id}, 'getDeviceType')">📋 Тип</button>
                <button class="method-btn btn-sm" onclick="callDeviceMethod(${d.id}, 'getShortDescription')">📝 Описание</button>
                <button class="delete-btn btn-sm" onclick="deleteDevice(${d.id})">🗑️</button>
            </div>
        </div>`;
    }).join('');
}

function buildDeviceSpecTags(d, type) {
    if (type === 'camera') {
        let tags = `<span class="device-card__spec">📷 ${d.resolution || '-'}p</span>`;
        tags += `<span class="device-card__spec">⏺ ${d.isRecording ? 'Да' : 'Нет'}</span>`;
        tags += `<span class="device-card__spec">🏃 ${d.motionDetection ? 'Да' : 'Нет'}</span>`;
        if (d.storageUsed !== undefined) {
            tags += `<span class="device-card__spec device-card__spec--bar">
                <span class="spec-bar-label">💾 ${d.storageUsed}%</span>
                <span class="spec-bar-track"><span class="spec-bar-fill spec-bar-fill--cam" style="width:${d.storageUsed}%"></span></span>
            </span>`;
        }
        return tags;
    }
    if (type === 'doorlock') {
        let tags = `<span class="device-card__spec">${d.isLocked ? '🔒 Заперт' : '🔓 Открыт'}</span>`;
        tags += `<span class="device-card__spec">🔐 Попыток: ${d.wrongAttempts || 0}</span>`;
        tags += `<span class="device-card__spec">🔒 Авто: ${d.autoLock ? 'Да' : 'Нет'}</span>`;
        return tags;
    }
    if (type === 'light') {
        const b = d.brightness !== undefined ? d.brightness : 0;
        let tags = `<span class="device-card__spec device-card__spec--bar">
            <span class="spec-bar-label">💡 ${b}%</span>
            <span class="spec-bar-track"><span class="spec-bar-fill spec-bar-fill--light" style="width:${b}%"></span></span>
        </span>`;
        tags += `<span class="device-card__spec">🎨 ${d.color || 'white'}</span>`;
        tags += `<span class="device-card__spec">🤖 ${d.autoMode ? 'Авто' : 'Ручной'}</span>`;
        return tags;
    }
    if (type === 'thermostat') {
        let tags = `<span class="device-card__spec">🌡️ ${d.currentTemperature || '-'}°C</span>`;
        tags += `<span class="device-card__spec">🎯 ${d.targetTemperature || '-'}°C</span>`;
        tags += `<span class="device-card__spec">⚙️ ${d.mode || 'auto'}</span>`;
        if (d.humidity !== undefined) {
            tags += `<span class="device-card__spec">💧 ${d.humidity}%</span>`;
        }
        return tags;
    }
    return `<span class="device-card__spec">📋 Тип: ${d.deviceType || d.type || '-'}</span>`;
}

async function callDeviceMethod(id, methodName) {
    showLoading(true);
    try {
        const response = await fetch(`${API_BASE}/device/call_method`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ id, methodInfo: { methodName, args: [] } })
        });
        const result = await response.json();
        if (result.status === 'success') {
            if (result.data !== undefined) {
                if (typeof result.data === 'object') {
                    showInfoModal(methodName, result.data);
                } else {
                    showNotificationWithOutput(getRussianName(methodName), String(result.data));
                }
            } else {
                showNotificationWithOutput(getRussianName(methodName), result.message || 'Выполнено');
            }
            if (methodName === 'turnOn' || methodName === 'turnOff') {
                await loadAllData();
            }
        } else {
            showNotification(result.message || 'Ошибка', 'error');
        }
    } catch (error) {
        showNotification('Ошибка вызова метода', 'error');
    } finally {
        showLoading(false);
    }
}

async function deleteDevice(id) {
    if (!confirm('Удалить устройство?')) return;
    try {
        const response = await fetch(`${API_BASE}/device/${id}`, { method: 'DELETE' });
        if (!response.ok) throw new Error('Failed');
        showNotification('Устройство удалено', 'success');
        await loadAllData();
    } catch (error) {
        showNotification('Ошибка удаления', 'error');
    }
}

// ============ CAMERAS ============
async function loadCameras() {
    if (!selectedHomeId) {
        document.getElementById('cameraTableBody').innerHTML = '<tr><td colspan="9" style="text-align:center;color:#999;">Выберите дом в панели «🏠 Контекст»</td></tr>';
        return;
    }
    try {
        const response = await fetch(`${API_BASE}/camera?homeId=${selectedHomeId}`);
        if (!response.ok) throw new Error('Failed');
        const cameras = await response.json();
        displayCameras(cameras);
    } catch (error) {
        console.error('Error loading cameras:', error);
    }
}

function displayCameras(cameras) {
    const tbody = document.getElementById('cameraTableBody');
    if (!tbody) return;
    if (!cameras || cameras.length === 0) {
        tbody.innerHTML = '<tr><td colspan="9" style="text-align:center;">Нет камер</td></tr>';
        return;
    }
    tbody.innerHTML = cameras.map(c => `
        <tr>
            <td>${c.id || '-'}</td>
            <td>${escapeHtml(c.name || '-')}</td>
            <td>${escapeHtml(c.manufacturer || '-')}</td>
            <td>${c.isOn ? '✅ Вкл' : '❌ Выкл'}</td>
            <td>${c.resolution || '-'}p</td>
            <td>${c.isRecording ? '✅ Да' : '❌ Нет'}</td>
            <td>${c.motionDetection ? '✅ Да' : '❌ Нет'}</td>
            <td>${c.storageUsed || 0}%</td>
            <td class="action-buttons">
                <button class="method-btn" onclick="callCameraMethod(${c.id}, 'displayInfo')">📄 Инфо</button>
                <button class="method-btn" onclick="callCameraMethod(${c.id}, 'turnOn')">🔌 Вкл</button>
                <button class="method-btn" onclick="callCameraMethod(${c.id}, 'turnOff')">🔌 Выкл</button>
                <button class="method-btn" onclick="callCameraMethod(${c.id}, 'startRecording')">⏺ Запись</button>
                <button class="method-btn" onclick="callCameraMethod(${c.id}, 'recordMotion')">🏃 Движение</button>
                <button class="method-btn" onclick="callCameraMethod(${c.id}, 'calculatePowerConsumption')">⚡ Энергия</button>
                <button class="delete-btn" onclick="deleteCamera(${c.id})">🗑️</button>
            </td>
        </tr>
    `).join('');
}

async function callCameraMethod(id, methodName) {
    showLoading(true);
    try {
        const response = await fetch(`${API_BASE}/camera/call_method`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ id, methodInfo: { methodName, args: [] } })
        });
        const result = await response.json();
        if (result.status === 'success') {
            if (result.data !== undefined) {
                if (typeof result.data === 'object') {
                    showInfoModal(methodName, result.data);
                } else {
                    showNotificationWithOutput(getRussianName(methodName), String(result.data));
                }
            } else {
                showNotificationWithOutput(getRussianName(methodName), result.message || 'Выполнено');
            }
            await loadAllData();
        } else {
            showNotification(result.message || 'Ошибка', 'error');
        }
    } catch (error) {
        showNotification('Ошибка', 'error');
    } finally {
        showLoading(false);
    }
}

async function deleteCamera(id) {
    if (!confirm('Удалить камеру?')) return;
    try {
        const response = await fetch(`${API_BASE}/camera/${id}`, { method: 'DELETE' });
        if (!response.ok) throw new Error('Failed');
        showNotification('Камера удалена', 'success');
        await loadAllData();
    } catch (error) {
        showNotification('Ошибка', 'error');
    }
}

// ============ DOORLOCKS ============
async function loadDoorLocks() {
    if (!selectedHomeId) {
        document.getElementById('doorlockTableBody').innerHTML = '<tr><td colspan="9" style="text-align:center;color:#999;">Выберите дом в панели «🏠 Контекст»</td></tr>';
        return;
    }
    try {
        const response = await fetch(`${API_BASE}/doorlock?homeId=${selectedHomeId}`);
        if (!response.ok) throw new Error('Failed');
        const locks = await response.json();
        displayDoorLocks(locks);
    } catch (error) {
        console.error('Error loading doorlocks:', error);
    }
}

function displayDoorLocks(locks) {
    const tbody = document.getElementById('doorlockTableBody');
    if (!tbody) return;
    if (!locks || locks.length === 0) {
        tbody.innerHTML = '<tr><td colspan="9" style="text-align:center;">Нет замков</td></tr>';
        return;
    }
    tbody.innerHTML = locks.map(l => `
        <tr>
            <td>${l.id || '-'}</td>
            <td>${escapeHtml(l.name || '-')}</td>
            <td>${escapeHtml(l.manufacturer || '-')}</td>
            <td>${l.isOn ? '✅ Вкл' : '❌ Выкл'}</td>
            <td>${l.isLocked ? '🔒 Заперт' : '🔓 Открыт'}</td>
            <td>${l.autoLock ? '✅ Да' : '❌ Нет'}</td>
            <td>${l.wrongAttempts || 0}</td>
            <td>${l.lastAccessTime || 'Никогда'}</td>
            <td class="action-buttons">
                <button class="method-btn" onclick="callDoorLockMethod(${l.id}, 'displayInfo')">📄 Инфо</button>
                <button class="method-btn" onclick="callDoorLockMethod(${l.id}, 'turnOn')">🔌 Вкл</button>
                <button class="method-btn" onclick="callDoorLockMethod(${l.id}, 'turnOff')">🔌 Выкл</button>
                <button class="method-btn" onclick="showUnlockModal(${l.id})">🔓 Открыть</button>
                <button class="method-btn" onclick="callDoorLockMethod(${l.id}, 'getLockInfo')">🔒 Инфо замка</button>
                <button class="delete-btn" onclick="deleteDoorLock(${l.id})">🗑️</button>
            </td>
        </tr>
    `).join('');
}

function showUnlockModal(id) {
    showInputModal('🔓 Открыть замок', 'Код доступа:', 'text', async (code) => {
        if (!code) { showNotification('Введите код!', 'error'); return; }
        showLoading(true);
        try {
            const response = await fetch(`${API_BASE}/doorlock/call_method`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ id, methodInfo: { methodName: 'tryUnlock', args: [] }, code })
            });
            const result = await response.json();
            if (result.status === 'success') {
                showNotificationWithOutput('Открытие замка', result.message || (result.data ? '✅ Дверь открыта' : '❌ Не удалось'));
                await loadAllData();
            } else {
                showNotification(result.message || 'Ошибка', 'error');
            }
        } catch (error) {
            showNotification('Ошибка', 'error');
        } finally {
            showLoading(false);
        }
    });
}

async function callDoorLockMethod(id, methodName) {
    showLoading(true);
    try {
        const response = await fetch(`${API_BASE}/doorlock/call_method`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ id, methodInfo: { methodName, args: [] } })
        });
        const result = await response.json();
        if (result.status === 'success') {
            if (result.data !== undefined) {
                if (typeof result.data === 'object') {
                    showInfoModal(methodName, result.data);
                } else {
                    showNotificationWithOutput(getRussianName(methodName), String(result.data));
                }
            } else {
                showNotificationWithOutput(getRussianName(methodName), result.message || 'Выполнено');
            }
            await loadAllData();
        } else {
            showNotification(result.message || 'Ошибка', 'error');
        }
    } catch (error) {
        showNotification('Ошибка', 'error');
    } finally {
        showLoading(false);
    }
}

async function deleteDoorLock(id) {
    if (!confirm('Удалить замок?')) return;
    try {
        const response = await fetch(`${API_BASE}/doorlock/${id}`, { method: 'DELETE' });
        if (!response.ok) throw new Error('Failed');
        showNotification('Замок удален', 'success');
        await loadAllData();
    } catch (error) {
        showNotification('Ошибка', 'error');
    }
}

// ============ LIGHTS ============
async function loadLights() {
    if (!selectedHomeId) {
        document.getElementById('lightTableBody').innerHTML = '<tr><td colspan="8" style="text-align:center;color:#999;">Выберите дом в панели «🏠 Контекст»</td></tr>';
        return;
    }
    try {
        const response = await fetch(`${API_BASE}/light?homeId=${selectedHomeId}`);
        if (!response.ok) throw new Error('Failed');
        const lights = await response.json();
        displayLights(lights);
    } catch (error) {
        console.error('Error loading lights:', error);
    }
}

function displayLights(lights) {
    const tbody = document.getElementById('lightTableBody');
    if (!tbody) return;
    if (!lights || lights.length === 0) {
        tbody.innerHTML = '<tr><td colspan="8" style="text-align:center;">Нет ламп</td></tr>';
        return;
    }
    tbody.innerHTML = lights.map(l => `
        <tr>
            <td>${l.id || '-'}</td>
            <td>${escapeHtml(l.name || '-')}</td>
            <td>${escapeHtml(l.manufacturer || '-')}</td>
            <td>${l.isOn ? '✅ Вкл' : '❌ Выкл'}</td>
            <td>${l.brightness !== undefined ? l.brightness + '%' : '-'}</td>
            <td>${l.color || 'white'}</td>
            <td>${l.autoMode ? '✅ Да' : '❌ Нет'}</td>
            <td class="action-buttons">
                <button class="method-btn" onclick="callLightMethod(${l.id}, 'displayInfo')">📄 Инфо</button>
                <button class="method-btn" onclick="callLightMethod(${l.id}, 'turnOn')">🔌 Вкл</button>
                <button class="method-btn" onclick="callLightMethod(${l.id}, 'turnOff')">🔌 Выкл</button>
                <button class="method-btn" onclick="showBrightnessModal(${l.id})">💡 Яркость</button>
                <button class="method-btn" onclick="callLightMethod(${l.id}, 'getLightInfo')">💡 Инфо лампы</button>
                <button class="delete-btn" onclick="deleteLight(${l.id})">🗑️</button>
            </td>
        </tr>
    `).join('');
}

function showBrightnessModal(id) {
    showInputModal('💡 Установить яркость', 'Яркость (0-100%):', 'number', async (level) => {
        const numLevel = parseInt(level);
        if (isNaN(numLevel) || numLevel < 0 || numLevel > 100) {
            showNotification('Введите число от 0 до 100', 'error');
            return;
        }
        showLoading(true);
        try {
            const response = await fetch(`${API_BASE}/light/call_method`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ id, level: numLevel, methodInfo: { methodName: 'setBrightness', args: [] } })
            });
            const result = await response.json();
            if (result.status === 'success') {
                showNotification(result.message || 'Яркость установлена', 'success');
                await loadAllData();
            } else {
                showNotification(result.message || 'Ошибка', 'error');
            }
        } catch (error) {
            showNotification('Ошибка', 'error');
        } finally {
            showLoading(false);
        }
    });
}

async function callLightMethod(id, methodName) {
    showLoading(true);
    try {
        const response = await fetch(`${API_BASE}/light/call_method`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ id, methodInfo: { methodName, args: [] } })
        });
        const result = await response.json();
        if (result.status === 'success') {
            if (result.data !== undefined) {
                if (typeof result.data === 'object') {
                    showInfoModal(methodName, result.data);
                } else {
                    showNotificationWithOutput(getRussianName(methodName), String(result.data));
                }
            } else {
                showNotificationWithOutput(getRussianName(methodName), result.message || 'Выполнено');
            }
            await loadAllData();
        } else {
            showNotification(result.message || 'Ошибка', 'error');
        }
    } catch (error) {
        showNotification('Ошибка', 'error');
    } finally {
        showLoading(false);
    }
}

async function deleteLight(id) {
    if (!confirm('Удалить лампу?')) return;
    try {
        const response = await fetch(`${API_BASE}/light/${id}`, { method: 'DELETE' });
        if (!response.ok) throw new Error('Failed');
        showNotification('Лампа удалена', 'success');
        await loadAllData();
    } catch (error) {
        showNotification('Ошибка', 'error');
    }
}

// ============ THERMOSTATS ============
async function loadThermostats() {
    if (!selectedHomeId) {
        document.getElementById('thermostatTableBody').innerHTML = '<tr><td colspan="9" style="text-align:center;color:#999;">Выберите дом в панели «🏠 Контекст»</td></tr>';
        return;
    }
    try {
        const response = await fetch(`${API_BASE}/thermostat?homeId=${selectedHomeId}`);
        if (!response.ok) throw new Error('Failed');
        const thermostats = await response.json();
        displayThermostats(thermostats);
    } catch (error) {
        console.error('Error loading thermostats:', error);
    }
}

function displayThermostats(thermostats) {
    const tbody = document.getElementById('thermostatTableBody');
    if (!tbody) return;
    if (!thermostats || thermostats.length === 0) {
        tbody.innerHTML = '<tr><td colspan="9" style="text-align:center;">Нет термостатов</td></tr>';
        return;
    }
    tbody.innerHTML = thermostats.map(t => `
        <tr>
            <td>${t.id || '-'}</td>
            <td>${escapeHtml(t.name || '-')}</td>
            <td>${escapeHtml(t.manufacturer || '-')}</td>
            <td>${t.isOn ? '✅ Вкл' : '❌ Выкл'}</td>
            <td>${t.currentTemperature !== undefined ? t.currentTemperature + '°C' : '-'}</td>
            <td>${t.targetTemperature !== undefined ? t.targetTemperature + '°C' : '-'}</td>
            <td>${t.mode || 'auto'}</td>
            <td>${t.humidity !== undefined ? t.humidity + '%' : '-'}</td>
            <td class="action-buttons">
                <button class="method-btn" onclick="callThermostatMethod(${t.id}, 'displayInfo')">📄 Инфо</button>
                <button class="method-btn" onclick="callThermostatMethod(${t.id}, 'turnOn')">🔌 Вкл</button>
                <button class="method-btn" onclick="callThermostatMethod(${t.id}, 'turnOff')">🔌 Выкл</button>
                <button class="method-btn" onclick="showTemperatureModal(${t.id})">🌡️ Tемп</button>
                <button class="method-btn" onclick="callThermostatMethod(${t.id}, 'getClimateInfo')">🌤️ Климат</button>
                <button class="delete-btn" onclick="deleteThermostat(${t.id})">🗑️</button>
            </td>
        </tr>
    `).join('');
}

function showTemperatureModal(id) {
    showInputModal('🌡️ Установить температуру', 'Температура (°C):', 'number', async (temp) => {
        const numTemp = parseFloat(temp);
        if (isNaN(numTemp)) {
            showNotification('Введите корректную температуру', 'error');
            return;
        }
        showLoading(true);
        try {
            const response = await fetch(`${API_BASE}/thermostat/call_method`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ id, temperature: numTemp, methodInfo: { methodName: 'setTemperature', args: [] } })
            });
            const result = await response.json();
            if (result.status === 'success') {
                showNotification(result.message || 'Температура установлена', 'success');
                await loadAllData();
            } else {
                showNotification(result.message || 'Ошибка', 'error');
            }
        } catch (error) {
            showNotification('Ошибка', 'error');
        } finally {
            showLoading(false);
        }
    });
}

async function callThermostatMethod(id, methodName) {
    showLoading(true);
    try {
        const response = await fetch(`${API_BASE}/thermostat/call_method`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ id, methodInfo: { methodName, args: [] } })
        });
        const result = await response.json();
        if (result.status === 'success') {
            if (result.data !== undefined) {
                if (typeof result.data === 'object') {
                    showInfoModal(methodName, result.data);
                } else {
                    showNotificationWithOutput(getRussianName(methodName), String(result.data));
                }
            } else {
                showNotificationWithOutput(getRussianName(methodName), result.message || 'Выполнено');
            }
            await loadAllData();
        } else {
            showNotification(result.message || 'Ошибка', 'error');
        }
    } catch (error) {
        showNotification('Ошибка', 'error');
    } finally {
        showLoading(false);
    }
}

async function deleteThermostat(id) {
    if (!confirm('Удалить термостат?')) return;
    try {
        const response = await fetch(`${API_BASE}/thermostat/${id}`, { method: 'DELETE' });
        if (!response.ok) throw new Error('Failed');
        showNotification('Термостат удален', 'success');
        await loadAllData();
    } catch (error) {
        showNotification('Ошибка', 'error');
    }
}

// ============ CONSTRUCTORS ============
function renderAllConstructors() {
    renderConstructors('camera', cameraConstructors, 'Camera');
    renderConstructors('doorlock', doorlockConstructors, 'DoorLock');
    renderConstructors('light', lightConstructors, 'Light');
    renderConstructors('thermostat', thermostatConstructors, 'Thermostat');
}

function renderConstructors(type, constructors, typeClass) {
    const container = document.getElementById(`${type}ConstructorsContainer`);
    if (!container) return;

    container.innerHTML = constructors.map((constructor, idx) => {
        const isDefault = constructor.endpoint === '/default';
        return `
        <div class="constructor-card">
            <h4>${constructor.name}</h4>
            <p class="constructor-desc">${constructor.description}</p>
            <div class="constructor-fields">
                ${renderConstructorFields(constructor.fields, type, idx)}
                ${!isDefault ? `
                <div class="field-group">
                    <label>Выбрать дом:</label>
                    <select id="home_select_${type}_${idx}" class="home-select">
                        <option value="">-- Без дома --</option>
                        ${homes.map(h => `<option value="${h.id}">${escapeHtml(h.name)} (ID: ${h.id})</option>`).join('')}
                    </select>
                </div>` : ''}
            </div>
            <button class="btn btn-primary" onclick="createDevice('${type}', ${idx})">Создать</button>
        </div>`;
    }).join('');
}

function renderConstructorFields(fields, type, idx) {
    if (!fields || fields.length === 0) return '<p style="color:#999;">Без параметров</p>';
    return fields.map(field => {
        if (field.type === 'checkbox') {
            return `<div class="field-group checkbox"><label>${field.label}:</label><input type="checkbox" id="${field.name}_${type}_${idx}"></div>`;
        }
        return `<div class="field-group"><label>${field.label}:</label><input type="${field.type}" id="${field.name}_${type}_${idx}" ${field.min !== undefined ? `min="${field.min}"` : ''} ${field.max !== undefined ? `max="${field.max}"` : ''} ${field.required ? 'required' : ''}></div>`;
    }).join('');
}

async function createDevice(type, constructorIdx) {
    let constructors;
    if (type === 'camera') constructors = cameraConstructors;
    else if (type === 'doorlock') constructors = doorlockConstructors;
    else if (type === 'light') constructors = lightConstructors;
    else if (type === 'thermostat') constructors = thermostatConstructors;
    else return;

    const constructor = constructors[constructorIdx];
    const data = {};

    for (const field of constructor.fields) {
        const input = document.getElementById(`${field.name}_${type}_${constructorIdx}`);
        if (input) {
            if (field.type === 'checkbox') {
                data[field.name] = input.checked;
            } else {
                const value = input.value.trim();
                if (value) {
                    data[field.name] = field.type === 'number' ? Number(value) : value;
                } else if (field.defaultVal !== undefined) {
                    data[field.name] = field.type === 'number' ? Number(field.defaultVal) : field.defaultVal;
                }
            }
        }
    }

    const homeSelect = document.getElementById(`home_select_${type}_${constructorIdx}`);
    if (homeSelect && homeSelect.value) {
        data.homeId = parseInt(homeSelect.value);
    }

    showLoading(true);
    try {
        const url = `${API_BASE}/${type}${constructor.endpoint}`;
        const response = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        if (!response.ok) throw new Error('Failed to create');
        showNotification(`${getTypeName(type)} создан(а)`, 'success');
        await loadAllData();
        for (const field of constructor.fields) {
            const input = document.getElementById(`${field.name}_${type}_${constructorIdx}`);
            if (input) {
                if (field.type === 'checkbox') input.checked = false;
                else input.value = '';
            }
        }
        if (homeSelect) homeSelect.value = '';
    } catch (error) {
        showNotification(`Ошибка создания ${getTypeName(type)}`, 'error');
    } finally {
        showLoading(false);
    }
}

function getTypeName(type) {
    const names = { camera: 'Камера', doorlock: 'Замок', light: 'Лампа', thermostat: 'Термостат' };
    return names[type] || type;
}

// ============ MODALS ============
function showContentModal(title, contentBuilder) {
    closeModal();
    const modal = document.createElement('div');
    modal.className = 'modal';
    modal.id = 'contentModal';
    modal.innerHTML = `
        <div class="modal-content" style="max-width: 900px; width: 95%; max-height: 80vh; overflow-y: auto;">
            <div class="modal-header">
                <strong>${title}</strong>
                <button class="modal-close" onclick="closeModal()">&times;</button>
            </div>
            <div class="modal-body" id="contentModalBody"></div>
        </div>
    `;
    document.body.appendChild(modal);
    contentBuilder(document.getElementById('contentModalBody'));
    modal.addEventListener('click', (e) => { if (e.target === modal) closeModal(); });
}

function showInfoModal(title, data) {
    closeModal();
    const fieldNames = {
        id: '🆔 ID', name: '📛 Название', manufacturer: '🏭 Производитель',
        isOn: '🔌 Включен', type: '📋 Тип', deviceType: '📋 Тип устройства',
        resolution: '📷 Разрешение', resolutionText: '📷 Разрешение',
        isRecording: '⏺ Запись', recording: '⏺ Запись',
        motionDetection: '🏃 Детекция', storageUsed: '💾 Память',
        lastMotionTime: '⏰ Последнее движение', warning: '⚠️ Предупреждение',
        isLocked: '🔒 Заперт', accessCode: '🔑 Код', wrongAttempts: '❌ Попыток',
        autoLock: '🔒 Автоблокировка', lastAccessTime: '⏰ Последний доступ',
        status: '📊 Статус', createdAt: '📅 Создан',
        brightness: '💡 Яркость', color: '🎨 Цвет', autoMode: '🤖 Авторежим',
        scheduledTime: '⏱ Расписание',
        currentTemperature: '🌡️ Текущая T', targetTemperature: '🎯 Целевая T',
        mode: '⚙️ Режим', humidity: '💧 Влажность',
        homeName: '🏠 Дом', homeId: '🏠 ID дома',
        isLocked: '🔒 Заперт', autoLock: '🔒 Автоблокировка'
    };
    const order = ['name', 'manufacturer', 'isOn', 'resolution', 'resolutionText', 'isRecording', 'recording',
        'motionDetection', 'storageUsed', 'lastMotionTime', 'brightness', 'color', 'autoMode',
        'isLocked', 'accessCode', 'wrongAttempts', 'autoLock', 'lastAccessTime',
        'currentTemperature', 'targetTemperature', 'mode', 'humidity',
        'deviceType', 'type', 'homeName', 'status', 'createdAt'];
    const excluded = ['warning'];

    let html = '';
    let hasData = false;
    for (const key of order) {
        if (data.hasOwnProperty(key) && data[key] !== undefined && data[key] !== null && data[key] !== '') {
            if (excluded.includes(key)) continue;
            hasData = true;
            const label = fieldNames[key] || key;
            let val = data[key];
            if (typeof val === 'boolean') val = val ? '✅ Да' : '❌ Нет';
            if (key === 'resolution' && typeof val === 'number') val = val + 'p';
            html += `<div style="margin:6px 0;padding:4px 0;border-bottom:1px solid #eee;">
                <span style="font-weight:bold;">${label}:</span> <span>${escapeHtml(String(val))}</span></div>`;
        }
    }
    for (const [key, value] of Object.entries(data)) {
        if (order.includes(key) || excluded.includes(key)) continue;
        if (value === undefined || value === null || value === '') continue;
        hasData = true;
        const label = fieldNames[key] || key;
        let val = value;
        if (typeof val === 'boolean') val = val ? '✅ Да' : '❌ Нет';
        html += `<div style="margin:6px 0;padding:4px 0;border-bottom:1px solid #eee;">
            <span style="font-weight:bold;">${label}:</span> <span>${escapeHtml(String(val))}</span></div>`;
    }
    if (data.warning) {
        html += `<div style="margin:10px 0;padding:8px;background:#ffebee;border-radius:6px;color:#c62828;font-weight:bold;">${escapeHtml(data.warning)}</div>`;
    }
    if (!hasData) html = '<p style="color:#999;">Нет данных для отображения</p>';

    const modal = document.createElement('div');
    modal.className = 'modal';
    modal.id = 'infoModal';
    modal.innerHTML = `
        <div class="modal-content" style="max-width:500px;">
            <div class="modal-header">
                <strong>${getRussianName(title)}</strong>
                <button class="modal-close" onclick="closeModal()">&times;</button>
            </div>
            <div class="modal-body">${html}</div>
        </div>
    `;
    document.body.appendChild(modal);
    modal.addEventListener('click', (e) => { if (e.target === modal) closeModal(); });
}

function showInputModal(title, label, inputType, onSubmit) {
    closeModal();
    const modal = document.createElement('div');
    modal.className = 'modal';
    modal.id = 'inputModal';
    modal.innerHTML = `
        <div class="modal-content" style="max-width:400px;">
            <div class="modal-header">
                <strong>${title}</strong>
                <button class="modal-close" onclick="closeModal()">&times;</button>
            </div>
            <div class="modal-body">
                <label style="display:block;margin-bottom:8px;font-weight:bold;">${label}</label>
                <input type="${inputType}" id="modalInput" style="width:100%;padding:10px;border:2px solid #e0e0e5;border-radius:8px;font-size:14px;">
            </div>
            <div class="modal-footer">
                <button onclick="closeModal()" style="background:#999;color:white;margin-right:8px;">Отмена</button>
                <button onclick="submitInputModal('${onSubmit.toString()}')" style="background:linear-gradient(135deg,#667eea,#764ba2);color:white;">OK</button>
            </div>
        </div>
    `;
    document.body.appendChild(modal);
    window._inputModalCallback = onSubmit;
    modal.addEventListener('click', (e) => { if (e.target === modal) closeModal(); });
    setTimeout(() => document.getElementById('modalInput')?.focus(), 100);
}

function submitInputModal() {
    const input = document.getElementById('modalInput');
    const val = input ? input.value : '';
    if (window._inputModalCallback) {
        window._inputModalCallback(val);
    }
    closeModal();
}

function closeModal() {
    document.querySelectorAll('.modal').forEach(m => m.remove());
    window._inputModalCallback = null;
}

// ============ HELPERS ============
function getRussianName(methodName) {
    const names = {
        'displayInfo': '📄 Информация',
        'calculatePowerConsumption': '⚡ Энергопотребление',
        'turnOn': '🔌 Включение',
        'turnOff': '🔌 Выключение',
        'getDeviceType': '📋 Тип устройства',
        'getShortDescription': '📝 Краткое описание',
        'getDetailedInfo': '📋 Детальная информация',
        'startRecording': '⏺ Запуск записи',
        'recordMotion': '🏃 Запись движения',
        'tryUnlock': '🔓 Попытка открыть',
        'getLockInfo': '🔒 Информация о замке',
        'setBrightness': '💡 Установка яркости',
        'getLightInfo': '💡 Информация о лампе',
        'setTemperature': '🌡️ Установка температуры',
        'getClimateInfo': '🌤️ Информация о климате'
    };
    return names[methodName] || methodName;
}

function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function showLoading(show) {
    const loader = document.getElementById('loadingIndicator');
    if (loader) loader.style.display = show ? 'block' : 'none';
}

function showError(message) {
    const errorMsg = document.getElementById('errorMessage');
    if (errorMsg) {
        errorMsg.textContent = message;
        errorMsg.style.display = 'block';
        setTimeout(() => errorMsg.style.display = 'none', 5000);
    }
}

function showNotification(message, type = 'info') {
    const notification = document.getElementById('notification');
    if (!notification) return;
    if (notificationTimeout) clearTimeout(notificationTimeout);
    notification.textContent = message;
    notification.className = `notification ${type} show`;
    notificationTimeout = setTimeout(() => {
        notification.classList.remove('show');
        notificationTimeout = null;
    }, 4000);
}

function showNotificationWithOutput(methodName, output) {
    const notification = document.getElementById('notification');
    if (!notification) return;
    if (notificationTimeout) clearTimeout(notificationTimeout);
    let formattedOutput = output;
    if (typeof output === 'string' && output.includes('\n')) {
        formattedOutput = output.replace(/\n/g, '<br>');
    }
    notification.innerHTML = `
        <strong>${methodName}</strong><br>
        <div style="font-family:monospace;font-size:12px;margin-top:8px;max-height:200px;overflow-y:auto;">${formattedOutput}</div>
    `;
    notification.className = 'notification success show';
    notificationTimeout = setTimeout(() => {
        notification.classList.remove('show');
        notificationTimeout = null;
    }, 6000);
}
