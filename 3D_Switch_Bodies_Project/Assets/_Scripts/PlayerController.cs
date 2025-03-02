using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Delegate for control logic
    public delegate void ControlDelegate();

    // Event to trigger when control is switched
    public static event ControlDelegate OnControlSwitch;

    private static GameObject currentControlledCreature;

    public static void SwitchControl(GameObject newCreature)
    {
        if (currentControlledCreature != null)
        {
            // Disable ALL components on the current creature
            foreach (var comp in currentControlledCreature.GetComponentsInChildren<MonoBehaviour>())
            {
                if (comp is Creature || comp is BatMovementDaryl || comp is HumanMovementDaryl)
                {
                    comp.enabled = false;
                }
            }
        }

        currentControlledCreature = newCreature;

        // Enable only the necessary components on the new creature
        var creature = currentControlledCreature.GetComponentInChildren<Creature>();
        if (creature != null)
        {
            creature.enabled = true;

            // Get the specific movement component
            var movement = currentControlledCreature.GetComponentInChildren<BatMovementDaryl>() 
                          ?? (MonoBehaviour)currentControlledCreature.GetComponentInChildren<HumanMovementDaryl>();
            
            if (movement != null)
            {
                movement.enabled = true;
            }
        }

        OnControlSwitch?.Invoke();
    }
}