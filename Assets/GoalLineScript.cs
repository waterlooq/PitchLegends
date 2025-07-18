using UnityEngine;

public class GoalLineScript : MonoBehaviour
{
    public bool onGoalLine = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            onGoalLine = true;
            Debug.Log("GOAL!");
            // You could also call GameManager.Instance.OnGoalScored(); here
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            onGoalLine = false;
            Debug.Log("Ball left the goal area.");
        }
    }
}
