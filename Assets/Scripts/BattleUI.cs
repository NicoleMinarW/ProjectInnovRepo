using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 
using UnityEngine.UI;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
[System.Serializable]
public class BattleUI : MonoBehaviour
{
    public TMPro.TextMeshProUGUI nameText; 
    public TMPro.TextMeshProUGUI hpText; 
    public Slider hpSlider; 
    public Button[] moveBtn; 
    public TMPro.TextMeshProUGUI[] moveBtnTxt; 
    private Player  player; 
    public BattleScript battleManager; 

    public void SetupUI(BaseMonster monster, Player user){
        player = user; 
        nameText.text = monster.data.monsterName; 
        hpText.text = $"{monster._currHP.ToString()}/{monster.data.maxHP}"; 
        hpSlider.maxValue = monster.data.maxHP; 
        hpSlider.value = monster._currHP; 
        UpdateMoveButtons(monster); 
    }
    public void UpdateHPSlider(float hp){
        hpSlider.value = hp; 
        hpText.text = $"{hp}/{hpSlider.maxValue}"; 
    }
    public void UpdateMoveButtons(BaseMonster monster){
        if (monster == null){
            Debug.LogError("UpdateMoveBUtton: Monster Null"); 
            return; 
        }
        if (monster.data == null){
            Debug.LogError("UpdateMoveButton: Monster.data null");
            return; 
        }

        List<MoveSet> moves = monster.GetMoves();   

        if (moves == null || moves.Count==0){
            Debug.LogError($"Monster {monster.data.monsterName} has no moves");
            return; 
        }

        for(int i=0; i< moveBtn.Length; i++){
            if(i<moves.Count && moves != null){
                Debug.Log($"Button {i} interactable before: {moveBtn[i].interactable}");
                moveBtn[i].gameObject.SetActive(true); 
                moveBtnTxt[i].text = moves[i].MoveName; 
                moveBtn[i].onClick.RemoveAllListeners(); 
                int moveIndex = i; 
                Debug.Log($"Adding listener for move {moves[moveIndex].MoveName} on button {moveIndex}");
                moveBtn[i].onClick.AddListener(() => OnMoveButtonPress(moves[moveIndex])); 
            }
            else{
                moveBtn[i].gameObject.SetActive(false);
            }
        }
        Debug.Log($"UpdateMoveButtons: {monster.data.monsterName} has {moves.Count} moves assigned.");

    }

    void OnMoveButtonPress(MoveSet chosenMove){
        FindObjectOfType<BattleScript>().ExecuteMove(chosenMove); 
        Debug.Log($"Move {chosenMove.MoveName} clicked!");

    }
}
