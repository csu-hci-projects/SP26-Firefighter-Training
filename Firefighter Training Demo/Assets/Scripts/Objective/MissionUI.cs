using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class MissionUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;
    public GameObject missionPanel;
    public GameObject resultsPanel;

    [Header("Text")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultsText;

    public void ShowStartScreen()
    {
        if (startPanel != null) startPanel.SetActive(true);
        if (missionPanel != null) missionPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(false);
    }

    public void ShowMissionScreen()
    {
        if (startPanel != null) startPanel.SetActive(false);
        if (missionPanel != null) missionPanel.SetActive(true);
        if (resultsPanel != null) resultsPanel.SetActive(false);
    }

    public void UpdateTimer(float time)
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void ShowResults(float finalTime, List<FireObjective> fires, List<HiddenGoal> goals)
    {
        if (startPanel != null) startPanel.SetActive(false);
        if (missionPanel != null) missionPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(true);

        if (resultsText == null)
            return;

        StringBuilder sb = new StringBuilder();

        int minutes = Mathf.FloorToInt(finalTime / 60f);
        int seconds = Mathf.FloorToInt(finalTime % 60f);

        sb.AppendLine("TRAINING COMPLETE");
        sb.AppendLine();
        sb.AppendLine($"Final Time: {minutes:00}:{seconds:00}");
        sb.AppendLine();

        sb.AppendLine("Fires Extinguished:");
        foreach (FireObjective fire in fires)
        {
            string status = fire.IsExtinguished ? "Completed" : "Missed";
            sb.AppendLine($"- {fire.fireName}: {status}");
        }

        sb.AppendLine();
        sb.AppendLine("Hidden Goals:");

        foreach (HiddenGoal goal in goals)
        {
            if (goal.completed)
                sb.AppendLine($"✓ {goal.completedMessage}");
            else
                sb.AppendLine($"✗ {goal.missedMessage}");
        }

        resultsText.text = sb.ToString();
    }

    public void StartMissionFromButton()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.StartMission();
    }
}