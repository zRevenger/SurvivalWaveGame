using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class BaseEnemy : IDamageable
{
    public TextMeshPro healthText;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    [ClientRpc]
    public override void OnDamage(int amount)
    {
        currentHealth -= amount;
        healthText.text = currentHealth.ToString();
        if (currentHealth <= 0)
            Die();
    }

    [Command(requiresAuthority = false)]
    public override void CmdDamage(int amount)
    {
        OnDamage(amount);
    }

    [Command(requiresAuthority = false)]
    public override void Die()
    {
        Destroy(gameObject);
    }

    [Command(requiresAuthority = false)]
    public override void Heal(int amount)
    {

    }
}
