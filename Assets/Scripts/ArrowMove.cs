using UnityEngine;

public class ArrowMove : MonoBehaviour
{
    public float swingAngle = 45f; // Maximum angle left/right from the center
    public float swingSpeed = 1f;  // Speed of the swing
    public Quaternion rotation;

    private float initialYRotation;

    void Start()
    {
        // Store the starting Y rotation
        initialYRotation = transform.eulerAngles.y;
    }

    void Update()
    {
        // Calculate angle using a sine wave
        float angle = Mathf.Sin(Time.time * swingSpeed) * swingAngle;

        // Apply rotation around Y-axis
        transform.rotation = Quaternion.Euler(0f, initialYRotation + angle, 0f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            swingSpeed = 0;
            rotation = transform.rotation;
        }

    }
    
}
