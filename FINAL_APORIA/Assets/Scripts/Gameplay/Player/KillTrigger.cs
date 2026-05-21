using UnityEngine;

public class KillTrigger : MonoBehaviour
{
    [Header("Damage Amount")]
    [SerializeField] private int lethalDamage = 9999;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered
        if (other.CompareTag("Player"))
        {
            // Try to get the Health component
            Health health = other.GetComponent<Health>();

            if (health != null)
            {
                // Deal lethal damage
                health.TakeDamage(lethalDamage);
            }
        }
    }
}