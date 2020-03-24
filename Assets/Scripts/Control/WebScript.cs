using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebScript : MonoBehaviour
{
    [Header("Control")]
    public Scores data;

    [Header("Debug")]
    private string URL = "https://tanks.roundsquare.site";

    IEnumerator Start()
    {
#if UNITY_WEBGL
        URL = "";
#endif
        yield return StartCoroutine(GetScores());
    }

    public IEnumerator WriteScore(string name, int score)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("score", score);

        using (UnityWebRequest request = UnityWebRequest.Post(URL + "/writeScore", form))
        {
            // Create WebRequest
            yield return request.SendWebRequest();

            // Check for Error
            if (request.error != null)
            {
                Debug.Log("Error: (/writeScores)" + request.error);
            }
        }

        yield return StartCoroutine(GetScores());
    }

    public IEnumerator GetScores()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(URL + "/readScores"))
        {
            // Create WebRequest
            yield return request.SendWebRequest();

            // Check for Error
            if (request.error != null)
            {
                Debug.Log("Error: (/readScores)" + request.error);
            }
            else
            {
                data = Scores.CreateFromJSON(request.downloadHandler.text);
            }
        }
    }
}

[System.Serializable]
public class Scores
{
    public Score[] scores = new Score[0];

    public static Scores CreateFromJSON(string JSON)
    {
        return JsonUtility.FromJson<Scores>(JSON);
    }
}

[System.Serializable]
public class Score
{
    public string name = "ERR";
    public int score = 404;

    public Score(string _name, int _score)
    {
        name = _name;
        score = _score;
    }

    public static Score CreateFromJSON(string JSON)
    {
        return JsonUtility.FromJson<Score>(JSON);
    }
}