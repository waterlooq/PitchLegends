using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Cinemachine;

public class CornerKick : MonoBehaviour
{
    [Header("Arrow Direction Settings")]
    public GameObject arrow;
    public Transform teammate;
    public float teammateSpeed = 30f;

    [Header("Ball Settings")]
    public Rigidbody ballRb;
    public Transform cornerSpot;
    public float minPower = 5f;
    public float maxPower = 25f;
    public float upwardForceMultiplier = 0.7f;

    [Header("Power Meter UI")]
    public GameObject powerMeter;
    public RectTransform powerArrow;
    public float powerSpeed = 200f;
    public float powerTop = 150f;
    public float powerBottom = -150f;

    [Header("Goal System")]
    public GoalLineScript goalScript;
    public GameObject goalMessage;
    public GameObject failMessage;
    public GoalieMove goalie;
    public AudioSource goalAudio;
    public AudioSource failAudio;
    public AudioSource kickSound;
    public AudioSource strikeSound;
    public float resultDelay = 2f;

    [Header("Cinemachine Cameras")]
    public CinemachineVirtualCamera vcam1;
    public CinemachineVirtualCamera vcam2;

    [Header("Level Progression")]
    public int currentLevelIndex;
    public string[] levelSceneNames;

    private bool isPowerSelecting = false;
    private bool powerGoingUp = true;
    private bool shotOff = false;
    private float currentPower = 0f;
    private float actualForce = 0f;
    private bool ballInAir = false;
    private bool teammateRunning = false;
    private Vector3 teammateTarget;

    private float initialYRotation;
    private Vector3 shootDirection;

    void Start()
    {

        if (vcam1 != null && vcam2 != null)
        {
            vcam1.Priority = 10;
            vcam2.Priority = 0;
        }

        initialYRotation = arrow.transform.eulerAngles.y;

        StartPowerMeter();

        if (!PlayerPrefs.HasKey("UnlockedLevels"))
            PlayerPrefs.SetInt("UnlockedLevels", 1);
    }

    void Update()
    {
        if (teammate != null)
            arrow.transform.LookAt(teammate.position);

        if (isPowerSelecting)
        {
            MovePowerArrow();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopPowerMeter();
                if (kickSound != null) kickSound.Play();
                TakeCorner();
            }
        }

        if (ballInAir && Input.GetKeyDown(KeyCode.F))
        {
            StrikeBall();
            goalie.Move();
        }

        if (shotOff && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (teammateRunning)
            MoveTeammate();
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

    void TakeCorner()
    {
        ballInAir = true;

        ballRb.isKinematic = false;
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        Vector3 direction = (teammate.position - ballRb.position).normalized;
        shootDirection = direction;
        Vector3 force = direction * actualForce + Vector3.up * actualForce * upwardForceMultiplier;
        ballRb.AddForce(force, ForceMode.Impulse);

        if (vcam1 != null && vcam2 != null)
        {
            vcam1.Priority = 0;
            vcam2.Priority = 10;
        }

        StartCoroutine(CheckGoalAfterDelay());
    }

    void StrikeBall()
    {
        if (!ballInAir) return;

        teammateRunning = true;
        teammateTarget = ballRb.position;

        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        Vector3 strikeDir = (goalScript.transform.position - ballRb.position).normalized;
        Vector3 strikeForce = strikeDir * (actualForce * 1.2f) + Vector3.up * (actualForce * 0.3f);

        ballRb.AddForce(strikeForce, ForceMode.Impulse);

        if (strikeSound != null) strikeSound.Play();

        ballInAir = false;
    }

    void MoveTeammate()
    {
        if (teammate == null) return;

        teammate.position = Vector3.MoveTowards(
            teammate.position,
            new Vector3(teammateTarget.x, teammate.position.y, teammateTarget.z),
            teammateSpeed * Time.deltaTime
        );

        if (Vector3.Distance(teammate.position, teammateTarget) < 0.2f)
            teammateRunning = false;
    }

    IEnumerator CheckGoalAfterDelay()
    {
        yield return new WaitForSeconds(resultDelay);

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
            failMessage.SetActive(true);
            if (failAudio != null) failAudio.Play();
            shotOff = true;
        }
    }

    void UnlockNextLevel()
    {
        int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);

        if (currentLevelIndex + 1 >= unlockedLevels && currentLevelIndex + 1 < levelSceneNames.Length)
        {
            PlayerPrefs.SetInt("UnlockedLevels", currentLevelIndex + 2);
            PlayerPrefs.Save();
        }
    }

    void LoadNextLevel()
    {
        if (currentLevelIndex + 1 < levelSceneNames.Length)
            SceneManager.LoadScene(levelSceneNames[currentLevelIndex + 1]);
    }

    void SwitchCameraManual()
    {
        if (vcam1 == null || vcam2 == null) return;

        bool usingCam1 = vcam1.Priority > vcam2.Priority;

        if (usingCam1)
        {
            vcam1.Priority = 0;
            vcam2.Priority = 10;
        }
        else
        {
            vcam1.Priority = 10;
            vcam2.Priority = 0;
        }
    }
}
