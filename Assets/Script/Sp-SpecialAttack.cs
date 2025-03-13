using UnityEngine; 

[CreateAssetMenu(fileName = "New Special Attack", menuName = "New Special Attack")]
public abstract class SpecialAttack : ScriptableObject {
    [SerializeField] private string _spName; 
    public string SpName => _spName;
    public abstract bool ApplyEffect(User P1, User P2, BaseMonster attacker, BaseMonster opponent);
    public abstract void RemoveEffect(BaseMonster attacker);
    public abstract void EffectReady(BaseMonster attacker); 


}