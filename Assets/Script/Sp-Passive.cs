using UnityEngine; 

[CreateAssetMenu(fileName = "New Special Attack", menuName = "New Passive: Scorpi-Rat")]
public class PassiveScorpiRat : SpecialAttack
{
    public override bool ApplyEffect(User P1, User P2, BaseMonster attacker, BaseMonster opponent){
        attacker._buff = 2;
        attacker._isOngoing = true;
        attacker._remainingDuration = 2;
        return false;
    }
    public override void RemoveEffect(BaseMonster attacker){
        attacker._buff = 0;
        attacker._isOngoing = false;
        attacker._isOnCooldown = true;
        attacker._remainingCooldown = 2; 
    }
    public override void EffectReady(BaseMonster attacker){
        attacker._isOnCooldown = false; 
        attacker._remainingCooldown = 2; 
    }
    public override float returnValue()
    {
        return 2f;
    }
    public override float returnCooldown()
    {
        return 2f; 
    }
}