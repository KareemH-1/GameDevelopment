using UnityEngine;
using System.Collections.Generic;

public class ShipShooting : MonoBehaviour
{
    public GameObject weaponPrefab;
    public Transform firePoint1; // Default fire point x=0 , y =0.35 with respect to the ship
    public Transform firePoint2; // Unlocked on first upgrade x = -0.1 , y =0.15 with respect to ship
    public Transform firePoint3; // Unlocked on second upgrade x = 0.1 , y =0.15 with respect to ship

    private List<Transform> activeFirePoints = new List<Transform>();
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    public AudioSource shootingSound;
    private int upgradeLevel = 0;

    void Start()
    {
        // Start with only the first fire point active
        activeFirePoints.Add(firePoint1);
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && Time.time >= nextFireTime)
        {
            FireWeapon();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FireWeapon()
    {
        foreach (Transform firePoint in activeFirePoints)
        {
            Instantiate(weaponPrefab, firePoint.position, Quaternion.identity);
        }

        if (shootingSound != null)
        {
            shootingSound.Play();
        }
    }

    public void UpgradeWeapon()
    {
        if (upgradeLevel == 0 && firePoint2 != null)
        {
            activeFirePoints.Add(firePoint2);
            upgradeLevel++;
        }
        else if (upgradeLevel == 1 && firePoint3 != null)
        {
            activeFirePoints.Add(firePoint3);
            upgradeLevel++;
        }
    }
}
