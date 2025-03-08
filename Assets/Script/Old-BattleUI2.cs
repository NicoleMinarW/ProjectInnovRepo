// using UnityEngine;
// using System.Collections; 
// using System.Collections.Generic; 
// using UnityEngine.UI;
// using System.Runtime.InteropServices.WindowsRuntime;
// using Unity.VisualScripting;
// [System.Serializable]
// public class BattleUI2 : MonoBehaviour
// {
//     public GameObject p1Panel; 
//     public GameObject p2Panel; 
//     public TMPro.TextMeshProUGUI turnName; 
//     public TMPro.TextMeshProUGUI nameText1; 
//     public TMPro.TextMeshProUGUI hpText1; 
//     public Slider hpSlider1; 
//     public Button[] moveBtn1; 
//     public TMPro.TextMeshProUGUI[] moveBtnTxt1; 
//     private User player1;
//     private User player2;  
//     public TMPro.TextMeshProUGUI nameText2; 
//     public TMPro.TextMeshProUGUI hpText2; 
//     public Slider hpSlider2; 
//     public Button[] moveBtn2; 
//     public TMPro.TextMeshProUGUI[] moveBtnTxt2; 
    
//     public BattleScript battleManager;

//     public void SetupUI(BaseMonster monster1, User user1, BaseMonster monster2, User user2){
//         player1 = user1; 
//         player2 = user2; 
//         nameText1.text = monster1.data.monsterName; 
//         nameText2.text = monster2.data.monsterName; 
//         hpText1.text = $"{monster1._currHP.ToString()}/{monster1.data.maxHP}"; 
//         hpText2.text = $"{monster2._currHP.ToString()}/{monster2.data.maxHP}"; 
//         hpSlider1.maxValue = monster1.data.maxHP; 
//         hpSlider2.maxValue = monster2.data.maxHP; 
//         hpSlider1.maxValue = monster1._currHP; 
//         hpSlider2.maxValue = monster2._currHP; 
//         // UpdateMoveButtons(monster1, monster2); 
//     }
//     // public void UpdateHPSlider(Player player, float hp){
//     //     hpSlider1.value = hp; 
//     //     hpText.text = $"{hp}/{hpSlider.maxValue}"; 
//     // }
//     // public void UpdateMoveButtons(BaseMonster monster1, BaseMonster monster2){
//     //     if (monster == null){
//     //         Debug.LogError("UpdateMoveBUtton: Monster Null"); 
//     //         return; 
//     //     }
//     //     if (monster.data == null){
//     //         Debug.LogError("UpdateMoveButton: Monster.data null");
//     //         return; 
//     //     }

//     //     List<MoveSet> moves = monster.GetMoves();   

//     //     if (moves == null || moves.Count==0){
//     //         Debug.LogError($"Monster {monster.data.monsterName} has no moves");
//     //         return; 
//     //     }

//     //     for(int i=0; i< moveBtn.Length; i++){
//     //         if(i<moves.Count && moves != null){
//     //             Debug.Log($"Button {i} interactable before: {moveBtn[i].interactable}");
//     //             moveBtn[i].gameObject.SetActive(true); 
//     //             moveBtnTxt[i].text = moves[i].MoveName; 
//     //             moveBtn[i].onClick.RemoveAllListeners(); 
//     //             int moveIndex = i; 
//     //             Debug.Log($"Adding listener for move {moves[moveIndex].MoveName} on button {moveIndex}");
//     //             moveBtn[i].onClick.AddListener(() => OnMoveButtonPress(moves[moveIndex])); 
//     //         }
//     //         else{
//     //             moveBtn[i].gameObject.SetActive(false);
//     //         }
//     //     }
//     //     Debug.Log($"UpdateMoveButtons: {monster.data.monsterName} has {moves.Count} moves assigned.");

//     // }

//     void OnMoveButtonPress(MoveSet chosenMove){
//         FindObjectOfType<BattleScript>().ExecuteMove(chosenMove); 
//         Debug.Log($"Move {chosenMove.MoveName} clicked!");

//     }
// }
