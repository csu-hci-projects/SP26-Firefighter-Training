using UnityEngine;

public class FireObjective : MonoBehaviour
{
    [Header("Fire Info")]
    public string fireName = "Kitchen Fire";

    [Header("Fire State")]
    public bool IsExtinguished;

    [Header("Optional Visuals")]
    public GameObject fireVisual;
    public ParticleSystem fireParticles;

    public void ResetFireObjective()
    {
        IsExtinguished = false;

        if (fireVisual != null)
            fireVisual.SetActive(true);

        if (fireParticles != null)
            fireParticles.Play();
    }

    public void ExtinguishFire()
    {
        if (IsExtinguished)
            return;

        IsExtinguished = true;

        if (fireParticles != null)
            fireParticles.Stop();

        if (fireVisual != null)
            fireVisual.SetActive(false);

        if (MissionManager.Instance != null)
            MissionManager.Instance.RegisterFireExtinguished(this);
    }
}