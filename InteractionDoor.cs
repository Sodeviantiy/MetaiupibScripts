﻿using UnityEngine;
using Photon.Pun;

public class InteractionDoor : MonoBehaviour
{
    public GameObject interactionUI; // UI, ������� ���������� "������� E"
    public Transform doorTransform; // ��������� �����
    public Vector3 openRotation;    // ������� ��� �������� �����
    public Vector3 closedRotation;  // ������� ��� �������� �����
    private bool isPlayerNearby = false;
    private bool isDoorOpen = false; // ��������� �����

    void Start()
    {
        interactionUI.SetActive(false);
        doorTransform.localEulerAngles = closedRotation; // ������������� ����� � �������� ���������
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E)) // ������ E
        {
            if (isDoorOpen)
            {
                CloseDoor(); // ������� �����
            }
            else
            {
                OpenDoor(); // ������� �����
            }

            interactionUI.SetActive(false); // ������ ��������� ����� ��������������
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ��������������, ��� ����� ����� ��� "Player"
        {
            var photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                interactionUI.SetActive(true); // �������� ���������
                isPlayerNearby = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                interactionUI.SetActive(false); // ������ ���������
                isPlayerNearby = false;
            }
        }
    }

    void OpenDoor()
    {
        doorTransform.localEulerAngles = openRotation; // ������������� ������� ��� �������� �����
        isDoorOpen = true;
    }

    void CloseDoor()
    {
        doorTransform.localEulerAngles = closedRotation; // ������������� ������� ��� �������� �����
        isDoorOpen = false;
    }
}
