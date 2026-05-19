using System;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] public int currentHealth;

    [Header("UI")]
    [SerializeField] private Slider healthSlider;


    [Header("Death Screen")]
    [SerializeField] private GameObject objectToHide;
    [SerializeField] private GameObject objectToShow;

    private bool isDead = false;

    public event Action OnDeath;
    public event Action OnDamage;

    private void Start()
    {
        currentHealth = maxHealth;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        healthSlider.value = currentHealth;

        OnDamage?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " is dead!");

        OnDeath?.Invoke();
    }

    public void ResetHealth()
    {
        isDead = false;

        currentHealth = maxHealth;
        healthSlider.value = currentHealth;

        Time.timeScale = 1f;
    }

    public float GetHealthRatio()
    {
        return (float)currentHealth / (float)maxHealth;
    }
}