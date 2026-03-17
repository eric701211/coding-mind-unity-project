using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; // Ensure this namespace is present!

public class WaveSpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    public int currentWave = 1;
    public int baseZombiesToSpawn = 5;
    public float timeBetweenWaves = 5f;
    public float spawnDelay = 1f;

    [Header("References")]
    public GameObject zombiePrefab;
    public float spawnRadius = 30f;
    public Text waveText; // Ensure this is public

    private int zombiesToSpawn;
    private bool isSpawning = false;

    void Start()
    {
        zombiesToSpawn = baseZombiesToSpawn;
        UpdateWaveUI(); // Initialize the text immediately
    }

    void Update()
    {
        int zombiesAlive = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (zombiesAlive == 0 && !isSpawning)
        {
            StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;
        
        // Update UI at the very start of the countdown
        UpdateWaveUI(); 

        yield return new WaitForSeconds(timeBetweenWaves);

        for (int i = 0; i < zombiesToSpawn; i++)
        {
            SpawnSingleZombie();
            yield return new WaitForSeconds(spawnDelay);
        }

        currentWave++;
        zombiesToSpawn += 3;
        isSpawning = false;
    }

    void SpawnSingleZombie()
    {
        Vector3 randomPoint = GetRandomSpawnPoint();
        Instantiate(zombiePrefab, randomPoint, Quaternion.identity);
    }

    // New helper method to ensure UI stays in sync
    void UpdateWaveUI()
    {
        if (waveText != null)
        {
            waveText.text = "WAVE: " + currentWave;
        }
    }

    Vector3 GetRandomSpawnPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection += transform.position; 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, spawnRadius, 1))
        {
            return hit.position;
        }
        return transform.position; 
    }
}