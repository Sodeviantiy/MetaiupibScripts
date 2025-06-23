using UnityEngine;
using Michsky.MUIP;
using Photon.Voice.PUN;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using TMPro;
using Photon.Voice.Unity;
using System.Linq;

public class MicrophoneButtonController : MonoBehaviour
{
    public PunVoiceClient voiceClient;
    public ButtonManager microphoneOnButton;
    public ButtonManager microphoneOffButton;
    public GameObject microphoneVisual;
    public ButtonManager calibrateButton; // Кнопка для калибровки
    public TMP_Text levelMeterText; // Текст для отображения уровня громкости
    [SerializeField] private float calibrationTime = 2f; // Время калибровки в секундах
    [SerializeField] private Toggle muteToggle; // Переключатель для отключения звука
    [SerializeField] private Toggle voiceDetectionToggle; // Переключатель для определения голоса
    [SerializeField] private Toggle debugEchoToggle; // Переключатель для режима отладки эха
    [SerializeField] private Toggle debugVoiceToggle; // Переключатель для отладки голоса
    [SerializeField] private TMP_Text debugText; // Текст для отображения отладочной информации
    [SerializeField] private GameObject debugPanel; // Панель для отображения отладочной информации
    [SerializeField] private TMP_Text punStateText; // Текст для отображения состояния PUN
    [SerializeField] private TMP_Text voiceStateText; // Текст для отображения состояния Voice
    [SerializeField] private TMP_Text voiceDebugText; // Текст для отображения отладочной информации о голосе
    [SerializeField] private TMP_Text devicesInfoText; // Текст для отображения информации о устройствах
    [SerializeField] private TMP_Text speakerInfoText; // Текст для отображения информации о динамике

    private PhotonVoiceView recorder;
    private bool isMicrophoneEnabled = true;
    private float volumeBeforeMute;
    private float calibrationStartTime; // Время начала калибровки
    private bool debugMode;
    private DebugLevel previousDebugLevel;
    private string debugLog = ""; // Буфер для хранения отладочных сообщений
    private const int maxDebugLines = 10; // Максимальное количество строк в отладочном выводе

    public delegate void OnDebugModeChanged(bool debugMode);
    public event OnDebugModeChanged DebugModeChanged;

    public bool DebugMode
    {
        get { return this.debugMode; }
        set
        {
            this.debugMode = value;
            if (this.debugMode)
            {
                this.previousDebugLevel = this.voiceClient.Client.LoadBalancingPeer.DebugOut;
                this.voiceClient.Client.LoadBalancingPeer.DebugOut = DebugLevel.ALL;
                if (debugPanel != null) debugPanel.SetActive(true);
            }
            else
            {
                this.voiceClient.Client.LoadBalancingPeer.DebugOut = this.previousDebugLevel;
                if (debugPanel != null) debugPanel.SetActive(false);
            }
            DebugModeChanged?.Invoke(this.debugMode);
        }
    }

    void Start()
    {
        // Find PunVoiceClient in scene
        voiceClient = Object.FindFirstObjectByType<PunVoiceClient>();

        if (voiceClient == null)
        {
            Debug.LogError("PunVoiceClient not found in scene! Please check if it exists.");
            return;
        }

        // Check if buttons are assigned
        if (microphoneOnButton == null || microphoneOffButton == null)
        {
            Debug.LogError("Microphone buttons are not assigned!");
            return;
        }

        // Проверяем наличие визуального объекта микрофона
        if (microphoneVisual == null)
        {
            Debug.LogWarning("Microphone visual object is not assigned!");
        }
        else
        {
            microphoneVisual.SetActive(true);
        }

        // Инициализируем переключатели
        InitializeToggles();

        // Подписываемся на события
       // CharacterInstantiation.CharacterInstantiated += OnCharacterInstantiated;
        voiceClient.Client.StateChanged += OnVoiceClientStateChanged;
        PhotonNetwork.NetworkingClient.StateChanged += OnPunClientStateChanged;

        // Initialize button states
        UpdateButtonsVisibility();
        UpdateCalibrateButton();

        // Инициализируем отладочную панель
        if (debugPanel != null) debugPanel.SetActive(false);

        // Выводим информацию о доступных микрофонах
        UpdateDevicesInfo();

        isMicrophoneEnabled = false;
        if (recorder != null && recorder.RecorderInUse != null)
        {
            recorder.RecorderInUse.TransmitEnabled = false;
        }

        // Обновляем информацию о состоянии PUN и Voice
        if (punStateText != null)
        {
            punStateText.text = string.Format("PUN: {0}", PhotonNetwork.NetworkClientState);
        }
        if (voiceStateText != null)
        {
            voiceStateText.text = string.Format("PhotonVoice: {0}", voiceClient.ClientState);
        }
    }

    private void UpdateDevicesInfo()
    {
        if (devicesInfoText != null)
        {
            using (var unityMicEnum = new AudioInEnumerator(this.voiceClient.Logger))
            {
                using (var photonMicEnum = Photon.Voice.Platform.CreateAudioInEnumerator(this.voiceClient.Logger))
                {
                    var unityMic = unityMicEnum.FirstOrDefault();
                    var photonMic = photonMicEnum.FirstOrDefault();

                    if (unityMic == null && photonMic == null)
                    {
                        devicesInfoText.enabled = true;
                        devicesInfoText.color = Color.red;
                        devicesInfoText.text = "Микрофоны не обнаружены!";
                    }
                    else
                    {
                        devicesInfoText.text = "Используемый микрофон: " + (unityMic != null ? unityMic.ToString() : photonMic.ToString());
                        
                    }
                }
            }
        }
    }

    private void InitializeToggles()
    {
        if (muteToggle != null)
        {
            muteToggle.isOn = AudioListener.volume <= 0.001f;
            muteToggle.onValueChanged.AddListener(OnMuteToggleChanged);
        }

        if (voiceDetectionToggle != null)
        {
            voiceDetectionToggle.onValueChanged.AddListener(OnVoiceDetectionToggleChanged);
        }

        if (debugEchoToggle != null)
        {
            debugEchoToggle.isOn = false; // Устанавливаем режим эха выключенным по умолчанию
            debugEchoToggle.onValueChanged.AddListener(OnDebugEchoToggleChanged);
        }

        if (debugVoiceToggle != null)
        {
            debugVoiceToggle.isOn = this.DebugMode;
            debugVoiceToggle.onValueChanged.AddListener(OnDebugVoiceToggleChanged);
        }
    }

    private void OnMuteToggleChanged(bool isOn)
    {
        if (isOn)
        {
            this.volumeBeforeMute = AudioListener.volume;
            AudioListener.volume = 0f;
        }
        else
        {
            AudioListener.volume = this.volumeBeforeMute;
            this.volumeBeforeMute = 0f;
        }
    }

    private void OnVoiceDetectionToggleChanged(bool isOn)
    {
        if (recorder != null && recorder.RecorderInUse != null)
        {
            recorder.RecorderInUse.VoiceDetection = isOn;
        }
    }

    private void OnDebugEchoToggleChanged(bool isOn)
    {
        if (recorder != null && recorder.RecorderInUse != null)
        {
            recorder.RecorderInUse.DebugEchoMode = isOn;
        }
    }

    private void OnDebugVoiceToggleChanged(bool isOn)
    {
        this.DebugMode = isOn;
        if (!isOn)
        {
            debugLog = "";
            if (debugText != null) debugText.text = "";
            if (voiceDebugText != null) voiceDebugText.text = "";
        }
    }

    void OnDestroy()
    {
        // Отписываемся от событий
        //CharacterInstantiation.CharacterInstantiated -= OnCharacterInstantiated;
        if (voiceClient != null)
        {
            voiceClient.Client.StateChanged -= OnVoiceClientStateChanged;
        }
        if (PhotonNetwork.NetworkingClient != null)
        {
            PhotonNetwork.NetworkingClient.StateChanged -= OnPunClientStateChanged;
        }

        // Отписываемся от событий переключателей
        if (muteToggle != null) muteToggle.onValueChanged.RemoveListener(OnMuteToggleChanged);
        if (voiceDetectionToggle != null) voiceDetectionToggle.onValueChanged.RemoveListener(OnVoiceDetectionToggleChanged);
        if (debugEchoToggle != null) debugEchoToggle.onValueChanged.RemoveListener(OnDebugEchoToggleChanged);
        if (debugVoiceToggle != null) debugVoiceToggle.onValueChanged.RemoveListener(OnDebugVoiceToggleChanged);
    }

    void Update()
    {
        // Обновляем уровень громкости и отладочную информацию о голосе
        if (recorder != null && recorder.RecorderInUse != null && recorder.RecorderInUse.LevelMeter != null)
        {
            if (levelMeterText != null)
            {
                levelMeterText.text = string.Format("Уровень: средний {0:0.000000}, пик {1:0.000000}", 
                    recorder.RecorderInUse.LevelMeter.CurrentAvgAmp, 
                    recorder.RecorderInUse.LevelMeter.CurrentPeakAmp);
            }

            if (voiceDebugText != null && debugMode)
            {
                voiceDebugText.text = string.Format("Амплитуда: средняя {0:0.000000}, пиковая {1:0.000000}", 
                    recorder.RecorderInUse.LevelMeter.CurrentAvgAmp, 
                    recorder.RecorderInUse.LevelMeter.CurrentPeakAmp);
            }
        }

        // Обновляем состояние кнопки калибровки
        if (calibrateButton != null && recorder != null && recorder.RecorderInUse != null)
        {
            bool isCalibrating = recorder.RecorderInUse.VoiceDetectorCalibrating;
            if (isCalibrating)
            {
                float remainingTime = (calibrationTime - (Time.time - calibrationStartTime) * 1000) / 1000f;
                if (remainingTime > 0)
                {
                    calibrateButton.SetText($"Калибровка... {remainingTime:F1}с");
                }
                else
                {
                    calibrateButton.SetText("Калибровка...");
                }
            }
            else
            {
                calibrateButton.SetText($"Калибровать ({calibrationTime:F1}с)");
                calibrateButton.Interactable(true);
            }
        }

        // Обновляем отладочную информацию
        if (debugMode)
        {
            if (debugText != null)
            {
                string currentState = $"Состояние PUN: {PhotonNetwork.NetworkClientState}\n" +
                                    $"Состояние Voice: {voiceClient.ClientState}\n" +
                                    $"Микрофон: {(isMicrophoneEnabled ? "Включен" : "Выключен")}\n" +
                                    $"Определение голоса: {(recorder?.RecorderInUse?.VoiceDetection ?? false ? "Включено" : "Выключено")}\n" +
                                    $"Режим эха: {(recorder?.RecorderInUse?.DebugEchoMode ?? false ? "Включен" : "Выключен")}\n" +
                                    $"Калибровка: {(recorder?.RecorderInUse?.VoiceDetectorCalibrating ?? false ? "В процессе" : "Завершена")}\n" +
                                    $"Подключенных игроков: {PhotonNetwork.CountOfPlayers}\n" +
                                    $"Комната: {PhotonNetwork.CurrentRoom?.Name ?? "Не подключен"}\n" +
                                    $"Пинг: {PhotonNetwork.GetPing()}мс";

                debugText.text = currentState;
            }

            // Обновляем информацию о состоянии PUN и Voice
            if (punStateText != null)
            {
                punStateText.text = string.Format("PUN: {0}", PhotonNetwork.NetworkClientState);
            }
            if (voiceStateText != null)
            {
                voiceStateText.text = string.Format("PhotonVoice: {0}", voiceClient.ClientState);
            }
        }
    }

    private void OnCharacterInstantiated(GameObject character)
    {
        // Получаем компонент PhotonVoiceView
        recorder = character.GetComponent<PhotonVoiceView>();
        if (recorder == null)
        {
            Debug.LogError("PhotonVoiceView not found on character!");
        }
        else
        {
            Debug.Log("PhotonVoiceView successfully initialized on character");
            UpdateCalibrateButton();
        }
    }

    private void OnVoiceClientStateChanged(ClientState fromState, ClientState toState)
    {
        Debug.Log($"Voice Client State: {toState}");
        
        if (voiceStateText != null)
        {
            voiceStateText.text = string.Format("PhotonVoice: {0}", toState);
        }
        
        UpdateButtonsVisibility();
        UpdateCalibrateButton();
    }

    private void OnPunClientStateChanged(ClientState fromState, ClientState toState)
    {
        Debug.Log($"PUN Client State: {toState}");
        
        if (punStateText != null)
        {
            punStateText.text = string.Format("PUN: {0}", toState);
        }
        
        UpdateButtonsVisibility();
        UpdateCalibrateButton();
    }

    public void ToggleMicrophone()
    {
        if (recorder == null)
        {
            Debug.LogError("PhotonVoiceView is not initialized! Please check if it exists on player.");
            return;
        }

        if (recorder.RecorderInUse == null)
        {
            Debug.LogError("RecorderInUse is null! Make sure PhotonVoiceView is properly initialized.");
            return;
        }

        isMicrophoneEnabled = !isMicrophoneEnabled;
        recorder.RecorderInUse.TransmitEnabled = isMicrophoneEnabled;
        
        // Обновляем визуальное состояние микрофона
        if (microphoneVisual != null)
        {
            // Здесь можно добавить визуальную индикацию состояния микрофона
            // Например, изменить цвет, анимацию и т.д.
        }
        
        Debug.Log("Microphone " + (isMicrophoneEnabled ? "enabled" : "disabled"));

        UpdateButtonsVisibility();
    }

    public void CalibrateMicrophone()
    {
        if (recorder != null && recorder.RecorderInUse != null && !recorder.RecorderInUse.VoiceDetectorCalibrating)
        {
            recorder.RecorderInUse.VoiceDetectorCalibrate((int)(calibrationTime * 1000));
            calibrationStartTime = Time.time; // Записываем время начала калибровки
            if (calibrateButton != null)
            {
                calibrateButton.SetText($"Калибровка... {calibrationTime / 1000f:F1}с");
            }
        }
    }

    private void UpdateButtonsVisibility()
    {
        bool isConnected = PhotonNetwork.IsConnected && voiceClient.ClientState == ClientState.Joined;
        
        microphoneOnButton.gameObject.SetActive(isMicrophoneEnabled && isConnected);
        microphoneOffButton.gameObject.SetActive(!isMicrophoneEnabled && isConnected);
        
        if (calibrateButton != null)
        {
            calibrateButton.gameObject.SetActive(isConnected);
        }
    }

    private void UpdateCalibrateButton()
    {
        if (calibrateButton != null && recorder != null && recorder.RecorderInUse != null)
        {
            bool isCalibrating = recorder.RecorderInUse.VoiceDetectorCalibrating;
            if (isCalibrating)
            {
                float remainingTime = (calibrationTime - (Time.time - calibrationStartTime) * 1000) / 1000f;
                if (remainingTime > 0)
                {
                    calibrateButton.SetText($"Калибровка... {remainingTime:F1}с");
                }
                else
                {
                    calibrateButton.SetText("Калибровка...");
                }
            }
            else
            {
                calibrateButton.SetText($"Калибровать ({calibrationTime:F1}с)");
            }
            calibrateButton.Interactable(!isCalibrating);
        }
    }
}
