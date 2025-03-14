using UnityEngine; 
using UnityEngine.UI; 

public class MoveButton : MonoBehaviour { 
    public TMPro.TextMeshProUGUI moveName;
    public TMPro.TextMeshProUGUI attackType; 
    public TMPro.TextMeshProUGUI value;
    public TMPro.TextMeshProUGUI AP; 

    public void SetButtons(MoveSet move, System.Action onClickAction){
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