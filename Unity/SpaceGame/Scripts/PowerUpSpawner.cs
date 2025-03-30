// add to empty gameobject
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject powerUpPrefab;

    public void TrySpawnPowerUp(Vector2 enemyPosition)
    {
        if (PowerUp.CanSpawnPowerUp())
        {
            // Spawn power-up exactly where the enemy was destroyed
            Instantiate(powerUpPrefab, enemyPosition, Quaternion.identity);
        }
    }
}
