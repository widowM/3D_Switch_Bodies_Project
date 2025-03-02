using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField] private GameObject creatureCameraGO;

    private static float switchCooldown = 1f; // Cooldown duration in seconds
    private static float lastSwitchTime = 0f;
    private static Creature activeCreature;
    private bool isBeingDisabled = false;

    protected virtual void Update()
    {
        if (!enabled) return; // Safety check
    }

    protected virtual void FixedUpdate()
    {
        if (!enabled) return; // Safety check
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only allow switching if this is the currently enabled creature
        if (other.gameObject.CompareTag("Creature") && 
            enabled && 
            Time.time > lastSwitchTime + switchCooldown)
        {
            GameObject targetCreature = other.gameObject.transform.parent.gameObject;
            
            // Check if target creature is already enabled
            Creature targetCreatureComponent = targetCreature.GetComponentInChildren<Creature>();
            if (targetCreatureComponent != null && !targetCreatureComponent.enabled)
            {
                Debug.Log("Switching to " + targetCreature.name);
                lastSwitchTime = Time.time;
                PlayerController.SwitchControl(targetCreature);
            }
        }  
    }

    protected virtual void OnEnable()
    {
        if (!isBeingDisabled && activeCreature != null && activeCreature != this)
        {
            activeCreature.isBeingDisabled = true;
            activeCreature.enabled = false;
            activeCreature.isBeingDisabled = false;
        }
        
        activeCreature = this;
        if (creatureCameraGO != null)
        {
            creatureCameraGO.SetActive(true);
        }
    }

    protected virtual void OnDisable()
    {
        if (!isBeingDisabled && activeCreature == this)
        {
            activeCreature = null;
        }

        if (creatureCameraGO != null)
        {
            creatureCameraGO.SetActive(false);
        }

        // Force disable ALL movement components
        var components = GetComponentsInChildren<MonoBehaviour>();
        foreach (var comp in components)
        {
            if (comp != this && (comp is BatMovementDaryl || comp is HumanMovementDaryl))
            {
                comp.enabled = false;
                Debug.Log($"Forcefully disabled {comp.GetType().Name} in OnDisable");
            }
        }
    }

    public bool IsActiveCreature()
    {
        return this == activeCreature;
    }

    private void Start()
    {
        // Force initial state check
        if (!IsActiveCreature())
        {
            enabled = false;
        }
    }
}