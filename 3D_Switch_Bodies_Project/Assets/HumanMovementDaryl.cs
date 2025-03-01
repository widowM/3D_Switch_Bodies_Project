using UnityEngine;

public class HumanMovementDaryl : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public float jumpForce;
    public float gravityForce;

    private bool grounded;

    public float speedMax;

    public GameObject targetCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        grounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
        rb.AddForce(Vector3.up * -gravityForce, ForceMode.Impulse);
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        rb.linearVelocity = new Vector3(
            Mathf.Clamp(rb.linearVelocity.x, -speedMax, speedMax),
            rb.linearVelocity.y,
            Mathf.Clamp(rb.linearVelocity.z, -speedMax, speedMax)
        );

        Vector3 frictionForce = -rb.linearVelocity * 5f; // Adjust multiplier for more/less friction
        rb.AddForce(frictionForce, ForceMode.Force);
    }

    void OnTriggerEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // Ensure ground objects have the "Ground" tag
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.thisCollider == targetCollider) // Check if the collision is with the specific collider
                {
                    grounded = true;
                }
            }
        }
    }

    // Detect when player leaves the ground
    void OnTriggerExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.thisCollider == targetCollider) // Check if the collision is with the specific collider
                {
                    grounded = false;
                }
            }
        }
    }
}