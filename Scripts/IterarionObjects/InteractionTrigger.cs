using UnityEngine;
using UnityEngine.UI;
using Michsky.MUIP;
using Photon.Pun;
using System.Collections;

public class InteractionTrigger : MonoBehaviour
{

    [SerializeField] public ModalWindowManager myModalWindow;
    public GameObject interactionUI; // UI, которое отображает "Нажмите F"
    public GameObject infoPanel;    // Панель с информацией
    private Coroutine closeInfoPanelCoroutine; // Ссылка на корутину для остановки, если нужно
    private bool isPlayerNearby = false;

    void Start()
    {
        interactionUI.SetActive(false);
        infoPanel.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            infoPanel.SetActive(true); // Открыть информационное окно
            myModalWindow.Open(); // Open window
            interactionUI.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

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
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
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
                Cursor.visible = false;
                myModalWindow.Close(); // Close window

                if (infoPanel.activeSelf) // Проверяем, открыта ли панель
                {
                    closeInfoPanelCoroutine = StartCoroutine(CloseInfoPanelAfterDelay(3f)); // Таймер на 3 секунды
                } 

                isPlayerNearby = false;
            }
             
        }
    }
    IEnumerator CloseInfoPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Ждём заданное время
        infoPanel.SetActive(false); // Закрываем панель
    }

}
