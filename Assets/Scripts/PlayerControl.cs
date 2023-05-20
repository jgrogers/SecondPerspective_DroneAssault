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
    private float azimuth = 0.0f;
    private float elevation = -15.0f;
    private FireControl fireControl;
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        cannonRotation.transform.localRotation = Quaternion.Euler(0.0f, azimuth, 0.0f);
        cannonElevation.transform.localRotation = Quaternion.Euler(elevation, 0.0f, 0.0f);
        fireControl = GetComponent<FireControl>();
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate() {
       if (Input.GetKey(KeyCode.W)) {
         myRigidbody.AddRelativeForce(new Vector3(0.0f, 0.0f, 15.0f));
       }
       if (Input.GetKey(KeyCode.S)) {
        myRigidbody.AddRelativeForce(new Vector3(0.0f, 0.0f, -15.0f));
       }
       if (Input.GetKey(KeyCode.A)) {
        myRigidbody.AddRelativeTorque(new Vector3(0.0f, -18.0f, 0.0f));
       }
       if (Input.GetKey(KeyCode.D)) {
        myRigidbody.AddRelativeTorque(new Vector3(0.0f, 18.0f, 0.0f));
       }
       if (Input.GetKey(KeyCode.Q)) {
        azimuth -= turretMoveRate * Time.deltaTime;
        cannonRotation.transform.localRotation = Quaternion.Euler(0.0f, azimuth, 0.0f);
        cannonElevation.transform.localRotation = Quaternion.Euler(elevation, 0.0f, 0.0f);
       }
       if (Input.GetKey(KeyCode.E)) {
        azimuth += turretMoveRate * Time.deltaTime;
        cannonRotation.transform.localRotation = Quaternion.Euler(0.0f, azimuth, 0.0f);
        cannonElevation.transform.localRotation = Quaternion.Euler(elevation, 0.0f, 0.0f);
       }
       if (Input.GetKey(KeyCode.LeftShift)) {
        elevation -= turretMoveRate * Time.deltaTime;
        cannonRotation.transform.localRotation = Quaternion.Euler(0.0f, azimuth, 0.0f);
        cannonElevation.transform.localRotation = Quaternion.Euler(elevation, 0.0f, 0.0f);
       }
       if (Input.GetKey(KeyCode.LeftControl)) {
        elevation += turretMoveRate * Time.deltaTime;
        cannonRotation.transform.localRotation = Quaternion.Euler(0.0f, azimuth, 0.0f);
        cannonElevation.transform.localRotation = Quaternion.Euler(elevation, 0.0f, 0.0f);
       }
        if (Input.GetKey(KeyCode.Space)) {
          if (fireControl.Fire()) {
            Debug.Log("Firing shell!");
            GameObject shell = Instantiate(Shell, FiringPoint.position, FiringPoint.rotation);
            shell.GetComponent<Shell>().SetSafeMask(gameObject.layer);
          } 
       }


        
    }
}
