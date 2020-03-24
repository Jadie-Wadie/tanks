using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SaveScript : MonoBehaviour
{
	[Header("Control")]
	public Scores data;
	public string fileName = "scores.json";

	[Header("Debug")]
	private string currentDirectory;

	void Awake()
	{
		// Set Directory
		currentDirectory = Application.dataPath + "/Data";

#if UNITY_STANDALONE
		LoadScoresFromFile();
#endif
	}

	public void LoadScoresFromFile()
	{
		// Try Reading File
		bool fileExists = File.Exists(currentDirectory + "\\" + fileName);
		if (!fileExists) return;

		StreamReader streamReader;
		try
		{
			streamReader = new StreamReader(currentDirectory + "\\" + fileName);
		}
		catch (Exception e)
		{
			Debug.Log(e.Message);
			return;
		}

		// Read
		string text = streamReader.ReadToEnd();
		data = Scores.CreateFromJSON(text);
		
		// Error Checking
		if (data == null) data = new Scores();

		// Close
		streamReader.Close();
	}

	public void SaveScoresToFile()
	{
		// Create Folder
		Directory.CreateDirectory(currentDirectory);

		// Try Writing File
		StreamWriter streamWriter;
		try
		{
			streamWriter = new StreamWriter(currentDirectory + "\\" + fileName);
		}
		catch (Exception e)
		{
			Debug.Log(e.Message);
			return;
		}

		// Write Scores
		string json = JsonUtility.ToJson(data, true);
		streamWriter.Write(json);

		// Close
		streamWriter.Close();
	}

	public void AddScore(string name, int value)
	{
		// Clone Array
		Score[] temp = data.scores;
		data.scores = new Score[temp.Length + 1];

		// Populate new Array
		int i = 0;
		foreach (Score score in temp)
		{
			data.scores[i] = temp[i];
			i++;
		}

		// Add new Score
		data.scores[temp.Length] = new Score(name, value);
	}
}
