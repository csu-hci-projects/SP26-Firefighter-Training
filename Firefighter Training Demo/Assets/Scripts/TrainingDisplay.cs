using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TrainingDisplay : MonoBehaviour
{
    [Header("UI References")]
    public Button startButton;
    public ToolSpawner toolSpawner;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI factText;
    public TextMeshProUGUI completionText;
    public Image backgroundPanel;

    public GameObject toolsPanel;

    public TextMeshProUGUI instructionText;

    public List<FireBehavior> fires = new List<FireBehavior>();

    public float distanceFromPlayer = 2f;
    public float heightOffset = 0f;

    public Transform playerTransform;

    public Color backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);
    public Color timerColor = new Color(1f, 0.8f, 0.2f, 1f);
    public Color factColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public Color completionColor = new Color(0.2f, 1f, 0.4f, 1f);

    private float elapsedTime = 0f;
    private bool timerRunning = false;
    private bool completed = false;
    private bool trainingStarted = false;

    private string[] fireSafetyFacts = new string[]
    {
        "A fire can double in size every minute. Act fast!",
        "Smoke alarms reduce the risk of dying in a fire by 50%.",
        "Most fire deaths occur at night when people are asleep.",
        "You should practice your fire escape plan twice a year.",
        "Never use an elevator during a fire — always use the stairs.",
        "A closed door can slow the spread of fire by up to 3 minutes.",
        "Kitchen fires are the #1 cause of home fires and injuries.",
        "CO2 extinguishers work by removing oxygen from the fire.",
        "PASS: Pull, Aim, Squeeze, Sweep when using a fire extinguisher.",
        "In a fire, stay low to avoid smoke — cleaner air is near the floor.",
        "Grease fires should never be extinguished with water.",
        "Check smoke alarm batteries every 6 months.",
        "Have a designated meeting point outside your building.",
        "Fire spreads through heat, flame, and smoke — all are dangerous.",
        "Firefighters can face temperatures exceeding 1000 degrees Fahrenheit."
    };

    void Start()
    {
        // Apply background color
        if (backgroundPanel != null)
            backgroundPanel.color = backgroundColor;

        // Set text colors
        if (timerText != null) timerText.color = timerColor;
        if (factText != null) factText.color = factColor;
        if (completionText != null) completionText.color = completionColor;

        SetInitialState();

        // Hook up button
        //if (startButton != null)
            //startButton.onClick.AddListener(OnStartPressed);

        // Position canvas in front of player
        PositionInFrontOfPlayer();
    }

    void Update()
    {
        if (timerRunning && !completed)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
            CheckAllFiresExtinguished();
        }
    }

    private void SetInitialState()
    {
        // Show only the start button
        if (startButton != null)   startButton.gameObject.SetActive(true);
        if (timerText != null)     timerText.gameObject.SetActive(false);
        if (factText != null)      factText.gameObject.SetActive(false);
        if (completionText != null) completionText.gameObject.SetActive(false);
    }

    public void OnStartPressed()
    {
        if (instructionText != null) instructionText.gameObject.SetActive(false);

        if (toolsPanel != null) toolsPanel.SetActive(false);

        if (trainingStarted) return;
        trainingStarted = true;
        timerRunning = true;

        // Hide button, show timer only
        if (startButton != null)   startButton.gameObject.SetActive(false);
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.text = "Time To Complete: 00:00";
        }

        // Keep fact and completion hidden until done
        if (factText != null)      factText.gameObject.SetActive(false);
        if (completionText != null) completionText.gameObject.SetActive(false);


        if (toolSpawner != null) toolSpawner.StartSpawning();
    }

    private void PositionInFrontOfPlayer()
    {
        if (playerTransform == null) return;

        Vector3 forward = playerTransform.forward;
        forward.y = 0;
        forward.Normalize();

        transform.position = playerTransform.position
            + forward * distanceFromPlayer
            + Vector3.up * heightOffset;

        transform.rotation = Quaternion.LookRotation(forward);
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"Time To Complete: {minutes:00}:{seconds:00}";
    }

    private void CheckAllFiresExtinguished()
    {
        if (fires.Count == 0) return;
        foreach (FireBehavior fire in fires)
        {
            if (fire != null && !fire.IsExtinguished())
                return;
        }
        CompleteTraining();
    }

    private void CompleteTraining()
    {
        completed = true;
        timerRunning = false;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        // Show final timer
        if (timerText != null)
            timerText.gameObject.SetActive(false);

        // Show completion message
        if (completionText != null)
        {
            completionText.gameObject.SetActive(true);
            completionText.text = $"Training Complete!\n\nAll fires extinguished in {minutes:00}:{seconds:00}\n\nGreat work, firefighter!";
        }

        // Show fire safety fact
        if (factText != null)
        {
            factText.gameObject.SetActive(true);
            factText.text = fireSafetyFacts[Random.Range(0, fireSafetyFacts.Length)];
        }

    }
}