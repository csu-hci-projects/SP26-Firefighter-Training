using UnityEngine;
using System.Collections.Generic;

public class ToolSpawner : MonoBehaviour
{
    public GameObject waterPot;

    public GameObject axe;

    public GameObject fireExtinguisher;

    public Transform sinkSpawnPoint;

    public List<Transform> axeSpawnPoints = new List<Transform>();

    public List<Transform> extinguisherSpawnPoints = new List<Transform>();
    public void StartSpawning()
    {
        MoveWaterPotToSink();
        MoveAxeToRandomPoint();
        MoveExtinguisherToRandomPoint();
    }

    private void MoveWaterPotToSink()
    {
        if (waterPot == null || sinkSpawnPoint == null)
        {
            Debug.LogWarning("ToolSpawner: Water pot or sink spawn point not assigned!");
            return;
        }

        waterPot.transform.position = sinkSpawnPoint.position;
        waterPot.transform.rotation = sinkSpawnPoint.rotation;

        // Re-enable in case it was disabled
        waterPot.SetActive(true);

    }

    private void MoveAxeToRandomPoint()
    {
        if (axe == null || axeSpawnPoints.Count == 0)
        {
            Debug.LogWarning("ToolSpawner: Axe or axe spawn points not assigned!");
            return;
        }

        Transform spawnPoint = axeSpawnPoints[Random.Range(0, axeSpawnPoints.Count)];
        axe.transform.position = spawnPoint.position;
        axe.transform.rotation = spawnPoint.rotation;
        axe.SetActive(true);

    }

    private void MoveExtinguisherToRandomPoint()
    {
        if (fireExtinguisher == null || extinguisherSpawnPoints.Count == 0)
        {
            Debug.LogWarning("ToolSpawner: Extinguisher or extinguisher spawn points not assigned!");
            return;
        }

        // Make sure extinguisher doesn't spawn at same point as axe
        List<Transform> availablePoints = new List<Transform>(extinguisherSpawnPoints);

        Transform spawnPoint = availablePoints[Random.Range(0, availablePoints.Count)];
        fireExtinguisher.transform.position = spawnPoint.position;
        fireExtinguisher.transform.rotation = spawnPoint.rotation;
        fireExtinguisher.SetActive(true);

    }
}