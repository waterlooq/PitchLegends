using UnityEngine;
using UnityEngine.UI;

public class Pass : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform sliderArrow;       // The moving arrow (black line)
    public RectTransform greenZone;         // The success green zone
    public GameObject sliderUI;             // Whole UI container (panel)

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
    public float defenderInterceptTime = 0.5f;
    public Vector3 startPos;

    private bool isPassing = false;

    void Start()
    {
        startPos = ball.transform.position;
        sliderUI.SetActive(false);
        StartPass();
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
    }

    void StartPass()
    {
        ball.transform.position = startPos;
        isPassing = true;
        sliderUI.SetActive(true);

        // Reset arrow to bottom of the bar (powerBottom)
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

        Vector3 direction = (passTarget.position - ball.position).normalized;
        rb.AddForce(direction * passForce, ForceMode.Impulse);

        Debug.Log("✅ Pass Successful!");

    }

    void FailedPass()
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;

        // Make the ball move towards the defender instead of pass target
        Vector3 direction = (defender.position - ball.position).normalized;
        rb.AddForce(direction * passForce, ForceMode.Impulse);

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
