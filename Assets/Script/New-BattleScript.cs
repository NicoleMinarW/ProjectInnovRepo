using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Data.Common;

public enum GameState {
    START, PLAYERTURN, ENEMYTURN, WON, LOST 
}

public class BattleScriptManager : MonoBehaviourPunCallbacks {
    public static BattleScriptManager Instance;
    public GameState state;
    public BaseMonster myMonster;
    public BaseMonster enemyMonster;
    public User player;
    public User enemy;
    private bool isMyTurn;

    public Transform player1Position;
    public Transform player2Position;
    List<GameObject> monsterPrefabs;

    public BattleUI player1UI;
    public BattleUI player2UI;
    public TMPro.TextMeshProUGUI turnIndicator;
    public UnityEngine.UI.Button endTurnButton;
    private Dictionary<string, GameObject> creatureDictionary;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }
    private void InitializeCreatureDictionary() {
        creatureDictionary = new Dictionary<string, GameObject>();
        foreach (var prefab in monsterPrefabs) {
            creatureDictionary[prefab.name] = prefab;
        }
    }
    public void RegisterPlayer(Player player, string cardID) {
        if (!creatureDictionary.ContainsKey(cardID)) {
            Debug.LogError("Invalid card ID: " + cardID);
            return;
        }

        if (PhotonNetwork.LocalPlayer == player) {
            GameObject monsterObj = Instantiate(creatureDictionary[cardID], player1Position.position, Quaternion.identity);
            myMonster = monsterObj.GetComponent<BaseMonster>();
            player1UI.SetupUI(myMonster, this.player);
        } 
        else {
            GameObject monsterObj = Instantiate(creatureDictionary[cardID], player2Position.position, Quaternion.identity);
            enemyMonster = monsterObj.GetComponent<BaseMonster>();
            player2UI.SetupUI(enemyMonster, this.enemy);
        }
    }

    public void StartBattle() {
        if(PhotonNetwork.IsMasterClient) {
            state = GameState.PLAYERTURN;
            isMyTurn = true;
            turnIndicator.text = "Your Turn!";
        } 
        else {
            state = GameState.ENEMYTURN;
            isMyTurn = false;
            turnIndicator.text = "Enemy's Turn!";
        }
        photonView.RPC("RPC_SyncTurn", RpcTarget.Others, state);
    }

    public void ExecuteMove(MoveSet chosenMove) {
        if (!isMyTurn || chosenMove == null) return;
        
        bool isDead = chosenMove.Execute(player, enemy, myMonster, enemyMonster);
        photonView.RPC("RPC_UpdateHP", RpcTarget.All, enemyMonster._currHP);
        
        if (isDead) {
            state = GameState.WON;
            photonView.RPC("RPC_EndBattle", RpcTarget.All, true);
        } else {
            isMyTurn = false;
            photonView.RPC("RPC_SyncTurn", RpcTarget.All, GameState.ENEMYTURN);
        }
    }

    public void EndTurn() {
        if (!isMyTurn) return;
        isMyTurn = false;
        photonView.RPC("RPC_SyncTurn", RpcTarget.All, isMyTurn ? GameState.PLAYERTURN : GameState.ENEMYTURN);
    }

    [PunRPC]
    void RPC_SyncTurn(GameState newState) {
        state = newState;
        if(state == GameState.PLAYERTURN && PhotonNetwork.IsMasterClient || state == GameState.ENEMYTURN && !PhotonNetwork.IsMasterClient) { 
            isMyTurn = true;
        }
        else{
            isMyTurn = false;
        }
        endTurnButton.interactable = isMyTurn;
    }

    [PunRPC]
    void RPC_UpdateHP(int newHP) {
        enemyMonster._currHP = newHP;
        player2UI.UpdateHPSlider(newHP);
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
    }
}
