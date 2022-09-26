using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InventoryManager : NetworkBehaviour
{
    public int selectedSlot;

    public Weapon primaryWeapon;
    public Weapon secondaryWeapon;

    private CombatManager combatManager;

    private bool uiLoading;
    private GameUIReference uiReference;

    public int currentPrimaryAmmo;
    public int currentSecondaryAmmo;

    public int totalRifleAmmo;
    public int totalShotgunAmmo;
    public int totalPistolAmmo;

    private void Start()
    {
        combatManager = GetComponent<CombatManager>();
        secondaryWeapon = combatManager.weaponData[0];
        selectedSlot = 1;

        currentSecondaryAmmo = secondaryWeapon.clipMaxAmmo;

        totalRifleAmmo = 500;
        totalPistolAmmo = 60;
        totalShotgunAmmo = 300;
    }

    private void Update()
    {
        if (GetComponent<PlayerObjectController>().IsInGameScene())
        {
            if (!hasAuthority) return;

            if (!SceneManager.GetSceneByName("GameUI").isLoaded && !uiLoading)
            {
                uiLoading = true;
                SceneManager.LoadSceneAsync("GameUI", LoadSceneMode.Additive);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
                SelectSlot(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                SelectSlot(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                SelectSlot(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                SelectSlot(3);

            UpdateUI();

            CheckPickup();
        }
        else
        {
            if (SceneManager.GetSceneByName("GameUI").isLoaded == true)
            {
                uiLoading = false;
                SceneManager.UnloadSceneAsync("GameUI");
            }
        }
    }

    private void UpdateUI()
    {
        if (uiReference == null && SceneManager.GetSceneByName("GameUI").isLoaded)
        {
            uiReference = FindObjectOfType<GameUIReference>().GetComponent<GameUIReference>();
            uiReference.SelectSlot(selectedSlot);
            UpdateCurrentAmmoUI(1);
            UpdateTotalAmmoUI(1);
        }

        if (uiReference == null) return;

        if(primaryWeapon == null)
        {
            uiReference.currentPrimaryAmmo.gameObject.SetActive(false);
            uiReference.totalPrimaryAmmo.gameObject.SetActive(false);
        } else
        {
            uiReference.currentPrimaryAmmo.gameObject.SetActive(true);
            uiReference.totalPrimaryAmmo.gameObject.SetActive(true);
        }

        if (secondaryWeapon == null)
        {
            uiReference.currentSecondaryAmmo.gameObject.SetActive(false);
            uiReference.totalSecondaryAmmo.gameObject.SetActive(false);
        } else
        {
            uiReference.currentSecondaryAmmo.gameObject.SetActive(true);
            uiReference.totalSecondaryAmmo.gameObject.SetActive(true);
        }
    }

    public void UpdateCurrentAmmoUI(int slot)
    {
        uiReference.UpdateCurrentAmmo(slot, slot == 0 ? currentPrimaryAmmo : currentSecondaryAmmo);
    }

    public void UpdateTotalAmmoUI(int slot)
    {
        int ammoToDisplay = slot == 0 ? (primaryWeapon.ammoType == EAmmoType.Rifle ? totalRifleAmmo : primaryWeapon.ammoType == EAmmoType.Shotgun ? totalShotgunAmmo : totalPistolAmmo)
            : (secondaryWeapon.ammoType == EAmmoType.Pistol ? totalPistolAmmo : secondaryWeapon.ammoType == EAmmoType.Shotgun ? totalShotgunAmmo : totalRifleAmmo);

        if (slot == 0 && primaryWeapon.ammoUsageType == EAmmoUsage.Infinite)
            ammoToDisplay = -1;
        if (slot == 1 && secondaryWeapon.ammoUsageType == EAmmoUsage.Infinite)
            ammoToDisplay = -1;
        uiReference.UpdateTotalAmmo(slot, ammoToDisplay);
    }

    private void SelectSlot(int slot)
    {
        selectedSlot = slot;
        uiReference.SelectSlot(selectedSlot);
    }

    private void CheckPickup()
    {
        RaycastHit hit;
        Ray ray = new Ray(GetComponent<CameraController>().camRef.transform.position, GetComponent<CameraController>().camRef.transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
            PickupObject pickupData = hit.collider.GetComponent<PickupObject>();
            if (pickupData != null)
            {
                uiReference.pickupText.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                    HandlePickup(pickupData);
            }
            else
                uiReference.pickupText.SetActive(false);
        }
    }

    private void HandlePickup(PickupObject pickupData)
    {
        if (pickupData != null)
        {
            if (pickupData.pickupType == EPickupType.Weapon)
            {
                int newClipSize = pickupData.clipAmmo;
                Weapon weapon = (Weapon)pickupData.objectData;
                if (weapon.weaponType == EWeaponType.Primary)
                {
                    if (primaryWeapon != null)
                    {
                        pickupData.objectData = primaryWeapon;
                        pickupData.UpdatePickup(currentPrimaryAmmo);
                    }
                    else
                        Destroy(pickupData.gameObject);

                    currentPrimaryAmmo = newClipSize;
                    primaryWeapon = weapon;
                    UpdateCurrentAmmoUI(0);
                    UpdateTotalAmmoUI(0);
                }
                else
                {
                    if (secondaryWeapon != null)
                    {
                        pickupData.objectData = secondaryWeapon;
                        pickupData.UpdatePickup(currentSecondaryAmmo);
                    }
                    else
                        Destroy(pickupData.gameObject);

                    currentSecondaryAmmo = newClipSize;
                    secondaryWeapon = weapon;
                    UpdateCurrentAmmoUI(1);
                    UpdateTotalAmmoUI(1);
                }
            }
        }
    }
}
