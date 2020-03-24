using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PowerupScript : MonoBehaviour
{
    public enum PowerupType
    {
        Speed,
        Shoot,
        Health
    };

    [Header("Control")]
    public PowerupType type;

    [Header("Images")]
    public Sprite[] sprites;

    [Header("Debug")]
    public Image image;

    private void Awake()
    {
        // Set Type and Sprite
        int random = Random.Range(0, (int)Enum.GetValues(typeof(PowerupType)).Cast<PowerupType>().Max() + 1);
        type = (PowerupType)random;
        image.sprite = sprites[random];
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check for Player
        if (other.gameObject.tag == "Player")
        {
            // Get Player Script
            PlayerPowerupScript script = other.GetComponent<PlayerPowerupScript>();
            if (script.isActive) return;

            // Apply Powerup
            switch (type)
            {
                case PowerupType.Speed:
                    script.Speed();
                    break;

                case PowerupType.Shoot:
                    script.Shoot();
                    break;

                case PowerupType.Health:
                    script.Health();
                    break;
            }

            // Destroy
            Destroy(gameObject);
        }
    }
}
