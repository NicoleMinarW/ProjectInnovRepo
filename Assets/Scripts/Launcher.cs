using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.InputSystem.LowLevel;
using Photon.Realtime;
using System.Collections.Generic;
using System;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    [SerializeField] TMPro.TMP_InputField roomNameInputField;
    [SerializeField] TMPro.TextMeshProUGUI errorText;
    [SerializeField] TMPro.TextMeshProUGUI roomNameText;
    [SerializeField] TMPro.TextMeshProUGUI roomNameCurrentText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;

    void Start()
    {
        Debug.Log("Connecting to Photon Master Server");
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

    }
    void Awake()
    {
        Instance = this;
    }
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
        PhotonNetwork.NickName = "Player " + UnityEngine.Random.Range(0, 1000).ToString("0000");
    }


    public void CreateRoom() 
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
            
        PhotonNetwork.CreateRoom(roomNameInputField.text, new RoomOptions { MaxPlayers = 2 });
        MenuManager.Instance.OpenMenu("loading");

    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room");
        roomNameCurrentText.text = PhotonNetwork.CurrentRoom.Name;


        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("title");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room creation failed: " + message;    
        Debug.LogError("Room creation failed: " + message); 
        MenuManager.Instance.OpenMenu("error");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
            {
                continue;
            }
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItems>().SetUp(roomList[i]);
        }
    }
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
    public void StartGame()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Loading Game Scene...");
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Debug.Log("Main Camera: " + mainCamera);
                mainCamera.gameObject.SetActive(false);
            }

            PhotonNetwork.LoadLevel(1);
        }
        else
        {
            Debug.LogError("Cannot start game! Not in a room.");
        }
    }

}

// Bless the person who made the PUN 2 documentation
// sincerely, the exchange students
