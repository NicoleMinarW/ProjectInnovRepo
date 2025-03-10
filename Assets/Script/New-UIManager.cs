using UnityEngine;
using System.Collections.Generic; 
using UnityEngine.UI;

[System.Serializable]
public class UIManager : MonoBehaviour
{
    private BattleScriptManager battleScriptManager;
    public TMPro.TextMeshProUGUI playerMonText; 
    public TMPro.TextMeshProUGUI enemyMonText;
    public TMPro.TextMeshProUGUI playerhpText; 
    public TMPro.TextMeshProUGUI enemyhpText;
    public TMPro.TextMeshProUGUI playerUsername;
    public TMPro.TextMeshProUGUI enemyUsername;
    public Slider playerHPSlider; 
    public Slider enemyHPSlider; 
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

    public void SetupUI(BaseMonster playerMonster, BaseMonster enemyMonster, User user1, User user2){
        playerMonText.text = playerMonster.data.monsterName; 
        enemyMonText.text = enemyMonster.data.monsterName;
        playerhpText.text = $"{playerMonster._currHP.ToString()}/{playerMonster.data.maxHP}";
        enemyhpText.text = $"{enemyMonster._currHP.ToString()}/{enemyMonster.data.maxHP}";
        playerHPSlider.maxValue = playerMonster.data.maxHP; 
        playerHPSlider.value = playerMonster._currHP;
        enemyHPSlider.maxValue = enemyMonster.data.maxHP;
        enemyHPSlider.value = enemyMonster._currHP;
        playerUsername.text = user1._username;
        enemyUsername.text = user2._username;
        UpdateMoveButtons(playerMonster); 
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

    public void UpdateEnemyHPSlider(float hp){
        enemyHPSlider.value = hp; 
        enemyhpText.text = $"{hp}/{enemyHPSlider.maxValue}"; 
    }
    public void UpdatePlayerHPSlider(float hp){
        playerHPSlider.value = hp; 
        playerhpText.text = $"{hp}/{playerHPSlider.maxValue}"; 
    }
    
    public void SetupAPDisplay(){
        for (int i=0; i<6; i++){
            GameObject icon = Instantiate(APIcon, APContainer.transform);
            APIcons.Add(icon); 
        }
        UpdateAPDisplay(4); 
    }
    public void UpdateAPDisplay(int currentAP)
    {
        Debug.Log($"Updating AP to {currentAP}");

        for (int i = 0; i < APIcons.Count; i++)
        {
            // APIcons[i].SetActive(i < currentAP); 
            APIcons[i].GetComponent<Image>().color = (i < currentAP) ? Color.white : Color.gray;
        }
    }
}
