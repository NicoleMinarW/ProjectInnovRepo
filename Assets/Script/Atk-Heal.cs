using UnityEngine;

[CreateAssetMenu(fileName = "New Heal", menuName = "New Heal")]
public class Heal : MoveSet 
{
    [SerializeField] private float _healAmount;
    public float HealAmount => _healAmount;
    
    public override bool Execute(User P1, User P2, BaseMonster attacker, BaseMonster opponent){
        Debug.Log($"Executing healing of {_healAmount} on {attacker.data.monsterName}");
        if(P1.costActionPoints(APCost)){
            attacker._currHP += _healAmount; 
            attacker._currHP = Mathf.Clamp(attacker._currHP, 0, attacker.data.maxHP);
            return false; 
        }
        return false; 
    }
}