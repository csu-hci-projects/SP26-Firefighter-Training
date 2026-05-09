using UnityEngine;

/// <summary>
/// Attach this to your fire extinguisher GameObject.
/// 
/// SETUP INSTRUCTIONS:
/// 1. Add this script to your extinguisher GameObject.
/// 2. Create a child GameObject called "NozzlePoint" at the tip of the nozzle - assign it to nozzleTransform.
/// 3. Create a CO2 spray Particle System as a child of NozzlePoint - assign it to sprayParticles.
/// 4. Add a Meta XR Grab Interactable component to the extinguisher.
/// 5. In the Grab Interactable's events, hook WhenSelectingStarted to Spray() and WhenSelectingStopped to StopSpray().
/// </summary>
public class Extinguisher : MonoBehaviour
{
    [Header("Extinguisher Settings")]
    [Tooltip("Total CO2 remaining (0-100). Depletes as you spray.")]
    public float co2Amount = 1000f;

    [Tooltip("How fast CO2 depletes per second while spraying.")]
    public float co2DrainRate = 1f;

    [Tooltip("How much intensity the extinguisher removes from fire per second.")]
    public float extinguishPower = 25f;

    [Tooltip("How far the CO2 spray reaches (in meters).")]
    public float sprayRange = 3f;

    [Header("References")]
    [Tooltip("The tip of the nozzle - spray shoots from here.")]
    public Transform nozzleTransform;

    [Tooltip("Particle System for the CO2 spray effect.")]
    public ParticleSystem sprayParticles;

    [Header("Audio")]
    public AudioClip spraySound;

    [Header("Layer Settings")]
    [Tooltip("Set this to the layer your fire GameObjects are on.")]
    public LayerMask fireLayer;

    // Internal state
    private bool isSpraying = false;
    private AudioSource audioSource;
    private FireBehavior currentFireTarget = null;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (sprayParticles != null)
            sprayParticles.Stop();

        if (nozzleTransform == null)
        {
            Debug.LogWarning("Extinguisher: No nozzleTransform assigned! Using this object's transform.");
            nozzleTransform = transform;
        }
    }

    void Update()
    {
        if (isSpraying && co2Amount > 0)
        {
            co2Amount -= co2DrainRate * Time.deltaTime;
            co2Amount = Mathf.Clamp(co2Amount, 0f, 100f);

            DetectAndExtinguishFire();

            if (co2Amount <= 0)
            {
                StopSpray();
                Debug.Log("Extinguisher empty!");
            }
        }
    }

    /// <summary>
    /// Call this to start spraying. Hook to VR trigger press event.
    /// </summary>
    public void Spray()
    {
        if (co2Amount <= 0 || isSpraying) return;

        isSpraying = true;

        if (sprayParticles != null)
            sprayParticles.Play();

        if (audioSource != null && spraySound != null)
        {
            audioSource.clip = spraySound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Call this to stop spraying. Hook to VR trigger release event.
    /// </summary>
    public void StopSpray()
    {
        if (!isSpraying) return;

        isSpraying = false;
        currentFireTarget = null;

        if (sprayParticles != null)
            sprayParticles.Stop();

        if (audioSource != null)
            audioSource.Stop();
    }

    private void DetectAndExtinguishFire()
    {
        Ray ray = new Ray(nozzleTransform.position, nozzleTransform.forward);
        RaycastHit hit;

        // First try a precise raycast
        if (Physics.Raycast(ray, out hit, sprayRange, fireLayer))
        {
            // Check on the hit object AND its parent in case FireBehavior is on the parent
            FireBehavior fire = hit.collider.GetComponent<FireBehavior>()
                             ?? hit.collider.GetComponentInParent<FireBehavior>();
            if (fire != null && !fire.IsExtinguished())
            {
                currentFireTarget = fire;
                fire.ApplyExtinguisher(extinguishPower);
                return;
            }
        }

        // Fallback: sphere cast for more forgiving VR aim
        if (Physics.SphereCast(ray, 0.2f, out hit, sprayRange, fireLayer))
        {
            FireBehavior fire = hit.collider.GetComponent<FireBehavior>()
                             ?? hit.collider.GetComponentInParent<FireBehavior>();
            if (fire != null && !fire.IsExtinguished())
            {
                currentFireTarget = fire;
                fire.ApplyExtinguisher(extinguishPower);
                return;
            }
        }

        currentFireTarget = null;
    }

    void OnDrawGizmosSelected()
    {
        if (nozzleTransform == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(nozzleTransform.position, nozzleTransform.forward * sprayRange);
        Gizmos.DrawWireSphere(nozzleTransform.position + nozzleTransform.forward * sprayRange, 0.2f);
    }

    public float GetCO2Normalized() => co2Amount / 100f;
    public bool IsSpraying() => isSpraying;
}