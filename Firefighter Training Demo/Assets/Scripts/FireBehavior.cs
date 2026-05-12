using UnityEngine;

public class FireBehavior : MonoBehaviour
{
    public float maxIntensity = 100f;
    public float rekinleRate = 2f;
    public float extinguishThreshold = 5f;
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

    public void ApplyExtinguisher(float amount)
    {
        if (isExtinguished) return;

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

    public void EnhanceFire(float amount)
    {
        if (isExtinguished) return;

        currentIntensity += amount;
        currentIntensity = Mathf.Clamp(currentIntensity, 0f, maxIntensity);

        UpdateFireVisuals();
    }
}