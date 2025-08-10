using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobManager1 : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnPointGroup
    {
        public List<Transform> spawnPoints;
    }

    [System.Serializable]
    public struct MobGroup
    {
        public GameObject mobPrefab;
        public int count;
        public int spawnPointGroupIndex;
    }

    [System.Serializable]
    public struct Wave
    {
        public List<MobGroup> mobGroups;
    }

    [Header("Wave Configuration")]
    public Wave[] waves;
    [Header("Spawn Points")]
    public SpawnPointGroup[] spawnPointGroups;
    [Header("UI Panels")]
    public GameObject youWonPanel;
    [Header("Level Progression")]
    public Database database; // Add this reference

    private List<GameObject> aliveMobs = new List<GameObject>();
    private int currentWaveIndex = -1;

    void Start()
    {
        StartNextWave();
    }

    public void StartNextWave()
    {
        currentWaveIndex++;
        if (currentWaveIndex >= waves.Length)
        {
            // FIXED: Don't pause time when level completes
            youWonPanel.SetActive(true);
            Debug.Log("All waves completed! Level finished.");

            // Update progress when level is actually completed
            if (database != null)
            {
                ProgressService.IncrementClearedLevelsAndAutoUnlock(database);
            }

            return;
        }

        StartCoroutine(SpawnWave(waves[currentWaveIndex]));
    }

    System.Collections.IEnumerator SpawnWave(Wave wave)
    {
        foreach (var mobGroup in wave.mobGroups)
        {
            if (mobGroup.spawnPointGroupIndex >= spawnPointGroups.Length)
            {
                Debug.LogError($"Invalid spawnPointGroupIndex: {mobGroup.spawnPointGroupIndex}");
                continue;
            }

            List<Transform> currentSpawnPoints = spawnPointGroups[mobGroup.spawnPointGroupIndex].spawnPoints;
            for (int i = 0; i < mobGroup.count; i++)
            {
                Transform spawnPoint = currentSpawnPoints[Random.Range(0, currentSpawnPoints.Count)];
                GameObject mobInstance = Instantiate(mobGroup.mobPrefab, spawnPoint.position, spawnPoint.rotation);
                IMob script = mobInstance.GetComponent<IMob>();
                script.SetManager(this);
                aliveMobs.Add(mobInstance);

                yield return new WaitForSeconds(0.2f);
            }
        }
        yield return null;
    }

    public void MobDied(GameObject mob)
    {
        aliveMobs.Remove(mob);
        if (aliveMobs.Count == 0 && currentWaveIndex < waves.Length)
        {
            Debug.Log("Wave clear! Starting next wave.");
            StartNextWave();
        }
    }
}
