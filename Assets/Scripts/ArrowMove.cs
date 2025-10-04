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
    public AudioSource kickSound;
    public float delay;
    public float passDelay;

    [Header("Level Progression")]
    public int currentLevelIndex;
    public string[] levelSceneNames;

    [Header("Wind Settings")]
    public bool windEnabled = false;             // Default: off
    public Vector3 windDirection = Vector3.zero; // Normalized direction
    public float windStrength = 0f;              // Magnitude of wind

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

        StartCoroutine(WaitOneSecond());

        initialYRotation = arrow.transform.eulerAngles.y;
        powerMeter.SetActive(false);

        //if (windEnabled)
        //{
        //    // Randomize wind if desired
        //    windDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        //    windStrength = Random.Range(1f, 5f); // Example strength
        //}

        if (!PlayerPrefs.HasKey("UnlockedLevels"))
            PlayerPrefs.SetInt("UnlockedLevels", 1);
    }

    void Update()
    {
        // Arrow swinging
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
                if (kickSound != null) kickSound.Play();
                ShootBall();
            }
        }

        if (shotOff && Input.GetKeyDown(KeyCode.R))
            ResetShot();

        if (isShooting)
            goalie.Move();
    }

    void FixedUpdate()
    {
        if (isShooting && windEnabled && ballRb != null)
        {
            ApplyWindCurl();
        }
    }

    IEnumerator WaitOneSecond()
    {
        Debug.Log("Waiting...");
        yield return new WaitForSeconds(passDelay);  // Delay for 1 second
        ball.transform.position = startPosition;
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        Debug.Log("1 second passed!");
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

        // Reset physics before shooting
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        ballRb.isKinematic = false;

        Vector3 force = shootDirection * actualForce + Vector3.up * actualForce * upwardForceMultiplier;
        ballRb.AddForce(force, ForceMode.Impulse);

        StartCoroutine(CheckGoalAfterDelay());
    }

    private void ApplyWindCurl()
    {
        // Horizontal wind only
        Vector3 horizontalWind = new Vector3(windDirection.x, 0f, windDirection.z);

        // Apply straight wind
        Vector3 windForce = horizontalWind * windStrength;

        // Curl effect perpendicular to wind
        Vector3 curl = Vector3.Cross(Vector3.up, horizontalWind).normalized * (windStrength * 0.3f);

        ballRb.AddForce(windForce + curl, ForceMode.Acceleration);
    }

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

    void ResetShot()
    {
        goalie.ResetPosition();

        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        ball.transform.position = startPosition;
        ballRb.GetComponentInChildren<TrailRenderer>().Clear();

        shotOff = false;
        isShooting = false;
        isPowerSelecting = false;

        if (failMessage.activeSelf)
            failMessage.SetActive(false);

        if (failAudio != null && failAudio.isPlaying)
            failAudio.Stop();

        swingSpeed = 1f;
        arrow.transform.rotation = Quaternion.Euler(0f, initialYRotation, 0f);

        goalScript.goal = false;
    }

    private void UnlockNextLevel()
    {
        int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);

        if (currentLevelIndex + 1 >= unlockedLevels && currentLevelIndex + 1 < levelSceneNames.Length)
        {
            PlayerPrefs.SetInt("UnlockedLevels", currentLevelIndex + 2);
            PlayerPrefs.Save();
        }
    }

    public void LoadNextLevel()
    {
        if (currentLevelIndex + 1 < levelSceneNames.Length)
            SceneManager.LoadScene(levelSceneNames[currentLevelIndex + 1]);
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
