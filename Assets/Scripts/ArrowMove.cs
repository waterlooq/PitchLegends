using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArrowMove : MonoBehaviour
{
    [Header("Arrow Direction Settings")]
    public GameObject arrow;                 // Arrow GameObject (now used for direction)
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
    }

    void Update()
    {
        if (!isShooting && !isPowerSelecting)
        {
            // Swinging arrow
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
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

        Debug.Log($"Power: {currentPower:F2}, Force: {actualForce:F2}");
    }

    void ShootBall()
    {
        isShooting = true;

        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        Vector3 force = shootDirection * actualForce + Vector3.up * actualForce * upwardForceMultiplier;
        ballRb.AddForce(force, ForceMode.Impulse);
    }

    public void BeginArrowSequence()
    {
        isShooting = false;
        isPowerSelecting = false;
        swingSpeed = 1f;
        powerMeter.SetActive(false);
        initialYRotation = arrow.transform.eulerAngles.y;

        Debug.Log("Arrow sequence started after pass.");
    }


}
