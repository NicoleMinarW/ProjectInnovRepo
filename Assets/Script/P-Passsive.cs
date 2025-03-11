using UnityEngine; 

[CreateAssetMenu(fileName = "New Passive", menuName = "New Passive")]
public abstract class Passive : ScriptableObject
{
    [SerializeField] private string _passiveName; 
    public string PassiveName => _passiveName; 

    public abstract void Execute(BaseMonster monster); 
}