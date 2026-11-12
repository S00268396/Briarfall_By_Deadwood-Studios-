using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"{gameObject.name} took {damageAmount} damage. Current Health: {currentHealth}");

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");
        Destroy(gameObject);
    }
}
