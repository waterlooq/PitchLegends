using UnityEngine;

public class GoalieMove : MonoBehaviour
{
    [Header("References")]
    public GameObject ball;

    [Header("Movement Settings")]
    public float speed = 5f;

    private Vector3 currentPosition;
    private Vector3 destination;

    void Update()
    {
        // Optional: Automatically track the ball every frame
        // Move();
    }

    // Call this method when the ball is shot or passed
    public void Move()
    {
        if (ball == null) return;

        currentPosition = transform.position;

        // Only move towards the ball's Z position
        float targetZ = ball.transform.position.z;
        float newZ = Mathf.MoveTowards(currentPosition.z, targetZ, speed * Time.deltaTime);

        // Keep X and Y the same
        transform.position = new Vector3(currentPosition.x, currentPosition.y, newZ);
    }
}
