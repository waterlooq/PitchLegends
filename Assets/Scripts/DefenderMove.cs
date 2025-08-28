using UnityEngine;
using UnityEngine.SceneManagement;

public class DefenderMove : MonoBehaviour
{
    public float distanceToMove = 3f;   // Total distance to travel left and right
    public float speed = 2f;            // Movement speed

    private Vector3 startLocalPos;
    private bool movingRight = true;
    public Pass pass;

    public bool isFreeKick = false;
    private bool intercepted = false;

    void Start()
    {
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        if (intercepted) return; // Stop moving when intercepted

        // Calculate movement direction
        float direction = movingRight ? 1f : -1f;
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime, Space.Self);

        // Measure how far we've moved from start
        float localXMoved = transform.localPosition.x - startLocalPos.x;

        // Check boundary
        if (Mathf.Abs(localXMoved) >= distanceToMove)
        {
            // Clamp the position so we don't overshoot
            float clampedX = Mathf.Clamp(localXMoved, -distanceToMove, distanceToMove);
            transform.localPosition = new Vector3(startLocalPos.x + clampedX, transform.localPosition.y, transform.localPosition.z);

            // Reverse direction only once
            movingRight = !movingRight;
        }

        // Allow restart on R
        if (intercepted && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ball") && !isFreeKick)
        {
            pass.FailedPass();
            intercepted = true;
        }
    }
}
