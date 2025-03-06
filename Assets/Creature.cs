using UnityEngine;
using System.Collections.Generic;

public class Creature : MonoBehaviour
{
    public string creatureName;
    public int attack;
    public int defense;
    public int hp;
    public List<string> attacks;

    public void PerformAttack()
    {
        Debug.Log($"{creatureName} is attacking with {attacks[Random.Range(0, attacks.Count)]}");
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
