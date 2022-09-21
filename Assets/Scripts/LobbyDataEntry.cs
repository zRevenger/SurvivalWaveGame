using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Steamworks;

public class LobbyDataEntry : MonoBehaviour
{
    //Data
    public CSteamID lobbyID;
    public string lobbyName;
    public string currentPlayers;
    public string maxPlayers;
    public TextMeshProUGUI lobbyNameText;
    public TextMeshProUGUI playerCountText;

    public void SetLobbyData()
    {
        if(lobbyName == "")
            lobbyNameText.text = "Empty";
        else
            lobbyNameText.text = lobbyName;

        playerCountText.text = currentPlayers + "/" + maxPlayers;
    }

    public void JoinLobby()
    {
        SteamLobby.instance.JoinLobby(lobbyID);
    }
}
