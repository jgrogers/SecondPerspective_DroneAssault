using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody myRigidbody;
    [SerializeField] private GameObject cannonElevation;
    [SerializeField] private GameObject cannonRotation;
    [SerializeField] private Transform FiringPoint;
    [SerializeField] private GameObject Shell;
    [SerializeField] private float turretMoveRate = 15.0f;
    [SerializeField] private int TreeLayer = 9;
    [SerializeField] private WheelCollider[] leftTracks;
    [SerializeField] private WheelCollider[] rightTracks;
    private float azimuth = 0.0f;
    private float elevation = 15.0f;
    private FireControl fireControl;
    [SerializeField] private float motorTorque = 150.0f;
    public event EventHandler onFireCannon;
    public event EventHandler onForwardBack;
    public event EventHandler onTurn;
    public event EventHandler onTurretTurn;
    public event EventHandler onBarrelAim;
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        cannonRotation.transform.localRotation = Quaternion.Euler(0.0f, azimuth, 0.0f);
        cannonElevation.transform.localRotation = Quaternion.Euler(elevation -90f, 0.0f, 0.0f);
        fireControl = GetComponent<FireControl>();
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate() {
      float leftTorque = 0.0f;
      float rightTorque = 0.0f;
      if (Input.GetKey(KeyCode.W)) {
        onForwardBack?.Invoke(this, EventArgs.Empty);
        leftTorque = motorTorque;
        rightTorque = motorTorque;
      }
      if (Input.GetKey(KeyCode.S)) {
        onForwardBack?.Invoke(this, EventArgs.Empty);
        leftTorque = -motorTorque;
        rightTorque = -motorTorque;
      }
      if (Input.GetKey(KeyCode.A)) {
        onTurn?.Invoke(this, EventArgs.Empty);
        leftTorque = -motorTorque;
        rightTorque = motorTorque;
      }
      if (Input.GetKey(KeyCode.D)) {
        onTurn?.Invoke(this, EventArgs.Empty);
        leftTorque = motorTorque;
        rightTorque = -motorTorque;
      }
      foreach(WheelCollider wheel in leftTracks) {
        wheel.motorTorque = leftTorque;
      }
      foreach(WheelCollider wheel in rightTracks) {
        wheel.motorTorque = rightTorque;
      }
       if (Input.GetKey(KeyCode.Q)) {
        onTurretTurn?.Invoke(this, EventArgs.Empty);
        azimuth -= turretMoveRate * Time.deltaTime;
       }
       if (Input.GetKey(KeyCode.E)) {
        onTurretTurn?.Invoke(this, EventArgs.Empty);
        azimuth += turretMoveRate * Time.deltaTime;
       }
       if (Input.GetKey(KeyCode.LeftShift)) {
        onBarrelAim?.Invoke(this, EventArgs.Empty);
        elevation += turretMoveRate * Time.deltaTime;
       }
       if (Input.GetKey(KeyCode.LeftControl)) {
        onBarrelAim?.Invoke(this, EventArgs.Empty);
        elevation -= turretMoveRate * Time.deltaTime;
       }
        cannonRotation.transform.localRotation = Quaternion.Euler(0.0f,  azimuth, 0.0f);
        cannonElevation.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, elevation);
        if (Input.GetKey(KeyCode.Space)) {
          if (fireControl.Fire()) {
            Debug.Log("Firing shell!");
            GameObject shell = Instantiate(Shell, FiringPoint.position, FiringPoint.rotation);
            shell.GetComponent<Shell>().SetSafeMask(gameObject.layer);
            onFireCannon?.Invoke(this, EventArgs.Empty);
          } 
       }


        
    }
      private void OnCollisionEnter(Collision other) {
        if (other.gameObject.layer == TreeLayer) {
          other.gameObject.AddComponent<Rigidbody>();
          other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(10f, transform.position, 10.0f*5f, 3.0f, ForceMode.Impulse);
          other.gameObject.layer = 10;

        }
    }

}
