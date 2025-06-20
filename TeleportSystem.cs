﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TeleportSystem : MonoBehaviour
{
    [System.Serializable]
    public class TeleportPoint
    {
        public string buttonText;
        public GameObject targetObject; // ������ �� ������ ��� ������������
        public string sceneName; // ��� ����� ��� ������������ (���� �����)
    }

    [Header("Teleport Settings")]
    public List<TeleportPoint> teleportPoints = new List<TeleportPoint>();

    [Header("UI References")]
    public GameObject teleportMenu;
    public GameObject buttonPrefab;
    public Transform buttonContainer;

    [Header("UI Customization")]
    public float buttonSpacing = 10f;
    public Vector2 buttonSize = new Vector2(200f, 200f);

    public GameObject player; // ������ �� ������, ������� ����� �����������������
    private bool isPlayerInTrigger = false;

    private void Start()
    {
        ValidateComponents();
        if (teleportMenu != null)
        {
            teleportMenu.SetActive(false);
        }
        SetupUI();
        SetupTeleportButtons();
    }

    private void ValidateComponents()
    {
        if (teleportMenu == null)
        {
            Debug.LogError("Teleportv2: teleportMenu �� ��������! ��������� UI ������ � ����������.");
        }

        if (buttonPrefab == null)
        {
            Debug.LogError("Teleportv2: buttonPrefab �� ��������! ��������� ������ ������ � ����������.");
        }

        if (buttonContainer == null)
        {
            Debug.LogError("Teleportv2: buttonContainer �� ��������! ��������� Transform ���������� ��� ������ � ����������.");
        }

        if (teleportPoints.Count == 0)
        {
            Debug.LogWarning("Teleportv2: ������ ����� ������������ ����! �������� ����� ������������ � ����������.");
        }

        if (player == null)
        {
            Debug.LogError("Teleportv2: player �� ��������! ��������� ������ ������ � ����������.");
        }
    }

    private void SetupUI()
    {
        if (buttonContainer != null)
        {
            RectTransform containerRect = buttonContainer.GetComponent<RectTransform>();
            if (containerRect != null)
            {
                containerRect.anchorMin = new Vector2(0, 0);
                containerRect.anchorMax = new Vector2(1, 1);
                containerRect.sizeDelta = Vector2.zero;
                containerRect.anchoredPosition = Vector2.zero;
            }

            // ������� ������������ layout �����������, ���� ����
            DestroyImmediate(buttonContainer.GetComponent<VerticalLayoutGroup>());
            DestroyImmediate(buttonContainer.GetComponent<HorizontalLayoutGroup>());

            var verticalLayout = buttonContainer.gameObject.AddComponent<VerticalLayoutGroup>();
            verticalLayout.spacing = buttonSpacing;
            verticalLayout.childAlignment = TextAnchor.LowerLeft;
            verticalLayout.childControlWidth = false;
            verticalLayout.childControlHeight = false;
            verticalLayout.childForceExpandWidth = false;
            verticalLayout.childForceExpandHeight = false;
            verticalLayout.padding = new RectOffset(10, 10, 10, 10);
        }
    }

    private void SetupTeleportButtons()
    {
        if (buttonContainer == null || buttonPrefab == null) return;

        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var point in teleportPoints)
        {
            // ���������, ��� � ��� ���� ���� ������� ������, ���� ��� �����
            if (point.targetObject == null && string.IsNullOrEmpty(point.sceneName)) continue;

            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = buttonSize;
            }

            Button button = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();

            if (buttonText != null)
            {
                buttonText.text = point.buttonText;
                buttonText.alignment = TextAnchor.MiddleCenter;
                buttonText.resizeTextForBestFit = true;
                buttonText.resizeTextMinSize = 12;
                buttonText.resizeTextMaxSize = 40;
            }

            if (button != null)
            {
                TeleportPoint pointCopy = point;
                // ����������� ������ � ������, ������� ����� ���������, ��� ������������
                button.onClick.AddListener(() => TryTeleportPlayer(pointCopy));
            }
        }
    }

    private void TryTeleportPlayer(TeleportPoint point)
    {
        if (player == null || !isPlayerInTrigger)
        {
            Debug.LogWarning("������������ ����������: player == null ��� ����� �� � ��������");
            return;
        }

        // ���� ������� ��� �����, ����������� �� ��
        if (!string.IsNullOrEmpty(point.sceneName))
        {
            Debug.Log($"������������ �� �����: {point.sceneName}");
            SceneManager.LoadScene(point.sceneName);
        }
        else if (point.targetObject != null)
        {
            // ���� ������� ������ �����, ������������� ������ � ����
            player.transform.position = point.targetObject.transform.position;
            Debug.Log($"������������ ������ � �������: {point.targetObject.name}");
        }
        else
        {
            Debug.LogWarning("������������ ����������: �� ������� ������, �� ��� ����� �� �������.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerInTrigger = true;
            ShowTeleportMenu();
            Debug.Log("����� ����� � ���� ������������");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerInTrigger = false;
            HideTeleportMenu();
            Debug.Log("����� ����� �� ���� ������������");
        }
    }

    private void ShowTeleportMenu()
    {
        if (teleportMenu != null)
        {
            teleportMenu.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void HideTeleportMenu()
    {
        if (teleportMenu != null)
        {
            teleportMenu.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
