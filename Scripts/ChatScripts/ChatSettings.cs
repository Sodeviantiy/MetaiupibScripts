using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Text;
using System.IO;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine.EventSystems;

public class ChatSettings : MonoBehaviourPun
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text chatHistory;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform content;       // Ссылка на объект Content
    [SerializeField] private GameObject messagePrefab; // Ссылка на префаб сообщения

    private const int MaxMessages = 50;
    private Queue<string> messageQueue = new Queue<string>();

    // Добавляем свойство для отслеживания состояния фокуса чата
    public bool IsChatFocused { get; private set; }

    void Start()
    {
        // Добавляем обработчики событий фокуса
        inputField.onSelect.AddListener((_) => OnChatFocused());
        inputField.onDeselect.AddListener((_) => OnChatUnfocused());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (inputField.text != "")
            {
                SendChatMessage(inputField.text);
                inputField.text = "";
            }

            // Деактивируем поле ввода и сбрасываем выбранный объект
            inputField.DeactivateInputField();
            EventSystem.current.SetSelectedGameObject(null);
            IsChatFocused = false;

            // Устанавливаем флаг игнорирования Alt до отпускания клавиши
            PersonController pc = FindObjectOfType<PersonController>();
            if (pc != null && pc.photonView.IsMine)
            {
                pc.ignoreAltUntilReleased = true;
            }
        }
    }


    // Методы для управления фокусом
    public void OnChatFocused()
    {
        IsChatFocused = true;
        Debug.Log("Чат в фокусе, управление заблокировано");
    }

    public void OnChatUnfocused()
    {
        IsChatFocused = false;
        Debug.Log("Чат вне фокуса, управление разблокировано");
    }

    public void AddMessage(string text)
    {
        // Создаём новый UI-объект из префаба
        GameObject newMessageObj = Instantiate(messagePrefab, content);

        // Ищем текст внутри префаба и задаём содержимое
        TMP_Text msgText = newMessageObj.GetComponentInChildren<TMP_Text>();
        if (msgText != null)
        {
            msgText.text = text;
        }

        StartCoroutine(ScrollToBottom());
    }

    public void SendChatMessage(string message)
    {
        photonView.RPC("ReceiveMessageRPC", RpcTarget.All, $"{message} :[{PhotonNetwork.LocalPlayer.NickName}]");
    }

    [PunRPC]
    private void ReceiveMessageRPC(string message)
    {
        // Применяем фильтр здесь:
        string filtered = FilterMessage(message);

        // Старый подход (очередь + общий TMP_Text) убираем или комментируем:
        //if (messageQueue.Count >= MaxMessages)
        //    messageQueue.Dequeue();

        //messageQueue.Enqueue(filtered);
        //UpdateChatDisplay();

        // Новый подход (динамическое создание префаба)
        AddMessage(filtered);
    }


    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncChatHistory", newPlayer,
                messageQueue.ToArray());
        }
    }

    [PunRPC]
    private void SyncChatHistory(string[] history)
    {
        messageQueue = new Queue<string>(history);
        UpdateChatDisplay();
    }

    private void UpdateChatDisplay()
    {
        chatHistory.text = string.Join("\n", messageQueue);
        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        scrollRect.normalizedPosition = Vector2.zero;
    }

    private string FilterMessage(string message)
    {
        var badWords = new[] { "анус", "бля", "блять", "пизда", "пиздец", "хуй", "нахуй", "Залупа конская", "Хуи моржовые", "Гандон штопаный", "Гандон", "Пердун старый", "Пиздобол", "Пиздун", "Говносос", "Мудозвон", "Мудак", "Заебись", "Залупа", "Минет", "Срать тебе в рот", "Срать ", "Засранец", "говно", "здохни", "Виебнуца", "Жопы", "Срака", "дегенерат", "Придурок", "пенис", "Нахуй", "На хуй", "Отъебись", "сука", "трахнул", "трахать", "Говнюк", "Жополиз", };
        string pattern = @"\b(" + string.Join("|", badWords) + @")\b";
        return Regex.Replace(message, pattern, "***", RegexOptions.IgnoreCase);
    }

    private byte[] CompressMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        using (var output = new MemoryStream())
        {
            using (var gzip = new GZipStream(output, CompressionMode.Compress))
            {
                gzip.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }
    }

    public void SendGlobalMessage(string message)
    {
        PhotonNetwork.RaiseEvent(
            1,
            message,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable);
    }
}