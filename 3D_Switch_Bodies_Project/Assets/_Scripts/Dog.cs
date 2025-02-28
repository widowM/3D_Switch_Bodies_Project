using UnityEngine;

public class Dog : Creature
{
    protected override void Update()
    {
        // Call base control logic
        base.Update(); 

        // Dog-specific controls
        if (Input.GetKeyDown(KeyCode.E))
        {
            Bark();
        }
    }

    private void Bark()
    {
        Debug.Log("Woof!");
    }
}