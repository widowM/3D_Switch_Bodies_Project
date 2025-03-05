using System.Collections;
using System.Collections.Generic;
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

    [Header("Ecolocation")]
    public GameObject dotPrefab;
    private GameObject currentDot;
    public int numberOfRays = 1; 

    public LayerMask layersToExclude;

    public float delayBetweenDots = 0.05f; 
    public float minDistance = 0.3f;
    
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

        if (Input.GetKey(KeyCode.E))
        {
            StartCoroutine(RayWave());
        }
       // Debug.Log(rb.linearVelocity);
    }

    private IEnumerator RayWave()
    {
        List<RaycastHit> hitList = new List<RaycastHit>();

        // Step 1: Collect all raycasts
        for (int x = 0; x < numberOfRays; x++)
        {
            for (int y = 0; y < numberOfRays; y++)
            {
                for (int z = 0; z < numberOfRays; z++)
                {
                    float mappedXValue = (2f * x / numberOfRays) - 1f;
                    float mappedYValue = (2f * y / numberOfRays) - 1f;
                    float mappedZValue = (2f * z / numberOfRays) - 1f;

                    Vector3 direction = new Vector3(mappedXValue, mappedYValue, mappedZValue).normalized;
                    Ray ray = new Ray(transform.position, direction);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~layersToExclude))
                    {
                        bool tooClose = false;
                        foreach (var existingHit in hitList)
                        {
                            if (Vector3.Distance(existingHit.point, hit.point) < minDistance)
                            {
                                tooClose = true;
                                break;
                            }
                        }

                        if (!tooClose)
                        {
                            hitList.Add(hit);
                        }
                    }
                }
            }
        }

        // Step 2: Sort hits by distance (closest first)
        hitList.Sort((a, b) => a.distance.CompareTo(b.distance));

        // Step 3: Instantiate dots one by one with a delay
        foreach (var hit in hitList)
        {
            GameObject currentDot = Instantiate(dotPrefab, hit.point, Quaternion.identity);
            currentDot.transform.up = hit.normal; // Align the dot with the surface

            yield return new WaitForSeconds(delayBetweenDots); // Small delay to create wave effect
        }
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