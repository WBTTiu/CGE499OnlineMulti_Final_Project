using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListElement : MonoBehaviour
{
    [SerializeField] TMP_Text lobbyNameText;
    [SerializeField] TMP_Text maxPlayerText;
    [SerializeField] Button joinButton;

    string lobbyCode;
    public void Initialize(string lobbyName , int maxPlayer, string lobbyCode)
    {
        lobbyNameText.text = lobbyName;
        maxPlayerText.text = "1/" + maxPlayer;
        this.lobbyCode = lobbyCode;
        joinButton.onClick.AddListener(OnClickJoinbutton);
    }

    private void OnClickJoinbutton()
    {
        LobbyManager.Instance.JoinRoom(lobbyCode);
    }
}
