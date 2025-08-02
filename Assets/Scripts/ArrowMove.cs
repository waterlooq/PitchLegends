using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ArrowMove : MonoBehaviour
{
    [Header("Arrow Direction Settings")]
    public GameObject arrow;
    public float swingAngle = 45f;
    public float swingSpeed = 1f;

    [Header("Ball Settings")]
    public Rigidbody ballRb;
    public float minPower = 5f;
    public float maxPower = 30f;
    public float upwardForceMultiplier = 0.5f;

    [Header("Power Meter UI")]
    public GameObject powerMeter;
    public RectTransform powerArrow;
    public float powerSpeed = 200f;
    public float powerTop = 150f;
    public float powerBottom = -150f;

    [Header("Goal System")]
    public GoalieMove goalie;
    public GoalLineScript goalScript;
    public GameObject goalMessage;
    public GameObject failMessage;
    public AudioSource goalAudio;
    public AudioSource failAudio;
    public float delay;

    [Header("Level Progression")]
    public int currentLevelIndex; // set this manually per scene (0 for Level 1, 1 for Level 2, etc.)
    public string[] levelSceneNames; // assign all scene names in order in the inspector

    private float initialYRotation;
    private bool isPowerSelecting = false;
    private bool isShooting = false;
    private bool powerGoingUp = true;
    private float currentPower = 0f;
    private float actualForce = 0f;
    private Vector3 shootDirection;

    void Start()
    {
        initialYRotation = arrow.transform.eulerAngles.y;
        powerMeter.SetActive(false);

        // Initialize unlocked levels if not already set
        if (!PlayerPrefs.HasKey("UnlockedLevels"))
        {
            PlayerPrefs.SetInt("UnlockedLevels", 1); // Level 1 unlocked by default
        }
    }

    void Update()
    {
        if (!isShooting && !isPowerSelecting)
        {
            float angle = Mathf.Sin(Time.time * swingSpeed) * swingAngle;
            arrow.transform.rotation = Quaternion.Euler(0f, initialYRotation + angle, 0f);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                swingSpeed = 0;
                shootDirection = arrow.transform.forward;
                StartPowerMeter();
            }
        }
        else if (isPowerSelecting)
        {
            MovePowerArrow();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopPowerMeter();
                ShootBall();
            }
        }

        if (isShooting)
        {
            goalie.Move();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void StartPowerMeter()
    {
        isPowerSelecting = true;
        powerMeter.SetActive(true);
        powerArrow.anchoredPosition = new Vector2(powerArrow.anchoredPosition.x, powerBottom);
        powerGoingUp = true;
    }

    void MovePowerArrow()
    {
        Vector2 pos = powerArrow.anchoredPosition;
        float move = powerSpeed * Time.deltaTime * (powerGoingUp ? 1 : -1);
        pos.y += move;

        if (pos.y >= powerTop)
        {
            pos.y = powerTop;
            powerGoingUp = false;
        }
        else if (pos.y <= powerBottom)
        {
            pos.y = powerBottom;
            powerGoingUp = true;
        }

        powerArrow.anchoredPosition = pos;
    }

    void StopPowerMeter()
    {
        isPowerSelecting = false;
        powerMeter.SetActive(false);

        currentPower = Mathf.InverseLerp(powerBottom, powerTop, powerArrow.anchoredPosition.y);
        actualForce = Mathf.Lerp(minPower, maxPower, currentPower);
    }

    void ShootBall()
    {
        isShooting = true;

        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        Vector3 force = shootDirection * actualForce + Vector3.up * actualForce * upwardForceMultiplier;
        ballRb.AddForce(force, ForceMode.Impulse);

        StartCoroutine(CheckGoalAfterDelay());
    }

    private IEnumerator CheckGoalAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        if (goalScript.goal)
        {
            goalMessage.SetActive(true);
            if (goalAudio != null) goalAudio.Play();

            UnlockNextLevel(); // Unlocks the next level
            yield return new WaitForSeconds(3f); // Optional delay before loading
            LoadNextLevel();
        }
        else
        {
            failMessage.SetActive(true);
            if (failAudio != null) failAudio.Play();
        }
    }

    private void UnlockNextLevel()
    {
        int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);

        // If current level index + 1 is higher than unlocked levels, update it
        if (currentLevelIndex + 1 >= unlockedLevels && currentLevelIndex + 1 < levelSceneNames.Length)
        {
            PlayerPrefs.SetInt("UnlockedLevels", currentLevelIndex + 2);
            PlayerPrefs.Save();
        }
    }

    // Call this from a button when the player wants to load the next level
    public void LoadNextLevel()
    {
        if (currentLevelIndex + 1 < levelSceneNames.Length)
        {
            SceneManager.LoadScene(levelSceneNames[currentLevelIndex + 1]);
        }
    }

    public void BeginArrowSequence()
    {
        isShooting = false;
        isPowerSelecting = false;
        swingSpeed = 1f;
        powerMeter.SetActive(false);
        initialYRotation = arrow.transform.eulerAngles.y;
    }
}
