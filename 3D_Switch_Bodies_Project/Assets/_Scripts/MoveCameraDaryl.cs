using UnityEngine;

public class MoveCameraDaryl : MonoBehaviour
{
    public Transform cameraPosition;  

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = cameraPosition.position;
    }
}