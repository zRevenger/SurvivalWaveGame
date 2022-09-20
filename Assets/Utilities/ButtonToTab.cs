using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonToTab : MonoBehaviour
{
    public TabButtonElement correspondingTab;

    public void SelectTab()
    {
        if (correspondingTab != null)
            correspondingTab.tabGroup.OnTabSelected(correspondingTab);
    }
}
