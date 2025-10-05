using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalieMove : MonoBehaviour
{
    [Header("References")]
    public GameObject ball;
    public Transform leftPost;
    public Transform rightPost;
    public GameObject pauseMenu;

    [Header("Movement Settings")]
    public float speed = 5f;

    private Vector3 currentPosition;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
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

    // function that resets goalies position (finally fixed this)
    public void ResetPosition()
    {
        transform.position = startPosition;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void Home()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

}
