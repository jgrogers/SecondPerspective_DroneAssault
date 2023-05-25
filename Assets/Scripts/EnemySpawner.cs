using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject thingToSpawn;
    // Start is called before the first frame update
    void Awake()
    {
       foreach (Transform t in GetComponentInChildren<Transform>()) {
            GameObject thing = Instantiate(thingToSpawn, t.position, Quaternion.identity);
            t.GetComponentInChildren<MeshRenderer>().enabled = false;
       } 
    }

}
