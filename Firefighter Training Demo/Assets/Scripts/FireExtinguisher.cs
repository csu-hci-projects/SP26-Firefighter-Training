using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class FireExtinguisher : MonoBehaviour
{
    public ParticleSystem sprayParticles;
    public Transform sprayPoint;

    public float sprayDistance = 5f;
    public float extinguishAmount = 25f;

    private bool isSpraying = false;
    private XRBaseInteractable interactable;

    void Start()
    {
        isSpraying = false;

        if (sprayParticles != null)
            sprayParticles.Stop();

        // Get the XR Grab Interactable on this object
        interactable = GetComponent<XRBaseInteractable>();

        if (interactable == null)
        {
            VRDebugDisplay.Log("ERROR: No XRBaseInteractable found!");
            return;
        }

        // Hook into grab/select events instead of controller input
        interactable.activated.AddListener(OnActivated);
        interactable.deactivated.AddListener(OnDeactivated);

        VRDebugDisplay.Log("Extinguisher ready!");
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.activated.RemoveListener(OnActivated);
            interactable.deactivated.RemoveListener(OnDeactivated);
        }
    }

    private void OnActivated(ActivateEventArgs args)
    {
        StartSpray();
    }

    private void OnDeactivated(DeactivateEventArgs args)
    {
        StopSpray();
    }

    void Update()
    {
        if (!isSpraying)
            return;

        if (Physics.Raycast(sprayPoint.position, sprayPoint.forward, out RaycastHit hit, sprayDistance))
        {
            Fire fire = hit.collider.GetComponentInParent<Fire>();

            if (fire != null)
            {
                VRDebugDisplay.Log("Hitting fire! Health: " + fire.fireHealth.ToString("F1"));
                fire.Extinguish(extinguishAmount * Time.deltaTime);
            }
            else
            {
                VRDebugDisplay.Log("Hit: " + hit.collider.name + " (no Fire script)");
            }
        }
        else
        {
            VRDebugDisplay.Log("Spraying - nothing hit");
        }
    }

    public void StartSpray()
    {
        isSpraying = true;
        VRDebugDisplay.Log("Spraying started");

        if (sprayParticles != null && !sprayParticles.isPlaying)
            sprayParticles.Play();
    }

    public void StopSpray()
    {
        isSpraying = false;
        VRDebugDisplay.Log("Spraying stopped");

        if (sprayParticles != null && sprayParticles.isPlaying)
            sprayParticles.Stop();
    }
}