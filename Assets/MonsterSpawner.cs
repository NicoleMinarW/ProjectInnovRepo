using UnityEngine;
using Vuforia;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab; // Assign your 3D monster in Inspector
    private GameObject spawnedMonster;
    private bool isTracking = false;

    void Start()
    {
        var trackable = GetComponent<ObserverBehaviour>();
        trackable.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
        {
            if (!isTracking)
            {
                SpawnMonster();
                isTracking = true;
            }
        }
        else
        {
            if (isTracking)
            {
                DestroyMonster();
                isTracking = false;
            }
        }
    }

    void SpawnMonster()
    {
        if (monsterPrefab != null && spawnedMonster == null)
        {
            spawnedMonster = Instantiate(monsterPrefab, transform.position, transform.rotation);
            spawnedMonster.transform.SetParent(transform);
        }
    }

    void DestroyMonster()
    {
        if (spawnedMonster != null)
        {
            Destroy(spawnedMonster);
        }
    }
}