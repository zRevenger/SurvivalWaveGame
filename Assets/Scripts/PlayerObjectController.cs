using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;

public class PlayerObjectController : NetworkBehaviour
{
    public GameObject playerModel;
    public Animator animator;

    //Player Data
    [SyncVar] public int connectionID;
    [SyncVar] public int playerIDNumber;
    [SyncVar] public ulong playerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string playerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool ready;

    //Class
    [SyncVar(hook = nameof(SendPlayerClass))] public int playerClass;

    /*private NSNetworkManager networkManager;

    private NSNetworkManager _networkManager
    {
        get
        {
            if (networkManager != null)
                return networkManager;

            return networkManager = NSNetworkManager.singleton as NSNetworkManager;
        }
    }*/

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private bool hasInitialized;

    private void Update()
    {
        if(IsInGameScene())
        {
            if (!hasInitialized)
            {
                hasInitialized = true;
            }

            if (!hasAuthority) return;

        }
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.instance.FindLocalPlayer();
        LobbyController.instance.InitializeLobbyName();
    }

    public override void OnStartClient()
    {
        NSNetworkManager.singleton.GetComponent<NSNetworkManager>().players.Add(this);
        LobbyController.instance.InitializeLobbyName();
        LobbyController.instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        NSNetworkManager.singleton.GetComponent<NSNetworkManager>().players.Remove(this);
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
        NSNetworkManager.singleton.GetComponent<NSNetworkManager>().StartGame(sceneName);
    }

    //Class
    [Command]
    public void CmdUpdatePlayerClass(int newValue)
    {
        SendPlayerClass(playerClass, newValue);
    }

    public void SendPlayerClass(int oldValue, int newValue)
    {
        if (isServer)
        {
            playerClass = newValue;
        }
        if(isClient && oldValue != newValue)
        {
            UpdateClass(newValue);
        }
    }

    //it's weird but it's done 'cause sometimes it's buggy
    void UpdateClass(int message)
    {
        playerClass = message;
    }
    
    public bool IsInGameScene()
    {
        return SceneManager.GetActiveScene().name == "Village";
    }

    public void LeaveLobby()
    {
        //Set the offline scene to null
        NSNetworkManager.singleton.GetComponent<NSNetworkManager>().offlineScene = "";

        //Make the active scene the offline scene
        SceneManager.LoadScene("MainMenu");

        //Leave Steam Lobby
        SteamLobby.instance.LeaveLobby();

        if (hasAuthority)
        {
            if (isServer)
            {
                NSNetworkManager.singleton.GetComponent<NSNetworkManager>().StopHost();
            }
            else
            {
                NSNetworkManager.singleton.GetComponent<NSNetworkManager>().StopClient();
            }
        }
    }
}
