using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MonsterBehaviour : MonoBehaviour
{
    public BaseMonster monster; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float curr_cd = 0f;
    private float max_cd = 5f; 

    public enum State {
        dead, 
        current, 
        waiting, 
        selecting, 
        attacking
    }
    public State current; 
    public Transform attackTarget; 
    private Vector3 originalPosition; 
    private bool isAttacking = false; 
    void Start()
    {
        originalPosition = transform.position; 
    }

    // Update is called once per frame
    void Update()
    {
        // if (current == State.waiting){

        // }
        if(current == State.attacking && !isAttacking){
            StartCoroutine(AttackAnimation());
        }
    }
    IEnumerator AttackAnimation(){
        isAttacking = true; 
        Vector3 targetPosition = attackTarget.position; 
        float attackSpeed = 5f; 
        float attackDuration = 0.3f; 
        float elapsedTime = 0f; 
        while (elapsedTime < attackDuration){
            transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime/attackDuration);
            elapsedTime += Time.deltaTime; 
            yield return null; 
        }
        elapsedTime = 0f; 
        while (elapsedTime < attackDuration){
            transform.position = Vector3.Lerp(targetPosition, originalPosition, elapsedTime/attackDuration);
            elapsedTime += Time.deltaTime; 
            yield return null; 
        }
        transform.position = originalPosition; 
        current = State.waiting; 
        isAttacking = false;
    }
    
    void UpdateBar(){
        curr_cd = curr_cd + Time.deltaTime; 
        float calc_cd = curr_cd / max_cd; 
        // ProgressBar.transform.localscale = new Vector3 (Mathf.Clamp (calc_cd, 0, 1, ProgressBar.transform.localscale.y, ProgressBar.transform.localscale.z));
    }
}
