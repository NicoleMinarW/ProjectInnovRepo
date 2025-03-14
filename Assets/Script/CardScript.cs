using System.Xml.Serialization;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;


public class CardScript : MonoBehaviour

{   
    public GameObject pCard, eCard;
    public UIManager currUI; 
    public Image playerImage; 
    public Image enemyImage;

    public void showEnemyCard()
    {
        pCard.SetActive(false);
        eCard.SetActive(true);
    }

    public void showPlayerCard()
    {
        pCard.SetActive(true);
        eCard.SetActive(false);
    }
    public void setCardImages(Sprite player, Sprite enemy){ 
        playerImage.sprite = player; 
        enemyImage.sprite = enemy; 
    }
}
