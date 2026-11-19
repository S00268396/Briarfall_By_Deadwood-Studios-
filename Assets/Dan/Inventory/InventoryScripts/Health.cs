using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float Width, Height;

    [SerializeField]
    private RectTransform healthBar;

    public float maxHealth = 100f;
    public float health;

    private void Start()
    {
        health = maxHealth;
    }

    public void SetHealth(float health)
    {
        float newWidth = (health / maxHealth) * Width;

        healthBar.sizeDelta = new Vector2(newWidth, Height);
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        health = Mathf.Max(health, 0);

        Debug.Log($"{gameObject.name} took {damageAmount} damage. Current Health: {health}");

        if(health <= 0)
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
