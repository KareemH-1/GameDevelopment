//add to Power-Up prefab and make sure that it has a Collider2D (Box or Circle Collider) with "Is Trigger" enabled.
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float fallSpeed = 2f;
    private static int maxUpgradesPerWeapon = 2;
    private static int currentUpgrades = 0;

    public static bool CanSpawnPowerUp()
    {
        return currentUpgrades < maxUpgradesPerWeapon && Random.Range(0f, 1f) <= 0.05f;
    }

    private void Update()
    {

        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);


        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<ShipShooting>().UpgradeWeapon(); // Upgrade weapon
            Destroy(gameObject);
        }
    }
}
