using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;

[DisallowMultipleComponent]
public class NSNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController gamePlayerPrefab;
    public List<PlayerObjectController> players { get; } = new List<PlayerObjectController>();

    public override void Awake()
    {
        base.Awake();
        //set transport to avoid issues
        transport = FindObjectOfType<Mirror.FizzySteam.FizzySteamworks>();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if(SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectController playerInstance = Instantiate(gamePlayerPrefab);

            playerInstance.connectionID = conn.connectionId;
            playerInstance.playerIDNumber = players.Count + 1;
            playerInstance.playerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.currentLobbyID, players.Count);

            SteamMatchmaking.SetLobbyData(new CSteamID(LobbyController.instance.currentLobbyID), "currentPlayers", players.Count.ToString());

            NetworkServer.AddPlayerForConnection(conn, playerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        NetworkServer.DestroyPlayerForConnection(conn);
        for(int i = 0; i < players.Count; i++)
        {
            if (players[i].connectionID == conn.connectionId)
            {
                players.Remove(players[i]);
                SteamMatchmaking.SetLobbyData(new CSteamID(LobbyController.instance.currentLobbyID), "currentPlayers", players.Count.ToString());
                break;
            }
        }
    }

    public void StartGame(string sceneName)
    {
        ServerChangeScene(sceneName);
    }
}