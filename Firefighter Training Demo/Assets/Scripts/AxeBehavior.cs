using UnityEngine;

public class AxeBehavior : MonoBehaviour
{
    public float damage = 25f;
    public float hitCooldown = 0.5f;

    public Transform axeHead;
    public AudioClip hitSound;
    public AudioClip breakSound;

    private float lastHitTime = -999f;
    private AudioSource audioSource;
    private Vector3 prevPosition;
    private float velocity;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (axeHead == null)
            axeHead = transform;

        prevPosition = axeHead.position;
    }

    void FixedUpdate()
    {
        // Track velocity
        velocity = (axeHead.position - prevPosition).magnitude / Time.fixedDeltaTime;
        prevPosition = axeHead.position;

        // Check for door hits if moving fast enough and cooldown passed
        if (velocity > 0.5f && Time.time - lastHitTime > hitCooldown)
        {
            CheckHit();
        }
    }

    private void CheckHit()
    {
        // Cast rays in multiple directions from axe head to ensure we catch the door
        Vector3[] directions = new Vector3[]
        {
            axeHead.forward,
            axeHead.up,
            axeHead.right,
            -axeHead.forward,
            -axeHead.up,
            -axeHead.right
        };

        foreach (Vector3 dir in directions)
        {
            RaycastHit hit;
            if (Physics.Raycast(axeHead.position, dir, out hit, 0.3f))
            {
                DoorBehavior door = hit.collider.GetComponent<DoorBehavior>()
                                 ?? hit.collider.GetComponentInParent<DoorBehavior>()
                                 ?? hit.collider.GetComponentInChildren<DoorBehavior>();

                if (door != null && !door.IsBroken())
                {
                    lastHitTime = Time.time;
                    bool broke = door.TakeHit(damage);

                    if (audioSource != null)
                    {
                        if (broke && breakSound != null)
                            audioSource.PlayOneShot(breakSound);
                        else if (hitSound != null)
                            audioSource.PlayOneShot(hitSound);
                    }
                    return;
                }
            }
        }

        // Also do an overlap sphere as fallback
        Collider[] colliders = Physics.OverlapSphere(axeHead.position, 0.15f);
        foreach (Collider col in colliders)
        {
            if (col.transform.IsChildOf(transform) || col.transform == transform) continue;

            DoorBehavior door = col.GetComponent<DoorBehavior>()
                             ?? col.GetComponentInParent<DoorBehavior>()
                             ?? col.GetComponentInChildren<DoorBehavior>();

            if (door != null && !door.IsBroken())
            {
                lastHitTime = Time.time;
                bool broke = door.TakeHit(damage);

                if (audioSource != null)
                {
                    if (broke && breakSound != null)
                        audioSource.PlayOneShot(breakSound);
                    else if (hitSound != null)
                        audioSource.PlayOneShot(hitSound);
                }
                return;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (axeHead == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(axeHead.position, 0.15f);
        Gizmos.DrawRay(axeHead.position, axeHead.forward * 0.3f);
    }
}