using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject gameController;

    [Space(10)]

    public GameObject entryPrefab;
    public GameObject entryParent;

    private void Start()
    {
        entryParent.GetComponent<GridLayoutGroup>().cellSize = new Vector3(-entryParent.GetComponent<RectTransform>().sizeDelta.x, 50f);
    }

    public void CreateEntries(Score[] scores)
    {
        foreach (Transform child in entryParent.transform)
        {
            Destroy(child.gameObject);
        }
        
        Array.Sort(scores, new ScoreComparer());

        for (int i = 0; i < scores.Length; i++)
        {
            Score score = scores[i];

            GameObject entry = Instantiate(entryPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            entry.transform.SetParent(entryParent.transform);

            ScoreEntryScript entryScript = entry.GetComponent<ScoreEntryScript>();
            entryScript.username = score.name;
            entryScript.score = score.score;
        }

        entryParent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, (scores.Length - 1) * 50);
    }
}

public class ScoreComparer : IComparer<Score>
{
    public int Compare(Score a, Score b)
    {
        return a.score - b.score;
    }
}
