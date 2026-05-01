using UnityEngine;

public class MissionStartTrigger : MonoBehaviour
{
    public bool disableAfterStart = true;

    private void OnTriggerEnter(Collider other)
    {
        if (MissionManager.Instance == null)
            return;

        if (MissionManager.Instance.missionActive)
            return;

        if (other.CompareTag("Player") || other.CompareTag("MainCamera"))
        {
            MissionManager.Instance.StartMission();

            if (disableAfterStart)
                gameObject.SetActive(false);
        }
    }
}