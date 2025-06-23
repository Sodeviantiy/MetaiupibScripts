using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
using Michsky.MUIP;
using Michsky.UI.Heat;
using TMPro;

public class Teleportv2 : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText; // Текст вопроса
        public string correctAnswer; // Правильный ответ
        public List<string> answerOptions; // Варианты ответа
    }

    [System.Serializable]
    public class TeleportPoint
    {
        public string buttonText;
        public Transform destination;
        public int floorNumber; // Этаж, связанный с точкой телепорта
        public List<Question> questions; // Список вопросов
    }


    [Header("Teleport Settings")]
    public List<TeleportPoint> teleportPoints = new List<TeleportPoint>();
    public int triggerFloor;
    public int currentFloor;

    [Header("UI References")]
    public GameObject teleportMenu;
    public Michsky.UI.Heat.ModalWindowManager questionMenu;
    public TMP_Text questionText; // Замените тип Text на TMP_Text
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public Transform answerButtonContainer;
    public GameObject answerButtonPrefab;

    [Header("UI Customization")]
    public float buttonSpacing = 10f;
    public Vector2 buttonSize = new Vector2(200f, 200f);

    [Header("UI Customization for Answer Buttons")]
    private float answerButtonSpacing = 10f; // Расстояние между кнопками
    private Vector2 answerButtonSize = new Vector2(200, 50); // Размер кнопок



    private GameObject currentPlayer;
    private bool isPlayerInTrigger = false;
    private TeleportPoint pendingTeleportPoint;

    private void Start()
    {
        currentPlayer = GameObject.FindGameObjectWithTag("Player");

        if (currentPlayer == null)
        {
            Debug.LogError("Игрок (Player) не найден! Убедитесь, что объект игрока имеет тег 'Player'.");
        }

        ValidateComponents();

        if (teleportMenu != null)
        {
            teleportMenu.SetActive(false);
        }

        if (questionMenu != null)
        {
            questionMenu.CloseWindow();
        }

        SetupUI();
        SetupTeleportButtons();
    }

    private void Update()
    {
        if (isPlayerInTrigger)
        {
            // Проверка нажатия цифровых клавиш 1-9
            for (int i = 1; i <= 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    int targetFloor = i;
                    TeleportPoint targetPoint = teleportPoints.Find(point => point.floorNumber == targetFloor);
                    
                    if (targetPoint != null)
                    {
                        TryTeleportPlayer(targetPoint);
                    }
                }
            }
        }
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
        else
        {
            foreach (var point in teleportPoints)
            {
                if (point.destination == null)
                {
                    Debug.LogError($"Teleportv2: точка назначения для кнопки '{point.buttonText}' не назначена!");
                }
            }
        }

        if (questionMenu == null || questionText == null || answerButtonPrefab == null || answerButtonContainer == null)
        {
            Debug.LogError("Teleportv2: Компоненты UI для проверки ответа не назначены!");
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

            var existingVertical = buttonContainer.GetComponent<VerticalLayoutGroup>();
            var existingHorizontal = buttonContainer.GetComponent<HorizontalLayoutGroup>();
            if (existingVertical != null) DestroyImmediate(existingVertical);
            if (existingHorizontal != null) DestroyImmediate(existingHorizontal);

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
        if (buttonContainer == null || buttonPrefab == null)
        {
            Debug.LogError("buttonContainer или buttonPrefab не назначены!");
            return;
        }

        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var point in teleportPoints)
        {
            if (point.destination == null)
            {
                Debug.LogWarning($"Точка назначения для {point.buttonText} не указана!");
                continue;
            }

            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            if (buttonObj == null)
            {
                Debug.LogError("Кнопка не была создана!");
                continue;
            }

            Michsky.MUIP.ButtonManager buttonManager = buttonObj.GetComponent<Michsky.MUIP.ButtonManager>();
            if (buttonManager != null)
            {
                buttonManager.SetText(point.buttonText);
                buttonManager.Interactable(point.floorNumber != currentFloor);

                TeleportPoint pointCopy = point;
                buttonManager.onClick.AddListener(() => ShowQuestionMenu(pointCopy));
            }
            else
            {
                Debug.LogError("ButtonManager не найден на кнопке!");
            }
        }
    }

    private void SetupAnswerButtons(Question selectedQuestion)
    {
        if (answerButtonContainer == null || answerButtonPrefab == null)
        {
            Debug.LogError("answerButtonContainer или answerButtonPrefab не назначен!");
            return;
        }

        // Удаляем старые кнопки
        foreach (Transform child in answerButtonContainer)
        {
            Destroy(child.gameObject);
        }

        //float currentXOffset = 0f; // Начальная позиция для кнопок
        for (int i = 0; i < selectedQuestion.answerOptions.Count; i++)
        {
            string answer = selectedQuestion.answerOptions[i].Trim(); // Убираем лишние пробелы
            GameObject answerButtonObj = Instantiate(answerButtonPrefab, answerButtonContainer);
            RectTransform buttonRect = answerButtonObj.GetComponent<RectTransform>();
            Michsky.UI.Heat.ButtonManager buttonManager = answerButtonObj.GetComponent<Michsky.UI.Heat.ButtonManager>();

            if (buttonRect != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(buttonRect); // Оставляем для обновления, если нужно
            }

            if (buttonManager != null)
            {
                // Устанавливаем текст кнопки и привязываем событие
                buttonManager.SetText(answer);
                buttonManager.onClick.AddListener(() => CheckAnswer(answer, selectedQuestion.correctAnswer));
            }
            else
            {
                Debug.LogError($"ButtonManager не найден для кнопки {i}!");
            }
        }

        // Принудительно обновляем Layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(answerButtonContainer.GetComponent<RectTransform>());
    }






    private void ShowQuestionMenu(TeleportPoint point)
    {
        /*
        if (questionMenu == null || questionText == null || answerButtonContainer == null || answerButtonPrefab == null)
        {
            Debug.LogError("UI для вопросов не настроено!");
            return;
        }

        // Проверяем наличие вопросов
        if (point.questions == null || point.questions.Count == 0)
        {
            Debug.LogWarning($"Для точки '{point.buttonText}' не задано вопросов!");
            return;
        }

        // Выбираем случайный вопрос
        Question selectedQuestion = point.questions[Random.Range(0, point.questions.Count)];

        pendingTeleportPoint = point;
        questionMenu.OpenWindow();
        questionText.text = selectedQuestion.questionText;
        teleportMenu.SetActive(false);

        // Настройка кнопок с ответами, передаем selectedQuestion в SetupAnswerButtons
        SetupAnswerButtons(selectedQuestion); // Передаем выбранный вопрос

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        */

        //Закоментить если нужно вопросы
        TryTeleportPlayer(point);
    }




    private void CheckAnswer(string selectedAnswer, string correctAnswer)
    {
        /*
        if (selectedAnswer == correctAnswer)
        {
            Debug.Log("Ответ правильный!");
            TryTeleportPlayer(pendingTeleportPoint);
        }
        else
        {
            Debug.Log("Ответ неправильный. Телепортация отменена.");
        }

        questionMenu.CloseWindow();
        teleportMenu.SetActive(true);
        */

        //Закоментить если нужно вопросы
        TryTeleportPlayer(pendingTeleportPoint);
        questionMenu.CloseWindow();
        teleportMenu.SetActive(true);
    }

    private void TryTeleportPlayer(TeleportPoint point)
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("Телепортация невозможна: currentPlayer == null");
            return;
        }

        if (point.destination == null)
        {
            Debug.LogWarning($"Телепортация невозможна: не указана точка назначения для этажа {point.floorNumber}");
            return;
        }

        Debug.Log($"Телепортация на позицию: {point.destination.position}");
        currentPlayer.transform.position = point.destination.position;

        // Проверяем, равен ли номер этажа 3, и если да, изменяем масштаб игрока
        if (point.floorNumber == 3)
        {
            currentPlayer.transform.localScale = new Vector3(2.9f, 2.9f, 2.9f);
            Debug.Log("Масштаб игрока изменен на (2.9, 2.9, 2.9)");
        }
        else if (point.floorNumber == 4)
        {
            currentPlayer.transform.localScale = new Vector3(2.7f, 2.7f, 2.7f);
            Debug.Log("Масштаб игрока изменен на (2.7, 2.7, 2.7)");
        }
        else if (point.floorNumber == 5)
        {
            currentPlayer.transform.localScale = new Vector3(2.7f, 2.7f, 2.7f);
            Debug.Log("Масштаб игрока изменен на (2.7, 2.7, 2.7)");
        }
        else
        {
            currentPlayer.transform.localScale = new Vector3(2f, 2f, 2f);
            Debug.Log("Масштаб игрока изменен на (2, 2, 2)");
        }
        

        currentFloor = point.floorNumber;
        Debug.Log($"Текущий этаж обновлен на: {currentFloor}");

        SetupTeleportButtons();
        HideTeleportMenu();
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
            questionMenu.CloseWindow();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                currentPlayer = other.gameObject;
                isPlayerInTrigger = true;

                // Устанавливаем текущий этаж в соответствии с триггером
                currentFloor = triggerFloor;

                // Обновляем кнопки с учетом текущего этажа
                SetupTeleportButtons();

                ShowTeleportMenu();
                Debug.Log($"Локальный игрок вошел в зону телепортации на этаже {triggerFloor}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                currentPlayer = null;
                isPlayerInTrigger = false;
                HideTeleportMenu();
                Debug.Log("Локальный игрок вышел из зоны телепортации");
            }
        }
    }


}
