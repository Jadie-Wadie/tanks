using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Control")]
    public Transform target;
    
    [Space(10)]

    public float damp = 0.2f;

    [Header("Zoom")]
    public float zoomSpeed = 5f;

    [Space(10)]

    public float minZoom = 10f;
    public float maxZoom = 20f;

    [Header("Debug")]
    private Vector3 velocity;
    private Vector3 targetPos;

    public GameControlScript gameControlScript;

    private void Start()
    {
        gameControlScript = GameObject.Find("GameController").GetComponent<GameControlScript>();
    }

    private void Update()
    {
		// Get Input
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float zoom = Camera.main.orthographicSize - scroll * zoomSpeed;

        // Only Scroll in Game
        if (gameControlScript.gameState == GameControlScript.GameState.Play)
        {
            // Scroll within Bounds
            if (minZoom < zoom && zoom < maxZoom)
            {
                Camera.main.orthographicSize = zoom;
            }
        }
    }

    private void FixedUpdate()
    {
		// Follow Target
        if (target != null)
        {
            targetPos = target.position;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, damp);
        }
    }
}