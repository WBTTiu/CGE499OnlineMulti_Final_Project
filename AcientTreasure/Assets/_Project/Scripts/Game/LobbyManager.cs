using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    private static LobbyManager instance;

    public string PlayerName;

    float heartbeatTimer = 0;

    Lobby hostLobby;
    Lobby joinedLobby;

    [SerializeField] LobbyUiManager lobbyUiManager;

    float refreshLobbyTimer = 0;

    bool isInRoom = false;
    string currentJoinCode = "";
    public static LobbyManager Instance
    {
        get
        {
            if (instance == null)
            {
                // If no instance exists, find one in the scene or create a new one
                instance = FindObjectOfType<LobbyManager>();

                if (instance == null)
                {
                    // If no instance exists in the scene,u7 create a new GameObject and attach the singleton script
                    GameObject singletonObject = new GameObject("LobbyManager");
                    instance = singletonObject.AddComponent<LobbyManager>();
                }
            }

            return instance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //This is only for debug purpose
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ListLobby();
        }

        SendHeartbeatTimer();

        RefreshLobbyRoom();
    }

    private void RefreshLobbyRoom()
    {
        if (!AuthenticationService.Instance.IsSignedIn) return;
        if (isInRoom == true) return;
        refreshLobbyTimer += Time.deltaTime;
        if (refreshLobbyTimer > 3)
        {
            refreshLobbyTimer = 0;
            ListLobby();
        }
    }

    private async void SendHeartbeatTimer()
    {
        if (hostLobby == null) return;
        heartbeatTimer += Time.deltaTime;
        if (heartbeatTimer > 15)
        {
            heartbeatTimer = 0;
            await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
        }
    }

    public async void LeaveRoom(string lobbyId)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(lobbyId, AuthenticationService.Instance.PlayerId);
            lobbyUiManager.HideInRoomPanel();
            lobbyUiManager.ShowAllLobbyRoom();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void CreateLobby(string lobbyName, int maxPlayer)
    {
        try
        {
            CreateLobbyOptions lobbyOption = new CreateLobbyOptions();

            Dictionary<string, PlayerDataObject> playerDataDict = new Dictionary<string, PlayerDataObject>();
            PlayerDataObject playerNameData = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerName);

            playerDataDict.Add("PlayerName", playerNameData);
            playerDataDict.Add("Rank", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "Platinum"));

            lobbyOption.Player = new Player(AuthenticationService.Instance.PlayerId, null, playerDataDict);

            Dictionary<string, DataObject> lobbyData = new Dictionary<string, DataObject>();
            lobbyData.Add("IsStart", new DataObject(DataObject.VisibilityOptions.Member, "false"));
            lobbyOption.Data = lobbyData;

            LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
            callbacks.LobbyChanged += OnLobbyChanged;

            hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, lobbyOption);
            Debug.Log(hostLobby.Data["IsStart"].Value);
      
            try
            {
                var addedCallback = LobbyService.Instance.SubscribeToLobbyEventsAsync(hostLobby.Id, callbacks);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e.Reason);
            }

            lobbyUiManager.ShowInRoomPanel(hostLobby);
            isInRoom = true;

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void OnLobbyChanged(ILobbyChanges onChange)
    {
        if (onChange.PlayerJoined.Value != null)
        {
            Debug.Log(onChange.PlayerJoined.Value[0].Player.Data["PlayerName"].Value);
            lobbyUiManager.JoinedPlayerName(onChange.PlayerJoined.Value[0].Player.Data["PlayerName"].Value);
        }

        if (onChange.PlayerLeft.Value!=null)
        {
            Debug.Log(onChange.PlayerLeft.Value[0]);
            lobbyUiManager.PlayerLeaveRoom(hostLobby);
        }

       // Debug.Log(onChange.Data.Changed);
       // Debug.Log(onChange.Data.ChangeType);
       // Debug.Log(onChange.Data.Value);
        if(onChange.Data.Changed == true)
        {
            Debug.Log(onChange.Data.Value["IsStart"].Value.Value);
            StartGameClient(onChange.Data.Value["IsStart"].Value.Value);
        }

    }

    private void StartGameClient(string joinCode)
    {
        lobbyUiManager.HideInRoomPanel();
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        SetClientRelayConnection(joinCode);
    }

    private async void SetClientRelayConnection(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayData = new RelayServerData(joinAllocation, "wss");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayData);
            NetworkManager.Singleton.StartClient();
        }
        catch( RelayServiceException e)
        {
            Debug.Log(e);
        }
       
    }

    public async void JoinRoom(string lobbyId)
    {
        try
        {
            JoinLobbyByIdOptions lobbyOption = new JoinLobbyByIdOptions();
            Dictionary<string, PlayerDataObject> playerDataDict = new Dictionary<string, PlayerDataObject>();

            PlayerDataObject playerNameData = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerName);

            playerDataDict.Add("PlayerName", playerNameData);
            playerDataDict.Add("Rank", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "Platinum"));

            lobbyOption.Player = new Player(AuthenticationService.Instance.PlayerId, null, playerDataDict);

            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, lobbyOption);
            lobbyUiManager.ShowInRoomPanel(joinedLobby);
            isInRoom = true;
            Debug.Log(joinedLobby.Data["IsStart"].Value);

            LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
            callbacks.LobbyChanged += OnLobbyChanged;
            try
            {
                var addedCallback = LobbyService.Instance.SubscribeToLobbyEventsAsync
                    (joinedLobby.Id, callbacks);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e.Reason);
            }

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    public async void ListLobby()
    {
        QueryResponse queryResult = await LobbyService.Instance.QueryLobbiesAsync();
        Debug.Log("Number of created lobby is : " + queryResult.Results.Count);
        foreach (var result in queryResult.Results)
        {
            Debug.Log(result.Name);
            Debug.Log(result.Players.Count);
        }
        lobbyUiManager.ListLobby(queryResult.Results);

    }
    public async void StartGame(string lobbyId)
    {
        // SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        SetHostRelaytConnection(lobbyId);
    }

    private async void SetHostRelaytConnection(string lobbyId)
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(5);

        RelayServerData relayServerData = new RelayServerData(allocation, "wss");

        currentJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log(currentJoinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartHost();

        try
        {
            UpdateLobbyOptions updateLobbyOption = new UpdateLobbyOptions();
            updateLobbyOption.Data = new Dictionary<string, DataObject>();
            updateLobbyOption.Data.Add("IsStart", new DataObject(DataObject.VisibilityOptions.Member, currentJoinCode));

            hostLobby = await LobbyService.Instance.UpdateLobbyAsync(lobbyId, updateLobbyOption);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        Debug.Log(hostLobby.Data["IsStart"].Value);
    }
}
