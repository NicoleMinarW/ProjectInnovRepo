using UnityEngine;
using System.Collections.Generic; 

[CreateAssetMenu(fileName = "New Monster", menuName = "Monster/Create New Monster")]
public class MonsterData : ScriptableObject
{
    public string monsterName; 
    public float maxHP; 
    public MonsterElement element; 
    public List<MoveSet> moveList = new List<MoveSet>(); 
}
