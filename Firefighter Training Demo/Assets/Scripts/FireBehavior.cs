using UnityEngine;

/// <summary>
/// Attach this to the ROOT fire GameObject (e.g. Fire001).
/// It will automatically find the ParticleSystem in children.
/// Make sure the Collider (Is Trigger) is on a child and set to the Fire layer.
/// </summary>
public class FireBehavior : MonoBehaviour
{
    [Header("Fire Settings")]
    [Tooltip("How much 'fuel' the fire has. Extinguisher drains this to 0.")]
    public float maxIntensity = 100f;

    [Tooltip("How fast the fire naturally grows back if not fully extinguished (set to 0 to disable).")]
    public float rekinleRate = 2f;

    [Tooltip("Below this intensity, fire is considered fully out.")]
    public float extinguishThreshold = 5f;

    [Header("Audio")]
    public AudioClip fireBurnSound;
    public AudioClip fireExtinguishSound;

    // Internal state
    private float currentIntensity;
    private ParticleSystem fireParticles;
    private AudioSource audioSource;
    private bool isExtinguished = false;
    private float defaultEmissionRate;

    void Start()
    {
        currentIntensity = maxIntensity;

        // Search on this object first, then in children
        fireParticles = GetComponent<ParticleSystem>();
        if (fireParticles == null)
            fireParticles = GetComponentInChildren<ParticleSystem>();

        if (fireParticles == null)
        {
            Debug.LogError($"FireBehavior on {gameObject.name}: No ParticleSystem found on this object or its children!");
        }
        else
        {
            var emission = fireParticles.emission;
            defaultEmissionRate = emission.rateOverTime.constant;
            Debug.Log($"FireBehavior on {gameObject.name}: Found ParticleSystem on {fireParticles.gameObject.name}");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (fireBurnSound != null)
        {
            audioSource.clip = fireBurnSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (isExtinguished) return;

        // Slowly rekindle if not fully extinguished
        if (currentIntensity > 0 && currentIntensity < maxIntensity)
        {
            currentIntensity += rekinleRate * Time.deltaTime;
            currentIntensity = Mathf.Clamp(currentIntensity, 0f, maxIntensity);
        }

        UpdateFireVisuals();

        if (currentIntensity <= extinguishThreshold)
            Extinguish();
    }

    /// <summary>
    /// Called by the Extinguisher script when CO2 hits this fire.
    /// </summary>
    public void ApplyExtinguisher(float amount)
    {
        if (isExtinguished) return;

        Debug.Log($"FireBehavior: ApplyExtinguisher called on {gameObject.name}, intensity: {currentIntensity}");

        currentIntensity -= amount * Time.deltaTime;
        currentIntensity = Mathf.Clamp(currentIntensity, 0f, maxIntensity);

        UpdateFireVisuals();

        if (currentIntensity <= extinguishThreshold)
            Extinguish();
    }

    private void UpdateFireVisuals()
    {
        if (fireParticles == null) return;

        float t = currentIntensity / maxIntensity;

        var emission = fireParticles.emission;
        emission.rateOverTime = defaultEmissionRate * t;

        var main = fireParticles.main;
        main.startSizeMultiplier = t;

        if (audioSource != null)
            audioSource.volume = t;
    }

    private void Extinguish()
    {
        if (isExtinguished) return;
        isExtinguished = true;

        Debug.Log($"{gameObject.name} has been extinguished!");

        if (fireParticles != null)
            fireParticles.Stop();

        if (audioSource != null)
        {
            audioSource.Stop();
            if (fireExtinguishSound != null)
                audioSource.PlayOneShot(fireExtinguishSound);
        }

        // Disable all colliders on this object and children
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;
    }

    public float GetIntensityNormalized() => currentIntensity / maxIntensity;
    public bool IsExtinguished() => isExtinguished;
}