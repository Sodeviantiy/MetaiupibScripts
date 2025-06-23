using UnityEngine;
using Photon.Pun;

public class PlayerKey : MonoBehaviourPunCallbacks
{
    [SerializeField] KeyCode PickUp = KeyCode.E;
    private Camera playerCamera;
    private PhotonView photonView;
    private Animator _animator; // Добавлено

    void Start()
    {
        photonView = GetComponentInParent<PhotonView>();
        playerCamera = GetComponentInChildren<Camera>();
        _animator = GetComponentInParent<Animator>(); // Получаем Animator от родительского объекта

        if (photonView == null)
            Debug.LogError("PhotonView component missing!", this);
        if (_animator == null)
            Debug.LogError("Animator component missing on the player!", this);
        else
            Debug.Log("Animator found on player: " + _animator.name);
    }

    void Update()
    {
        if (!photonView.IsMine || playerCamera == null) return;

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);

            if (hit.collider.CompareTag("Door") && Input.GetKeyDown(PickUp))
            {
                

                var doorPhotonView = hit.collider.GetComponent<PhotonView>();
                if (doorPhotonView != null)
                {
                    photonView.RPC("TryOpenDoorRPC", RpcTarget.All, doorPhotonView.ViewID);
                }
            }
            else if (hit.collider.CompareTag("Chair") && Input.GetKeyDown(PickUp))
            {
                var chairPhotonView = hit.collider.GetComponent<PhotonView>();
                if (chairPhotonView != null)
                {
                    photonView.RPC("ChairInteractionRPC", RpcTarget.All,
                        chairPhotonView.ViewID,
                        photonView.ViewID);
                }
            }
        }
    }

    [PunRPC]
    void UnlockDoorRPC(int keyViewID)
    {
        PhotonView.Find(keyViewID)?.GetComponent<KeyEvent>()?.UnlockDoor();
    }

    [PunRPC]
    void TryOpenDoorRPC(int doorViewID)
    {
        PhotonView.Find(doorViewID)?.GetComponent<DoorEvent>()?.TryOpen();
    }

    [PunRPC]
    void ChairInteractionRPC(int chairViewID, int playerViewID)
    {
        var chair = PhotonView.Find(chairViewID)?.GetComponent<ChairEvent>();
        var player = PhotonView.Find(playerViewID)?.gameObject;

        if (player == null)
        {
            Debug.LogError("Player object is null!");
            return;
        }

        var controller = player.GetComponentInParent<PersonController>();
        if (controller == null)
        {
            Debug.LogError("PersonController component missing on player!");
            return;
        }

        // Добавляем проверку на активность управления
        if (player.GetComponentInParent<PersonController>().enabled)
        {
            chair?.ToggleSitting(player);
        }
        else
        {
            chair?.ToggleSitting(player); // Повторный вызов для вставания
        }
    }

    [PunRPC]
    public void PlaySitAnimation()
    {
        _animator.ResetTrigger("StandUp");
        _animator.SetTrigger("SitDown");
    }

    [PunRPC]
    public void PlayStandAnimation()
    {
        _animator.ResetTrigger("SitDown");
        _animator.SetTrigger("StandUp");
    }
}