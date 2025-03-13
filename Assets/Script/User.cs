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
    public int userTurnCount = 1; 
    public User(Player player, string username, BaseMonster monster){
        currentplayer = player;
        _username = username;
        PlayerMonster = monster; 
        _AP = 4; 
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

    public void refreshAP(){
        _AP += 4; 
        _AP = Mathf.Clamp(_AP, 0, 6);

    }
    public void assignUser(User user, Player player, string username, BaseMonster monster){
        user.currentplayer = player;
        user._username = username;
        user.PlayerMonster = monster;
        user._AP = 4; 
    }


    
}
