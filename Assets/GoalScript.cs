using UnityEngine;

public class GoalScript : MonoBehaviour
{
    private bool inGoal = false;
    private bool goalScored = false;

    public GoalLineScript goalLine;

    private void Update()
    {
        if (!goalScored && inGoal && !goalLine.onGoalLine)
        {
            goalScored = true;
            Debug.Log("GOAL!");
            // Call GameManager.Instance.OnGoalScored(); or any goal logic here
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            inGoal = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            inGoal = false;
        }
    }

    public void ResetGoal()
    {
        goalScored = false;
    }
}
