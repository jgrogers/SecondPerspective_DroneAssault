using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneDock : MonoBehaviour
{
    [SerializeField] private Transform DockingPort;
    public Transform GetDockingPort() {return DockingPort;}
}
