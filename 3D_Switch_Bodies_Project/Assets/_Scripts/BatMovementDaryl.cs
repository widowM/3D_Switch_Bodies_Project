using UnityEngine;

public class BatMovementDaryl : Creature
{
    [Header("Movement")]
    public float moveSpeed;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public float flyForce;
    public float slowForce;


    public float speedMax;

    [Header("Body Rotation")]
    public float bodyRotationSpeed = 10f;
    public Transform batModel;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected override void Update()
    {
        if (!IsActiveCreature() || !enabled)
        {
            enabled = false;
            return;
        }
        base.Update();
        GetInput();
    }

    protected override void FixedUpdate()
    {
        if (!IsActiveCreature() || !enabled)
        {
            enabled = false;
            return;
        }
        base.FixedUpdate();
        MovePlayer();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!IsActiveCreature())
        {
            rb.isKinematic = true;
            enabled = false;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
    }

     private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * flyForce, ForceMode.Impulse);
        }

        if (Input.GetKey(KeyCode.X))
        {
            rb.AddForce(Vector3.up * -flyForce, ForceMode.Impulse);
        }

        if (rb.linearVelocity.y > 0)
        {
            rb.AddForce(Vector3.up * (-slowForce), ForceMode.Impulse);
        }
        else if (rb.linearVelocity.y < 0)
        {
            rb.AddForce(Vector3.up * (slowForce), ForceMode.Impulse);
        }

       // Debug.Log(rb.linearVelocity);
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
    }
}