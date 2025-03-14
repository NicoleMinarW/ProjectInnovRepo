using Unity.VisualScripting;
using UnityEngine; 

[CreateAssetMenu(fileName = "New Defense", menuName = "New Defense")]
public class Defense : MoveSet{
    [SerializeField] private float _defense; 
    public float Damage => _defense;
    [SerializeField] private int _duration; 
    public float Duration => _duration;
    public override bool Execute(User P1, User P2, BaseMonster attacker, BaseMonster opponent){
        Debug.Log($"Adding defense stat to user {_defense} for {attacker.data.monsterName}");
        if(P1.costActionPoints(APCost)){
            attacker._defense += _defense; 
            setTurnDefense(attacker, P1.userTurnCount, _duration); 
        }
        return false; 
    }
    public void setTurnDefense(BaseMonster monster, int turn, int duration){
        monster._Def_endDuration=turn + duration + 1; 
        monster._defenseOn = true;
    }
    public override float returnValue()
    {
        return _defense; 
    }
    public override float returnDuration()
    {
        return Duration; 
    }
    public override string moveType()
    {
        return "Defense"; 
    }
}