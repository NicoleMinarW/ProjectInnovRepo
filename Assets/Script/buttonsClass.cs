using UnityEngine; 
using UnityEngine.UI; 

public class MoveButton : MonoBehaviour { 
    public TMPro.TextMeshProUGUI moveName;
    public TMPro.TextMeshProUGUI attackType; 
    public TMPro.TextMeshProUGUI value;
    public TMPro.TextMeshProUGUI AP; 
    private Button button; 
    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("MoveButton is missing a Button component!");
        }
    }
    public void SetButtons(MoveSet move, System.Action onClickAction){
        if (move == null)
        {
            Debug.LogError("Move data is NULL in SetButton!");
            return;
        }
        moveName.text = move.MoveName;
        attackType.text = move.moveType();
        if(move.moveType() == "Heal"){
            value.text = move.returnValue() + " HP";
        }
        else if(move.moveType() == "Defense" || move.moveType() == "Buff"){
            value.text = move.returnValue().ToString(); 
        }
        else{
            value.text = move.returnValue() + " ATK";
        }        
        AP.text = move.APCost.ToString(); 
        GetComponent<Button>().onClick.RemoveAllListeners(); 
        GetComponent<Button>().onClick.AddListener(() => onClickAction());

    }

    public void setSPButton(SpecialAttack SPMove, System.Action<SpecialAttack> onClickAction){

    }

}