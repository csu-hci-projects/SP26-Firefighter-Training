using UnityEngine;

public class HiddenGoal : MonoBehaviour
{
    public string goalId = "checked_bedroom";
    public string goalName = "Checked the bedroom";
    public string missedMessage = "You did not check the bedroom.";
    public string completedMessage = "You checked the bedroom.";

    public bool completed;

    public void ResetGoal()
    {
        completed = false;
    }

    public void MarkCompleted()
    {
        completed = true;
    }
}