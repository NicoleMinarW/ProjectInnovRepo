using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngineInternal;
using Photon.Pun; 
using Photon.Realtime;

public class User : MonoBehaviourPunCallbacks
{
    public Player currentplayer; 
    public string _username;
    public int _AP;
    public BaseMonster PlayerMonster;
    public User(Player player, string username, BaseMonster monster){
        currentplayer = player;
        _username = username;
        PlayerMonster = monster; 
    }
    public bool costActionPoints(int cost){
        if(_AP >= cost){
            _AP -= cost; 
            return true; 
        }
        else{
            return false; 
        }
    }


    
}
