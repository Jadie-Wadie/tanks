using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebScript : MonoBehaviour
{
	[Header("Control")]
	public Score[] scores = new Score[0];

	[Header("Debug")]
	private const string projectID = "portfolio-c5fe5";
	private string URL = $"https://firestore.googleapis.com/v1/projects/{projectID}/databases/(default)/documents";

	void Start()
	{
		StartCoroutine(GetScores());
	}

	public IEnumerator WriteScore(string name, int score)
	{
		string json = "{\"fields\": {\"score\": {\"integerValue\": \"" + score + "\"}, \"name\": {\"stringValue\": \"" + name + "\"} } }";

		using (UnityWebRequest request = UnityWebRequest.Put($"{URL}/scores", json))
		{
			// Create WebRequest
			request.method = UnityWebRequest.kHttpVerbPOST;
			request.SetRequestHeader("Content-Type", "application/json");
			request.SetRequestHeader("Accept", "application/json");

			Debug.Log(request.GetRequestHeader("Content-Type") + " " + request.method);

			yield return request.SendWebRequest();

			// Check for Error
			if (request.error != null)
			{
				Debug.Log("Error: (WriteScore)" + request.error);
			}
		}

		StartCoroutine(GetScores());
	}

	public IEnumerator GetScores()
	{
		using (UnityWebRequest request = UnityWebRequest.Get($"{URL}/scores"))
		{
			// Create WebRequest
			yield return request.SendWebRequest();

			// Check for Error
			if (request.error != null)
			{
				Debug.Log("Error: (GetScores)" + request.error);
			}
			else
			{
				FireRes data = FireRes.CreateFromJSON(request.downloadHandler.text);

				try
				{
					int i = 0;
					scores = new Score[data.documents.Length];
					foreach (FireDoc doc in data.documents)
					{
						scores[i++] = new Score(doc.fields.name.stringValue, doc.fields.score.integerValue);
					}
				} catch (NullReferenceException)
				{
					scores = new Score[0];
				}
			}
		}
	}
}

[System.Serializable]
public class FireRes
{
	public FireDoc[] documents;

	public FireRes(FireDoc[] d)
	{
		documents = d;
	}

	public static FireRes CreateFromJSON(string JSON)
	{
		return JsonUtility.FromJson<FireRes>(JSON);
	}
}

[System.Serializable]
public class FireDoc
{
	public FireFields fields;

	public FireDoc(FireFields f)
	{
		fields = f;
	}

	public static FireDoc CreateFromJSON(string JSON)
	{
		return JsonUtility.FromJson<FireDoc>(JSON);
	}
}

[System.Serializable]
public class FireFields
{
	public FireScore score;
	public FireName name;

	public FireFields(FireScore s, FireName n)
	{
		score = s;
		name = n;
	}

	public static FireFields CreateFromJSON(string JSON)
	{
		return JsonUtility.FromJson<FireFields>(JSON);
	}
}

[System.Serializable]
public class FireScore
{
	public int integerValue;

	public FireScore(int i)
	{
		integerValue = i;
	}

	public static FireScore CreateFromJSON(string JSON)
	{
		return JsonUtility.FromJson<FireScore>(JSON);
	}
}

[System.Serializable]
public class FireName
{
	public string stringValue;

	public FireName(string s)
	{
		stringValue = s;
	}

	public static FireName CreateFromJSON(string JSON)
	{
		return JsonUtility.FromJson<FireName>(JSON);
	}
}
