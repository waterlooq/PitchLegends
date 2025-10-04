using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pass : MonoBehaviour
{
    [Header("Mode Toggle")]
    public bool isFinalPass = false;
    public Pass nextPass;

    [Header("UI Elements")]
    public RectTransform sliderArrow;
    public RectTransform greenZone;
    public GameObject sliderUI;

    [Header("Arrow Movement")]
    public float moveSpeed = 200f;
    public float powerTop = 150f;
    public float powerBottom = -150f;
    private bool goingUp = true;

    [Header("Game Objects")]
    public Transform ball;
    public Transform passTarget;
    public Transform defender;
    public float passForce = 10f;
    public Vector3 startPos;
    public GameObject failMessage;
    public AudioSource failAudio;
    public AudioSource kickSound;

    [Header("ArrowMove Reference")]
    public ArrowMove arrowMoveScript;

    private bool isPassing = false;
    public bool passFailed = false;

    [Header("Cameras")]
    public CinemachineVirtualCamera cam1;
    public CinemachineVirtualCamera cam2;

    public Rigidbody ballRb;

    void Start()
    {
        ballRb = ball.GetComponent<Rigidbody>();
        ballRb.isKinematic = false;
        startPos = ball.transform.position;
        sliderUI.SetActive(false);
        StartPass();

        cam1.Priority = 10;
        cam2.Priority = 0;
    }

    void Update()
    {
        if (isPassing)
        {
            MoveSliderArrowVertically();

            if (Input.GetKeyDown(KeyCode.Space) && passFailed == false)
            {
                EvaluatePass();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void StartPass()
    {
        ballRb.isKinematic = false;
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        isPassing = true;
        sliderUI.SetActive(true);

        Vector2 pos = sliderArrow.anchoredPosition;
        pos.y = powerBottom;
        sliderArrow.anchoredPosition = pos;

        goingUp = true;
    }

    void MoveSliderArrowVertically()
    {
        Vector2 pos = sliderArrow.anchoredPosition;
        float move = moveSpeed * Time.deltaTime * (goingUp ? 1 : -1);
        pos.y += move;

        if (pos.y >= powerTop)
        {
            pos.y = powerTop;
            goingUp = false;
        }
        else if (pos.y <= powerBottom)
        {
            pos.y = powerBottom;
            goingUp = true;
        }

        sliderArrow.anchoredPosition = pos;
    }

    void EvaluatePass()
    {
        ballRb.isKinematic = false;
        isPassing = false;
        sliderUI.SetActive(false);

        float arrowY = sliderArrow.anchoredPosition.y;
        float greenMin = greenZone.anchoredPosition.y - greenZone.rect.height / 2f;
        float greenMax = greenZone.anchoredPosition.y + greenZone.rect.height / 2f;

        if (arrowY >= greenMin && arrowY <= greenMax)
        {
            kickSound.Play();
            SuccessfulPass();

            if (isFinalPass)
            {

                cam1.Priority = 0;
                cam2.Priority = 10;

                if (!arrowMoveScript.gameObject.activeInHierarchy)
                    arrowMoveScript.gameObject.SetActive(true);

                arrowMoveScript.enabled = true;
                arrowMoveScript.BeginArrowSequence();

                this.enabled = false;
            }
            else
            {
                nextPass.enabled = true;
                this.enabled = false;
            }
        }
        else
        {
            FailedPass();
        }
    }

    void SuccessfulPass()
    {
        ballRb.isKinematic = false;
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        Vector3 direction = (passTarget.position - ball.position).normalized;
        ballRb.AddForce(direction * passForce, ForceMode.Impulse);
    }

    public void FailedPass()
    {
        ballRb.isKinematic = false;
        kickSound.Play();
        passFailed = true;
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        Vector3 direction = (defender.position - ball.position).normalized;
        ballRb.AddForce(direction * passForce, ForceMode.Impulse);

        failMessage.SetActive(true);
        if (failAudio != null) failAudio.Play();

        sliderUI.SetActive(false);
        

        if (!isFinalPass)
        {
            nextPass.enabled = false;
        }
       
    }

    private void OnCollisionEnter(Collision other)
    {
        // Stop ball immediately if it touches the teammate
        if (other.gameObject.tag == "Teammate")
        {
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
            ballRb.isKinematic = true;
        }
    }
}
