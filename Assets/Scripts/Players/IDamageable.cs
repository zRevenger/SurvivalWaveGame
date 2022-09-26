using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class IDamageable : NetworkBehaviour
{
    public int maxHealth;
    public int currentHealth;

    public abstract void OnDamage(int amount);
    public abstract void Die();
    public abstract void Heal(int amount);
}
