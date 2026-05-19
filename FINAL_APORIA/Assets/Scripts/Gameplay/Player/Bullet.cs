using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        // Destroy the enemy
        print("Hit " + collision.gameObject.name + "!");

        Health ObjectHealth = collision.gameObject.GetComponentInParent<Health>();

        // Check if the bullet collides with an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (ObjectHealth != null && !this.gameObject.CompareTag("EnemyBullet"))
            {
                ObjectHealth.TakeDamage(damage);
                Debug.Log("Enemy hit! Remaining health: " + ObjectHealth.currentHealth);
            }
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (ObjectHealth != null && !this.gameObject.CompareTag("PlayerBullet"))
            {
                ObjectHealth.TakeDamage(damage);
                Debug.Log("Player hit! Remaining health: " + ObjectHealth.currentHealth);
            }
        }

        // Destroy the bullet after collision
        Destroy(gameObject);
    }
}