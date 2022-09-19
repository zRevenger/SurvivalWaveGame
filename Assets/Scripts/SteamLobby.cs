using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby instance;

    //Callbacks
    protected Callback<LobbyCreated_t> LobbyCreatedCallback;
    protected Callback<GameLobbyJoinRequested_t> JoinRequestCallback;
    protected Callback<LobbyEnter_t> LobbyEnteredCallback;

    //Lobbies callbacks
    protected Callback<LobbyMatchList_t> LobbyListCallback;
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdatedCallback;

    public List<CSteamID> lobbyIDS = new List<CSteamID>();

    //Vars
    public ulong currentLobbyID;
    private const string hostAddressKey = "HostAddress";
    private NSNetworkManager networkManager;

    private void Start()
    {
        //Check if steam is opened
        if (!SteamManager.Initialized) return;

        if (instance == null) instance = this;

        networkManager = GetComponent<NSNetworkManager>();

        LobbyCreatedCallback = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequestCallback = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEnteredCallback = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        LobbyListCallback = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdatedCallback = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, networkManager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        //if something goes wrong with lobby creation don't continue executing 
        if (callback.m_eResult != EResult.k_EResultOK) return;

        Debug.Log("Lobby Created");

        networkManager.StartHost();

        //Set host key
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey, SteamUser.GetSteamID().ToString());
        //Set lobby name
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Recieved join request");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //Actions executed for everyone
        currentLobbyID = callback.m_ulSteamIDLobby;

        //Client actions
        if (NetworkServer.active) return;

        networkManager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey);

        networkManager.StartClient();
    }

    public void JoinLobby(CSteamID lobbyID)
    {
        SteamMatchmaking.JoinLobby(lobbyID);
    }

    public void GetLobbiesList()
    {
        if (lobbyIDS.Count > 0) lobbyIDS.Clear();

        SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide);
        SteamMatchmaking.RequestLobbyList();
    }

    public void OnGetLobbyList(LobbyMatchList_t result)
    {
        Debug.Log("getting lobbies");
        if (LobbiesListManager.instance.listOfLobbies.Count > 0)
            LobbiesListManager.instance.DestroyLobbies();

        for(int i = 0; i< result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDS.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }

    public void OnGetLobbyData(LobbyDataUpdate_t result) => LobbiesListManager.instance.DisplayLobbies(lobbyIDS, result);
}
