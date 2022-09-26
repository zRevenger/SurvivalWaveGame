using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CombatManager : IDamageable
{
    public List<Weapon> weaponData;

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
        currentHealth = maxHealth;
    }

    public override void Die()
    {
        Debug.Log("died");
    }

    public override void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public override void OnDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }

    private void Update()
    {
        if(GetComponent<PlayerObjectController>().IsInGameScene())
        {
            if (!hasAuthority) return;

            if (Input.GetButtonDown("Fire1"))
                Attack();

            if (Input.GetButtonDown("Reload"))
                Reload();
        }
    }

    public void Attack()
    {
        if (inventoryManager.selectedSlot == 0 && inventoryManager.currentPrimaryAmmo > 0)
        {
            inventoryManager.currentPrimaryAmmo--;
            inventoryManager.UpdateCurrentAmmoUI(0);
        }

        if (inventoryManager.selectedSlot == 1 && inventoryManager.currentSecondaryAmmo > 0 && !inventoryManager.secondaryWeapon.isMelee)
        {
            inventoryManager.currentSecondaryAmmo--;
            inventoryManager.UpdateCurrentAmmoUI(1);
        }
        
    }

    public void Reload()
    {
        if(inventoryManager.selectedSlot == 0)
        {
            if (inventoryManager.primaryWeapon.ammoUsageType == EAmmoUsage.Infinite)
                inventoryManager.currentPrimaryAmmo = inventoryManager.primaryWeapon.clipMaxAmmo;
            else
            {
                int requiredAmmo = inventoryManager.primaryWeapon.clipMaxAmmo - inventoryManager.currentPrimaryAmmo;
                switch (inventoryManager.primaryWeapon.ammoType)
                {
                    case EAmmoType.Pistol:
                        if (inventoryManager.totalPistolAmmo >= requiredAmmo)
                        {
                            inventoryManager.totalPistolAmmo -= requiredAmmo;
                            inventoryManager.currentPrimaryAmmo = inventoryManager.primaryWeapon.clipMaxAmmo;
                        }
                        else
                        {
                            inventoryManager.currentPrimaryAmmo += inventoryManager.totalPistolAmmo;
                            inventoryManager.totalPistolAmmo = 0;
                        }
                        break;
                    case EAmmoType.Rifle:
                        if (inventoryManager.totalRifleAmmo >= requiredAmmo)
                        {
                            inventoryManager.totalRifleAmmo -= requiredAmmo;
                            inventoryManager.currentPrimaryAmmo = inventoryManager.primaryWeapon.clipMaxAmmo;
                        }
                        else
                        {
                            inventoryManager.currentPrimaryAmmo += inventoryManager.totalRifleAmmo;
                            inventoryManager.totalRifleAmmo = 0;
                        }
                        break;
                    case EAmmoType.Shotgun:
                        if (inventoryManager.totalShotgunAmmo >= requiredAmmo)
                        {
                            inventoryManager.totalShotgunAmmo -= requiredAmmo;
                            inventoryManager.currentPrimaryAmmo = inventoryManager.primaryWeapon.clipMaxAmmo;
                        }
                        else
                        {
                            inventoryManager.currentPrimaryAmmo += inventoryManager.totalShotgunAmmo;
                            inventoryManager.totalShotgunAmmo = 0;
                        }
                        break;
                }
            }
            inventoryManager.UpdateCurrentAmmoUI(0);
            inventoryManager.UpdateTotalAmmoUI(0);
        }
        if (inventoryManager.selectedSlot == 1 && !inventoryManager.secondaryWeapon.isMelee)
        {
            if (inventoryManager.secondaryWeapon.ammoUsageType == EAmmoUsage.Infinite)
                inventoryManager.currentSecondaryAmmo = inventoryManager.secondaryWeapon.clipMaxAmmo;
            else
            {
                int requiredAmmo = inventoryManager.secondaryWeapon.clipMaxAmmo - inventoryManager.currentSecondaryAmmo;
                Debug.LogAssertion(requiredAmmo);
                switch(inventoryManager.secondaryWeapon.ammoType)
                {
                    case EAmmoType.Pistol:
                        if (inventoryManager.totalPistolAmmo >= requiredAmmo)
                        {
                            inventoryManager.totalPistolAmmo -= requiredAmmo;
                            inventoryManager.currentSecondaryAmmo = inventoryManager.secondaryWeapon.clipMaxAmmo;
                        } 
                        else
                        {
                            inventoryManager.currentSecondaryAmmo += inventoryManager.totalPistolAmmo;
                            inventoryManager.totalPistolAmmo = 0;
                        }
                        break;
                    case EAmmoType.Rifle:
                        if (inventoryManager.totalRifleAmmo >= requiredAmmo)
                        {
                            inventoryManager.totalRifleAmmo -= requiredAmmo;
                            inventoryManager.currentSecondaryAmmo = inventoryManager.secondaryWeapon.clipMaxAmmo;
                        } 
                        else
                        {
                            inventoryManager.currentSecondaryAmmo += inventoryManager.totalRifleAmmo;
                            inventoryManager.totalRifleAmmo = 0;
                        }
                        break;
                    case EAmmoType.Shotgun:
                        if (inventoryManager.totalShotgunAmmo >= requiredAmmo)
                        {
                            inventoryManager.totalShotgunAmmo -= requiredAmmo;
                            inventoryManager.currentSecondaryAmmo = inventoryManager.secondaryWeapon.clipMaxAmmo;
                        }
                        else
                        {
                            inventoryManager.currentSecondaryAmmo += inventoryManager.totalShotgunAmmo;
                            inventoryManager.totalShotgunAmmo = 0;
                        }
                        break;
                }
            }
            inventoryManager.UpdateCurrentAmmoUI(1);
            inventoryManager.UpdateTotalAmmoUI(1);
        }
    }
}
