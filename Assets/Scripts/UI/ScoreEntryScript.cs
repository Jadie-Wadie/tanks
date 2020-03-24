using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreEntryScript : MonoBehaviour
{
    [Header("Control")]
    public string username = "USR";
    public int score = 64;

    [Header("GameObjects")]
    public Text userText;
    public Text scoreText;

    void Update()
    {
        userText.text = username;
        scoreText.text = FormatTime(score);
    }

    string FormatTime(float seconds)
    {
        return $"{Mathf.Floor(seconds / 60f).ToString().PadLeft(2, '0')} : {Mathf.Floor(seconds % 60).ToString().PadLeft(2, '0')}";
    }
}
