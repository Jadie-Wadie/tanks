using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using UnityEngine.AI;

public class GameControlScript : MonoBehaviour
{
	public enum GameState
	{
		Menu,
		Scores,
		Start,
		Play,
		Over
	};

	[Header("Control")]
	public GameState gameState;

	[Space(10)]

	public int timeToStart = 3;

	[Space(10)]

	public bool isAlive = true;
	public int enemyCount = 4;
	public int powerupCount = 2;

	[Header("Time")]
	public float roundStart = 0f;
	public float roundTime = 0f;

	[Header("GameObjects")]

	public GameObject playerPrefab;
	public GameObject enemyPrefab;
	public GameObject powerupPrefab;

	[Space(10)]

	private GameObject[] playerSpawns;
	private GameObject[] enemySpawns;
	private GameObject[] powerupSpawns;

	[Space(10)]

	private GameObject playerParent;
	private GameObject enemyParent;
	private GameObject powerupParent;

	[Header("UI")]
	public GameObject[] panels;

	[Space(10)]

	public Text countText;

	[Space(10)]

	public Text enemyText;
	public Text timeText;
	public Text highText;

	[Space(10)]

	public Text titleText;
	public Text scoreText;
	public Text recordText;

	[Space(10)]

	public Text fpsText;

	[Space(10)]

	public InputField nameInput;

	[Space(10)]

	private ScoreScript scoreScript;

	[Space(10)]

	private Image globalImage;
	private Image localImage;

	[Header("Debug")]
	private SaveScript saveScript;
	private WebScript webScript;

	[Space(10)]

	public Animator[] animators;

	[Space(10)]

	private float fpsTimer = 0f;

	[Space(10)]

	private int enemiesSpawned = 0;

	void Awake()
	{
		saveScript = GetComponent<SaveScript>();
		webScript = GetComponent<WebScript>();

		playerParent = GameObject.Find("Entities");
		enemyParent = GameObject.Find("Enemies");
		powerupParent = GameObject.Find("Powerups");

		playerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");
		enemySpawns = GameObject.FindGameObjectsWithTag("EnemySpawn");
		powerupSpawns = GameObject.FindGameObjectsWithTag("PowerupSpawn");

		panels = GameObject.FindGameObjectsWithTag("Panel");
		animators = new Animator[panels.Length];
		for (int i = 0; i < panels.Length; i++)
		{
			animators[i] = panels[i].GetComponent<Animator>();
		}

		nameInput = GameObject.Find("NameInput").GetComponent<InputField>();
		nameInput.text = PlayerPrefs.GetString("username", "USR");

		scoreScript = GameObject.Find("ScorePanel").GetComponent<ScoreScript>();

		globalImage = GameObject.Find("Global").GetComponent<Image>();
		localImage = GameObject.Find("Local").GetComponent<Image>();

#if UNITY_WEBGL
        Destroy(localImage.gameObject);
#endif
	}

	void Update()
	{
		switch (gameState)
		{
			case GameState.Menu:
				// Update UI Panels
				ShowPanel("MenuPanel");
				break;

			case GameState.Scores:
				// Update UI Panels
				ShowPanel("ScorePanel");
				break;

			case GameState.Start:
				// Update RoundTime
				roundTime = Time.time - roundStart - timeToStart;

				// Check for Countdown End
				if (roundTime > 0)
				{
					gameState = GameState.Play;

					SpawnPlayer();
					SpawnEnemies();
					SpawnPowerups();
				}

				// Update UI Panels
				ShowPanel("LoadPanel");

				// Update Countdown Text
				countText.text = $"{Mathf.Ceil(Mathf.Abs(roundTime))}";
				break;

			case GameState.Play:
				// Update RoundTime
				roundTime = Time.time - roundStart - timeToStart;

				// Check for Round End
				if (enemyCount == 0 || !isAlive)
				{
					gameState = GameState.Over;

					// Freeze Tanks
					foreach (GameObject tank in FindObjectsWithLayer(LayerMask.GetMask("Tank")))
					{
						DisableTank(tank);
					}

					// Remove Powerups
					foreach (GameObject powerup in GameObject.FindGameObjectsWithTag("Powerup"))
					{
						Destroy(powerup);
					}

					// Remove Bullets
					foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("Bullet"))
					{
						Destroy(bullet);
					}

					// Remove Powerup Ring
					Destroy(GameObject.Find("PowerupRing(Clone)"));

					// Save Score
					if (enemyCount == 0)
					{
						saveScript.AddScore(nameInput.text, (int)Mathf.Floor(roundTime));
						StartCoroutine(webScript.WriteScore(nameInput.text, (int)Mathf.Floor(roundTime)));
#if UNITY_STANDALONE
						saveScript.SaveScoresToFile();
#endif
					}
				}

				// Update UI Panels
				ShowPanel("GamePanel");

				// Update HUD
				countText.text = "";

				enemyText.text = $"{enemyCount} / {enemiesSpawned}";
				timeText.text = FormatTime(roundTime);
				highText.text = saveScript.data.scores.Length > 0 ? FormatTime(saveScript.data.scores[0].score) : FormatTime(0);
				break;

			case GameState.Over:
				// Update UI Panels
				ShowPanel("RestartPanel");

				// Update Menu
				titleText.text = isAlive ? "You win!" : "You lose!";
				scoreText.text = FormatTime(roundTime);
				recordText.text = saveScript.data.scores.Length > 0 ? FormatTime(saveScript.data.scores[0].score) : FormatTime(0);
				break;
		}

		// Display FPS
		if (fpsTimer < Time.time - 0.25f)
		{
			fpsText.text = $"{Mathf.RoundToInt(1f / Time.deltaTime).ToString().PadLeft(3, '0')}";
			fpsTimer = Time.time;
		}
	}

	void SpawnPlayer()
	{
		// Choose a Spawn
		GameObject playerSpawn = playerSpawns[Random.Range(0, playerSpawns.Length)];

		// Create the Object
		GameObject player = Instantiate(playerPrefab, playerSpawn.transform.position, Quaternion.identity);
		player.transform.SetParent(playerParent.transform);

		player.GetComponent<HealthScript>().team = 1;

		// Camera Target
		Camera.main.transform.parent.GetComponent<CameraScript>().target = player.transform;
	}

	void SpawnEnemies()
	{
		enemiesSpawned = 0;

		for (int i = 0; i < enemyCount; i++)
		{
			// Choose a Spawn
			GameObject enemySpawn = enemySpawns[Random.Range(0, enemySpawns.Length)];

			// Create the Object
			GameObject enemy = Instantiate(enemyPrefab, enemySpawn.transform.position, Quaternion.identity);
			enemy.transform.SetParent(enemyParent.transform);

			enemy.GetComponent<HealthScript>().team = 0;

			enemiesSpawned++;
		}
	}

	void SpawnPowerups()
	{
		// Select Unique Spawns
		List<int> indexes = new List<int>();
		for (int i = 0; i < powerupSpawns.Length; i++)
		{
			indexes.Add(i);
		}

		while (indexes.Count() > powerupCount)
		{
			indexes.RemoveAt(Random.Range(0, indexes.Count()));
		}

		// Spawn Powerups
		foreach (int index in indexes)
		{
			GameObject powerupSpawn = powerupSpawns[index];

			GameObject powerup = Instantiate(powerupPrefab, powerupSpawn.transform.position, Quaternion.identity);
			powerup.transform.SetParent(powerupParent.transform);
		}
	}

	public void DisableTank(GameObject tank)
	{
		// Freeze and Remove HealthBar
		tank.GetComponent<Rigidbody>().isKinematic = true;
		tank.GetComponent<HealthScript>().DestroyHealthBar();

		// Disable all Scripts
		foreach (MonoBehaviour script in tank.GetComponents<MonoBehaviour>())
		{
			script.enabled = false;
		}

		// Stop Looking and AI
		if (tank.tag == "Player") tank.transform.Find("TankRenderers/TankTurret").GetComponent<PlayerLookScript>().enabled = false;
		if (tank.tag == "Enemy") tank.GetComponent<NavMeshAgent>().enabled = false;
	}

	public void ShowPanel(string name)
	{
		// Hide all Panels
		foreach (Animator animator in animators)
		{
			animator.SetBool("isVisible", animator.gameObject.name == name);
		}
	}

	public void Restart()
	{
		// Reset Variables
		isAlive = true;
		if (enemiesSpawned > 0) enemyCount = enemiesSpawned;

		roundStart = Time.time;
		roundTime = 0f;

		// Destory old Tanks
		foreach (GameObject tank in FindObjectsWithLayer(LayerMask.GetMask("Tank")))
		{
			Destroy(tank);
		}

		gameState = GameState.Start;
	}

	public void Menu()
	{
		gameState = GameState.Menu;
	}

	public void Scores()
	{
		gameState = GameState.Scores;
		scoreScript.CreateEntries(webScript.scores);
	}

	public void SelectLocal()
	{
		// Check for WebGL
		if (localImage == null) return;

		// Load Entries
		scoreScript.CreateEntries(saveScript.data.scores);

		// Update Button Colour
		localImage.color = new Color(0f, 0f, 0f, 30f / 255f);
		globalImage.color = new Color(0f, 0f, 0f, 20f / 255f);
	}

	public void SelectGlobal()
	{
		// Check for WebGL
		if (localImage == null) return;

		// Load Entries
		scoreScript.CreateEntries(webScript.scores);

		// Update Button Colour
		localImage.color = new Color(0f, 0f, 0f, 20f / 255f);
		globalImage.color = new Color(0f, 0f, 0f, 30f / 255f);
	}

	public void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}

	public void UpdateUsername()
	{
		nameInput.text = nameInput.text.ToUpper();
	}

	public void SaveUsername()
	{
		PlayerPrefs.SetString("username", nameInput.text);
		PlayerPrefs.Save();
	}

	GameObject[] FindObjectsWithLayer(LayerMask layer)
	{
		// Get all GameObjects
		GameObject[] objects = FindObjectsOfType<GameObject>();
		List<GameObject> filtered = new List<GameObject>();

		foreach (GameObject obj in objects)
		{
			// Check Layers
			if (1 << obj.layer == layer.value)
			{
				filtered.Add(obj);
			}
		}

		return filtered.ToArray();
	}

	string FormatTime(float seconds)
	{
		return $"{Mathf.Floor(seconds / 60f).ToString().PadLeft(2, '0')} : {Mathf.Floor(seconds % 60).ToString().PadLeft(2, '0')}";
	}
}