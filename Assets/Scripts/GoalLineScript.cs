using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalLineScript : MonoBehaviour
{
    public bool goal = false;
    public GameObject goalMessage;
    public AudioSource goalAudio;

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            if (!goal)
            {
                goal = true;
            }
        }
        else
        {
            goal = false;
            Debug.Log("NO GOAL!");
        }
    }
}
