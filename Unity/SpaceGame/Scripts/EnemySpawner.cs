//attach to empty gameobject
using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float minSpawnTime = 2f, maxSpawnTime = 5f;
    private float screenLeft, screenRight, screenTop;
    private bool isSpawning = true; // New flag to stop spawning

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    void Update()
    {
        CalculateScreenBounds();
    }

    void CalculateScreenBounds()
    {
        Camera cam = Camera.main;
        screenLeft = cam.ScreenToWorldPoint(Vector3.zero).x;
        screenRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        screenTop = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
    }

    IEnumerator SpawnEnemies()
    {
        while (isSpawning) // Only spawn when allowed
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSecondsRealtime(waitTime);

            int enemyCount = Random.Range(1, 4);
            for (int i = 0; i < enemyCount; i++)
            {
                float randomX = Random.Range(screenLeft + 0.5f, screenRight - 0.5f);
                Vector3 spawnPosition = new Vector3(randomX, screenTop + 1f, 0f);
                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    public void StopSpawning() //stop enemy spawning
    {
        isSpawning = false;
    }
}
