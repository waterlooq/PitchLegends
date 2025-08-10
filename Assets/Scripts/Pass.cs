using Cinemachine;
using System.Net.Mail;
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

    [Header("ArrowMove Reference")]
    public ArrowMove arrowMoveScript;

    private bool isPassing = false;
    private bool passOff = false;

    [Header("Cameras")]
    public CinemachineVirtualCamera cam1;
    public CinemachineVirtualCamera cam2;

    void Start()
    {
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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                EvaluatePass();
            }
        }

        if(passOff == true && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }

    void StartPass()
    {
        ball.transform.position = startPos;
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
        isPassing = false;
        sliderUI.SetActive(false);

        float arrowY = sliderArrow.anchoredPosition.y;
        float greenMin = greenZone.anchoredPosition.y - greenZone.rect.height / 2f;
        float greenMax = greenZone.anchoredPosition.y + greenZone.rect.height / 2f;

        if (arrowY >= greenMin && arrowY <= greenMax)
        {
            SuccessfulPass();

            if (isFinalPass)
            {
                Debug.Log("✅ Final pass succeeded → Switching camera + shooting mode");

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
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 direction = (passTarget.position - ball.position).normalized;
        rb.AddForce(direction * passForce, ForceMode.Impulse);
    }

    public void FailedPass()
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 direction = (defender.position - ball.position).normalized;
        rb.AddForce(direction * passForce, ForceMode.Impulse);

        failMessage.SetActive(true);
        if (failAudio != null) failAudio.Play();

        passOff = true;

        Debug.Log("❌ Pass failed! Defender intercepted.");
    }

    void StartDefenderIntercept()
    {
        StartCoroutine(MoveDefenderToBall());
    }

    System.Collections.IEnumerator MoveDefenderToBall()
    {
        Vector3 start = defender.position;
        Vector3 end = ball.position;
        float duration = 0.1f;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            defender.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        Debug.Log("⚠️ Defender reached the ball.");
    }
}
