using UnityEngine;

/// <summary>
/// Temporary debug script - attach to AxeHead to test if trigger is working.
/// Remove once door breaking is working.
/// </summary>
public class TriggerDebug : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"TRIGGER ENTER: {gameObject.name} hit {other.gameObject.name} on layer {LayerMask.LayerToName(other.gameObject.layer)}");
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log($"TRIGGER STAY: {gameObject.name} touching {other.gameObject.name}");
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log($"COLLISION ENTER: {gameObject.name} hit {other.gameObject.name}");
    }
}