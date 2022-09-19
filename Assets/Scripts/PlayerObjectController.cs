using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    //Player Data
    [SyncVar] public int connectionID;
    [SyncVar] public int playerIDNumber;
    [SyncVar] public ulong playerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string playerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool ready;

    private NSNetworkManager networkManager;

    private NSNetworkManager _networkManager
    {
        get
        {
            if (networkManager != null)
                return networkManager;

            return networkManager = NSNetworkManager.singleton as NSNetworkManager;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.instance.FindLocalPlayer();
        LobbyController.instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        _networkManager.players.Add(this);
        LobbyController.instance.UpdateLobbyName();
        LobbyController.instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        networkManager.players.Remove(this);
        LobbyController.instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string name)
    {
        this.PlayerNameUpdate(playerName, name);
    }

    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if(isServer)
            playerName = newValue;

        if (isClient)
            LobbyController.instance.UpdatePlayerList();
    }

    private void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer)
            ready = newValue;

        if (isClient)
            LobbyController.instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerReady()
    {
        PlayerReadyUpdate(ready, !ready);
    }

    public void ChangeReady()
    {
        if (hasAuthority)
            CmdSetPlayerReady();
    }

    //Start Game
    public void CanStartGame(string sceneName)
    {
        if(hasAuthority)
        {
            CmdCanStartGame(sceneName);
        }
    }

    [Command]
    public void CmdCanStartGame(string sceneName)
    {
        networkManager.StartGame(sceneName);
    }
}
