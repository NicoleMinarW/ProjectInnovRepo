using UnityEngine;
using Photon.Pun;
using Vuforia;
using TMPro;
using System.Collections.Generic;

public class ARCardManager : MonoBehaviourPunCallbacks
{
    public static ARCardManager Instance;
    public GameObject startButton;
    private string assignedCardID;
    private bool isLocked = false;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }    
    void Start()
    {
        // Try to get ObserverBehaviour component attached to this GameObject
        var observerBehaviour = GetComponentInChildren<ObserverBehaviour>();

        if (observerBehaviour == null)
        {
            Debug.LogError("ObserverBehaviour NOT found! Ensure it is correctly attached to the Image Target.");
            observerBehaviour = GetComponentInChildren<ObserverBehaviour>(); 
            Debug.Log("ObserverBehaviour found in children.");

        }
        
        if (observerBehaviour != null)
        {
            Debug.Log("ObserverBehaviour found and event subscribed!");
            observerBehaviour.OnTargetStatusChanged += OnARCardDetected;
        }

    }


    private void OnARCardDetected(ObserverBehaviour behaviour, TargetStatus status)
    {
        Debug.Log($"OnARCardDetected triggered! Target: {behaviour.TargetName}, Status: {status.Status}");
        // Check if the target is tracked and no creature has been spawned yet
        if (status.Status != Status.TRACKED || !PhotonNetwork.IsConnected)
        {
            Debug.Log("Target not tracked or Photon not connected.");
            return;
        }
        string targetName = behaviour.TargetName; 
        Debug.Log("Card Detected: " + targetName);
        AssignCard(targetName);
    }
    public void AssignCard(string cardID)
    {
        if (isLocked) {
            return;
        }
        assignedCardID = cardID;
        Debug.Log($"Card {cardID} assigned to {PhotonNetwork.NickName}");


        BattleScriptManager.Instance.RegisterPlayer(PhotonNetwork.LocalPlayer, cardID);
    }
    public void StartGame()
    {
        Debug.Log("StartGame called");
        if (string.IsNullOrEmpty(assignedCardID)) {
            return;
        }
        isLocked = true;
        Debug.Log($"{PhotonNetwork.NickName} has locked in {assignedCardID}");
        startButton.SetActive(false);
        BattleScriptManager.Instance.PlayerReady(PhotonNetwork.LocalPlayer);

    }

}
