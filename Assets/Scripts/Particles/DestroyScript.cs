using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyScript : MonoBehaviour
{
    [Header("Control")]
    public float lifeTime = 1f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
