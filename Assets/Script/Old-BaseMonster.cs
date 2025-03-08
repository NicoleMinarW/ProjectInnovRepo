using UnityEngine;
using System.Collections.Generic;

// [System.Serializable]
    // [CreateAssetMenu(fileName = "New Monster", menuName = "New Monster")]
    public class BaseMonster : MonoBehaviour    
    {
        public MonsterData data; 
        public float _currHP; 
        public float _defense;
        public int _defCounter;
        public float _buff;
        private void Awake(){
            if(data!=null){
                _currHP = data.maxHP; 
                _defense = 0; 
                _buff = 0; 
            }
            else{
                Debug.LogError($"{gameObject.name} MonsterData NULL");
                return; 
            }
            if (data.moveList == null || data.moveList.Count == 0){
                Debug.LogError($"{gameObject.name}: MonsterData.moveList is EMPTY! Assign moves in the MonsterData ScriptableObject.");
            } 
            else {
                Debug.Log($"{gameObject.name} has {data.moveList.Count} moves.");
            }
        }
        public List<MoveSet> GetMoves(){
            if(data == null){
                Debug.LogError("Can't get move, MonsterData null");
                return new List<MoveSet>(); 
            }
            return data.moveList; 
        }
        // public void ExecuteMove()

    }

    [System.Serializable]
    public enum MonsterElement{ 
        Water, 
        Grass,
        Fire, 
        Poison
    };


