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
        // isMyTurn = PhotonNetwork.IsMasterClient;
        if (PhotonNetwork.IsMasterClient)
        {
            state = GameState.PLAYERTURN; 
            isMyTurn = true; 
            photonView.RPC("RPC_SyncTurn", RpcTarget.Others, 2);
        }
        else{
            state = GameState.ENEMYTURN; 
            isMyTurn = false; 
        }

        StartCoroutine(SetupBattle()); 
    }


    IEnumerator SetupBattle(){
        player = player1.GetComponent<User>();
        enemy = player2.GetComponent<User>();

        if (player == null || enemy == null){
            Debug.LogError("Player object null in Player.cs"); 
            yield break; 
        }
        // how to set GameOBJ player to class Player  
        yield return new WaitForSeconds(2f); 
        
        if (myMonster == null) {
            GameObject monster1Obj = Instantiate(monsterAPrefab, player1Position.position, Quaternion.identity);
            //monster1 = monster1Obj.GetComponent<BaseMonster>();
            //UnitPlayer1.addMonster(monster1);
            myMonster = monster1Obj.GetComponent<BaseMonster>();
        }

        if (enemyMonster == null) {
            GameObject monster2Obj = Instantiate(monsterBPrefab, player2Position.position, Quaternion.identity); // maybe call card manager?? 
            //monster2 = monster2Obj.GetComponent<BaseMonster>();
            //UnitPlayer2.addMonster(monster2);
            enemyMonster = monster2Obj.GetComponent<BaseMonster>();
        }
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
        isMyTurn = (PhotonNetwork.IsMasterClient && newTurn ==1) || (!PhotonNetwork.IsMasterClient && newTurn ==2);
        endTurnButton.interactable = isMyTurn; 
        if (isMyTurn){
            state = GameState.PLAYERTURN; 
        }
        else{
            state=GameState.ENEMYTURN; 
        }
    }

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
    public void EndTurn()
    {
        if (!PhotonNetwork.IsMasterClient) return; // Only Master Client manages turns

        int nextTurn = isMyTurn ? 2 : 1; // Swap turns (1 → 2, 2 → 1)
        photonView.RPC("RPC_SyncTurn", RpcTarget.All, nextTurn);
    }
    
}