using UnityEngine;
using System.Collections.Generic;

// [System.Serializable]
    // [CreateAssetMenu(fileName = "New Monster", menuName = "New Monster")]
    public class BaseMonster : MonoBehaviour    
    {
        [SerializeField] public Animator monsterAnimator; 
        public MonsterData data; 
        private float currHP;
        public float _currHP {get => currHP; set => currHP=value;} 
        private float defense; 
        public float _defense {get=>defense; set=>defense=value;}
        private int defCounter;
        public int _defCounter {get=>defCounter; set=>defCounter=value;}
        private float buff; 
        public float _buff{get=>buff; set=>buff=value;}
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
        public void AttackAnimation(){
            monsterAnimator.SetTrigger("move1");
        }
        public void GetHitAnimation(){
            monsterAnimator.SetTrigger("healthDecreased");
        }

    }

    [System.Serializable]
    public enum MonsterElement{ 
        Water, 
        Grass,
        Fire, 
        Poison
    };


