using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PersonController : MonoBehaviourPun
{
    // ������������ ���������� �� PlayerController
    [Header("Animation Settings")]
    [SerializeField] private float walkAnimationThreshold = 0.1f; // ����� ��� �������� ������
    [SerializeField] private float runAnimationThreshold = 5f;    // ����� ��� �������� ����
    [SerializeField] private float rotationSpeed = 10f;           // �������� ��������

    // ������������ ���������� �� FirstPersonMovement
    [Header("Movement Settings")]
    public float speed = 5;         // ������� �������� (������ ������������ ��� walk speed)
    public float runSpeed = 9;      // �������� ����
    public bool canRun = true;
    public KeyCode runningKey = KeyCode.LeftShift;
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    // ����������
    private Animator _animator;
    private Rigidbody _rigidbody;
    private PhotonView _photonView;
    private Vector3 _movement;
    private ChatSettings _chatSettings;

    public bool IsRunning { get; private set; }
    public bool cursorActive { get; private set; }

    public bool ignoreAltUntilReleased = false;


    // ��������� ��������� �������� ��� �������� ���������� ����������
    public bool IsControlLocked
    {
        get
        {
            // ���������� �������������, ���� ������ ������� ��� ��� � ������
            return cursorActive || (_chatSettings != null && _chatSettings.IsChatFocused);
        }
    }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();

        // ������� ��������� ChatSettings � �����
        _chatSettings = FindObjectOfType<ChatSettings>();
    }

    void Update()
    {
        if (!_photonView.IsMine) return;

        // 1) ��������� ����� ����
        bool chatFocused = _chatSettings != null && _chatSettings.IsChatFocused;

        // 2) ���� ��� � ������, ��������� ������ ���������������� � ���������� Alt
        if (chatFocused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cursorActive = true;

            // ������������� �������� � ��������, ��� ��� � ������
            return;
        }
        else
        {
            // 3) ���� ��� �� � ������ � ������������ Alt ��� ������
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                cursorActive = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                cursorActive = false;
            }
        }

        // ���������, ������������� �� ����������
        if (IsControlLocked)
        {
            // ���� ���������� �������������, ������������� ������� ��������
            _movement = Vector3.zero;
        }
        else
        {
            // ��������� ����� ������ ���� ���������� �� �������������
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            _movement = new Vector3(horizontal, 0, vertical).normalized;

            // ����������� ��������� ����
            IsRunning = canRun && Input.GetKey(runningKey);
        }

        // ������ �������� ��� ��������
        float currentSpeed = _movement.magnitude * (IsRunning ? runSpeed : speed);

        // ���������� �����
        Debug.Log($"Animation Speed: {currentSpeed} | IsRunning: {IsRunning} | ControlLocked: {IsControlLocked}");

        // ���������� ��������� ���������
        if (_animator != null)
        {
            _animator.SetFloat("Speed", currentSpeed);
        }

        // ������� ��������� ������ ���� ���������� �� �������������
        if (_movement != Vector3.zero && !IsControlLocked)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_movement);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void FixedUpdate()
    {
        if (!_photonView.IsMine) return;

        // ���������, ������������� �� ����������
        if (IsControlLocked)
        {
            // ���� ���������� �������������, ������������� ��������
            _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
            return;
        }

        // ������������ ������ ��������
        float targetMovingSpeed = IsRunning ? runSpeed : speed;

        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        Vector2 targetVelocity = new Vector2(
            Input.GetAxis("Horizontal") * targetMovingSpeed,
            Input.GetAxis("Vertical") * targetMovingSpeed
        );

        _rigidbody.linearVelocity = transform.rotation *
            new Vector3(targetVelocity.x, _rigidbody.linearVelocity.y, targetVelocity.y);
    }
}