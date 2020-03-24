using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    [Header("Control")]
    public Color color;

    void OnDrawGizmos()
    {
		// Render Sphere
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, 1f);
    }
}