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

    public float pickupRange = 2.5f;

    private PlayerObjectController playerObjectController;
    private CameraController cameraController;

    private void Start()
    {
        combatManager = GetComponent<CombatManager>();
        playerObjectController = GetComponent<PlayerObjectController>();
        cameraController = GetComponent<CameraController>();
        secondaryWeapon = combatManager.weaponData[0];

        currentSecondaryAmmo = secondaryWeapon.clipMaxAmmo;

        totalRifleAmmo = 500;
        totalPistolAmmo = 60;
        totalShotgunAmmo = 300;
    }

    private void Update()
    {
        if (playerObjectController.IsInGameScene())
        {

            if (!hasAuthority) return;

            if (!SceneManager.GetSceneByName("GameUI").isLoaded && !uiLoading)
            {
                uiLoading = true;
                SceneManager.LoadSceneAsync("GameUI", LoadSceneMode.Additive);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                int newSlot = selectedSlot == 3 ? primaryWeapon == null ? 1 : 0 : ++selectedSlot;
                SelectSlot(newSlot);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                int newSlot = selectedSlot == 0 ? 3 
                            : selectedSlot == 1 && primaryWeapon == null ? 3 
                            : --selectedSlot;
                SelectSlot(newSlot);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) && primaryWeapon != null)
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
            SelectSlot(1);
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
        if ((slot == 0 && primaryWeapon != null) || (slot == 1 && secondaryWeapon != null))
        {
            CmdChangeWeaponModel(slot);
        }
    }

    [ClientRpc]
    private void ChangeWeaponModel(int slot)
    {
        ItemPlacingReference reference = playerObjectController.playerModel.GetComponent<ItemPlacingReference>();
        if (reference.gunPlacingTransform.childCount > 0)
            Destroy(reference.gunPlacingTransform.GetChild(0).gameObject);

        GameObject objectToInstantiate = slot == 0 ? primaryWeapon.weaponPrefab : secondaryWeapon.weaponPrefab;

        GameObject instantiatedWeapon = Instantiate(objectToInstantiate, reference.gunPlacingTransform);
        instantiatedWeapon.transform.localRotation = Quaternion.Euler(new Vector3(-170, 85, 90));
    }

    [Command]
    private void CmdChangeWeaponModel(int slot)
    {
        ChangeWeaponModel(slot);
    }

    private void CheckPickup()
    {
        RaycastHit hit;
        Ray ray = new Ray(GetComponent<CameraController>().camRef.transform.position, GetComponent<CameraController>().camRef.transform.forward);
        if (Physics.Raycast(ray, out hit, pickupRange))
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
        } else
            uiReference.pickupText.SetActive(false);
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
                        for(int i = 0; i < combatManager.weaponData.Count; i++)
                        {
                            if (combatManager.weaponData[i] == primaryWeapon)
                            {
                                pickupData.CmdUpdatePickup(i, currentPrimaryAmmo);
                                break;
                            }
                        }
                    }
                    else
                        pickupData.CmdClearPickup();

                    currentPrimaryAmmo = newClipSize;
                    primaryWeapon = weapon;
                    UpdateCurrentAmmoUI(0);
                    UpdateTotalAmmoUI(0);
                    if (selectedSlot == 0)
                        CmdChangeWeaponModel(0);
                }
                else
                {
                    if (secondaryWeapon != null)
                    {
                        for (int i = 0; i < combatManager.weaponData.Count; i++)
                        {
                            if (combatManager.weaponData[i] == secondaryWeapon)
                            {
                                pickupData.CmdUpdatePickup(i, currentSecondaryAmmo);
                                break;
                            }
                        }
                    }
                    else
                        pickupData.CmdClearPickup();

                    currentSecondaryAmmo = newClipSize;
                    secondaryWeapon = weapon;
                    UpdateCurrentAmmoUI(1);
                    UpdateTotalAmmoUI(1);
                    if(selectedSlot == 1)
                        CmdChangeWeaponModel(1);
                }
            }
        }
    }
}
