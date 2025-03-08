using UnityEngine;
using System.Collections.Generic; 
using UnityEngine.UI;

[System.Serializable]
public class UIManager : MonoBehaviour
{
    private BattleScriptManager battleScriptManager;
    public TMPro.TextMeshProUGUI nameText; 
    public TMPro.TextMeshProUGUI hpText; 
    public Slider hpSlider; 
    public Button[] moveBtn; 
    public TMPro.TextMeshProUGUI[] moveBtnTxt; 
    public GameObject APContainer; 
    public GameObject APIcon; 
    private List<GameObject> APIcons = new List<GameObject>();
    
    void Start() {
        battleScriptManager = FindFirstObjectByType<BattleScriptManager>();
        if (battleScriptManager == null) {
            Debug.LogError("BattleScriptManager not found in the scene!");
        }
    }

    public void SetupUI(BaseMonster monster, User user){
        nameText.text = monster.data.monsterName; 
        hpText.text = $"{monster._currHP.ToString()}/{monster.data.maxHP}"; 
        hpSlider.maxValue = monster.data.maxHP; 
        hpSlider.value = monster._currHP; 
        UpdateMoveButtons(monster); 
    }

    public void UpdateMoveButtons(BaseMonster monster){
        List<MoveSet> moves = monster.GetMoves();   

        if (moves == null || moves.Count==0){
            Debug.LogError($"Monster {monster.data.monsterName} has no moves");
            return; 
        }

        for(int i=0; i< moveBtn.Length; i++){
            if(i<moves.Count && moves != null){
                moveBtn[i].gameObject.SetActive(true); 
                moveBtnTxt[i].text = moves[i].MoveName; 
                moveBtn[i].onClick.RemoveAllListeners(); 
                MoveSet currMove = moves[i];
                Debug.Log($"Adding listener for move {currMove} on button {i}");
                moveBtn[i].onClick.AddListener(() => OnMoveButtonPress(currMove)); 
            }
            else{
                moveBtn[i].gameObject.SetActive(false);
            }
        }
        Debug.Log($"UpdateMoveButtons: {monster.data.monsterName} has {moves.Count} moves assigned.");

    }
    void OnMoveButtonPress(MoveSet chosenMove){
        // battleScriptManager = FindFirstObjectByType<BattleScriptManager>();
        battleScriptManager.ExecuteMove(chosenMove); 
        Debug.Log($"Move {chosenMove.MoveName} clicked!");

    }
    
    public void UpdateAPDisplay(int currentAP){
        foreach(GameObject icon in APIcons){
            Destroy(icon);
        }
        APIcons.Clear(); 
        for(int i=0; i<currentAP; i++){
            GameObject icon = Instantiate(APIcon, APContainer.transform); 
            APIcons.Add(icon); 
        }
    }
}
