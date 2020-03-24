using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    [Header("Control")]
    public GameObject quitButton;

    [Space(10)]

    public RectTransform scoreButton;
    public RectTransform scoreText;

    void Start()
    {
#if UNITY_WEBGL
        Vector2 position = scoreButton.anchoredPosition;
        position.x = 0;
        scoreButton.anchoredPosition = position;
        scoreButton.sizeDelta = new Vector2(380f, 65f);

        scoreText.GetComponent<Text>().fontSize = 36;

        Destroy(quitButton);
#endif
    }
}