using UnityEngine;

public class Fire : MonoBehaviour
{
    public float fireHealth = 100f;

    public void Extinguish(float amount)
    {
         Debug.Log("Fire hit! Health: " + fireHealth);

        fireHealth -= amount;

        if (fireHealth <= 0)
        {
            Debug.Log("Fire destroyed!");

            Destroy(gameObject);
        }
    }
}