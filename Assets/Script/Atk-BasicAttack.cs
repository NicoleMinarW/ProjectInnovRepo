using UnityEngine;

[CreateAssetMenu(fileName = "New Basic Attack", menuName = "New Basic Attack")]
public class BasicAttack : MoveSet 
{
    [SerializeField] private float _damage; 
    public float Damage => _damage; 
    public override bool Execute(User P1, User P2, BaseMonster attacker, BaseMonster opponent){
        Debug.Log($"Executing {MoveName}: {attacker.data.monsterName} attacks {opponent.data.monsterName}");
        if(P1.costActionPoints(APCost)){
            opponent._currHP -= (_damage + attacker._buff) - ((_damage + attacker._buff) * (opponent._defense/100)); 
            Debug.Log($"Damage: {_damage}, Buff: {attacker._buff}, Opponent Defense: {opponent._defense}");
            Debug.Log($"Opponent's new HP: {opponent._currHP}");
            return CheckDeath(opponent); 
        }
        //insert function for showing that AP is not enough 
        return false; 
        
    }

}
