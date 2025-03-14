using UnityEngine;

public class CardScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject playerCard, enemyCard;
    public Sprite playerCardImage, enemyCardImage;

    public void setPlayerCardImage(Sprite image)
    {
        playerCardImage = image;
        playerCard.GetComponent<SpriteRenderer>().sprite = playerCardImage;
    }
    public void setEnemyCardImage(Sprite image)
    {
        enemyCardImage = image;
        enemyCard.GetComponent<SpriteRenderer>().sprite = enemyCardImage;
    }

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
