using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public static MultiplayerManager Instance;
    public bool isMyTurn = false;

    void Awake()
    {
        Instance = this;
        PhotonNetwork.AutomaticallySyncScene = true; // Sync scene for all players
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // Connect to Photon Master Server
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    // When the player joins the lobby, create or join a room
    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom("BattleRoom", new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
    }

    // When the player joins the room, assign turns
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            isMyTurn = true; // First player starts
        }
    }

    // Switch turns using an RPC
    [PunRPC]
    public void EndTurn()
    {
        isMyTurn = !isMyTurn;
    }

    // Call this method to end your turn
    public void OnEndTurnButton()
    {
        if (isMyTurn)
        {
            photonView.RPC("EndTurn", RpcTarget.All);
        }
    }
}