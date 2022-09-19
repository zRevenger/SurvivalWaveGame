using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerClassManager : MonoBehaviour
{
    public int currentClassIndex = 0;
    public PlayableClass[] playerClasses;
    public Image currentClassImage;
    public TextMeshProUGUI currentClassText;

    private void Start()
    {
        currentClassIndex = PlayerPrefs.GetInt("currentClassIndex", 0);
        currentClassImage.color = playerClasses[currentClassIndex].tempColor.color;
        currentClassText.text = playerClasses[currentClassIndex].className;
        LobbyController.instance.localPlayerController.CmdUpdatePlayerClass(currentClassIndex);
    }

    public void NextClass()
    {
        if (currentClassIndex < playerClasses.Length - 1)
        {
            currentClassIndex++;
            PlayerPrefs.SetInt("currentClassIndex", currentClassIndex);
            currentClassImage.color = playerClasses[currentClassIndex].tempColor.color;
            currentClassText.text = playerClasses[currentClassIndex].className;
            LobbyController.instance.localPlayerController.CmdUpdatePlayerClass(currentClassIndex);
        }
    }

    public void PreviousClass()
    {
        if (currentClassIndex > 0)
        {
            currentClassIndex--;
            PlayerPrefs.SetInt("currentClassIndex", currentClassIndex);
            currentClassImage.color = playerClasses[currentClassIndex].tempColor.color;
            currentClassText.text = playerClasses[currentClassIndex].className;
            LobbyController.instance.localPlayerController.CmdUpdatePlayerClass(currentClassIndex);
        }
    }
}
