using UnityEngine;
using UnityEngine.UI;
using Photon.Voice.PUN;
using TMPro;
using Photon.Pun;

public class MicrophoneHighlighter : MonoBehaviour
{
    private Canvas canvas;
    private PhotonVoiceView photonVoiceView;
    private MicrophoneButtonController microphoneController;
    private Camera playerCamera;
    private PhotonView photonView;
    private bool isInitialized = false;

    [SerializeField]
    private Image recorderSprite;

    [SerializeField]
    private Image speakerSprite;

    [SerializeField]
    private TMP_Text bufferLagText;

    private bool showSpeakerLag;

    private void OnEnable()
    {
        if (microphoneController != null)
        {
            microphoneController.DebugModeChanged += OnDebugModeChanged;
        }
        InitializeComponents();
    }

    private void OnDisable()
    {
        if (microphoneController != null)
        {
            microphoneController.DebugModeChanged -= OnDebugModeChanged;
        }
    }

    private void OnDebugModeChanged(bool debugMode)
    {
        this.showSpeakerLag = debugMode;
    }

    private void Awake()
    {
        this.canvas = this.GetComponent<Canvas>();
        this.photonView = this.GetComponentInParent<PhotonView>();
        
        // Находим MicrophoneButtonController в сцене
        this.microphoneController = FindObjectOfType<MicrophoneButtonController>();
        if (this.microphoneController == null)
        {
            Debug.LogError("MicrophoneButtonController не найден в сцене!");
            return;
        }

        // Получаем PhotonVoiceView из родительского объекта
        this.photonVoiceView = this.GetComponentInParent<PhotonVoiceView>();
        if (this.photonVoiceView == null)
        {
            Debug.LogError("PhotonVoiceView не найден в родительском объекте!");
            return;
        }
    }

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        if (isInitialized) return;

        // Ищем камеру игрока в сцене
        var cameras = FindObjectsOfType<Camera>();
        foreach (var camera in cameras)
        {
            var cameraPhotonView = camera.GetComponentInParent<PhotonView>();
            if (cameraPhotonView != null && cameraPhotonView.IsMine)
            {
                playerCamera = camera;
                break;
            }
        }

        if (playerCamera == null)
        {
            Debug.LogWarning("Камера игрока не найдена! Попробуйте позже...");
            return;
        }

        canvas.worldCamera = playerCamera;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized)
        {
            InitializeComponents();
        }

        if (photonVoiceView == null) return;

        // Обновляем видимость иконок
        if (recorderSprite != null)
        {
            recorderSprite.enabled = photonVoiceView.IsRecording;
        }

        if (speakerSprite != null)
        {
            speakerSprite.enabled = photonVoiceView.IsSpeaking;
        }

        // Обновляем текст задержки
        if (bufferLagText != null)
        {
            bufferLagText.enabled = showSpeakerLag && photonVoiceView.IsSpeaking;
            if (bufferLagText.enabled && photonVoiceView.SpeakerInUse != null)
            {
                bufferLagText.text = string.Format("{0}", photonVoiceView.SpeakerInUse.Lag);
            }
        }
    }

    private void LateUpdate()
    {
        if (!isInitialized || this.canvas == null || this.playerCamera == null) return;
        
        // Поворачиваем UI элементы лицом к камере наблюдателя
        Vector3 targetPosition = playerCamera.transform.position;
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // Игнорируем вертикальное смещение
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // Добавляем поворот на 180 градусов по оси Y для отражения
            targetRotation *= Quaternion.Euler(0, 180, 0);
            transform.rotation = targetRotation;
        }
    }
} 