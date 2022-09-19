using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class LobbyController : MonoBehaviour
{
    public static LobbyController instance;

    //UI elements
    public TextMeshProUGUI lobbyNameText;

    //Player Data
    public GameObject playerListViewContent;
    public GameObject playerListItemPrefab;
    public GameObject localPlayerObject;

    //Other Data
    public ulong currentLobbyID;
    public bool playerItemCreated = false;
    private List<PlayerListItem> playerListItems = new List<PlayerListItem>();
    public PlayerObjectController localPlayerController;

    //Ready
    public Button startGameButton;
    public TextMeshProUGUI readyButtonText;

    //Network Manager
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

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void ReadyPlayer()
    {
        localPlayerController.ChangeReady();
    }

    public void UpdateButton()
    {
        if (localPlayerController.ready)
            readyButtonText.text = "Not Ready";
        else
            readyButtonText.text = "Ready";
    }

    public void CheckIfAllReady()
    {
        bool allReady = false;

        foreach (PlayerObjectController player in _networkManager.players)
        {
            if (player.ready)
                allReady = true;
            else
            {
                allReady = false;
                break;
            }
        }

        if(allReady)
        {
            //if you're host and everyone is ready you can click start game
            if (localPlayerController.playerIDNumber == 1)
                startGameButton.interactable = true;
            else
                startGameButton.interactable = false;
        } else
        {
            startGameButton.interactable = false;
        }
    }

    public void StartGame(string sceneName)
    {
        localPlayerController.CanStartGame(sceneName);
    }

    public void UpdateLobbyName()
    {
        currentLobbyID = networkManager.GetComponent<SteamLobby>().currentLobbyID;
        lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(currentLobbyID), "name");
    }

    public void UpdatePlayerList()
    {
        if (!playerItemCreated)
            CreateHostPlayerItem(); //Host

        if (playerListItems.Count < _networkManager.players.Count)
            CreateClientPlayerItem();
        if (playerListItems.Count > _networkManager.players.Count)
            RemovePlayerItem();
        if (playerListItems.Count == _networkManager.players.Count)
            UpdatePlayerItem();
    }

    public void FindLocalPlayer()
    {
        localPlayerObject = GameObject.Find("LocalGamePlayer");
        localPlayerController = localPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void CreateHostPlayerItem()
    {
        foreach(PlayerObjectController player in _networkManager.players)
        {
            GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
            PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

            newPlayerItemScript.playerName = player.playerName;
            newPlayerItemScript.connectionID = player.connectionID;
            newPlayerItemScript.playerSteamID = player.playerSteamID;
            newPlayerItemScript.ready = player.ready;
            newPlayerItemScript.SetPlayerValues();

            newPlayerItem.transform.SetParent(playerListViewContent.transform);
            newPlayerItem.transform.localScale = Vector3.one;

            playerListItems.Add(newPlayerItemScript);
        }
        playerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in networkManager.players)
        {
            if(!playerListItems.Any(b => b.connectionID == player.connectionID))
            {
                GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
                PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

                newPlayerItemScript.playerName = player.playerName;
                newPlayerItemScript.connectionID = player.connectionID;
                newPlayerItemScript.playerSteamID = player.playerSteamID;
                newPlayerItemScript.ready = player.ready;
                newPlayerItemScript.SetPlayerValues();

                newPlayerItem.transform.SetParent(playerListViewContent.transform);
                newPlayerItem.transform.localScale = Vector3.one;

                playerListItems.Add(newPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in networkManager.players)
        {
            foreach(PlayerListItem playerListItemScript in playerListItems)
            {
                if(playerListItemScript.connectionID == player.connectionID)
                {
                    playerListItemScript.playerName = player.playerName;
                    playerListItemScript.ready = player.ready;
                    playerListItemScript.SetPlayerValues();
                    if(player == localPlayerController)
                    {
                        UpdateButton();
                    }
                }
            }
        }
        CheckIfAllReady();
    }

    public void RemovePlayerItem()
    {
        List<PlayerListItem> itemsToRemove = new List<PlayerListItem>();

        foreach(PlayerListItem item in playerListItems)
        {
            if (!networkManager.players.Any(b => b.connectionID == item.connectionID))
            {
                itemsToRemove.Add(item);
            }
        }

        if (itemsToRemove.Count > 0)
        {
            foreach(PlayerListItem item in itemsToRemove)
            {
                GameObject objectToRemove = item.gameObject;
                playerListItems.Remove(item);
                Destroy(objectToRemove);
                objectToRemove = null;
            }
        }
    }
}
