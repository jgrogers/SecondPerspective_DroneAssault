using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireControl : MonoBehaviour
{
    [SerializeField] private float reloadTime = 4.0f;
    private float reloadPercent = 1.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
       if (reloadPercent < 1.0f) reloadPercent += Time.deltaTime/ reloadTime; 
       if (reloadPercent > 1.0f) reloadPercent = 1.0f;
    }
    public float GetReloadPercent() {
        return reloadPercent;
    }
    public bool Fire() {
        if (reloadPercent >= 1.0f) {
            reloadPercent = 0f;
            return true;
        } else return false;
    }
}
