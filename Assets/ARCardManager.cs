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

    private Dictionary<string, GameObject> spawnedCreatures = new Dictionary<string, GameObject>();
    private Creature playerCreature, opponentCreature;
    private Dictionary<string, GameObject> creaturePrefabs;
    private bool hasAssignedCreature = false;

    void Start()
    {
        // Try to get ObserverBehaviour component attached to this GameObject
        var observerBehaviour = GetComponentInChildren<ObserverBehaviour>();

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
            {"test1_scaled", creatureA},
            {"test2_scaled", creatureB}
        };
        attackButton.SetActive(false);
        Debug.Log("Creature Data initialized successfully.");
        Debug.Log("Creature Prefabs: " + creaturePrefabs.Count);
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
            Debug.Log("Creature Prefab found!");

            // Check if a creature has already been spawned
            if (!hasAssignedCreature)
            {
                if (!spawnedCreatures.ContainsKey(targetName))
                {
                    Debug.Log($"Creature Prefab found for {targetName}!");
                    GameObject newCreature = PhotonNetwork.Instantiate(creaturePrefab.name, behaviour.transform.position, behaviour.transform.rotation);
                    newCreature.transform.parent = behaviour.transform;

                    spawnedCreatures[targetName] = newCreature; // Store in dictionary
                    PhotonView photonView = newCreature.GetComponent<PhotonView>();
                    Debug.Log($"PhotonView IsMine: {photonView.IsMine}");

                    if (photonView.IsMine)
                    {
                        playerCreature = newCreature.GetComponent<Creature>();
                        hasAssignedCreature = true;

                        if (targetName == "test2_scaled")
                        {
                            playerCreature.Initialize("Creature A", 10, 8, 100, new List<string> { "Fireball", "Slash" });
                        }
                        else if (targetName == "test1_scaled")
                        {
                            playerCreature.Initialize("Creature B", 12, 6, 120, new List<string> { "Ice Blast", "Bite" });
                        }

                        UpdateUI(playerCreature, true);
                        attackButton.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("Opponent's creature detected.");
                        opponentCreature = newCreature.GetComponent<Creature>();
                        UpdateUI(opponentCreature, false);
                    }
                }
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
