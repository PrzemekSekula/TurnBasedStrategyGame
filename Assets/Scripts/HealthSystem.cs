using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler onDead;
    public event EventHandler onDamaged;

    [SerializeField] private int health = 100;
    private int healthMax;

    private void Awake()
    {
        healthMax = health;
    }

    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
        }
        onDamaged?.Invoke(this, EventArgs.Empty);
        
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die() 
    {
        onDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return (float) health / healthMax;
    }
}
