using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "NSObjects/WeaponScriptableObject", order = 2)]
public class Weapon : ScriptableObject
{
    public GameObject weaponPrefab;
    public Sprite weaponIcon;
    public EWeaponType weaponType;
    public EAmmoUsage ammoUsageType;
    public EAmmoType ammoType;

    public int damagePerBullet;
    public int bulletAmount;

    public int clipMaxAmmo;

    public bool isMelee;
}

public enum EAmmoType
{
    Rifle,
    Pistol,
    Shotgun
}

public enum EAmmoUsage
{
    Limited,
    Infinite,
    NoAmmo
}

public enum EWeaponType
{
    Primary,
    Secondary
}
