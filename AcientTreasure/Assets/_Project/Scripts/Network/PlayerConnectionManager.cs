using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerConnectionManager : NetworkBehaviour
{
    [FormerlySerializedAs("_playerPrefab")] [SerializeField] GameObject playerPrefab;

    [FormerlySerializedAs("_startPostionArray")] [FormerlySerializedAs("StartPostionArray")] [SerializeField] private Vector3[] startPostionArray;
    int startPositionIndex = 0;

    [FormerlySerializedAs("_authenticationManager")] [SerializeField] AuthenticationManager authenticationManager;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("Spawn");
        if (IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            NetworkManager.Singleton.OnClientDisconnectCallback += OnclientDisconnected;

        }

        if(!AuthenticationService.Instance.IsSignedIn)
             authenticationManager.SignInAnonymouslyAsync();


        //Todo Check if game already have this player car 
        CheckPlayerCarOnServerRpc(AuthenticationService.Instance.PlayerId);

       
    }

    [ServerRpc(RequireOwnership =false)]
    void CheckPlayerCarOnServerRpc(string Playerid, ServerRpcParams param= default)
    {
        var sender = param.Receive.SenderClientId;

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { sender }
            }
        };
        if (GameManager.Instance.PlayerListDictionary.ContainsKey(Playerid))
        {
         
            var playerNetworkObject = GameManager.Instance.PlayerListDictionary[Playerid].GetComponent<NetworkObject>();
            playerNetworkObject.ChangeOwnership(sender);
            AlreadyHasCarClientRpc(/*GameManager.Instance.CountedTime,*/ true, clientRpcParams);
        }
        else
        {
            AlreadyHasCarClientRpc(/*GameManager.Instance.CountedTime, */false, clientRpcParams);
        }
    }

    [ClientRpc]
    void AlreadyHasCarClientRpc(/*float passCountedTime,*/bool alreadyHas,ClientRpcParams param)
    {
        if(alreadyHas)
        {
            Debug.Log("Server already has this player's obj");
            var players = FindObjectsOfType<FirstPersonController>();
            foreach(var player in players)
            {
                player.enabled = true;
            }
            // GameManager.Instance.gameIsstart = true;
            // GameManager.Instance.CountedTime = passCountedTime;
        }
        else
        {
            Debug.Log("Server not has this player's obj");
            Invoke("SpawnPlayer", 1);
        }
    }
    void SpawnPlayer()
    {
        SpawnCarServerRpc(AuthenticationService.Instance.PlayerId);
    }

    private void OnclientDisconnected(ulong obj)
    {
        foreach(var clientObject in NetworkManager.Singleton.SpawnManager.GetClientOwnedObjects(obj))
        {
            clientObject.RemoveOwnership();
            var disconnectedCar = clientObject.GetComponent<FirstPersonController>();
            if(disconnectedCar!=null)
            {
                disconnectedCar.enabled = false;
            }
        }
    }

    [ServerRpc(RequireOwnership =false)]
    void SpawnCarServerRpc(string SenderPlayerId,ServerRpcParams param = default)
    {
      //  Debug.Log("Sender Player Id =" + SenderPlayerId);
        var tmp = Instantiate(playerPrefab, startPostionArray[startPositionIndex], Quaternion.identity);
        tmp.GetComponent<NetworkObject>().Spawn(true);
        tmp.GetComponent<NetworkObject>().ChangeOwnership(param.Receive.SenderClientId);
        startPositionIndex += 1;

        //GameManager.Instance.CarListDictionary.Add(SenderPlayerId, tmp.GetComponent<CarController>());

    }
    private void OnClientConnected(ulong obj)
    {
        // Debug.LogError("Player id  : " + obj + "is connect to server");
     
    }
}
