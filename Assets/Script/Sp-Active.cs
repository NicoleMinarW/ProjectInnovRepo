using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "New Special Attack", menuName = "New Active: Batlamandr")]
public class ActiveBatlamandr : SpecialAttack
{
    public ElementalAttack Explosion;
    public override bool ApplyEffect(User P1, User P2, BaseMonster attacker, BaseMonster opponent){
        Explosion.Execute(P1, P2, attacker, opponent);
        attacker._isOnCooldown = true;
        attacker._remainingCooldown = 3;
        return false; 
    }
    public override void RemoveEffect(BaseMonster attacker){

    }
    public override void EffectReady(BaseMonster attacker)
    {
        attacker._isOnCooldown = false; 
    }
    public override float returnValue()
    {
        return 10f; 
    }
    public override float returnCooldown()
    {
        return 3f;
    } 
}