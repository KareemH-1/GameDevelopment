//This script allows ship to shoot the rocket prefab with a 0.5s delay between shots.
//The shooting sound plays on shot.
// Make sure to asign a firepoint and adjust its spawn location , i set mine to x=-0.1 ,y =0.15, z=0 with respect to the ship (it should be a child of it)
using UnityEngine;

public class ShipShooting : MonoBehaviour
{
    public GameObject weaponPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    public AudioSource shootingSound;

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
        Instantiate(weaponPrefab, firePoint.position, Quaternion.identity);

        if (shootingSound != null)
        {
            shootingSound.Play();
        }
    }
}
using UnityEngine;

public class ShipShooting : MonoBehaviour
{
    public GameObject weaponPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    public AudioSource shootingSound;

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
        Instantiate(weaponPrefab, firePoint.position, Quaternion.identity);

        if (shootingSound != null)
        {
            shootingSound.Play();
        }
    }
}
