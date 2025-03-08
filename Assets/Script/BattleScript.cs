using UnityEngine; 
using System.Collections;
using UnityEngine.UIElements;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;

public enum GameState {
    START, PLAYERTURN, ENEMYTURN, WAITING, WON, LOST 
}

public class BattleScript : MonoBehaviourPunCallbacks {

    public static BattleScript Instance; 
    public GameState state;
    public BaseMonster myMonster; 
    public BaseMonster enemyMonster;
    public User player;
    public User enemy;
    public MoveSet selectedMove;
    private bool isMyTurn; 

    public GameObject player1; 
    public GameObject player2; 
    public Transform player1Position; 
    public Transform player2Position; 
    User UnitPlayer1;
    User UnitPlayer2;
    public GameObject monsterAPrefab;
    public GameObject monsterBPrefab;
    public BattleUI player1UI; 
    public BattleUI player2UI; 
    public TMPro.TextMeshProUGUI turnIndicator;
    public UnityEngine.UI.Button endTurnButton;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start(){
        isMyTurn = PhotonNetwork.IsMasterClient;
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_SyncTurn", RpcTarget.Others, 1);
        }
    
        if (PhotonNetwork.IsMasterClient)
        {
            state = GameState.PLAYERTURN; 
        }
        else
        {
            state = GameState.ENEMYTURN; 
        }
        //state = GameState.START; 

        StartCoroutine(SetupBattle()); 
    }


    IEnumerator SetupBattle(){
        // GameObject Player1Obj = new GameObject("Player")
        //UnitPlayer1 = player1.GetComponent<User>(); 
        //UnitPlayer2 = player2.GetComponent<User>(); 
        // GameObject monster1Obj = Instantiate(monsterAPrefab, player1Position.position, Quaternion.identity);
        // GameObject monster2Obj = Instantiate(monsterBPrefab, player2Position.position, Quaternion.identity);  

        player = player1.GetComponent<User>();
        enemy = player2.GetComponent<User>();

        if (player == null || enemy == null){
            Debug.LogError("Player object null in Player.cs"); 
            yield break; 
        }
        // how to set GameOBJ player to class Player  
        yield return new WaitForSeconds(2f); 
        
        //BaseMonster monster1 = null; 
        //BaseMonster monster2 = null; 
        // BaseMonster monster1 = monster1Obj.GetComponent<BaseMonster>();
        // BaseMonster monster2 = monster2Obj.GetComponent<BaseMonster>();

        if (myMonster == null) {
            GameObject monster1Obj = Instantiate(monsterAPrefab, player1Position.position, Quaternion.identity);
            //monster1 = monster1Obj.GetComponent<BaseMonster>();
            //UnitPlayer1.addMonster(monster1);
            myMonster = monster1Obj.GetComponent<BaseMonster>();
        }
        //else{
        //    monster1 = UnitPlayer1.PlayerMonster; 
        //}
        if (enemyMonster == null) {
            GameObject monster2Obj = Instantiate(monsterBPrefab, player2Position.position, Quaternion.identity); // maybe call card manager?? 
            //monster2 = monster2Obj.GetComponent<BaseMonster>();
            //UnitPlayer2.addMonster(monster2);
            enemyMonster = monster2Obj.GetComponent<BaseMonster>();
        }
        //else{
        //    monster2 = UnitPlayer2.PlayerMonster; 
        //}
        //if (UnitPlayer1.PlayerMonster == null && UnitPlayer1.MonsterList.Count > 0) {
        //    UnitPlayer1.PlayerMonster = UnitPlayer1.MonsterList[0];
        //}
        //if (UnitPlayer2.PlayerMonster == null && UnitPlayer2.MonsterList.Count > 0) {
        //    UnitPlayer2.PlayerMonster = UnitPlayer2.MonsterList[0];
        //}

        // BaseMonster monster1 = Player.
        // monster1.data = new MonsterData(); 
        // monster2.data = new MonsterData(); 
        //monster1.data = Resources.Load<MonsterData>("Baldy"); 
        //monster2.data = Resources.Load<MonsterData>("Vulcano");

        //if (monster1.data == null || monster2.data == null) {
        //    Debug.LogError("MonsterData is NULL! Make sure MonsterData is in a Resources folder.");
        //} else {
        //    Debug.Log($"Assigned {monster1.data.monsterName} to Player 1's monster.");
        //    Debug.Log($"Assigned {monster2.data.monsterName} to Player 2's monster.");
        //}
        //monster1._currHP = monster1.data.maxHP; 
        //monster2._currHP = monster2.data.maxHP; 

        //UnitPlayer1.addMonster(monster1);
        //UnitPlayer2.addMonster(monster2);

        //Debug.Log($"Before Assignment: UnitPlayer1.MonsterList Count = {UnitPlayer1.MonsterList.Count}");
        //Debug.Log($"Before Assignment: UnitPlayer2.MonsterList Count = {UnitPlayer2.MonsterList.Count}");
        //if (UnitPlayer1.MonsterList.Count > 0) {
        //    UnitPlayer1.PlayerMonster = UnitPlayer1.MonsterList[0];
        //} else {
        //    Debug.LogError("UnitPlayer1.MonsterList is EMPTY when trying to assign PlayerMonster!");
        //}

        //if (UnitPlayer2.MonsterList.Count > 0) {
        //    UnitPlayer2.PlayerMonster = UnitPlayer2.MonsterList[0];
        //} else {
        //    Debug.LogError("UnitPlayer2.MonsterList is EMPTY when trying to assign PlayerMonster!");
        //}
        //// UnitPlayer1.PlayerMonster = UnitPlayer1.MonsterList[0];
        //// UnitPlayer2.PlayerMonster = UnitPlayer2.MonsterList[0];

        //player1UI.SetupUI(UnitPlayer1.PlayerMonster, UnitPlayer1); 
        //player2UI.SetupUI(UnitPlayer2.PlayerMonster, UnitPlayer2); 

        player1UI.SetupUI(myMonster, player);
        player2UI.SetupUI(enemyMonster, enemy);

        state = GameState.PLAYERTURN;
        UpdateTurn();
        // player1Turn(); 
        yield return null; 
        
    }

    public void AssignPlayerCreature(BaseMonster creature, string creatureaName)
    {
        myMonster = creature;
        player1UI.SetupUI(myMonster, player);
    }

    public void AssignOpponentCreature(BaseMonster creature, string creatureaName)
    {
        myMonster = creature;
        player1UI.SetupUI(myMonster, player);
    }
    public void AttackBtn1(){
        if(state != GameState.PLAYERTURN){
            return; 
        }
        // StartCoroutine(PlayerAttack(UnitPlayer1, UnitPlayer2, chosenMove)); 

    }
    public void ExecuteMove(MoveSet chosenMove){
        if(!isMyTurn)
        {
            return;
        }
        if(chosenMove != null)
        {
            //if (!UnitPlayer1.costActionPoints(chosenMove.APCost))
            //{
            //    Debug.Log("Not enough AP to use this move!");
            //    return;
            //}
            bool isDead = chosenMove.Execute(player, enemy, myMonster, enemyMonster);
            photonView.RPC("RPC_ApplyDamage", RpcTarget.All, enemyMonster._currHP);
            if (isDead)
            {
                Debug.Log($"{enemyMonster.data.monsterName} has been defeated!");
                EndBattle();
            }
            else
            {
                // Change turns
                isMyTurn = false;
                photonView.RPC("RPC_SyncTurn", RpcTarget.All, 0);
            }
        }
    }
    [PunRPC]
    void RPC_ApplyDamage(int newHP)
    {
        enemyMonster._currHP = newHP;
    }

    [PunRPC]
    void RPC_SyncTurn(int newTurn)
    {
        isMyTurn = (newTurn == 1);
    }
    //IEnumerator PlayerUseMove(User user, User opponent, MoveSet chosenMove)
    //{
    //    Debug.Log($"[PlayerUseMove] Called by: {user.gameObject.name}");

    //    if (state != GameState.PLAYERTURN && state != GameState.ENEMYTURN)
    //    {
    //        yield break;
    //    }

    //    BaseMonster attacker = user.PlayerMonster;
    //    BaseMonster target = opponent.PlayerMonster;

    //    bool isTargetDefeated = chosenMove.Execute(user, opponent, attacker, target);

    //    player1UI.UpdateHPSlider(UnitPlayer1.PlayerMonster._currHP);
    //    player2UI.UpdateHPSlider(UnitPlayer2.PlayerMonster._currHP);

    //    yield return new WaitForSeconds(1f);

    //    if (isTargetDefeated)
    //    {
    //        if (state == GameState.PLAYERTURN)
    //        {
    //            state = GameState.WON;
    //        }
    //        else
    //        {
    //            state = GameState.LOST;
    //        }
    //        EndBattle();
    //    }
    //    else
    //    {
    //        UpdateTurn();
    //    }

    //}
    IEnumerator PlayerUseMove(User user, User opponent, MoveSet chosenMove)
    {
        Debug.Log($"[PlayerUseMove] Called by: {user.gameObject.name}");

        // Ensure only the correct player executes moves
        if (!isMyTurn || state != GameState.PLAYERTURN && state != GameState.ENEMYTURN)
        {
            Debug.LogWarning("Not your turn or invalid game state!");
            yield break;
        }

        BaseMonster attacker = myMonster;
        BaseMonster target = enemyMonster;

        if (chosenMove == null || attacker == null || target == null)
        {
            Debug.LogError("Move execution failed: Missing data!");
            yield break;
        }

        // Deduct action points before executing move
        if (!user.costActionPoints(chosenMove.APCost))
        {
            Debug.Log("Not enough AP to use this move!");
            yield break;
        }

        // Execute move
        bool isTargetDefeated = chosenMove.Execute(user, opponent, attacker, target);

        // Sync HP across both players
        photonView.RPC("RPC_UpdateHP", RpcTarget.All, attacker._currHP, target._currHP);

        yield return new WaitForSeconds(1f);

        if (isTargetDefeated)
        {
            state = (state == GameState.PLAYERTURN) ? GameState.WON : GameState.LOST;
            EndBattle();
        }
        else
        {
            // Switch turns
            isMyTurn = !isMyTurn;
            photonView.RPC("RPC_SyncTurn", RpcTarget.All, isMyTurn ? 1 : 0);
        }
    }

    // RPC to update HP across the network
    [PunRPC]
    void RPC_UpdateHP(float attackerHP, float targetHP)
    {
        player1UI.UpdateHPSlider(myMonster._currHP);
        player2UI.UpdateHPSlider(enemyMonster._currHP);
    }



    public void UpdateTurn(){
        if (PhotonNetwork.IsMasterClient)
        {
            state = GameState.ENEMYTURN;
            //UnitPlayer1._AP = Mathf.Min(UnitPlayer1._AP + 4, 6); 
        }
        else
        {
            state = GameState.PLAYERTURN; 
            //UnitPlayer2._AP = Mathf.Min(UnitPlayer2._AP + 4, 6);
        }
        photonView.RPC("RPC_SyncAP", RpcTarget.All, player._AP, enemy._AP);
        photonView.RPC("SyncTurn", RpcTarget.Others, (int)state);
    }

    [PunRPC]
    //void SyncTurn(int newState)
    //{
    //    state = (GameState)newState;
    //}

    public void EndBattle(){
        foreach(UnityEngine.UI.Button btn in player1UI.moveBtn) {
            btn.interactable = false; 
        }
        foreach(UnityEngine.UI.Button btn in player2UI.moveBtn){
            btn.interactable = false; 
        }
        if (state == GameState.WON){
            turnIndicator.text = "Player 1 Wins"; 
        }
        else if (state == GameState.LOST){
            turnIndicator.text = "Player 2 Wins"; 
        }
        // return to somewhere 
    }
    //void RPC_SyncAP(int player1AP, int player2AP)
    //{
    //    UnitPlayer1._AP = player1AP;
    //    UnitPlayer2._AP = player2AP;
    //}
    public void UpdateTurnUI(){
        foreach (UnityEngine.UI.Button btn in player1UI.moveBtn)
            btn.interactable = (state == GameState.PLAYERTURN);
        foreach (UnityEngine.UI.Button btn in player2UI.moveBtn)
            btn.interactable = (state == GameState.ENEMYTURN);
    }
    void SetMoveBtn(BattleUI ui, bool set){
        foreach(UnityEngine.UI.Button btn in ui.moveBtn){
            btn.interactable = set; 
        }
    }
    
}