using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public float ShellVelocity = 50.0f;
    public float ShellLifetime = 10.0f;
    public float EnemyAimingErrorRadius = 10.0f;
    public float EnemyDetectionRadius = 100.0f;
    public float EnemyWheelTorque = 150.0f;
    public float EnemyTurretSpeed = 20.0f;
    public float DroneSpeed = 15.0f;
    public float DroneBatteryLife = 120.0f;

    public List<GameObject> PlayerTeam;
    public List<GameObject> EnemyTeam;
    
    // Start is called before the first frame update
    private void Awake() 
    { 
    // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Debug.LogError("Only one game manager please!");
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    void Start()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
        PlayerTeam = new List<GameObject>();
        EnemyTeam = new List<GameObject>();
        foreach(GameObject enemy in enemies) {
            EnemyTeam.Add(enemy);
        }
        foreach(GameObject ally in allies) {
            PlayerTeam.Add(ally);
        }
        Debug.Log("Have " + EnemyTeam.Count + " enemies and " + PlayerTeam.Count + " allies");
     }
     public bool CheckObjectExists(GameObject go) {
        return PlayerTeam.Contains(go) || EnemyTeam.Contains(go);
     }

    public void RemoveObject(GameObject go) {
        PlayerTeam.RemoveAll(t => t == go);
        EnemyTeam.RemoveAll(t => t == go);
        Debug.Log("Have " + EnemyTeam.Count + " enemies and " + PlayerTeam.Count + " allies");
    }
}
