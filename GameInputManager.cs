using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }
    
    [SerializeField] private GameObject menuPanel;
    
    private bool _isInputLocked;
    private bool _forceInputLock;
    private bool _isCursorForceVisible;
    public bool IsInputLocked => (_isInputLocked || _forceInputLock) && !_isCursorForceVisible;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }

    // Метод для полной блокировки управления
    public void SetForceInputLock(bool locked)
    {
        _forceInputLock = locked;
        _isCursorForceVisible = false;
        UpdateCursorState();
    }

    // Новый метод только для управления курсором без блокировки управления
    public void SetCursorVisible(bool visible)
    {
        _isCursorForceVisible = visible;
        UpdateCursorState();
    }

    private void UpdateCursorState()
    {
        if (_isInputLocked || _forceInputLock || _isCursorForceVisible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        ChatSettings chatSettings = FindObjectOfType<ChatSettings>();
        bool isChatFocused = chatSettings != null && chatSettings.IsChatFocused;
        bool isMenuActive = menuPanel != null && menuPanel.activeSelf;
        bool isCursorVisible = Cursor.visible && Cursor.lockState == CursorLockMode.None && !_isCursorForceVisible;

        _isInputLocked = isChatFocused || isMenuActive || isCursorVisible;
        UpdateCursorState();
    }
}