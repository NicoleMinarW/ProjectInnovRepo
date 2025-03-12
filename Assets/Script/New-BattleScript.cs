using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using System;
using Unity.Burst;
using UnityEngine.UIElements;
using Vuforia; 

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
    public TMPro.TextMeshProUGUI turnCountText;
    //private bool instantiatedCharacter = false;
    int Def_endDuration = 0; 
    int Buff_endDuration = 0; 
    bool defenseOn = false; 
    bool buffOn = false;
    private bool creatureSpawned = false;


    public List<GameObject> monsterPrefabs;

    public UIManager playerUI;

    public TMPro.TextMeshProUGUI turnIndicator;
    public GameObject endTurnButton;
    private Dictionary<string, GameObject> creatureDictionary;
    public GameObject gameOverScreen; 
    public TMPro.TextMeshProUGUI gameOverText;

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
                Debug.Log("Player 1 is ready");
            } else {
                isPlayer2Ready = true;
                photonView.RPC("RPC_UpdatePlayerReadyState", RpcTarget.Others, false);
                Debug.Log("Player 2 is ready");
            }
        }

        if (isPlayer1Ready && isPlayer2Ready) {
            photonView.RPC("RPC_InitializeUI", RpcTarget.All);
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
    public void RegisterPlayer(Player player, string cardID, ObserverBehaviour behaviour) {
        if (!creatureDictionary.ContainsKey(cardID)) {
            Debug.LogError("Invalid card ID: " + cardID);
            return;
        }

        Transform cardTransform = ARCardManager.Instance.GetTrackedCardTransform(cardID);

        if (cardTransform == null) {
            Debug.LogError("AR Card not found: " + cardID);
            return;
        }
        if (creatureSpawned == true)
        {
            Debug.LogWarning("A monster is already assigned to this card.");
            return;
        }

        creatureSpawned = true;

        Debug.Log("Registering player: " + player.NickName);

        GameObject monsterObj = PhotonNetwork.Instantiate(creatureDictionary[cardID].name, 
                                                            cardTransform.position, 
                                                            cardTransform.rotation);
        Debug.Log($"Monster spawned: {monsterObj.name}, Owner: {monsterObj.GetComponent<PhotonView>().Owner.NickName}, IsMine: {monsterObj.GetComponent<PhotonView>().IsMine}");
        AnchorBehaviour anchor = behaviour.GetComponent<AnchorBehaviour>();
        monsterObj.transform.SetParent(cardTransform);
        if (anchor != null)
        {
            monsterObj.transform.SetParent(anchor.transform);
        }
        else
        {
            monsterObj.transform.SetParent(behaviour.transform); // Fallback
        }
        
        BaseMonster newMonster = monsterObj.GetComponent<BaseMonster>();
        newMonster.data = creatureDictionary[cardID].GetComponent<BaseMonster>().data; 
        
        userplayer = new User(player, player.NickName, newMonster);
        myMonster = newMonster; 
        
        photonView.RPC("RPC_SetEnemyMonster", RpcTarget.Others, cardID, cardTransform.position, cardTransform.rotation); // it's either RpcTarget.Others or RpcTarget.OthersBuffered
        // photonView.RPC("RPC_SetEnemyMonster", RpcTarget.Others, cardID);
        Debug.Log($"Monster with ID {cardID} attached to {player.NickName}");
    }

    public void StartBattle()
    {
        if (!PhotonNetwork.IsMasterClient) {
            Debug.LogError("Trying to start battle without being the MasterClient!");
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
        if (userplayer._username == null)
        {
            Debug.LogError("Userplayer username is null!");
            return;
        }
        turnIndicator.text = userplayer._username + "'s Turn";
        endTurnButton.SetActive(isMyTurn);
        Debug.Log("Battle started (battlescript)");
        photonView.RPC("RPC_SyncTurn", RpcTarget.Others, state);

    }


    public void ExecuteMove(MoveSet chosenMove) {
        if (!isMyTurn || chosenMove == null) return;
        
        bool isDead = chosenMove.Execute(userplayer, enemyplayer, myMonster, enemyMonster);
        playerUI.UpdatePlayerHPSlider(myMonster._currHP);
        playerUI.UpdateEnemyHPSlider(enemyMonster._currHP);
        
        if (isDead) {
            state = GameState.WON;
            photonView.RPC("RPC_EndBattle", RpcTarget.All);
        } 
        
        playerUI.UpdateAPDisplay(userplayer._AP);
        photonView.RPC("RPC_SyncMonstersHP", RpcTarget.Others, enemyMonster._currHP, myMonster._currHP);
    }

    public void displayGameOver(GameState currentState){
        if (currentState == GameState.WON){
            gameOverText.text = "You WIN!"; 
        }
        else if (currentState == GameState.LOST){
            gameOverText.text = "You LOSE!"; 
        }
        gameOverScreen.SetActive(true);
    }

    public void EndTurn() {
        Debug.Log("End Turn Button Clicked");
        if (isMyTurn == false) {
            Debug.Log("It is not your turn"); 
            return;
        }
        userplayer.refreshAP();
        playerUI.UpdateAPDisplay(userplayer._AP);
        if(state == GameState.PLAYERTURN){
            state = GameState.ENEMYTURN;
        }
        else if (state == GameState.ENEMYTURN){
            state = GameState.PLAYERTURN;
        }
        userplayer.userTurnCount += 1; 
        turnCountText.text = "Turn: " + userplayer.userTurnCount.ToString();
        if(defenseOn){
            checkDefense(userplayer.userTurnCount, Def_endDuration);
            photonView.RPC("RPC_syncDef", RpcTarget.Others, myMonster._defense);
        }
        if (buffOn){
            checkBuff(userplayer.userTurnCount, Buff_endDuration);
            photonView.RPC("RPC_syncBuff", RpcTarget.Others, myMonster._buff);
        }
        Debug.Log("Ending turn"); 
        photonView.RPC("RPC_SyncTurn", RpcTarget.All, state);

    }

    public void setTurnDefense(int currTurns, int duration){
        Def_endDuration = currTurns + duration +1;
        defenseOn = true; 
    }
    public void checkDefense(int currTurn, int lastTurn){
        if(currTurn <= lastTurn){
            myMonster._defense = 0;
            defenseOn = false;
        }
    }
    public void setTurnBuff(int currTurns, int duration){
        Buff_endDuration = currTurns + duration +1;
        buffOn = true; 
    }
    public void checkBuff(int currTurn, int lastTurn){
        if(currTurn <= lastTurn){
            myMonster._buff = 0;
            buffOn = false;
        }
    }


    [PunRPC]
    // void RPC_SetEnemyMonster(string cardID, Vector3 position, Quaternion rotation) {
    void RPC_SetEnemyMonster(string cardID){
    
        if (!creatureDictionary.ContainsKey(cardID)) {
            Debug.LogError("Invalid card ID received in RPC_SetEnemyMonster: " + cardID);
            return;
        }

        GameObject monsterObj = GameObject.Find(creatureDictionary[cardID].name + "(Clone)");
        // GameObject monsterObj = Instantiate(creatureDictionary[cardID], position, rotation);
        // monsterObj.transform.SetParent(cardTransform);
        if(monsterObj == null){
            Debug.LogError("Could not find existing monster for card"+ cardID);
            return; 
            // monsterObj.transform.SetParent(cardTransform);
        }
        Transform cardTransform = ARCardManager.Instance.GetTrackedCardTransform(cardID);
        if (cardTransform == null) {
            Debug.LogError("Card Transform is nULL for: " + cardID);
            return;
        }
        monsterObj.transform.SetParent(cardTransform, true);

        Debug.Log($"Setting enemy monster on card {cardID}");

        // monsterObj.transform.SetParent(arCard.transform);

        enemyMonster = monsterObj.GetComponent<BaseMonster>();
        // enemyMonster = creatureDictionary[cardID].GetComponent<BaseMonster>();
        enemyplayer = new User(PhotonNetwork.PlayerListOthers[0], PhotonNetwork.PlayerListOthers[0].NickName, enemyMonster);
        Debug.Log($"Enemy monster {enemyMonster.name}");
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
            turnIndicator.text = isMyTurn ? (userplayer._username + "'s turn"): (enemyplayer._username + "'s turn");
        }
        else
        {
            Debug.Log("Non-MasterClient setting turn for all players");
            isMyTurn = (state == GameState.ENEMYTURN);
            turnIndicator.text = isMyTurn ? (userplayer._username + "'s turn"): (enemyplayer._username + "'s turn");
        }

        endTurnButton.SetActive(isMyTurn);
        Debug.Log($"RPC_SyncTurn: isMyTurn set to {isMyTurn}");
    }

    [PunRPC]
    public void RPC_EndBattle() {
        if(state != GameState.WON){
            state = GameState.LOST; 
        }
        displayGameOver(state);
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
    public void RPC_SyncMonstersHP(float myHP, float enemyHP)
    {
        myMonster._currHP = myHP; 
        enemyMonster._currHP = enemyHP;
        playerUI.UpdatePlayerHPSlider(myHP);
        playerUI.UpdateEnemyHPSlider(enemyHP);
        
    }
    [PunRPC]
    public void RPC_InitializeUI()
    {
        turnCountText.text = "Turn: " + userplayer.userTurnCount.ToString();
        playerUI.SetupUI(myMonster, enemyMonster, this.userplayer, this.enemyplayer);
    }
    [PunRPC]
    public void RPC_syncDef(int def){
        enemyMonster._defense = def;
    }
    
    [PunRPC]
    public void RPC_syncBuff(int buff){
        enemyMonster._buff = buff;
    }

}   



// as 2 people who 3 weeks ago did not know about Photon PUN2 and Vuforia 
// we are proud of what we have accomplished 
// (ﾉ◕ヮ◕)ﾉ*:･ﾟ✧

// sincerely, exchange students x2