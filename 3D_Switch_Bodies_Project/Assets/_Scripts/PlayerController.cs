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
            currentControlledCreature.GetComponent<Creature>().enabled = false;
        }

        currentControlledCreature = newCreature;

        currentControlledCreature.GetComponent<Creature>().enabled = true;

        OnControlSwitch?.Invoke();
    }
}