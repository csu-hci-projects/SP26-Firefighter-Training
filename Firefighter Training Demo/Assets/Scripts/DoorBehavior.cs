using UnityEngine;


public class DoorBehavior : MonoBehaviour
{
    public float doorHealth = 100f;
    public float breakForce = 300f;
    public float breakTorque = 150f;
    public GameObject breakEffect;
    public GameObject hitEffect;
    public AudioClip breakSound;

    private float maxHealth;
    private bool isBroken = false;
    private AudioSource audioSource;

    void Start()
    {
        maxHealth = doorHealth;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public bool TakeHit(float damage)
    {
        if (isBroken) return false;

        doorHealth -= damage;
        doorHealth = Mathf.Clamp(doorHealth, 0f, maxHealth);

        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);

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

        // Disable hinge joint so door detaches from frame
        HingeJoint hinge = GetComponent<HingeJoint>();
        if (hinge != null) Destroy(hinge);

        // Get or add rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;

        // Knock door down
        Vector3 fallDirection = -transform.forward + Vector3.down * 0.5f;
        rb.AddForce(fallDirection.normalized * breakForce, ForceMode.Impulse);
        rb.AddTorque(transform.right * breakTorque, ForceMode.Impulse);

        if (breakEffect != null)
            Instantiate(breakEffect, transform.position + Vector3.up, Quaternion.identity);

        if (audioSource != null && breakSound != null)
            audioSource.PlayOneShot(breakSound);

        Destroy(gameObject, 5f);
    }

    private System.Collections.IEnumerator FlashDamage()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Color[] originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_BaseColor"))
            {
                originalColors[i] = renderers[i].material.GetColor("_BaseColor");
                renderers[i].material.SetColor("_BaseColor", Color.red);
            }
        }

        yield return new WaitForSeconds(0.15f);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].material.HasProperty("_BaseColor"))
                renderers[i].material.SetColor("_BaseColor", originalColors[i]);
        }
    }

    public bool IsBroken() => isBroken;
    public float GetHealthNormalized() => doorHealth / maxHealth;
}