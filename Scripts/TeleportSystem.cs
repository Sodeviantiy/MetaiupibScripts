using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TeleportSystem : MonoBehaviour
{
    [System.Serializable]
    public class TeleportPoint
    {
        public string buttonText;
        public GameObject targetObject; // Ссылка на объект для телепортации
        public string sceneName; // Имя сцены для переключения (если нужно)
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

    public GameObject player; // Ссылка на игрока, который будет телепортироваться
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
            Debug.LogError("Teleportv2: teleportMenu не назначен! Назначьте UI панель в инспекторе.");
        }

        if (buttonPrefab == null)
        {
            Debug.LogError("Teleportv2: buttonPrefab не назначен! Назначьте префаб кнопки в инспекторе.");
        }

        if (buttonContainer == null)
        {
            Debug.LogError("Teleportv2: buttonContainer не назначен! Назначьте Transform контейнера для кнопок в инспекторе.");
        }

        if (teleportPoints.Count == 0)
        {
            Debug.LogWarning("Teleportv2: список точек телепортации пуст! Добавьте точки телепортации в инспекторе.");
        }

        if (player == null)
        {
            Debug.LogError("Teleportv2: player не назначен! Назначьте объект игрока в инспекторе.");
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

            // Удаляем существующие layout группировки, если есть
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
            // Проверяем, что у нас есть либо целевой объект, либо имя сцены
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
                // Привязываем кнопку к методу, который будет проверять, что использовать
                button.onClick.AddListener(() => TryTeleportPlayer(pointCopy));
            }
        }
    }

    private void TryTeleportPlayer(TeleportPoint point)
    {
        if (player == null || !isPlayerInTrigger)
        {
            Debug.LogWarning("Телепортация невозможна: player == null или игрок не в триггере");
            return;
        }

        // Если указано имя сцены, переключаем на неё
        if (!string.IsNullOrEmpty(point.sceneName))
        {
            Debug.Log($"Переключение на сцену: {point.sceneName}");
            SceneManager.LoadScene(point.sceneName);
        }
        else if (point.targetObject != null)
        {
            // Если целевой объект задан, телепортируем игрока к нему
            player.transform.position = point.targetObject.transform.position;
            Debug.Log($"Телепортация игрока к объекту: {point.targetObject.name}");
        }
        else
        {
            Debug.LogWarning("Телепортация невозможна: ни целевой объект, ни имя сцены не указаны.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerInTrigger = true;
            ShowTeleportMenu();
            Debug.Log("Игрок вошел в зону телепортации");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerInTrigger = false;
            HideTeleportMenu();
            Debug.Log("Игрок вышел из зоны телепортации");
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
