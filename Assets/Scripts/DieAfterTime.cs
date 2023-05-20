using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieAfterTime : MonoBehaviour
{
    [SerializeField] float deathTimer = 1.0f;
    // Update is called once per frame
    void Update()
    {
        deathTimer -= Time.deltaTime;
        if (deathTimer <= 0) Destroy(gameObject);
        
    }
}
