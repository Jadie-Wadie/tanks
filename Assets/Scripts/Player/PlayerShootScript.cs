using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootScript : MonoBehaviour
{
    [Header("Control")]
    public float force = 30f;
    public float delay = 0.25f;

    [Header("GameObjects")]
    public GameObject bulletPrefab;

    [Space(10)]

    public Transform bulletSpawn;

    [Header("Debug")]
    private Transform bulletParent;
    private float prevTime;

    void Awake()
    {
        bulletParent = GameObject.Find("Bullets").transform;
        prevTime = -delay;
    }

    void Update()
    {
        // Check for Input and Cooldown
        if (Input.GetButton("Fire1") && Time.time > prevTime + delay)
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