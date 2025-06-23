using UnityEngine;
using Photon.Pun;

public class ChairEvent : MonoBehaviourPun
{
    [SerializeField] private Transform sitPosition;
    private bool isOccupied;
    private GameObject currentPlayer;
    private Vector3 _playerOriginalPosition; // Сохраняем исходную позицию
    
    [PunRPC]
    public void ToggleSitting(GameObject player)
    {
        Debug.Log($"Toggling sitting. Occupied: {isOccupied}, Player: {player.name}");

        if (!isOccupied)
        {
            currentPlayer = player;
            player.GetComponent<PhotonView>().RPC("PlaySitAnimation", RpcTarget.All);

            // Отключаем управление
            var controller = player.GetComponentInParent<PersonController>();
            controller.enabled = false;

            // Перемещаем персонажа на стул
            player.transform.position = sitPosition.position;
            player.transform.rotation = sitPosition.rotation;

            // Фиксируем камеру на голове персонажа
            var camera = player.GetComponentInChildren<Camera>();
            if (camera != null)
            {
                camera.transform.localPosition = new Vector3(0, 1.6f, 0); // Примерное положение камеры на голове
                camera.transform.localRotation = Quaternion.identity;
            }

            isOccupied = true;
        }
        else if (currentPlayer == player)
        {
            player.GetComponent<PhotonView>().RPC("PlayStandAnimation", RpcTarget.All);

            // Включаем управление
            var controller = player.GetComponentInParent<PersonController>();
            controller.enabled = true;

            // Возвращаем камеру в исходное положение
            var camera = player.GetComponentInChildren<Camera>();
            if (camera != null)
            {
                camera.transform.localPosition = new Vector3(0, 1.6f, 0); // Примерное положение камеры на голове
                camera.transform.localRotation = Quaternion.identity;
            }

            isOccupied = false;
        }
    }
}