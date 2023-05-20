using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneControl : MonoBehaviour
{
    private Rigidbody myRigidbody;
    // Start is called before the first frame update
    void Awake()
    {
       myRigidbody = GetComponent<Rigidbody>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate() {
        if (Input.GetKey(KeyCode.UpArrow)) {
         myRigidbody.AddRelativeForce(new Vector3(0.0f, 0.0f, 5.0f));
       }
       if (Input.GetKey(KeyCode.DownArrow)) {
        myRigidbody.AddRelativeForce(new Vector3(0.0f, 0.0f, -5.0f));
       }
       if (Input.GetKey(KeyCode.LeftArrow)) {
        myRigidbody.AddRelativeForce(new Vector3(-5.0f, 0.0f, 0.0f));
       }
       if (Input.GetKey(KeyCode.RightArrow)) {
        myRigidbody.AddRelativeForce(new Vector3(5.0f, 0.0f, 0.0f));
       }
    }
}
