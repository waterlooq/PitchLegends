using UnityEngine;
using UnityEngine.SceneManagement;

public class DefenderMove : MonoBehaviour
{
    public float distanceToMove = 3f;
    public float speed = 2f;

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
        if (intercepted) return; 

        float direction = movingRight ? 1f : -1f;
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime, Space.Self);

        float localXMoved = transform.localPosition.x - startLocalPos.x;

        if (Mathf.Abs(localXMoved) >= distanceToMove)
        {
            // freeze ball pos so it doesnt overshoot (GOTTA FIX THIS LATER)
            float clampedX = Mathf.Clamp(localXMoved, -distanceToMove, distanceToMove);
            transform.localPosition = new Vector3(startLocalPos.x + clampedX, transform.localPosition.y, transform.localPosition.z);

            // Reverse direction only ONCE
            movingRight = !movingRight;
        }

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
