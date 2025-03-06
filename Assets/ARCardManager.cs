using UnityEngine;
using Photon.Pun;
using Vuforia;
using TMPro;
using System.Collections.Generic;

public class ARCardManager : MonoBehaviour
{
    public GameObject creatureA, creatureB;
    public TextMeshProUGUI nameText, attackText, defenseText, hpText, attacksText;
    public GameObject attackButton;

    private GameObject spawnedCreature;
    private Creature playerCreature, opponentCreature;
    private Dictionary<string, GameObject> creaturePrefabs;

    [System.Serializable]
    public struct CreatureStats
    {
        public string creatureName;
        public int attack;
        public int defense;
        public int hp;
        public List<string> attacks;
    }

    void Start()
    {
        // Try to get ObserverBehaviour component attached to this GameObject
        var observerBehaviour = GetComponent<ObserverBehaviour>();

        if (observerBehaviour == null)
        {
            observerBehaviour = GetComponentInChildren<ObserverBehaviour>(); // Double-check in children
        }

        if (observerBehaviour != null)
        {
            Debug.Log("ObserverBehaviour found and event subscribed!");
            observerBehaviour.OnTargetStatusChanged += OnARCardDetected;
        }
        else
        {
            Debug.LogError("ObserverBehaviour NOT found! Ensure it is correctly attached to the Image Target.");
        }

        // Initialize creature data
        creaturePrefabs = new Dictionary<string, GameObject>
    {
        { "test2_scaled", creatureA},
        { "test1_scaled", creatureB}
    };
        attackButton.SetActive(false);
        Debug.Log("Creature Data initialized successfully.");
    }


    private void OnARCardDetected(ObserverBehaviour behaviour, TargetStatus status)
    {
        Debug.Log($"OnARCardDetected triggered! Target: {behaviour.TargetName}, Status: {status.Status}");
        // Check if the target is tracked and no creature has been spawned yet
        if (status.Status != Status.TRACKED || !PhotonNetwork.IsConnected)
        {
            Debug.Log("Target not tracked or Photon not connected.");
            return;
        }

        // Get the name of the scanned card
        string targetName = behaviour.TargetName; 
        Debug.Log("Card Detected: " + targetName);

        if (creaturePrefabs.TryGetValue(targetName, out GameObject creaturePrefab))
        {
            if (spawnedCreature == null)
            {
                spawnedCreature = PhotonNetwork.Instantiate(creaturePrefab.name, behaviour.transform.position, behaviour.transform.rotation);
                spawnedCreature.transform.parent = behaviour.transform;

                // Assign the Creature component
                playerCreature = spawnedCreature.GetComponent<Creature>();
                if (playerCreature == null)
                {
                    playerCreature = spawnedCreature.AddComponent<Creature>();
                }

                // Assign stats
                if (targetName == "test2_scaled")
                {
                    playerCreature.creatureName = "Creature A";
                    playerCreature.attack = 10;
                    playerCreature.defense = 8;
                    playerCreature.hp = 100;
                    playerCreature.attacks = new List<string> { "Fireball", "Slash" };
                }
                else if (targetName == "test1_scaled")
                {
                    playerCreature.creatureName = "Creature B";
                    playerCreature.attack = 12;
                    playerCreature.defense = 6;
                    playerCreature.hp = 120;
                    playerCreature.attacks = new List<string> { "Ice Blast", "Bite" };
                }

                UpdateUI(playerCreature);
                attackButton.SetActive(true); // Show attack button for the player
            }
        }

    }

    private void UpdateUI(Creature stats)
    {
        if (nameText && attackText && defenseText && hpText && attacksText)
        {
            nameText.text = $"Summoned: {stats.creatureName}";
            attackText.text = $"Attack: {stats.attack}";
            defenseText.text = $"Defense: {stats.defense}";
            hpText.text = $"HP: {stats.hp}";
            attacksText.text = $"Attacks: {string.Join(", ", stats.attacks)}";
        }
        else
        {
            Debug.LogError("UI Text components not assigned!");
        }
    }

    public void OnAttackButtonPressed()
    {
        if (playerCreature != null)
        {
            playerCreature.PerformAttack();
        }
    }

}
