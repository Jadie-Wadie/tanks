using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [Header("Control")]
    public GameObject owner;
    public int team;

    [Space(10)]

    public int damage = 25;

    [Header("GameObjects")]
    public GameObject explosionPrefab;
    private GameObject explosionParent;

    void Start()
    {
        explosionParent = GameObject.Find("Explosions");
    }

    private void OnTriggerEnter(Collider other)
    {
		// Ignore Owner
        if (other.isTrigger) return;
        if (ReferenceEquals(other.gameObject, owner)) return;

		// Get Components
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        HealthScript healthScript = other.gameObject.GetComponent<HealthScript>();

		// Check for Tank
        if (rb != null && healthScript != null)
        {
			// Don't Damage Team
            if (healthScript.team != team)
            {
                healthScript.Damage(damage);
            }
        }

        Hit();
    }

    private void Hit()
    {
		// Spawn Explosion
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.transform.SetParent(explosionParent.transform);

        Destroy(gameObject);
    }
}