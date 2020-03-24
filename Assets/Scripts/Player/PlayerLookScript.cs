using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLookScript : MonoBehaviour
{
    [Header("Control")]
    public LayerMask layerMask;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

		// Look at Cursor
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Vector3 targetPostition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(targetPostition);
        }
    }
}