//add to Power-Up prefab and make sure that it has a Collider2D (Box or Circle Collider) with "Is Trigger" enabled.
using UnityEngine;
using TMPro; // Import TMP


public class PowerUp : MonoBehaviour
{
    public float fallSpeed = 2f;
    private static int maxUpgradesPerWeapon = 2;
    private static int currentUpgrades = 0;
    private static TextMeshProUGUI upgradeText; //Make sure to have a TMP object in the scene with the name of "Upgrade Text"

    private void Start()
    {

        if (upgradeText == null)
        {
            upgradeText = FindFirstObjectByType<TextMeshProUGUI>(); 
        }

        UpdateUpgradeText();
    }

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
        if (collision.CompareTag("Player") && currentUpgrades < maxUpgradesPerWeapon)
        {
            ShipShooting shipShooting = collision.GetComponent<ShipShooting>();
            if (shipShooting != null)
            {
                shipShooting.UpgradeWeapon();
                currentUpgrades++;
                UpdateUpgradeText();
            }

            Destroy(gameObject);
        }
    }

    private void UpdateUpgradeText()
    {
        if (upgradeText != null)
        {
            upgradeText.text = "Upgrade Level: " + currentUpgrades + " / " + maxUpgradesPerWeapon;
        }
    }
}
