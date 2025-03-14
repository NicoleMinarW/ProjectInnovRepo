using UnityEngine;

public class CardScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject playerCard, enemyCard;
    public void showEnemyCard()
    {
        playerCard.SetActive(false);
        enemyCard.SetActive(true);
    }

    public void showPlayerCard()
    {
        playerCard.SetActive(true);
        enemyCard.SetActive(false);
    }
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
