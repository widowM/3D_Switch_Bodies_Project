using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject dog;
    public GameObject bird;

    // private void Start()
    // {
    //     // Disable both creatures initially
    //     if(dog != null) dog.GetComponent<Creature>().enabled = false;
    //     if(bird != null) bird.GetComponent<Creature>().enabled = false;
        
    //     // Now enable only the dog
    //     PlayerController.SwitchControl(dog);
    // }

    // private void Update()
    // {
    //     // Switch control between dog and bird using keys
    //     if (Input.GetKeyDown(KeyCode.Keypad1))
    //     {
    //         PlayerController.SwitchControl(dog);
    //     }
    //     else if (Input.GetKeyDown(KeyCode.Keypad2))
    //     {
    //         PlayerController.SwitchControl(bird);
    //     }
    // }
}