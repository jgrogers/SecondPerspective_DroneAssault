using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPaths : MonoBehaviour
{
    [SerializeField] private List<Vector3>[] patrol_paths;
    static public PatrolPaths Instance {get; private set;}
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this) {
            Debug.LogError("Only one patrol paths please");
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }
    public List<Vector3> GetPatrolPath(int ind) {
        return patrol_paths[ind];
    }
 
}
