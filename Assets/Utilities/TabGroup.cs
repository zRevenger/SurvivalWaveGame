using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabButtonElement> tabButtons;
    public Color tabIdle;
    public Color tabHover;
    public Color tabSelected;

    public TabButtonElement selectedTab;
    public List<GameObject> objectsToSwap;

    public void Subscribe(TabButtonElement button)
    {
        if (tabButtons == null)
            tabButtons = new List<TabButtonElement>();

        tabButtons.Add(button);

        if (button == selectedTab)
            OnTabSelected(button);
    }

    public void OnTabEnter(TabButtonElement button)
    {
        ResetTabs();
        if(selectedTab == null || button != selectedTab)
            button.backgroud.color = tabHover;
    }

    public void OnTabExit(TabButtonElement button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButtonElement button)
    {
        if(selectedTab != null)
            selectedTab.Deselect();

        selectedTab = button;

        selectedTab.Select();

        ResetTabs();
        button.backgroud.color = tabSelected;
        int index = button.transform.GetSiblingIndex();
        for(int i = 0; i < objectsToSwap.Count; i++)
        {
            if(i == index)
                objectsToSwap[i].SetActive(true);
            else
                objectsToSwap[i].SetActive(false);

        }
    }

    public void ResetTabs()
    {
        foreach (TabButtonElement button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab) continue;
            button.backgroud.color = tabIdle;
        }
    }
}
