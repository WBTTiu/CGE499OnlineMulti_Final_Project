using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    private static GameManager instance;

    public Dictionary<string, FirstPersonController> PlayerListDictionary = new Dictionary<string, FirstPersonController>();
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                // If no instance exists, find one in the scene or create a new one
                instance = FindObjectOfType<GameManager>();

                if (instance == null)
                {
                    // If no instance exists in the scene,u7 create a new GameObject and attach the singleton script
                    GameObject singletonObject = new GameObject("GameManager");
                    instance = singletonObject.AddComponent<GameManager>();
                }
            }

            return instance;
        }
    }
}
