using UnityEngine; 
using System.Collections;
using UnityEngine.UIElements;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;

public enum GameState {
    START, PLAYER1TURN, PLAYER2TURN, WON, LOST 
}

public class BattleScript : MonoBehaviour {
    public GameState state; 
    public GameObject player1; 
    public GameObject player2; 
    // public GameObject playerprefab;
    public Transform player1Position; 
    public Transform player2Position; 
    Player UnitPlayer1; 
    Player UnitPlayer2; 
    public GameObject monsterAPrefab; 
    public GameObject monsterBPrefab; 
    public BattleUI player1UI; 
    public BattleUI player2UI; 
    public TMPro.TextMeshProUGUI PlayerTurn; 


    void Start(){
        state = GameState.START; 
        StartCoroutine(SetupBattle()); 
    }
    IEnumerator SetupBattle(){
        // GameObject Player1Obj = new GameObject("Player")
        UnitPlayer1 = player1.GetComponent<Player>(); 
        UnitPlayer2 = player2.GetComponent<Player>(); 
        // GameObject monster1Obj = Instantiate(monsterAPrefab, player1Position.position, Quaternion.identity);
        // GameObject monster2Obj = Instantiate(monsterBPrefab, player2Position.position, Quaternion.identity);  
        if (UnitPlayer1 == null || UnitPlayer2 == null){
            Debug.LogError("Player object null in Player.cs"); 
            yield break; 
        }
        // how to set GameOBJ player to class Player  
        yield return new WaitForSeconds(2f); 
        
        BaseMonster monster1 = null; 
        BaseMonster monster2 = null; 
        // BaseMonster monster1 = monster1Obj.GetComponent<BaseMonster>();
        // BaseMonster monster2 = monster2Obj.GetComponent<BaseMonster>();
        if (UnitPlayer1.PlayerMonster == null) {
            GameObject monster1Obj = Instantiate(monsterAPrefab, player1Position.position, Quaternion.identity);
            monster1 = monster1Obj.GetComponent<BaseMonster>();
            UnitPlayer1.addMonster(monster1);
        }
        else{
            monster1 = UnitPlayer1.PlayerMonster; 
        }
        if (UnitPlayer2.PlayerMonster == null) {
            GameObject monster2Obj = Instantiate(monsterBPrefab, player2Position.position, Quaternion.identity);
            monster2 = monster2Obj.GetComponent<BaseMonster>();
            UnitPlayer2.addMonster(monster2);
        }
        else{
            monster2 = UnitPlayer2.PlayerMonster; 
        }
        if (UnitPlayer1.PlayerMonster == null && UnitPlayer1.MonsterList.Count > 0) {
            UnitPlayer1.PlayerMonster = UnitPlayer1.MonsterList[0];
        }
        if (UnitPlayer2.PlayerMonster == null && UnitPlayer2.MonsterList.Count > 0) {
            UnitPlayer2.PlayerMonster = UnitPlayer2.MonsterList[0];
        }
        // BaseMonster monster1 = Player.
        // monster1.data = new MonsterData(); 
        // monster2.data = new MonsterData(); 
        monster1.data = Resources.Load<MonsterData>("Baldy"); 
        monster2.data = Resources.Load<MonsterData>("Vulcano");

        if (monster1.data == null || monster2.data == null) {
            Debug.LogError("MonsterData is NULL! Make sure MonsterData is in a Resources folder.");
        } else {
            Debug.Log($"Assigned {monster1.data.monsterName} to Player 1's monster.");
            Debug.Log($"Assigned {monster2.data.monsterName} to Player 2's monster.");
        }
        monster1._currHP = monster1.data.maxHP; 
        monster2._currHP = monster2.data.maxHP; 

        UnitPlayer1.addMonster(monster1);
        UnitPlayer2.addMonster(monster2);
        
        Debug.Log($"Before Assignment: UnitPlayer1.MonsterList Count = {UnitPlayer1.MonsterList.Count}");
        Debug.Log($"Before Assignment: UnitPlayer2.MonsterList Count = {UnitPlayer2.MonsterList.Count}");
        if (UnitPlayer1.MonsterList.Count > 0) {
            UnitPlayer1.PlayerMonster = UnitPlayer1.MonsterList[0];
        } else {
            Debug.LogError("UnitPlayer1.MonsterList is EMPTY when trying to assign PlayerMonster!");
        }

        if (UnitPlayer2.MonsterList.Count > 0) {
            UnitPlayer2.PlayerMonster = UnitPlayer2.MonsterList[0];
        } else {
            Debug.LogError("UnitPlayer2.MonsterList is EMPTY when trying to assign PlayerMonster!");
        }
        // UnitPlayer1.PlayerMonster = UnitPlayer1.MonsterList[0];
        // UnitPlayer2.PlayerMonster = UnitPlayer2.MonsterList[0];

        player1UI.SetupUI(UnitPlayer1.PlayerMonster, UnitPlayer1); 
        player2UI.SetupUI(UnitPlayer2.PlayerMonster, UnitPlayer2); 
        state = GameState.PLAYER1TURN;
        UpdateTurn();
        // player1Turn(); 
        yield return null; 
        
    }
    // public void player1Turn(){
    //     PlayerTurn.text = "Player 1"; 
    // }
    // public void player2Turn(){
    //     PlayerTurn.text = "Player 2"; 
    // }
    public void AttackBtn1(){
        if(state != GameState.PLAYER1TURN){
            return; 
        }
        // StartCoroutine(PlayerAttack(UnitPlayer1, UnitPlayer2, chosenMove)); 

    }
    // IEnumerator PlayerAttack(Player attacker, Player opponent, MoveSet chosenAttack){
    //     if(state != GameState.PLAYER1TURN && state != GameState.PLAYER2TURN){
    //         yield break; 
    //     }
    //     BaseMonster attackerMonster = attacker.PlayerMonster; 
    //     BaseMonster opponentMonster = opponent.PlayerMonster; 
    //     bool isEnemyDead = chosenAttack.Execute(attacker, opponent, attackerMonster, opponentMonster); 
    //     yield return new WaitForSeconds(3f); 
    //     if (isEnemyDead){
    //         state = GameState.WON; 
    //     }
    //     else{
    //         state = GameState.PLAYER2TURN; 
    //     }

    // }
    public void ExecuteMove(MoveSet chosenMove){
        if(state == GameState.PLAYER1TURN){
            StartCoroutine(PlayerUseMove(UnitPlayer1, UnitPlayer2, chosenMove)); 
        }
        else if (state == GameState.PLAYER2TURN){
            StartCoroutine(PlayerUseMove(UnitPlayer2, UnitPlayer1, chosenMove)); 

        }
    }
    IEnumerator PlayerUseMove(Player user, Player opponent, MoveSet chosenMove)
    {
        Debug.Log($"[PlayerUseMove] Called by: {user.gameObject.name}");

        if (state != GameState.PLAYER1TURN && state != GameState.PLAYER2TURN){
            yield break;
        }

        BaseMonster attacker = user.PlayerMonster;
        BaseMonster target = opponent.PlayerMonster;

        bool isTargetDefeated = chosenMove.Execute(user, opponent, attacker, target);

        player1UI.UpdateHPSlider(UnitPlayer1.PlayerMonster._currHP);
        player2UI.UpdateHPSlider(UnitPlayer2.PlayerMonster._currHP);

        yield return new WaitForSeconds(1f);

        if (isTargetDefeated)
        {
            if (state == GameState.PLAYER1TURN){
                state = GameState.WON; 
            }
            else{
                state = GameState.LOST; 
            }
            EndBattle();
        }
        else
        {
            UpdateTurn();
        }

    }
    public void UpdateTurn(){ 
        Debug.Log($"Before turn switch: {state.ToString()}");
        if (state == GameState.PLAYER1TURN){
            state = GameState.PLAYER2TURN; 
        }
        else if(state == GameState.PLAYER2TURN){
            state = GameState.PLAYER1TURN; 
        }
        UpdateTurnUI(); 
        Debug.Log($"current turn:{state.ToString()}");
    }

    public void EndBattle(){
        foreach(UnityEngine.UI.Button btn in player1UI.moveBtn) {
            btn.interactable = false; 
        }
        foreach(UnityEngine.UI.Button btn in player2UI.moveBtn){
            btn.interactable = false; 
        }
        if (state == GameState.WON){
            PlayerTurn.text = "Player 1 Wins"; 
        }
        else if (state == GameState.LOST){
            PlayerTurn.text = "Player 2 Wins"; 
        }
        // return to somewhere 
    }
    public void UpdateTurnUI(){
        // if (state == GameState.PLAYER1TURN){
        //     PlayerTurn.text = "Player 1 Turn"; 
        //     SetMoveBtn(player1UI, true); 
        //     SetMoveBtn(player2UI, false); 
        // }
        // else if (state == GameState.PLAYER2TURN){
        //     PlayerTurn.text= "Player 2 Turn"; 
        //     SetMoveBtn(player1UI, false); 
        //     SetMoveBtn(player2UI, true); 
        // }
        foreach (UnityEngine.UI.Button btn in player1UI.moveBtn)
            btn.interactable = (state == GameState.PLAYER1TURN);
        foreach (UnityEngine.UI.Button btn in player2UI.moveBtn)
            btn.interactable = (state == GameState.PLAYER2TURN);
    }
    void SetMoveBtn(BattleUI ui, bool set){
        foreach(UnityEngine.UI.Button btn in ui.moveBtn){
            btn.interactable = set; 
        }
    }
    
}