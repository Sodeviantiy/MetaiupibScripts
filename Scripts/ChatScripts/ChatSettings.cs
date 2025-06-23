﻿using System.Collections;
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
    [SerializeField] private Transform content;       // ������ �� ������ Content
    [SerializeField] private GameObject messagePrefab; // ������ �� ������ ���������

    private const int MaxMessages = 50;
    private Queue<string> messageQueue = new Queue<string>();

    // ��������� �������� ��� ������������ ��������� ������ ����
    public bool IsChatFocused { get; private set; }

    void Start()
    {
        // ��������� ����������� ������� ������
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

            // ������������ ���� ����� � ���������� ��������� ������
            inputField.DeactivateInputField();
            EventSystem.current.SetSelectedGameObject(null);
            IsChatFocused = false;

            // ������������� ���� ������������� Alt �� ���������� �������
            PersonController pc = FindObjectOfType<PersonController>();
            if (pc != null && pc.photonView.IsMine)
            {
                pc.ignoreAltUntilReleased = true;
            }
        }
    }


    // ������ ��� ���������� �������
    public void OnChatFocused()
    {
        IsChatFocused = true;
        Debug.Log("��� � ������, ���������� �������������");
    }

    public void OnChatUnfocused()
    {
        IsChatFocused = false;
        Debug.Log("��� ��� ������, ���������� ��������������");
    }

    public void AddMessage(string text)
    {
        // ������ ����� UI-������ �� �������
        GameObject newMessageObj = Instantiate(messagePrefab, content);

        // ���� ����� ������ ������� � ����� ����������
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
        // ��������� ������ �����:
        string filtered = FilterMessage(message);

        // ������ ������ (������� + ����� TMP_Text) ������� ��� ������������:
        //if (messageQueue.Count >= MaxMessages)
        //    messageQueue.Dequeue();

        //messageQueue.Enqueue(filtered);
        //UpdateChatDisplay();

        // ����� ������ (������������ �������� �������)
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
        var badWords = new[] { "����", "���", "�����", "�����", "������", "���", "�����", "������ �������", "��� ��������", "������ ��������", "������", "������ ������", "��������", "������", "��������", "��������", "�����", "�������", "������", "�����", "����� ���� � ���", "����� ", "��������", "�����", "������", "��������", "����", "�����", "���������", "��������", "�����", "�����", "�� ���", "��������", "����", "�������", "�������", "������", "�������", };
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