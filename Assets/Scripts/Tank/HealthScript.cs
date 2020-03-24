using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{
    [Header("Control")]
    public int team;

    [Space(10)]

    public float health = 100f;
    public bool isDead = false;

    [Space(10)]

    public bool isHealing = false;
    public float healing = 0f;

    [Space(10)]

    public float healthBarHeight = 3f;

    [Header("GameObjects")]
    public GameObject explosionPrefab;
    private GameObject explosionParent;

    [Space(10)]

    public GameObject healthBarPrefab;
    public GameObject healthBarParent;

    [Header("Debug")]
    private GameControlScript gameControlScript;

    private float initialHealth;

    private GameObject healthBar;
    private Slider healthBarScript;

    private float lastHeal;

    void Awake()
    {
        explosionParent = GameObject.Find("Explosions");
        gameControlScript = GameObject.Find("GameController").GetComponent<GameControlScript>();

        healthBarParent = GameObject.Find("HealthBars");

        healthBar = Instantiate(healthBarPrefab, transform.position + new Vector3(0f, healthBarHeight, 0f), Quaternion.Euler(40f, 60f, 0f));
        healthBar.transform.SetParent(healthBarParent.transform);
        healthBarScript = healthBar.GetComponent<Slider>();

        initialHealth = health;
    }

    void Update()
    {
        // Translate HealthBar
        healthBar.transform.position = transform.position + new Vector3(0f, healthBarHeight, 0f);

        // Check Heal Cooldown
        if (isHealing)
        {
            if (lastHeal < Time.time - 1f)
            {
                // Apply Healing
                health = Mathf.Min(initialHealth, health + healing);
                healthBarScript.value = Mathf.Max(0f, health / initialHealth);

                lastHeal = Time.time;
            }
        }
    }

    public float Damage(int damage)
    {
        // Apply Damage
        health -= damage;
        healthBarScript.value = Mathf.Max(0f, health / initialHealth);
        
        // Check for Death
        if (health <= 0 && !isDead)
        {
            isDead = true;

            // Update GameControlScript
            if (gameObject.tag == "Player") gameControlScript.isAlive = false;
            if (gameObject.tag == "Enemy") gameControlScript.enemyCount--;

            Explode();
        }

        return health;
    }

    public void Explode()
    {
        // Spawn Explosion
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.transform.SetParent(explosionParent.transform);

        // Destroy
        Destroy(gameObject);
        Destroy(healthBar);
    }

    public void DestroyHealthBar()
    {
        Destroy(healthBar);
    }
}