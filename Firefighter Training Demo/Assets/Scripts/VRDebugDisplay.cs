using UnityEngine;
using UnityEngine.UI;

public class VRDebugDisplay : MonoBehaviour
{
    public static VRDebugDisplay Instance;
    public Text debugText; // assign a world-space canvas Text in Inspector

    void Awake()
    {
        Instance = this;
    }

    public static void Log(string message)
    {
        if (Instance != null && Instance.debugText != null)
            Instance.debugText.text = message;
    }
}