using UnityEngine;

[CreateAssetMenu(fileName = "New Move", menuName = "New Move")]
public abstract class MoveSet : ScriptableObject
{
    [SerializeField] private string _moveName; 
    [SerializeField] private int _APCost; 
    public string MoveName => _moveName; 
    public int APCost => _APCost; 

    public abstract bool Execute(User P1, User P2, BaseMonster attacker, BaseMonster opponent); 


    public bool CheckDeath(BaseMonster opponent)
    {
        if(opponent._currHP <= 0){
            return true; 
        }
        else {
            return false; 
        }   
    }
    public virtual float returnValue(){
        return 0f;
    }
    public virtual float returnDuration(){
        return 0f;
    }
    public virtual string moveType(){
        return "None";
    }
    
}
