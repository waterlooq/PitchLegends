using UnityEngine;

public class ArrowMove : MonoBehaviour
{
    public float swingAngle = 45f;
    public float swingSpeed = 1f;
    public float ballSpeed = 20f;
    public GameObject ball;
    public GameObject powerMeter;

    private float initialYRotation;
    private bool isShooting = false;
    private Vector3 shootDirection;

    void Start()
    {
        initialYRotation = transform.eulerAngles.y;
        powerMeter.SetActive(false);
    }

    void Update()
    {
        if (!isShooting)
        {
            // Arrow swinging
            float angle = Mathf.Sin(Time.time * swingSpeed) * swingAngle;
            transform.rotation = Quaternion.Euler(0f, initialYRotation + angle, 0f);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Stop swinging and prepare to shoot
                swingSpeed = 0;
                shootDirection = transform.forward;
                ball.transform.rotation = transform.rotation;
                PowerMeter();
            }
        }
        else
        {
            // Move ball forward
            ball.transform.position += shootDirection * ballSpeed * Time.deltaTime;
        }
    }

    void PowerMeter()
    {
        powerMeter.SetActive(true);
    }

}
