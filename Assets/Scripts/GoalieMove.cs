using UnityEngine;

public class GoalieMove : MonoBehaviour
{
    [Header("References")]
    public GameObject ball;
    public Transform leftPost;   // Assign in Inspector
    public Transform rightPost;  // Assign in Inspector

    [Header("Movement Settings")]
    public float speed = 5f;

    private Vector3 currentPosition;

    public void Move()
    {
        if (ball == null || leftPost == null || rightPost == null) return;

        currentPosition = transform.position;

        // Get the min and max Z from posts
        float minZ = Mathf.Min(leftPost.position.z, rightPost.position.z);
        float maxZ = Mathf.Max(leftPost.position.z, rightPost.position.z);

        // Target Z = ball Z, clamped to posts
        float targetZ = Mathf.Clamp(ball.transform.position.z, minZ, maxZ);

        // Smooth movement
        float newZ = Mathf.MoveTowards(currentPosition.z, targetZ, speed * Time.deltaTime);

        transform.position = new Vector3(currentPosition.x, currentPosition.y, newZ);
    }
}
