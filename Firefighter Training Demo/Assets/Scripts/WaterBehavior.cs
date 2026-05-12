using UnityEngine;


public class WaterBehavior : MonoBehaviour
{
    public float fireEnhanceRate = 20f;

    public ParticleSystem waterParticles;

    public GameObject smokeEffect;

    public float smokeDuration = 4f;

    public TMPro.TextMeshProUGUI warningText;

    public float warningDuration = 10f;

    public AudioClip waterPourSound;
    public AudioClip waterOnFireSound;

    // Internal
    private bool isPouring = false;
    private AudioSource audioSource;
    private FireBehavior currentFireTarget = null;
    private bool warningShowing = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (waterParticles != null)
            waterParticles.Stop();

        // Make sure warning is hidden at start
        if (warningText != null)
            warningText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPouring)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f);
            foreach (Collider hit in hits)
            {
                FireBehavior fire = hit.GetComponent<FireBehavior>()
                                 ?? hit.GetComponentInParent<FireBehavior>();

                if (fire != null && !fire.IsExtinguished())
                {
                    currentFireTarget = fire;
                    fire.EnhanceFire(fireEnhanceRate * Time.deltaTime);

                    if (!warningShowing)
                    {
                        SpawnSmoke(hit.transform.position);
                        StartCoroutine(ShowWarning());
                    }
                    return;
                }
            }
        }
    }

    public void ActivateWater()
    {
        isPouring = true;

        if (waterParticles != null)
            waterParticles.Play();

        if (audioSource != null && waterPourSound != null)
        {
            audioSource.clip = waterPourSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void DeactivateWater()
    {
        isPouring = false;
        currentFireTarget = null;

        if (waterParticles != null)
            waterParticles.Stop();

        if (audioSource != null)
            audioSource.Stop();
    }

    private void SpawnSmoke(Vector3 position)
    {
        if (smokeEffect != null)
        {
            GameObject smoke = Instantiate(smokeEffect, position + Vector3.up * 0.5f, Quaternion.identity);
            Destroy(smoke, smokeDuration);
        }

        if (audioSource != null && waterOnFireSound != null)
            audioSource.PlayOneShot(waterOnFireSound);
    }

    private System.Collections.IEnumerator ShowWarning()
    {
        if (warningText == null) yield break;

        warningShowing = true;
        warningText.gameObject.SetActive(true);
        warningText.text = "WARNING: Water enhances grease fires!\nUse a CO2 extinguisher instead!";
        warningText.color = Color.red;

        yield return new WaitForSeconds(warningDuration);

        warningText.gameObject.SetActive(false);
        warningShowing = false;
    }
}