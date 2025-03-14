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
    public Image playerCard;
    public Image enemyCard;
    public Slider playerHPSlider; 
    public Slider enemyHPSlider; 
    public Button[] moveBtn; 
    public Button SPButton; 
    public TMPro.TextMeshProUGUI SPButtonTxt; 
    public TMPro.TextMeshProUGUI[] moveBtnTxt; 
    public GameObject APContainer; 
    public GameObject APIcon; 
    private List<GameObject> APIcons = new List<GameObject>();
    public GameObject moveButtonpref;
    public Transform moveButtonContainer; 
    
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
        playerCard = playerMonster.data.sprite;
        enemyCard = enemyMonster.data.sprite;
       if (playerMonster.SPMove != null) {
            SPButtonTxt.text = playerMonster.SPMove.SpName;
            SPButton.onClick.RemoveAllListeners();
            SPButton.onClick.AddListener(() => OnSPButtonPress(playerMonster.SPMove));
        }
        SetupAPDisplay(); 
        UpdateMoveButtons(playerMonster); 
    }

    public void UpdateMoveButtons(BaseMonster monster){
        List<MoveSet> moves = monster.GetMoves();   
        foreach (Transform child in moveButtonContainer) {
            Destroy(child.gameObject);
            
        }
        if (moves == null || moves.Count==0){
            Debug.LogError($"Monster {monster.data.monsterName} has no moves");
            return; 
        }
        foreach (MoveSet move in moves){
            GameObject buttonObj = Instantiate(moveButtonpref, moveButtonContainer);
            MoveButton moveButton = buttonObj.GetComponent<MoveButton>();
            moveButton.SetButtons(move, () => OnMoveButtonPress(move));

        }
    }

    public void OnMoveButtonPress(MoveSet chosenMove){
        // battleScriptManager = FindFirstObjectByType<BattleScriptManager>();
        battleScriptManager.ExecuteMove(chosenMove); 
        Debug.Log($"Move {chosenMove.MoveName} clicked!");
    }
    public void OnSPButtonPress(SpecialAttack chosenSP){
        battleScriptManager.ExecuteSP(chosenSP);
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

    public void UpdateSPButton(BaseMonster monster){
        if(monster._isOnCooldown || monster._isOngoing){
            SPButton.enabled = false; 
        }
        else{
            SPButton.enabled = true; 
        }
    }
}
