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
using Photon.Pun.Demo.PunBasics;

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
    public void RegisterPlayer(Player player, string cardID, ObserverBehaviour behaviour)
    {
        
        if (!creatureDictionary.ContainsKey(cardID))
        {
            Debug.LogError("Invalid card ID: " + cardID);
            return;
        }

        Transform cardTransform = ARCardManager.Instance.GetTrackedCardTransform(cardID);

        if (cardTransform == null)
        {
            Debug.LogError("AR Card not found: " + cardID);
            return;
        }

        if (creatureSpawned)
        {
            Debug.LogWarning("A monster is already assigned to this card.");
            return;
        }

        creatureSpawned = true;

        Debug.Log("Registering player: " + player.NickName);

        Debug.Log($"instantiating............ {creatureDictionary[cardID].name}");
        Component[] listOfComponents= creatureDictionary[cardID].GetComponents<Component>(); 
        foreach (Component component in listOfComponents){
            Debug.Log("----->>>> "+ component.GetType().Name); 
        }


        // Spawn player's own creature on top of their card
        GameObject monsterObj = PhotonNetwork.Instantiate(
            creatureDictionary[cardID].name,
            cardTransform.position, 
            cardTransform.rotation
        );
        Debug.Log($"!!!!! Spawned monster with game object name: {monsterObj.name}");
        // Debug.Log($"Monster spawned: {monsterObj.name}, Owner: {monsterObj.GetComponent<PhotonView>().Owner.NickName}, IsMine: {monsterObj.GetComponent<PhotonView>().IsMine}");

        // Attach to AR Card
        AnchorBehaviour anchor = behaviour.GetComponent<AnchorBehaviour>();
        if (anchor != null)
        {
            monsterObj.transform.SetParent(anchor.transform);
        }
        else
        {
            monsterObj.transform.SetParent(behaviour.transform); // Fallback
        }
        PhotonView view = monsterObj.GetComponent<PhotonView>();
        if (view == null) Debug.LogError("PhotonView is missing on spawned Batlamandr!");
        // Assign creature data
        BaseMonster newMonster = monsterObj.GetComponent<BaseMonster>();
        newMonster.data = creatureDictionary[cardID].GetComponent<BaseMonster>().data;
        Debug.Log($"!!!!!!!! Monster has been found, assigned to the current player: {newMonster.data.monsterName}");

        userplayer = new User(player, player.NickName, newMonster);
        myMonster = newMonster;
        Debug.Log($"!!!!!!!Current myMonster: {myMonster.data.monsterName}");

        // Send this player's card position to the opponent
        photonView.RPC("RPC_SetEnemyMonster", RpcTarget.Others, cardID, cardTransform.position, cardTransform.rotation, monsterObj.GetComponent<PhotonView>().ViewID);
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
        myMonster.AttackAnimation();
        photonView.RPC("RPC_TrigGetHit", RpcTarget.Others);
        playerUI.UpdatePlayerHPSlider(myMonster._currHP);
        playerUI.UpdateEnemyHPSlider(enemyMonster._currHP);
        
        if (isDead) {
            state = GameState.WON;
            photonView.RPC("RPC_EndBattle", RpcTarget.All);
        } 
        
        playerUI.UpdateAPDisplay(userplayer._AP);
        photonView.RPC("RPC_SyncMonstersHP", RpcTarget.Others, enemyMonster._currHP, myMonster._currHP);
    }

    public void ExecuteSP(SpecialAttack chosenSP){
        if (!isMyTurn || chosenSP == null) return;
        if(myMonster._isOnCooldown == false && myMonster._isOngoing == false){
            bool isDead = chosenSP.ApplyEffect(userplayer, enemyplayer, myMonster, enemyMonster);
            if(isDead){
                state = GameState.WON; 
                photonView.RPC("RPC_EndBattle", RpcTarget.All);
            }
        }
        
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
        if(myMonster._defenseOn){
            myMonster._defenseOn = checkDefense(userplayer.userTurnCount, myMonster._Def_endDuration);
            photonView.RPC("RPC_syncDef", RpcTarget.Others, myMonster._defense);
        }
        runThroughSP(myMonster);
        Debug.Log("Ending turn"); 
        photonView.RPC("RPC_SyncTurn", RpcTarget.All, state);

    }

    public bool checkDefense(int currTurn, int lastTurn){
        if(currTurn <= lastTurn){
            myMonster._defense = 0;
            return false; 
        }
        return true; 
    }
    public void runThroughSP(BaseMonster monster){
        if (monster._isOnCooldown){
            monster.tickDownCD(monster);
        }
        if (monster._isOngoing){
            monster.tickDownDuration(monster);
        }
        playerUI.UpdateSPButton(myMonster);
    }

    public void RestartGame()
    {
        PhotonNetwork.LoadLevel(PhotonNetwork.CurrentRoom.PlayerCount);  
    }

    public void QuitGame()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }


    [PunRPC]
    void RPC_SetEnemyMonster(string cardID, Vector3 position, Quaternion rotation, int enemyCreatureID)
    {
        Debug.Log($"RPC_SetEnemyMonster: cardID={cardID}, position={position}, rotation={rotation}, enemyCreatureID={enemyCreatureID}");
        if (!creatureDictionary.ContainsKey(cardID))
        {
            Debug.LogError("Invalid card ID received in RPC_SetEnemyMonster: " + cardID);
            return;
        }

        // Find the opponent’s monster using PhotonView ID
        PhotonView enemyView = PhotonView.Find(enemyCreatureID);
        if (enemyView == null)
        {
            Debug.LogError("Enemy monster PhotonView not found!");
            return;
        }

        GameObject enemyMonsterPrefab = enemyView.gameObject;
        enemyMonster = enemyMonsterPrefab.GetComponent<BaseMonster>();
        enemyMonster.data = enemyMonsterPrefab.GetComponent<BaseMonster>().data;

        if (enemyMonster == null)
        {
            Debug.LogError("Enemy monster component not found!");
            return;
        }

        Debug.Log($"Setting enemy monster on card {cardID}");

        // Assign opponent monster data

        Debug.Log($"{PhotonNetwork.PlayerListOthers}");
        enemyplayer = new User(PhotonNetwork.PlayerListOthers[0], PhotonNetwork.PlayerListOthers[0].NickName, enemyMonster);
        //enemyplayer.assignUser(enemyplayer, PhotonNetwork.PlayerListOthers[0], PhotonNetwork.PlayerListOthers[0].NickName, enemyMonster);


        Debug.Log($"Enemy monster {enemyMonster.name}");
        Debug.Log($"original location {position}");

        Vector3 newEnemyPosition = position + rotation * new Vector3(0, 0, 1);
        enemyMonsterPrefab.transform.position = newEnemyPosition;

        Vector3 direction = position - newEnemyPosition;
        direction.y = 0;
        enemyMonsterPrefab.transform.rotation = Quaternion.LookRotation(direction);

        Debug.Log($"Repositioned enemy creature at {newEnemyPosition}");
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
        playerUI.SetupUI(myMonster, enemyMonster, this.userplayer, this.enemyplayer);
    }
    [PunRPC]
    public void RPC_TrigGetHit(){
        myMonster.GetHitAnimation();
    }
}




// as 2 people who 3 weeks ago did not know about Photon PUN2 and Vuforia 
// we are proud of what we have accomplished 
// (ﾉ◕ヮ◕)ﾉ*:･ﾟ✧

// sincerely, exchange students x2