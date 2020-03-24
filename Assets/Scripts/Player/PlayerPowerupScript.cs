using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class PlayerPowerupScript : MonoBehaviour
{
	[Header("Control")]
	public bool isActive = false;
	public float duration = 5f;

	[Space(10)]

	public float speed = 18f;
	public float shoot = 0.125f;
	public float health = 20;

	[Space(10)]

	public float saveSpeed;
	public float saveShoot;

	[Header("GameObjects")]
	public GameObject ringPrefab;

	[Space(10)]

	public UICircle ring;

	[Header("Debug")]
	public float currentTime = 0f;
	public float startTime = 0f;

	void Start()
	{
		// Spawn Ring
		ring = Instantiate(ringPrefab, transform.position, Quaternion.Euler(90f, 180f, 0f)).GetComponent<UICircle>();
		ring.transform.SetParent(GameObject.Find("World").transform);

		// Save Normal Values
		saveSpeed = GetComponent<PlayerMoveScript>().speed;
		saveShoot = GetComponent<PlayerShootScript>().delay;
		GetComponent<HealthScript>().healing = health;

		startTime = -duration;
	}

	void Update()
	{
		// Move the Ring
		ring.transform.position = transform.position + new Vector3(0f, 0.01f, 0f);

		if (isActive)
		{
			currentTime = Time.time - startTime;
			if (duration < currentTime)
			{
				isActive = false;

				// Reset Values
				GetComponent<PlayerMoveScript>().speed = saveSpeed;
				GetComponent<PlayerShootScript>().delay = saveShoot;
				GetComponent<HealthScript>().isHealing = false;

				// Hide Ring
				ring.Arc = 0;
				ring.SetVerticesDirty();
			}
			else
			{
				// Update Ring
				ring.Arc = 1 - currentTime / duration;
				ring.SetVerticesDirty();
			}
		}
	}

	public void Speed()
	{
		isActive = true;
		startTime = Time.time;

		// Set Speed
		GetComponent<PlayerMoveScript>().speed = speed;
	}

	public void Shoot()
	{
		isActive = true;
		startTime = Time.time;

		// Set Shoot Cooldown
		GetComponent<PlayerShootScript>().delay = shoot;
	}

	public void Health()
	{
		isActive = true;
		startTime = Time.time;

		// Enable Healing
		GetComponent<HealthScript>().isHealing = true;
	}
}
