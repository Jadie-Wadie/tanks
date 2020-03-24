using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootScript : MonoBehaviour
{
    [Header("Control")]
    public float force = 30f;
    public float delay = 1f;

    [Space(10)]

    public bool canShoot = false;

    [Header("GameObjects")]
    public GameObject bulletPrefab;

    [Space(10)]

    public Transform bulletSpawn;

    [Header("Debug")]
    private Transform bulletParent;

    [Space(10)]

    private float prevTime = 0f;

    void Awake()
    {
        bulletParent = GameObject.Find("Bullets").transform;
    }

    void Update()
    {
        if (canShoot)
        {
			// Wait for Cooldown
            if (Time.time > prevTime + delay)
            {
				// Spawn Bullet
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
                bullet.transform.SetParent(bulletParent.transform);

				// Set Bullet Owner and Team
                BulletScript bulletScript = bullet.GetComponent<BulletScript>();
                bulletScript.owner = gameObject;
                bulletScript.team = GetComponent<HealthScript>().team;

				// Apply Force
                bullet.GetComponent<Rigidbody>().velocity = force * bulletSpawn.forward;

                prevTime = Time.time;
            }
        }
    }
}
