using UnityEngine;
using Photon.Pun;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField]
    Transform character;
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    Vector2 velocity;
    Vector2 frameVelocity;
    private PhotonView photonView;
    private PersonController personController;

    void Reset()
    {
        // Get the character from the FirstPersonMovement in parents.
        character = GetComponentInParent<PersonController>().transform;
    }

    void Start()
    {
        photonView = GetComponentInParent<PhotonView>();
        personController = GetComponentInParent<PersonController>();

        if (photonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // Отключаем компонент камеры для чужих игроков
            GetComponent<Camera>().enabled = false;
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // Проверяем, заблокировано ли управление
        if (personController.IsControlLocked)
        {
            // Если управление заблокировано, пропускаем обработку вращения камеры
            return;
        }

        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }
}