using UnityEngine;

public class Fire : MonoBehaviour
{
    public float fireHealth = 100f;

    public void Extinguish(float amount)
    {
        fireHealth -= amount;
        VRDebugDisplay.Log("Fire health: " + fireHealth.ToString("F1"));

        if (fireHealth <= 0)
        {
            VRDebugDisplay.Log("Fire extinguished!");
            Destroy(gameObject);
        }
    }
}