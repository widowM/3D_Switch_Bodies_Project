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
    
    private List<GameObject> existingDots = new List<GameObject>(); // Track active dots
    public int maxDots = 100;

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

    private IEnumerator  RayWave()
    {
        List<RaycastHit> hitList = new List<RaycastHit>();
        HashSet<Vector3> hitPoints = new HashSet<Vector3>();
        WaitForSeconds wait = new WaitForSeconds(delayBetweenDots);

        Vector3 origin = transform.position;

        for (int i = 0; i < numberOfRays * numberOfRays; i++)  // Reduce iteration count
        {
            Vector3 direction = Random.insideUnitSphere.normalized; 
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~layersToExclude))
            {
                if (hitPoints.Add(hit.point)) // HashSet avoids duplicates automatically
                {
                    hitList.Add(hit);
                }
            }
        }

        // Step 2: Sort hits by distance (closest first)
        hitList.Sort((a, b) => a.distance.CompareTo(b.distance));

        // Step 3: Instantiate dots one by one with a delay
        foreach (var hit in hitList)
        {
            if (existingDots.Count >= maxDots)
            {
                Destroy(existingDots[0]); // Destroy the oldest dot
                existingDots.RemoveAt(0);
            }

            GameObject currentDot = Instantiate(dotPrefab, hit.point, Quaternion.identity);
            currentDot.transform.up = hit.normal; // Align the dot with the surface

            if (hit.collider.CompareTag("Living"))
            {
                currentDot.GetComponent<Renderer>().material.color = Color.red; // Set to red
            }
            else
            {
                currentDot.GetComponent<Renderer>().material.color = Color.white; // Set to black
            }

            yield return wait; // Small delay to create wave effect
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