using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPaths : MonoBehaviour
{
    public static PatrolPaths Instance { get; private set; }
    [System.Serializable]
    public struct PatrolPath {
        public List<Transform> waypoints;
    }
    [SerializeField] private PatrolPath[] patrol_paths;
    [SerializeField] private float test;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this) {
            Debug.LogError("Only one patrol paths please");
            Destroy(gameObject);
        } else {
            Instance = this;
        }
        foreach (PatrolPath p in patrol_paths) {
            foreach (Transform t in p.waypoints) {
                t.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
            }
        }
    }
    public PatrolPath GetPatrolPath(int ind) {
        return patrol_paths[ind];
    }
 
}
