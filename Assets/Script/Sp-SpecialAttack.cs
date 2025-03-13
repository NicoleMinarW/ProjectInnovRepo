using UnityEngine; 

[CreateAssetMenu(fileName = "New Special Attack", menuName = "New Special Attack")]
public abstract class SpecialAttack : ScriptableObject {
    [SerializeField] private string _spName; 
    public string SpName => _spName;
    [SerializeField] private float _spValue; 
    public float SpValue => _spValue;
    private bool SPReady = false; 
    private bool SPOn = false; 

    public void ApplyEffect(BaseMonster attacker, BaseMonster opponent){
        return; 
    }



}