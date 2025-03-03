using UnityEngine;

public class FourLeggedController : Creature
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 100f;
    public float jumpForce = 5f;
    public float dogStrafingMultiplier = 0.5f; // Reduce strafing speed
    public Transform playerCamera;

    [Header("Head Bobbing")]
    public float headBobFrequency = 1.5f; // How fast the head bobs
    public float headBobHeight = 0.3f; // How high the head bobs
    public float headBobSwayAngle = 1.5f; // Increased for more noticeable effect
    public float headBobSwayFrequency = 0.33f; // Control sway speed independently
    public float headBobLerpSpeed = 15f; // Smooth transition for sway
    public float headCenteringSpeed = 10f; // Speed at which the head centers smoothly
    public float headCenteringSpeedAir = 10f; // Speed at which the head centers smoothly
    private float headBobTimer = 0f;

    private float xRotation = 0f;
    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 cameraOriginalPosition;
    private Vector3 currentHeadBobRotation;

    private float pendingMouseX;
    private float pendingMouseY;
    private float pendingMoveX;
    private float pendingMoveZ;
    private bool jumpRequested;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        cameraOriginalPosition = playerCamera.localPosition; // Store the original camera position
    }

    protected override void Update()
    {
        // Cache all inputs
        pendingMouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pendingMouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        pendingMoveX = Input.GetAxis("Horizontal");
        pendingMoveZ = Input.GetAxis("Vertical");
        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequested = true;
        }
    }

    protected override void FixedUpdate()
    {
        // Handle physics-based movement
        float moveX = pendingMoveX * moveSpeed * dogStrafingMultiplier;
        float moveZ = pendingMoveZ * moveSpeed;

        Vector3 movement = transform.right * moveX + transform.forward * moveZ;
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);

        // Handle jumping
        if (jumpRequested)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            jumpRequested = false;
        }
    }

    void LateUpdate()
    {
        // Apply mouse look
        xRotation -= pendingMouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.Rotate(Vector3.up * pendingMouseX);

        // Handle head bobbing
        if (isGrounded)
        {
            if (rb.linearVelocity.magnitude > 0.1f)
            {
                headBobTimer += Time.deltaTime * headBobFrequency;
                float headBobOffset = Mathf.Sin(headBobTimer) * headBobHeight;
                
                // More precise sway calculation
                float headBobSway = Mathf.Sin(headBobTimer * headBobSwayFrequency * Mathf.PI) * headBobSwayAngle;
                
                playerCamera.localPosition = Vector3.Lerp(
                    playerCamera.localPosition, 
                    cameraOriginalPosition + new Vector3(0, headBobOffset, 0), 
                    Time.deltaTime * headBobLerpSpeed);
                
                float currentSway = Mathf.Lerp(currentHeadBobRotation.y, headBobSway, Time.deltaTime * headBobLerpSpeed);
                playerCamera.localRotation = Quaternion.Euler(xRotation, currentSway, 0);
                currentHeadBobRotation = new Vector3(xRotation, currentSway, 0);
            }
            else
            {
                playerCamera.localPosition = Vector3.Lerp(
                    playerCamera.localPosition, 
                    cameraOriginalPosition, 
                    Time.deltaTime * headCenteringSpeed);
                
                playerCamera.localRotation = Quaternion.Euler(xRotation, 0, 0);
            }
        }
        else
        {
            playerCamera.localPosition = Vector3.Lerp(
                playerCamera.localPosition,
                cameraOriginalPosition,
                Time.deltaTime * headCenteringSpeedAir);

            playerCamera.localRotation = Quaternion.Euler(xRotation, 0, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player is grounded
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        isGrounded = true;
    }
}