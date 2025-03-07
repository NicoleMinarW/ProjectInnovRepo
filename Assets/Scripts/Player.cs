using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngineInternal;

public class Player : MonoBehaviour
{
    public string _username;
    public int _AP;
    [SerializeField] BaseMonster[] _monsterList; 
    public List<BaseMonster> MonsterList{get; private set;} = new List<BaseMonster>(); 
    public BaseMonster PlayerMonster;
    public bool costActionPoints(int cost){
        if(_AP >= cost){
            _AP -= cost; 
            return true; 
        }
        else{
            return false; 
        }
    }

    public  void addMonster(BaseMonster monster){
        MonsterList.Add(monster); 
    }
    public void changeMonster(){
        PlayerMonster = MonsterList.ElementAt(MonsterList.IndexOf(PlayerMonster)+1); 
    }



    
}
