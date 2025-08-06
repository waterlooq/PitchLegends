using UnityEngine;

public class DefenderMove : MonoBehaviour
{
    public float distanceToMove = 3f;  // Total distance to travel left and right
    public float speed = 2f;           // Movement speed

    private Vector3 startLocalPos;
    private bool movingRight = true;

    void Start()
    {
        // Save the initial local position
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        // Move along local X (side to side)
        float direction = movingRight ? 1f : -1f;
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime, Space.Self);

        // Measure how far it's moved from the start point on local X
        float localXMoved = transform.localPosition.x - startLocalPos.x;

        // Reverse direction when the boundary is reached
        if (Mathf.Abs(localXMoved) >= distanceToMove)
        {
            movingRight = !movingRight;
        }
    }
}
