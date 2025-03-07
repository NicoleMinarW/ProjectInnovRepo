using UnityEngine;
using Photon.Pun;
using Vuforia;
using TMPro;
using System.Collections.Generic;

public class ARCardManager : MonoBehaviourPunCallbacks
{
    public GameObject creatureA, creatureB;
    public TextMeshProUGUI nameText, attackText, defenseText, hpText, attacksText;
    public TextMeshProUGUI opponentNameText, opponentHpText;
    public GameObject attackButton;

    private GameObject spawnedCreature;
    private Creature playerCreature, opponentCreature;
    private Dictionary<string, GameObject> creaturePrefabs;
    private bool hasAssignedCreature = false;

    void Start()
    {
        // Try to get ObserverBehaviour component attached to this GameObject
        var observerBehaviour = GetComponent<ObserverBehaviour>();

        if (observerBehaviour == null)
        {
            Debug.LogError("ObserverBehaviour NOT found! Ensure it is correctly attached to the Image Target.");
            observerBehaviour = GetComponentInChildren<ObserverBehaviour>(); 
            Debug.Log("ObserverBehaviour found in children.");

        }
        
        if (observerBehaviour != null)
        {
            Debug.Log("ObserverBehaviour found and event subscribed!");
            observerBehaviour.OnTargetStatusChanged += OnARCardDetected;
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
        }

        // Get the name of the scanned card
        string targetName = behaviour.TargetName; 
        Debug.Log("Card Detected: " + targetName);

        if (creaturePrefabs.TryGetValue(targetName, out GameObject creaturePrefab))
        {
            Debug.Log("Creature Prefab found!");

            // Check if a creature has already been spawned
            if (spawnedCreature == null)
            {
                Debug.Log("Spawning creature...");
                spawnedCreature = PhotonNetwork.Instantiate(creaturePrefab.name, behaviour.transform.position, behaviour.transform.rotation);
                spawnedCreature.transform.parent = behaviour.transform;

                // Assign the Creature component
                playerCreature = spawnedCreature.GetComponent<Creature>();
                if (playerCreature == null)
                {
                    Debug.Log("Creature component not found. Adding...");
                    playerCreature = spawnedCreature.AddComponent<Creature>();
                }

                PhotonView photonView = spawnedCreature.GetComponent<PhotonView>();
                if (photonView.IsMine)
                { 
                    Debug.Log("Creature spawned by local player.");
                    if (targetName == "test2_scaled")
                    {
                        playerCreature.Initialize("Creature A", 
                            10, 8, 100, 
                            new List<string> { "Fireball", "Slash" });
                    }
                    else if (targetName == "test1_scaled")
                    {
                        playerCreature.Initialize("Creature B", 
                            12, 6, 120, 
                            new List<string> { "Ice Blast", "Bite" });
                    }
                    hasAssignedCreature = true;
                    UpdateUI(playerCreature, true);
                    attackButton.SetActive(true); // Show attack button for the player
                }
            }
            else
            {
                Debug.Log("Creature already spawned!");
                UpdateUI(playerCreature, false);
                attackButton.SetActive(false); 
            }
        }
        else
        {
            Debug.LogError("Creature Prefab not found!");
        }
    }

    private void UpdateUI(Creature stats, bool isOwner)
    {
        if (nameText && attackText && defenseText && hpText && attacksText)
        {
            if (isOwner)
            {
                Debug.Log("Updating UI for player creature...");
                nameText.text = $"{PhotonNetwork.NickName}'s Summoned: {stats.creatureName}";
                attackText.text = $"Attack: {stats.attack}";
                defenseText.text = $"Defense: {stats.defense}";
                hpText.text = $"HP: {stats.hp}";
                attacksText.text = $"Attacks: {string.Join(", ", stats.attacks)}";
            }
            else
            {
                Debug.Log("Updating UI for opponent creature...");
                opponentNameText.text = $"Opponent: {stats.creatureName}";
                opponentHpText.text = $"HP: {stats.hp}";
            }
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
