using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [Header("Mission State")]
    public bool missionActive;
    public float missionTime;

    [Header("Objectives")]
    public List<FireObjective> fireObjectives = new List<FireObjective>();
    public List<HiddenGoal> hiddenGoals = new List<HiddenGoal>();

    [Header("UI")]
    public MissionUI missionUI;

    private bool missionEnded;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        missionActive = false;
        missionEnded = false;
        missionTime = 0f;

        if (missionUI != null)
        {
            missionUI.ShowStartScreen();
            missionUI.UpdateTimer(0f);
        }
    }

    private void Update()
    {
        if (!missionActive || missionEnded)
            return;

        missionTime += Time.deltaTime;

        if (missionUI != null)
            missionUI.UpdateTimer(missionTime);

        CheckMissionComplete();
    }

    public void StartMission()
    {
        if (missionActive)
            return;

        missionActive = true;
        missionEnded = false;
        missionTime = 0f;

        foreach (HiddenGoal goal in hiddenGoals)
            goal.ResetGoal();

        foreach (FireObjective fire in fireObjectives)
            fire.ResetFireObjective();

        if (missionUI != null)
            missionUI.ShowMissionScreen();
    }

    public void RegisterFireExtinguished(FireObjective fire)
    {
        CheckMissionComplete();
    }

    public void CompleteHiddenGoal(string goalId)
    {
        foreach (HiddenGoal goal in hiddenGoals)
        {
            if (goal.goalId == goalId)
            {
                goal.MarkCompleted();
                return;
            }
        }
    }

    private void CheckMissionComplete()
    {
        foreach (FireObjective fire in fireObjectives)
        {
            if (!fire.IsExtinguished)
                return;
        }

        EndMission();
    }

    private void EndMission()
    {
        if (missionEnded)
            return;

        missionEnded = true;
        missionActive = false;

        if (missionUI != null)
            missionUI.ShowResults(missionTime, fireObjectives, hiddenGoals);
    }
}



