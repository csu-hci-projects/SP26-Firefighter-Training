using UnityEngine;

public class FireExtinguisher : MonoBehaviour
{
    public ParticleSystem sprayParticles;
    public Transform sprayPoint;

    public float sprayDistance = 5f;
    public float extinguishAmount = 35f;

    private bool isSpraying = false;

    void Update()
    {
        if (!isSpraying)
            return;

        Debug.DrawRay(sprayPoint.position, sprayPoint.forward * sprayDistance, Color.blue);

        if (Physics.Raycast(sprayPoint.position, sprayPoint.forward, out RaycastHit hit, sprayDistance))
        {
            Debug.Log("Hit: " + hit.collider.name);
            
            Fire fire = hit.collider.GetComponent<Fire>();

            if (fire != null)
            {
                fire.Extinguish(extinguishAmount * Time.deltaTime);
            }
        }
    }

    public void StartSpray()
    {
        isSpraying = true;

        if (sprayParticles != null && !sprayParticles.isPlaying)
            sprayParticles.Play();
    }

    public void StopSpray()
    {
        isSpraying = false;

        if (sprayParticles != null && sprayParticles.isPlaying)
            sprayParticles.Stop();
    }
}