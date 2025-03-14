using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Monster", menuName = "Monster/Create New Monster")]
public class MonsterData : ScriptableObject
{
    public string monsterName; 
    public float maxHP; 
    public MonsterElement element; 
    public List<MoveSet> moveList = new List<MoveSet>(); 
    public List<SpecialAttack> specialAttackList = new List<SpecialAttack>();
    public Sprite sprite;
}
