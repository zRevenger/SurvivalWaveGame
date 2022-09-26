using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIReference : MonoBehaviour
{
    public Color unselectedColor;
    public Color selectedColor;

    public Image primaryWeaponBorder;
    public Image secondaryWeaponBorder;
    public Image grenadeBorder;
    public Image healingBorder;

    public TextMeshProUGUI currentPrimaryAmmo;
    public TextMeshProUGUI totalPrimaryAmmo;
    public TextMeshProUGUI currentSecondaryAmmo;
    public TextMeshProUGUI totalSecondaryAmmo;

    public GameObject pickupText;

    public void SelectSlot(int select)
    {
        primaryWeaponBorder.color = unselectedColor;
        secondaryWeaponBorder.color = unselectedColor;
        grenadeBorder.color = unselectedColor;
        healingBorder.color = unselectedColor;
        switch(select)
        {
            case 0:
                primaryWeaponBorder.color = selectedColor;
                break;
            case 1:
                secondaryWeaponBorder.color = selectedColor;
                break;
            case 2:
                grenadeBorder.color = selectedColor;
                break;
            case 3:
                healingBorder.color = selectedColor;
                break;
        }
    }

    public void UpdateCurrentAmmo(int slot, int newVal)
    {
        if (slot == 0)
            currentPrimaryAmmo.text = newVal.ToString();
        else if (slot == 1)
            currentSecondaryAmmo.text = newVal.ToString();
    }

    public void UpdateTotalAmmo(int slot, int newVal)
    {
        if (slot == 0)
        {
            if (newVal > -1)
                totalPrimaryAmmo.text = newVal.ToString();
            else
                totalPrimaryAmmo.text = "Infinite";
        }
        else if (slot == 1)
        {
            if (newVal > -1)
                totalSecondaryAmmo.text = newVal.ToString();
            else
                totalSecondaryAmmo.text = "Infinite";
        }
    }
}
