using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;

public enum GameState {
    START, PLAYERTURN, ENEMYTURN, WON, LOST 
}

public class BattleScriptManager : MonoBehaviourPunCallbacks {
    public static BattleScriptManager Instance;
    public GameState state;
    public BaseMonster myMonster;
    public BaseMonster enemyMonster;
    public User userplayer;
    public User enemyplayer;
    private bool isMyTurn;
    private bool isPlayer1Ready = false;
    private bool isPlayer2Ready = false;


    public List<GameObject> monsterPrefabs;

    public UIManager playerUI;

    public TMPro.TextMeshProUGUI turnIndicator;
    public GameObject endTurnButton;
    private Dictionary<string, GameObject> creatureDictionary;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }
    public void Start(){
        InitializeCreatureDictionary();
    }

    public void PlayerReady(Player player) {
        if (player == PhotonNetwork.LocalPlayer) {
            if (PhotonNetwork.IsMasterClient) {
                isPlayer1Ready = true;
                photonView.RPC("RPC_UpdatePlayerReadyState", RpcTarget.Others, true);
            } else {
                isPlayer2Ready = true;
                photonView.RPC("RPC_UpdatePlayerReadyState", RpcTarget.Others, false);
            }
        }

        if (isPlayer1Ready && isPlayer2Ready) {
            StartBattle();
        }
    }

    private void InitializeCreatureDictionary() {
        Debug.Log("Initializing creature dictionary");
        creatureDictionary = new Dictionary<string, GameObject>();
        foreach (var prefab in monsterPrefabs) {
            Debug.Log("Adding prefab: " + prefab.name);
            creatureDictionary[prefab.name] = prefab;
        }

    }
    public void RegisterPlayer(Player player, string cardID) {
        if (!creatureDictionary.ContainsKey(cardID)) {
            Debug.LogError("Invalid card ID: " + cardID);
            return;
        }
        GameObject arCard = GameObject.Find(cardID);
        if (arCard == null) {
            Debug.LogError("AR Card not found: " + cardID);
            return;
        }
        GameObject monsterObj = Instantiate(creatureDictionary[cardID], arCard.transform.position, Quaternion.identity);
        monsterObj.transform.SetParent(arCard.transform);
        BaseMonster newMonster = monsterObj.GetComponent<BaseMonster>();
        newMonster.data = creatureDictionary[cardID].GetComponent<BaseMonster>().data; 
        userplayer = new User(player, player.NickName, newMonster);
        myMonster = newMonster; 
        // if (PhotonNetwork.LocalPlayer == player) {
        //     myMonster = newMonster;
        //     photonView.RPC("RPC_SetEnemyMonster", RpcTarget.OthersBuffered, cardID);
        // } 
        PhotonView pv = monsterObj.GetComponent<PhotonView>();
        // if (pv==null){
        //     pv = monsterObj.AddComponent<PhotonView>();
        // }
        // pv.ViewID = PhotonNetwork.AllocateViewID();
        photonView.RPC("RPC_SetEnemyMonster", RpcTarget.Others, cardID); // it's either RpcTarget.Others or RpcTarget.OthersBuffered
        Debug.Log($"Monster with ID {cardID} attached to {player.NickName}");
    }

    public void StartBattle()
    {
        if (!PhotonNetwork.IsMasterClient) {
            return;
        }

        Debug.Log("Starting Battle (BattleScript)");

        // Ensure photonView is not null
        if (photonView == null)
        {
            Debug.LogError("PhotonView is missing on BattleScriptManager!");
            return;
        }

        // Ensure turnIndicator is not null
        if (turnIndicator == null)
        {
            Debug.LogError("turnIndicator is not assigned!");
            return;
        }

        // Set initial game state based on MasterClient
        state = GameState.PLAYERTURN;
        isMyTurn = true;
        turnIndicator.text = "Player1 Turn!";
        endTurnButton.SetActive(isMyTurn);

        photonView.RPC("RPC_SyncTurn", RpcTarget.Others, state);

    }


    public void ExecuteMove(MoveSet chosenMove) {
        if (!isMyTurn || chosenMove == null) return;
        
        bool isDead = chosenMove.Execute(userplayer, enemyplayer, myMonster, enemyMonster);
        playerUI.UpdateEnemyHPSlider(enemyMonster._currHP);
        
        if (isDead) {
            state = GameState.WON;
            photonView.RPC("RPC_EndBattle", RpcTarget.All, true);
        } else {
            //isMyTurn = false;
            //photonView.RPC("RPC_SyncTurn", RpcTarget.All, GameState.ENEMYTURN);
        }
        photonView.RPC("RPC_SyncMonsters", RpcTarget.Others, enemyMonster, myMonster);
        playerUI.UpdateAPDisplay(userplayer._AP);
    }

    public void EndTurn() {
        Debug.Log("End Turn Button Clicked");
        if (isMyTurn == false) {
            Debug.Log("It is not your turn"); 
            return;
        }
        // isMyTurn = false;
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     state = (state == GameState.PLAYERTURN) ? GameState.ENEMYTURN : GameState.PLAYERTURN;
        //     Debug.Log("MasterClient switching turn to: " + state);
        //     photonView.RPC("RPC_SyncTurn", RpcTarget.All, state); // Sync turn for all players
        // }
        if(state == GameState.PLAYERTURN){
            state = GameState.ENEMYTURN;
        }
        else if (state == GameState.ENEMYTURN){
            state = GameState.PLAYERTURN;
        }
        Debug.Log("Ending turn"); 
        photonView.RPC("RPC_SyncTurn", RpcTarget.All, state);

    }

    [PunRPC]
    void RPC_SetEnemyMonster(string cardID) {
        if (!creatureDictionary.ContainsKey(cardID)) {
            Debug.LogError("Invalid card ID received in RPC_SetEnemyMonster: " + cardID);
            return;
        }
        GameObject arCard = GameObject.Find(cardID);
        GameObject monsterObj = Instantiate(creatureDictionary[cardID], arCard.transform.position, Quaternion.identity);
        enemyMonster = monsterObj.GetComponent<BaseMonster>();
        enemyplayer = new User(PhotonNetwork.PlayerListOthers[0], PhotonNetwork.PlayerListOthers[0].NickName, enemyMonster);
        Debug.Log($"Enemy monster {enemyMonster.name}");
        playerUI.SetupUI(myMonster, enemyMonster, this.userplayer, this.enemyplayer);
    }

    [PunRPC]
    void RPC_SyncTurn(GameState newState)
    {
        if (turnIndicator == null)
        {
            Debug.LogError("Turn Indicator is not assigned!");
            return;
        }

        state = newState;
        Debug.Log($"RPC_SyncTurn: State changed to {state}");

        // Correctly set turns for each player
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("MasterClient setting turn for all players");
            isMyTurn = (state == GameState.PLAYERTURN);
            turnIndicator.text = isMyTurn ? "Player 1 Turn" : "Player 2 Turn";
        }
        else
        {
            Debug.Log("Non-MasterClient setting turn for all players");
            isMyTurn = (state == GameState.ENEMYTURN);
            turnIndicator.text = isMyTurn ? "Player 1 Turn" : "Player 2 Turn";
        }

        endTurnButton.SetActive(isMyTurn);
        Debug.Log($"RPC_SyncTurn: isMyTurn set to {isMyTurn}");
    }

    [PunRPC]
    void RPC_EndBattle(bool playerWon) {
        if(playerWon == true) {
            state = GameState.WON;
            turnIndicator.text = "You Win!";
        } else{
            state = GameState.LOST;
            turnIndicator.text = "You Lose!";
        };
        isPlayer1Ready = false; 
        isPlayer2Ready = false;
    }
    [PunRPC]
    public void RPC_UpdatePlayerReadyState(bool isPlayer1)
    {
        if (isPlayer1)
        {
            isPlayer1Ready = true;
        }
        else
        {
            isPlayer2Ready = true;
        }

        if (isPlayer1Ready && isPlayer2Ready)
        {
            StartBattle();
        }
    }
    [PunRPC]
    public void RPC_SyncMonsters(BaseMonster p1, BaseMonster p2)
    {
        myMonster._currHP = p1._currHP;
        enemyMonster._currHP = p2._currHP;
        playerUI.UpdatePlayerHPSlider(myMonster._currHP);
        playerUI.UpdateEnemyHPSlider(enemyMonster._currHP);
        
    }
}
