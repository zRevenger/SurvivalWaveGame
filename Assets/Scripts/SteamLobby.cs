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

    private void Start()
    {
        //Check if steam is opened
        if (!SteamManager.Initialized) return;

        if (instance == null) instance = this;

        LobbyCreatedCallback = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequestCallback = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEnteredCallback = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        LobbyListCallback = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdatedCallback = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, NSNetworkManager.singleton.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        //if something goes wrong with lobby creation don't continue executing 
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.Log(callback.m_eResult);
            return;
        }

        NSNetworkManager.singleton.StartHost();

        //Set host key
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey, SteamUser.GetSteamID().ToString());
        //Set lobby name
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "currentPlayers", "0");
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "maxPlayers", NetworkManager.singleton.maxConnections.ToString());
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //Actions executed for everyone
        currentLobbyID = callback.m_ulSteamIDLobby;

        //Client actions
        if (NetworkServer.active) return;

        NSNetworkManager.singleton.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey);

        NSNetworkManager.singleton.StartClient();
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
