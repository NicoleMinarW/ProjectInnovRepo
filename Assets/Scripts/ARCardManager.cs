using UnityEngine;
using Photon.Pun;
using Vuforia;
using TMPro;
using System.Collections.Generic;

public class ARCardManager : MonoBehaviourPunCallbacks
{
    public static ARCardManager Instance;
    public GameObject gameUI, startUI;
    private string assignedCardID;
    private bool isLocked = false;
    private void Awake()
    {
        // PhotonNetwork.AutoCleanUpPlayerObjects = false;
        if (Instance == null)
            Instance = this;
        
    }
    void Start()
    {
        // Get all ObserverBehaviour components in children (to handle multiple image targets)
        ObserverBehaviour[] observerBehaviours = GetComponentsInChildren<ObserverBehaviour>();

        if (observerBehaviours.Length == 0)
        {
            Debug.LogError("No ObserverBehaviours found! Ensure Image Targets are correctly attached.");
            return;
        }

        int observerCount = 0;

        foreach (var observer in observerBehaviours)
        {
            if (observer != null)
            {
                Debug.Log($"ObserverBehaviour found: {observer.TargetName}");
                observer.OnTargetStatusChanged += OnARCardDetected;
                observerCount++;
                Debug.Log($"there are {observerCount} observer count");
            }
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

        AssignCard(targetName, behaviour);
    }

    public void AssignCard(string cardID, ObserverBehaviour behaviour)
    {
        if (isLocked) {
            return;
        }
        assignedCardID = cardID;
        Debug.Log($"Card {cardID} assigned to {PhotonNetwork.NickName}");

        BattleScriptManager.Instance.RegisterPlayer(PhotonNetwork.LocalPlayer, cardID, behaviour);
    }
    public void StartGame()
    {
        Debug.Log("StartGame called");
        if (string.IsNullOrEmpty(assignedCardID)) {
            return;
        }

        isLocked = true;
        Debug.Log($"{PhotonNetwork.NickName} has locked in {assignedCardID}");
        startUI.SetActive(false);
        gameUI.SetActive(true);
        BattleScriptManager.Instance.PlayerReady(PhotonNetwork.LocalPlayer);

    }

    public Transform GetTrackedCardTransform(string cardID)
    {
        ObserverBehaviour[] observerss = GetComponentsInChildren<ObserverBehaviour>();
        foreach(var observer in observerss)
        {
            if (observer.TargetName == cardID && observer.TargetStatus.Status == Status.TRACKED)
            {
                return observer.transform;
            }
        }
        return null; 
    }
    private void OnDestroy()
    {
        Debug.Log("Monster instance destroyed: " + gameObject.name);
    }

}
