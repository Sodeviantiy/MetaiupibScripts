using UnityEngine;
using Photon.Pun;

public class InteractionDoor : MonoBehaviour
{
    public GameObject interactionUI; // UI, которое отображает "Нажмите E"
    public Transform doorTransform; // Трансформ двери
    public Vector3 openRotation;    // Ротация для открытой двери
    public Vector3 closedRotation;  // Ротация для закрытой двери
    private bool isPlayerNearby = false;
    private bool isDoorOpen = false; // Состояние двери

    void Start()
    {
        interactionUI.SetActive(false);
        doorTransform.localEulerAngles = closedRotation; // Устанавливаем дверь в закрытое положение
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E)) // Кнопка E
        {
            if (isDoorOpen)
            {
                CloseDoor(); // Закрыть дверь
            }
            else
            {
                OpenDoor(); // Открыть дверь
            }

            interactionUI.SetActive(false); // Скрыть подсказку после взаимодействия
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Предполагается, что игрок имеет тэг "Player"
        {
            var photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                interactionUI.SetActive(true); // Показать подсказку
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
                interactionUI.SetActive(false); // Скрыть подсказку
                isPlayerNearby = false;
            }
        }
    }

    void OpenDoor()
    {
        doorTransform.localEulerAngles = openRotation; // Устанавливаем ротацию для открытой двери
        isDoorOpen = true;
    }

    void CloseDoor()
    {
        doorTransform.localEulerAngles = closedRotation; // Устанавливаем ротацию для закрытой двери
        isDoorOpen = false;
    }
}
