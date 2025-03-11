using UnityEngine; 

[CreateAssetMenu(fileName = "New Defense", menuName = "New Defense")]
public class Defense : MoveSet{
    public override bool Execute(User P1, User P2, BaseMonster attacker, BaseMonster opponent){
        return false; 
    }
}