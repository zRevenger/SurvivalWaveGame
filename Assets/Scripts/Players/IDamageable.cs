using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class IDamageable : NetworkBehaviour
{
    [SyncVar]
    public int maxHealth;
    [SyncVar]
    public int currentHealth;

    public abstract void OnDamage(int amount);

    public abstract void CmdDamage(int amount);

    public abstract void Die();

    public abstract void Heal(int amount);
}
