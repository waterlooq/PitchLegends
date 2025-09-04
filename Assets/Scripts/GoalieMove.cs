using UnityEngine;

public class GoalieMove : MonoBehaviour
{
    [Header("References")]
    public GameObject ball;
    public Transform leftPost;
    public Transform rightPost;

    [Header("Movement Settings")]
    public float speed = 5f;

    private Vector3 currentPosition;
    private Vector3 startPosition; // Store initial position

    void Start()
    {
        startPosition = transform.position; // Save start position
    }

    public void Move()
    {
        if (ball == null || leftPost == null || rightPost == null) return;

        currentPosition = transform.position;

        float minZ = Mathf.Min(leftPost.position.z, rightPost.position.z);
        float maxZ = Mathf.Max(leftPost.position.z, rightPost.position.z);

        float targetZ = Mathf.Clamp(ball.transform.position.z, minZ, maxZ);

        float newZ = Mathf.MoveTowards(currentPosition.z, targetZ, speed * Time.deltaTime);

        transform.position = new Vector3(currentPosition.x, currentPosition.y, newZ);
    }

    // New function to reset goalie to start position
    public void ResetPosition()
    {
        transform.position = startPosition;
    }

}
