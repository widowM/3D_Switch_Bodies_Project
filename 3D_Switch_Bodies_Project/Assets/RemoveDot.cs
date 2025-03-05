using UnityEngine;

public class RemoveDot : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.5f); // Destroy this object after 1 second
    }
}
