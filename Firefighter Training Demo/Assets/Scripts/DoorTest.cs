using UnityEngine;

/// <summary>
/// Temporary test script - attach to Interior_Door.
/// Remove after confirming door breaking works.
/// </summary>
public class DoorTest : MonoBehaviour
{
    void Start()
    {
        DoorBehavior db = GetComponent<DoorBehavior>();
        if (db != null)
            Debug.Log($"DoorTest: DoorBehavior FOUND on {gameObject.name}");
        else
            Debug.LogError($"DoorTest: DoorBehavior NOT FOUND on {gameObject.name}");

        Collider col = GetComponent<Collider>();
        if (col != null)
            Debug.Log($"DoorTest: Collider FOUND - Is Trigger: {col.isTrigger}");
        else
            Debug.LogError($"DoorTest: NO COLLIDER found on {gameObject.name}!");
    }
}