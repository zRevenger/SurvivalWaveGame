using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickupObject : NetworkBehaviour
{
    public EPickupType pickupType;
    public ScriptableObject objectData;

    public int clipAmmo;

    private void Start()
    {
        OnPickupSpawn();
    }

    private void OnPickupSpawn()
    {
        if (pickupType == EPickupType.Weapon)
        {
            Instantiate(((Weapon)objectData).weaponPrefab, transform);
            clipAmmo = ((Weapon)objectData).clipMaxAmmo;
        }
    }

    public void UpdatePickup(int clipAmmo)
    {
        if (pickupType == EPickupType.Weapon)
        {
            if(transform.childCount > 0)
                Destroy(transform.GetChild(0).gameObject);
            Instantiate(((Weapon)objectData).weaponPrefab, transform);
            this.clipAmmo = clipAmmo;
        }
    }
}

public enum EPickupType
{
    Weapon,
    Healing,
    Grenade
}
