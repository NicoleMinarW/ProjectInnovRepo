using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Elemental Attack", menuName = "New Elemental Attack")]
public class ElementalAttack : BasicAttack
{   

    [SerializeField] private MonsterElement _attackElement; 
    public MonsterElement AttackElement => _attackElement; 
    // public ElementalAttack(string attackName, float damage, int APCost, MonsterElement element) : base(attackName, damage, APCost){
    //     _attackElement = element; 
    // }
    public override bool Execute(User P1, User P2, BaseMonster attacker, BaseMonster opponent){
        float mult = 1f; 
        if(P1.costActionPoints(APCost)){
            if((_attackElement== MonsterElement.Fire && opponent.data.element == MonsterElement.Grass) || 
            (_attackElement == MonsterElement.Water && opponent.data.element == MonsterElement.Fire) || 
            (_attackElement == MonsterElement.Grass && opponent.data.element == MonsterElement.Poison) || 
            (_attackElement == MonsterElement.Poison && opponent.data.element == MonsterElement.Water)){
                mult = 1.5f; 
            }
            opponent._currHP -= (Damage * mult )+ attacker._buff - (((Damage * mult )+ attacker._buff) * (opponent._defense/100)); 
            opponent._currHP = Mathf.Clamp(opponent._currHP, 0, opponent.data.maxHP);
            return CheckDeath(opponent); 
        }
        return false; 
    }
    public override float returnValue()
    {
        return Damage; 
    }
    public override string moveType()
    {
        return "Elemental"; 
    }
}
