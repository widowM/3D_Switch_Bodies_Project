using UnityEngine;

public class Bird : Creature
{
    protected override void Update()
    {
        // Call base control logic
        base.Update();

        // Bird-specific controls
        if (Input.GetKeyDown(KeyCode.F))
        {
            Fly();
        }
    }

    private void Fly()
    {
        Debug.Log("Flying!");
    }
}