using UnityEngine;

public class Creature : MonoBehaviour
{
    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    protected virtual void Jump()
    {
        Debug.Log($"{gameObject.name} is jumping!");
    }
}