using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] public float speed;
    public float jumpForce;
    public bool isGrounded;
    private KeyBindManager keyBindManager;
    private Rigidbody rb;

    void Start()
    {
        keyBindManager = FindFirstObjectByType<KeyBindManager>();
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        Vector3 moveInput = new Vector3(0.0f, 0.0f, 0.0f);

        if (Input.GetKey(keyBindManager.GetKey("Up")))
            moveInput.z = 1;
        else if (Input.GetKey(keyBindManager.GetKey("Down")))
            moveInput.z = -1;

        if (Input.GetKey(keyBindManager.GetKey("Right")))
            moveInput.x = 1;
        else if (Input.GetKey(keyBindManager.GetKey("Left")))
            moveInput.x = -1;

        Vector3 movement = moveInput * speed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);

        if (isGrounded && Input.GetKey(keyBindManager.GetKey("Jump")))
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f);
    }
}
