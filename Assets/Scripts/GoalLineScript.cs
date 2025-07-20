using UnityEngine;

public class GoalLineScript : MonoBehaviour
{
    public bool goal = false;
    public GameObject goalMessage;
    public AudioSource goalAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            if (!goal)
            {
                goal = true;
                goalMessage.SetActive(true);
                if (goalAudio != null)
                {
                    goalAudio.Play();
                }
            }
        }
        else
        {
            goal = false;
            Debug.Log("NO GOAL!");
        }
    }
}
