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
    public Sprite playerCard;
    public Sprite enemyCard;
    public Slider playerHPSlider; 
    public Slider enemyHPSlider; 
    public Button[] moveBtn; 
    public Button SPButton; 
    public TMPro.TextMeshProUGUI SPButtonTxt; 
    public TMPro.TextMeshProUGUI SPvalue; 
    public TMPro.TextMeshProUGUI SPCD; 
    public TMPro.TextMeshProUGUI[] moveBtnTxt1;
    public TMPro.TextMeshProUGUI[] moveBtnTxt2;  
    public GameObject APContainer; 
    public GameObject APIcon; 
    private List<GameObject> APIcons = new List<GameObject>();
    public GameObject moveButtonpref;
    public Transform moveButtonContainer; 
    private List<GameObject> moveButtons;
    
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
            if(playerMonster.SPMove.returnValue().ToString()== null){
                Debug.LogError("The return value is null");
            }
            SPvalue.text = playerMonster.SPMove.returnValue().ToString() + "Damage"; 
            SPCD.text = playerMonster.SPMove.returnCooldown().ToString() + "Cooldown"; 
            SPButton.onClick.RemoveAllListeners();
            SPButton.onClick.AddListener(() => OnSPButtonPress(playerMonster.SPMove));
        }
        SetupAPDisplay(); 
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
                moveBtnTxt1[i].text = moves[i].MoveName + moves[i].GetType(); 
                moveBtnTxt2[i].text = moves[i].returnValue() + " DMG | " + moves[i].APCost + " AP"; 
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

    // public void UpdateMoveButtons(BaseMonster monster){
    //     List<MoveSet> moves = monster.GetMoves();   
    //     foreach (Transform child in moveButtonContainer) {
    //         Destroy(child.gameObject);
            
    //     }
    //     if (moves == null || moves.Count==0){
    //         Debug.LogError($"Monster {monster.data.monsterName} has no moves");
    //         return; 
    //     }
    //     foreach (MoveSet move in moves){
    //         GameObject buttonObj = Instantiate(moveButtonpref, moveButtonContainer);
    //         MoveButton moveButton = buttonObj.GetComponent<MoveButton>();
    //         moveButton.SetButtons(move, () => OnMoveButtonPress(move));

    //     }
    // }
    // public void UpdateMoveButtons(BaseMonster monster)
    // {
    //     // Remove old buttons before adding new ones
    //     foreach (GameObject btn in moveButtons)
    //     {
    //         Destroy(btn);
    //     }
    //     moveButtons.Clear();

    //     // Create new buttons
    //     foreach (MoveSet move in monster.GetMoves())
    //     {
    //         GameObject buttonObj = Instantiate(moveButtonpref, moveButtonContainer);
    //         MoveButton moveButton = buttonObj.GetComponent<MoveButton>();

    //         if (moveButton != null)
    //         {
    //             moveButton.SetButtons(move, () => OnMoveButtonPress(move));
    //             moveButtons.Add(buttonObj); 
    //         }
    //         else
    //         {
    //             Debug.LogError("MoveButton script is missing on the instantiated move button!");
    //         }
    //     }
    // }

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
            APIcons[i].SetActive(i < currentAP); 
            // APIcons[i].GetComponent<Image>().color = (i < currentAP) ? Color.white : Color.gray;
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
