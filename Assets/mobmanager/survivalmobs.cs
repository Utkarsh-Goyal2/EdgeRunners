using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this for TextMeshPro

public class survivalmobs : MonoBehaviour
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
        public int spawnPointGroupIndex;
    }

    [System.Serializable]
    public struct Wave
    {
        public List<MobGroup> mobGroups;
    }

    [Header("Spawn Points")]
    public SpawnPointGroup[] spawnPointGroups;
    public Wave wave;

    [Header("Wave Display UI")] // Add this section
    public TMP_Text waveText; // Assign your wave display text
    public string waveTextPrefix = "Wave: "; // "Wave: 1", "Wave: 2", etc.

    private List<GameObject> aliveMobs = new List<GameObject>();
    private int currentWaveIndex = 0;

    void Start()
    {
        StartNextWave();
    }

    public void StartNextWave()
    {
        currentWaveIndex++;

        // Update wave display directly here
        UpdateWaveDisplay();

        StartCoroutine(SpawnWave());
    }

    void UpdateWaveDisplay()
    {
        if (waveText != null)
        {
            waveText.text = waveTextPrefix + currentWaveIndex;
        }
    }

    System.Collections.IEnumerator SpawnWave()
    {
        int count = Random.Range(1, currentWaveIndex);
        foreach (var mobGroup in wave.mobGroups)
        {
            if (mobGroup.spawnPointGroupIndex >= spawnPointGroups.Length)
            {
                Debug.LogError($"Invalid spawnPointGroupIndex: {mobGroup.spawnPointGroupIndex}");
                continue;
            }

            List<Transform> currentSpawnPoints = spawnPointGroups[mobGroup.spawnPointGroupIndex].spawnPoints;
            for (int i = 0; i < count; i++)
            {
                Transform spawnPoint = currentSpawnPoints[Random.Range(0, currentSpawnPoints.Count)];
                GameObject mobInstance = Instantiate(mobGroup.mobPrefab, spawnPoint.position, spawnPoint.rotation);
                ISurvivalMob script = mobInstance.GetComponent<ISurvivalMob>();
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
        if (aliveMobs.Count == 0)
        {
            Debug.Log("Wave clear! Starting next wave.");
            StartNextWave();
        }
    }
}
