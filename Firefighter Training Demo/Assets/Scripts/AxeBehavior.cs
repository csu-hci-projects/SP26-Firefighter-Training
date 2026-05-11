using UnityEngine;

/// <summary>
/// Attach to the ROOT axe GameObject.
/// Uses raycast instead of trigger colliders - much more reliable.
/// No special collider setup needed on the axe head.
/// </summary>
public class AxeBehavior : MonoBehaviour
{
    [Header("Axe Settings")]
    public float damage = 25f;

    [Tooltip("Minimum speed the axe must be moving to register a hit.")]
    public float minSwingVelocity = 0.5f;

    [Tooltip("Cooldown between hits.")]
    public float hitCooldown = 0.5f;

    [Tooltip("How far to raycast from axe head.")]
    public float hitRange = 0.3f;

    [Header("References")]
    [Tooltip("The axe head transform - drag AxeHead here.")]
    public Transform axeHead;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip breakSound;

    private float lastHitTime = -999f;
    private AudioSource audioSource;
    private Vector3 lastPosition;
    private float currentVelocity;
    private Vector3 lastAxeHeadPos;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (axeHead == null)
        {
            axeHead = transform;
            Debug.LogWarning("AxeBehavior: No axeHead assigned, using root transform.");
        }

        lastAxeHeadPos = axeHead.position;
    }

    void Update()
    {
        // Calculate velocity of axe head
        currentVelocity = (axeHead.position - lastAxeHeadPos).magnitude / Time.deltaTime;
        lastAxeHeadPos = axeHead.position;

        // Only check for hits if moving fast enough and cooldown passed
        if (currentVelocity >= minSwingVelocity && Time.time - lastHitTime >= hitCooldown)
        {
            CheckForHit();
        }
    }

    private void CheckForHit()
    {
        // Cast a sphere around the axe head to detect nearby doors
        Collider[] hits = Physics.OverlapSphere(axeHead.position, hitRange);

        foreach (Collider hit in hits)
        {
            // Skip self
            if (hit.transform.IsChildOf(transform) || hit.transform == transform)
                continue;

            // Look for DoorBehavior on hit object or any parent
            DoorBehavior door = hit.GetComponent<DoorBehavior>()
                             ?? hit.GetComponentInParent<DoorBehavior>();

            if (door != null && !door.IsBroken())
            {
                lastHitTime = Time.time;
                bool broke = door.TakeHit(damage);

                if (audioSource != null)
                {
                    if (broke && breakSound != null)
                        audioSource.PlayOneShot(breakSound);
                    else if (!broke && hitSound != null)
                        audioSource.PlayOneShot(hitSound);
                }

                Debug.Log($"Axe hit {hit.gameObject.name} at velocity {currentVelocity:F1} m/s!");
                return;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (axeHead == null) return;
        Gizmos.color = currentVelocity >= minSwingVelocity ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(axeHead.position, hitRange);
    }
}