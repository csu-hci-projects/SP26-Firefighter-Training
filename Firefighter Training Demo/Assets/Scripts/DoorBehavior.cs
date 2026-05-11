using UnityEngine;

/// <summary>
/// Attach to your door GameObject.
/// The door takes damage from axe hits and ragdolls when health reaches 0.
/// 
/// SETUP INSTRUCTIONS:
/// 1. Attach this script to your door GameObject.
/// 2. Make sure the door has a Collider on it (or a child).
/// 3. The door will automatically get a Rigidbody added when it breaks.
/// 4. Optionally assign a broken door particle effect (dust/splinters).
/// </summary>
public class DoorBehavior : MonoBehaviour
{
    [Header("Door Health")]
    [Tooltip("How many health points the door has. Axe damage drains this.")]
    public float doorHealth = 100f;

    [Tooltip("Show a debug log of remaining health on each hit.")]
    public bool showHealthDebug = true;

    [Header("Ragdoll Settings")]
    [Tooltip("How much force to apply when the door falls.")]
    public float breakForce = 300f;

    [Tooltip("How much upward force to add so it tumbles realistically.")]
    public float breakTorque = 150f;

    [Header("Effects")]
    [Tooltip("Optional particle effect (dust, splinters) spawned when door breaks.")]
    public GameObject breakEffect;

    [Tooltip("Optional particle effect (dust, wood chips) spawned on each hit.")]
    public GameObject hitEffect;

    [Header("Audio")]
    public AudioClip breakSound;

    // Internal
    private float maxHealth;
    private bool isBroken = false;
    private Rigidbody rb;
    private AudioSource audioSource;

    void Start()
    {
        maxHealth = doorHealth;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// Called by the Axe script when it hits this door.
    /// Returns true if the door broke on this hit.
    /// </summary>
    public bool TakeHit(float damage)
    {
        if (isBroken) return false;

        doorHealth -= damage;
        doorHealth = Mathf.Clamp(doorHealth, 0f, maxHealth);

        if (showHealthDebug)
            Debug.Log($"{gameObject.name} health: {doorHealth}/{maxHealth}");

        // Spawn hit effect
        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);

        // Flash the door red to indicate damage
        StartCoroutine(FlashDamage());

        if (doorHealth <= 0f)
        {
            Break();
            return true;
        }

        return false;
    }

    private void Break()
    {
        if (isBroken) return;
        isBroken = true;

        Debug.Log($"{gameObject.name} has been broken down!");

        // Add Rigidbody to make it ragdoll/fall
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;

        // Apply a force to knock it down dramatically
        Vector3 fallDirection = -transform.forward + Vector3.down * 0.5f;
        rb.AddForce(fallDirection.normalized * breakForce, ForceMode.Impulse);
        rb.AddTorque(transform.right * breakTorque, ForceMode.Impulse);

        // Spawn break effect
        if (breakEffect != null)
            Instantiate(breakEffect, transform.position + Vector3.up, Quaternion.identity);

        // Play break sound
        if (audioSource != null && breakSound != null)
            audioSource.PlayOneShot(breakSound);

        // Disable after 5 seconds to clean up
        Destroy(gameObject, 5f);
    }

    private System.Collections.IEnumerator FlashDamage()
    {
        // Flash all renderers red briefly
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Color[] originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                originalColors[i] = renderers[i].material.color;
                renderers[i].material.color = Color.red;
            }
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].material.HasProperty("_Color"))
                renderers[i].material.color = originalColors[i];
        }
    }

    /// <summary>
    /// Returns true if the door has already been broken.
    /// </summary>
    public bool IsBroken() => isBroken;

    /// <summary>
    /// Returns 0-1 representing remaining door health.
    /// </summary>
    public float GetHealthNormalized() => doorHealth / maxHealth;

    void OnDrawGizmosSelected()
    {
        // Draw a health bar-like indicator in scene view
        Gizmos.color = Color.Lerp(Color.red, Color.green, doorHealth / maxHealth);
        Gizmos.DrawWireCube(transform.position + Vector3.up * 2f, new Vector3(1f, 0.1f, 0.1f));
    }
}
