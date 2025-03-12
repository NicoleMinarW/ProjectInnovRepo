using Unity.VisualScripting;
using UnityEngine; 

[CreateAssetMenu(fileName = "New Buff", menuName = "New Buff")]
public class Buff : MoveSet{
    [SerializeField] private float _buff; 
    public float BuffValue => _buff;
    [SerializeField] private int _duration; 
    public float Duration => _duration;
    public override bool Execute(User P1, User P2, BaseMonster attacker, BaseMonster opponent){
        Debug.Log($"Adding buff stat to user {_buff} for {attacker.data.monsterName}");
        if(P1.costActionPoints(APCost)){
            attacker._buff += _buff; 
            BattleScriptManager.Instance.setTurnBuff(P1.userTurnCount, _duration);
        }
        return false; 
    }
}