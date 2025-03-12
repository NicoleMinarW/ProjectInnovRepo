using UnityEngine; 
[CreateAssetMenu(fileName = "New Special Attack", menuName = "New Special Attack")]
public abstract class SpecialAttack : ScriptableObject {
    [SerializeField] private string _a;
}