using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Elemental Attack", menuName = "New Elemental Attack")]
public class ElementalAttack : BasicAttack
{   

    [SerializeField] private MonsterElement _attackElement; 
    private float totalDamage;
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
            totalDamage = (Damage * mult) + attacker._buff;
            totalDamage -= totalDamage * (opponent._defense / 100); 
            opponent._currHP -= totalDamage;
            opponent._currHP = Mathf.Clamp(opponent._currHP, 0, opponent.data.maxHP);
            return CheckDeath(opponent); 
        }
        return false; 
    }
}
