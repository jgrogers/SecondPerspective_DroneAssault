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
    [SerializeField] private WheelCollider[] leftTracks;
    [SerializeField] private WheelCollider[] rightTracks;
    private float azimuth = 0.0f;
    private float elevation = 15.0f;
    private FireControl fireControl;
    [SerializeField] private float motorTorque = 150.0f;
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
        leftTorque = motorTorque;
        rightTorque = motorTorque;
      }
      if (Input.GetKey(KeyCode.S)) {
        leftTorque = -motorTorque;
        rightTorque = -motorTorque;
      }
      if (Input.GetKey(KeyCode.A)) {
        leftTorque = -motorTorque;
        rightTorque = motorTorque;
      }
      if (Input.GetKey(KeyCode.D)) {
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
        azimuth -= turretMoveRate * Time.deltaTime;
       }
       if (Input.GetKey(KeyCode.E)) {
        azimuth += turretMoveRate * Time.deltaTime;
       }
       if (Input.GetKey(KeyCode.LeftShift)) {
        elevation += turretMoveRate * Time.deltaTime;
       }
       if (Input.GetKey(KeyCode.LeftControl)) {
        elevation -= turretMoveRate * Time.deltaTime;
       }
        cannonRotation.transform.localRotation = Quaternion.Euler(0.0f,  azimuth, 0.0f);
        cannonElevation.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, elevation);
        if (Input.GetKey(KeyCode.Space)) {
          if (fireControl.Fire()) {
            Debug.Log("Firing shell!");
            GameObject shell = Instantiate(Shell, FiringPoint.position, FiringPoint.rotation);
            shell.GetComponent<Shell>().SetSafeMask(gameObject.layer);
          } 
       }


        
    }
}
