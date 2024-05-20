using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUiManager : MonoBehaviour
{
    [SerializeField] TMP_InputField roomNameTextInput;
    [SerializeField] TMP_InputField maxPlayerTextInput;
    [SerializeField] Button createLobbyButton;

    [SerializeField] TMP_InputField roomCodeTextInput;
    [SerializeField] Button joinRoomButton;

    [SerializeField] TMP_InputField playerNameTextInput;
    [SerializeField] Button okPlayerNameButton;

    [SerializeField] GameObject enterPlayerNameUiGroup;

    [SerializeField] Button confirmCreateLobbyButton;
    [SerializeField] Button cancelCreateLobbyButton;

    [SerializeField] GameObject createLobbyPanelGroup;

    [SerializeField] GameObject lobbyListElementPrefab;
    [SerializeField] Transform lobbyScrollContentParent;

    [SerializeField] GameObject inLobbyRoomPanelGroup;
    [SerializeField] TMP_Text playerInLobbyRoomListText;
    [SerializeField] TMP_Text currentPlayerInLobbyRoomText;

    [SerializeField] GameObject lobbyRoomScrollPanel;

 

    int currentPlayerNumber = 0;
    int currentMaxPlayerNumber = 0;

    [SerializeField] Button leaveRoomButton;
    [SerializeField] Button startGameButton;

    string currentLobbyId;


    void Start()
    {
        createLobbyButton.onClick.AddListener(OnClickCreateLobbyButton);
        joinRoomButton.onClick.AddListener(OnClickJoinRoomButton);

        okPlayerNameButton.onClick.AddListener(OnClickOkPlayerNameButton);

        confirmCreateLobbyButton.onClick.AddListener(OnClickConfirmCreateLobbyButton);
        cancelCreateLobbyButton.onClick.AddListener(OnClickCancelCreateLobbyButton);

        leaveRoomButton.onClick.AddListener(OnClickLeaveRoomButton);

        startGameButton.onClick.AddListener(OnClickStartGameButton);
    }

    private void OnClickStartGameButton()
    {
        inLobbyRoomPanelGroup.SetActive(false);
        LobbyManager.Instance.StartGame(currentLobbyId);
    }

    private void OnClickLeaveRoomButton()
    {
        LobbyManager.Instance.LeaveRoom(currentLobbyId);
    }

    private void OnClickCancelCreateLobbyButton()
    {
        createLobbyPanelGroup.SetActive(false);
    }

    private void OnClickConfirmCreateLobbyButton()
    {
        LobbyManager.Instance.CreateLobby(roomNameTextInput.text, int.Parse(maxPlayerTextInput.text));
        createLobbyPanelGroup.SetActive(false);
    }

    private void OnClickOkPlayerNameButton()
    {
        LobbyManager.Instance.PlayerName = playerNameTextInput.text;

        enterPlayerNameUiGroup.SetActive(false);

        ShowAllLobbyRoom();
    }

    internal void HideInRoomPanel()
    {
        inLobbyRoomPanelGroup.SetActive(false);
    }

    public void ShowAllLobbyRoom()
    {
        lobbyRoomScrollPanel.SetActive(true);
    }

    private void OnClickJoinRoomButton()
    {
        LobbyManager.Instance.JoinRoom(roomCodeTextInput.text);
    }

    private void OnClickCreateLobbyButton()
    {
        createLobbyPanelGroup.SetActive(true);
    }

    public void ListLobby(List<Lobby> lobbys )
    {
        foreach(Transform elementTransform in lobbyScrollContentParent)
        {
            Destroy(elementTransform.gameObject);
        }
        foreach (var lobby in lobbys)
        {
            var tmp = Instantiate(lobbyListElementPrefab, lobbyScrollContentParent);
            LobbyListElement lobbylistElement = tmp.GetComponent<LobbyListElement>();
            lobbylistElement.Initialize(lobby.Name, lobby.MaxPlayers, lobby.Id);
        }
    }

    public void ShowInRoomPanel(Lobby lobby)
    {
        lobbyRoomScrollPanel.SetActive(false);
        inLobbyRoomPanelGroup.SetActive(true);

        currentPlayerInLobbyRoomText.text = lobby.Players.Count + " / " + lobby.MaxPlayers;

      //  Debug.Log("Host name is : " + lobby.Players[0].Data["PlayerName"].Value);
        // Debug.Log("Host Rank is : " + lobby.Players[0].Data["Rank"].Value);

        playerInLobbyRoomListText.text = "";
        Debug.Log(lobby.Players.Count);
        foreach(var player in lobby.Players)
        {
            playerInLobbyRoomListText.text += player.Data["PlayerName"].Value + "\n";
        }
        currentPlayerNumber = lobby.Players.Count;
        currentMaxPlayerNumber = lobby.MaxPlayers;
        currentLobbyId = lobby.Id;

        Debug.Log("Host id : " + lobby.HostId);
        Debug.Log("Playe id  : " + AuthenticationService.Instance.PlayerId);
        if (AuthenticationService.Instance.PlayerId == lobby.HostId)
        {
            startGameButton.gameObject.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void JoinedPlayerName(string joinedPlayerName)
    {
        playerInLobbyRoomListText.text += joinedPlayerName + "\n";
        currentPlayerNumber += 1;
        currentPlayerInLobbyRoomText.text = currentPlayerNumber + " / " + currentMaxPlayerNumber;

    }

    public void PlayerLeaveRoom(Lobby lobby)
    {
        playerInLobbyRoomListText.text = "";
        Debug.Log("Ramin Pla" + lobby.Players.Count);
        foreach (var player in lobby.Players)
        {
            playerInLobbyRoomListText.text += player.Data["PlayerName"].Value + "\n";
        }
    }

}
