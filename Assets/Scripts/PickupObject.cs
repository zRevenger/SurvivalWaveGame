using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickupObject : NetworkBehaviour
{
    [SyncVar]
    public EPickupType pickupType;

    public List<Weapon> possibleWeaponData;

    public ScriptableObject objectData;

    [SyncVar]
    public int clipAmmo;

    private void Start()
    {
        CmdOnPickupSpawn();
    }

    [ClientRpc]
    private void OnPickupSpawn()
    {
        if (pickupType == EPickupType.Weapon)
        {
            Instantiate(((Weapon)objectData).weaponPrefab, transform);
            clipAmmo = ((Weapon)objectData).clipMaxAmmo;
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdOnPickupSpawn()
    {
        OnPickupSpawn();
    }

    [ClientRpc]
    public void UpdatePickup(int id, int clipAmmo)
    {
        if (pickupType == EPickupType.Weapon)
        {
            if (transform.childCount > 0)
            {
                Debug.Log("destroying existing prefab");
                Destroy(transform.GetChild(0).gameObject);
            }
            objectData = possibleWeaponData[id];
            Instantiate(((Weapon)objectData).weaponPrefab, transform);
            this.clipAmmo = clipAmmo;
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdUpdatePickup(int id, int clipAmmo)
    {
        UpdatePickup(id, clipAmmo);
    }

    [ClientRpc]
    public void ClearPickup() => Destroy(gameObject);

    [Command(requiresAuthority = false)]
    public void CmdClearPickup() => ClearPickup();
}

public enum EPickupType
{
    Weapon,
    Healing,
    Grenade
}
