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
    public int currentLevelIndex;
    public string[] levelSceneNames;

    private float initialYRotation;
    private bool isPowerSelecting = false;
    private bool isShooting = false;
    private bool powerGoingUp = true;
    private bool shotOff = false;
    private float currentPower = 0f;
    private float actualForce = 0f;
    private Vector3 shootDirection;

    public Vector3 startPosition;
    public GameObject ball;

    void Start()
    {
        initialYRotation = arrow.transform.eulerAngles.y;
        powerMeter.SetActive(false);

        if (!PlayerPrefs.HasKey("UnlockedLevels"))
        {
            PlayerPrefs.SetInt("UnlockedLevels", 1);
        }
    }

    void Update()
    {
        // === AIMING STATE ===
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
        // === POWER METER STATE ===
        else if (isPowerSelecting)
        {
            MovePowerArrow();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopPowerMeter();
                ShootBall();
            }
        }

        // === GOALIE MOVEMENT ===
        if (isShooting)
        {
            goalie.Move();
        }

        // === RESET SYSTEM ===
        if (shotOff && Input.GetKeyDown(KeyCode.R))
        {
            ResetShot();
        }
    }

    // -------------------------
    // START POWER METER
    // -------------------------
    void StartPowerMeter()
    {
        isPowerSelecting = true;
        powerMeter.SetActive(true);
        powerArrow.anchoredPosition = new Vector2(powerArrow.anchoredPosition.x, powerBottom);
        powerGoingUp = true;
    }

    // -------------------------
    // MOVE POWER ARROW
    // -------------------------
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

    // -------------------------
    // STOP POWER METER
    // -------------------------
    void StopPowerMeter()
    {
        isPowerSelecting = false;
        powerMeter.SetActive(false);

        currentPower = Mathf.InverseLerp(powerBottom, powerTop, powerArrow.anchoredPosition.y);
        actualForce = Mathf.Lerp(minPower, maxPower, currentPower);
    }

    // -------------------------
    // SHOOT BALL
    // -------------------------
    void ShootBall()
    {
        isShooting = true;

        // Reset physics before shooting
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        Vector3 force = shootDirection * actualForce + Vector3.up * actualForce * upwardForceMultiplier;
        ballRb.AddForce(force, ForceMode.Impulse);

        StartCoroutine(CheckGoalAfterDelay());
    }

    // -------------------------
    // CHECK GOAL RESULT
    // -------------------------
    private IEnumerator CheckGoalAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        if (goalScript.goal)
        {
            goalMessage.SetActive(true);
            if (goalAudio != null) goalAudio.Play();

            UnlockNextLevel();
            yield return new WaitForSeconds(3f);
            LoadNextLevel();
        }
        else
        {
            shotOff = true;
            failMessage.SetActive(true);
            if (failAudio != null) failAudio.Play();
        }
    }

    // -------------------------
    // RESET SHOT SYSTEM (R KEY)
    // -------------------------
    void ResetShot()
    {

        goalie.ResetPosition();

        // Stop all ball physics
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        // Move ball back to starting position
        ball.transform.position = startPosition;

        // Reset states
        shotOff = false;
        isShooting = false;
        isPowerSelecting = false;

        // Hide fail message
        if (failMessage.activeSelf)
            failMessage.SetActive(false);

        // Stop fail sound if playing
        if (failAudio != null && failAudio.isPlaying)
            failAudio.Stop();

        // Reset arrow for aiming again
        swingSpeed = 1f;
        arrow.transform.rotation = Quaternion.Euler(0f, initialYRotation, 0f);

        // Reset goal flag for next shot
        goalScript.goal = false;
    }

    // -------------------------
    // UNLOCK NEXT LEVEL
    // -------------------------
    private void UnlockNextLevel()
    {
        int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);

        if (currentLevelIndex + 1 >= unlockedLevels && currentLevelIndex + 1 < levelSceneNames.Length)
        {
            PlayerPrefs.SetInt("UnlockedLevels", currentLevelIndex + 2);
            PlayerPrefs.Save();
        }
    }

    // -------------------------
    // LOAD NEXT LEVEL
    // -------------------------
    public void LoadNextLevel()
    {
        if (currentLevelIndex + 1 < levelSceneNames.Length)
        {
            SceneManager.LoadScene(levelSceneNames[currentLevelIndex + 1]);
        }
    }

    // -------------------------
    // RESTART AIMING SEQUENCE
    // -------------------------
    public void BeginArrowSequence()
    {
        isShooting = false;
        isPowerSelecting = false;
        swingSpeed = 1f;
        powerMeter.SetActive(false);
        initialYRotation = arrow.transform.eulerAngles.y;
    }
}
