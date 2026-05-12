using UnityEngine;


public class Extinguisher : MonoBehaviour
{
    public float co2Amount = 1000f;
    public float co2DrainRate = 1f;
    public float extinguishPower = 25f;
       public float sprayRange = 3f;
    public Transform nozzleTransform;
    public ParticleSystem sprayParticles;
    public AudioClip spraySound;
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