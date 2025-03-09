using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class Creature : MonoBehaviourPun
{
    public string creatureName;
    public int attack;
    public int defense;
    public int hp;
    public List<string> attacks;

    public void Initialize(string name, int attack, int defense, int hp, List<string> attacks)
    {
        creatureName = name;
        this.attack = attack;
        this.defense = defense;
        this.hp = hp;
        this.attacks = new List<string>(attacks);
    }
    public void PerformAttack()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        Debug.Log($"{creatureName} is attacking");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
