using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneControl : MonoBehaviour
{
    [SerializeField] private float MotionForce = 15.0f;
    [SerializeField] private Transform Mothership;
    [SerializeField] private float Charge = 120.0f;
    [SerializeField] private float MaxCharge = 120.0f;
    [SerializeField] private bool Launched = true;
    [SerializeField] private bool LandingMode = false;
    private Rigidbody myRigidbody;
    private SphereCollider myCollider;
    private DroneDock mothershipDockingPort;
    // Start is called before the first frame update
    void Awake()
    {
       myRigidbody = GetComponent<Rigidbody>(); 
       myCollider = GetComponent<SphereCollider>();
       mothershipDockingPort = Mothership.GetComponent<DroneDock>();
       Land();
    }

    void Takeoff() {
        transform.parent = null;
        myCollider.enabled = true;
        myRigidbody = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
        myRigidbody.useGravity = false;
        myRigidbody.mass = 1.0f;
        myRigidbody.drag = 1.0f;
        myRigidbody.angularDrag = 1.0f;
        Launched = true;
        LandingMode = false;
        myRigidbody.AddRelativeForce(new Vector3(0.0f, MotionForce, 0.0f), ForceMode.Impulse);
    }
    void Land() {
        transform.position = mothershipDockingPort.GetDockingPort().position;
        transform.rotation = Mothership.transform.rotation;
        transform.SetParent(Mothership);
        Destroy(myRigidbody);
        Launched = false;
        LandingMode = false;
        myCollider.enabled = false;
    }
    private void OnCollisionEnter(Collision other) {
        if (Charge <=0.0f && Launched) {
            if (other.transform.root == Mothership.transform) {
                Land();
            }
        }
    }
    private void FixedUpdate() {
        if (Launched) Charge -= Time.deltaTime; 
        else Charge += 10.0f * Time.deltaTime;
        if (Charge > MaxCharge) Charge = MaxCharge;
        if (Charge < 0f) Charge = 0f;

        if (Charge > 0.0f) {
            if (LandingMode) {
                //zero out the xz error, then drop to the docking port
                Vector3 error = mothershipDockingPort.GetDockingPort().position - transform.position;
                error = transform.InverseTransformDirection(error);
                Vector3 errorxz = error;
                errorxz.y = 0f;
                Vector3 errorxzDir = errorxz.normalized;
                float errorxzMag = errorxz.magnitude;
                if (errorxzMag > 1.0f) {
                    if (errorxzMag < MotionForce) {
                        myRigidbody.AddRelativeForce(errorxzDir * errorxzMag);
                    } else {
                        myRigidbody.AddRelativeForce(errorxzDir * MotionForce);
                    }
                } else {
                   myRigidbody.AddRelativeForce(error.normalized * MotionForce/3.0f);
                }
                if (error.magnitude < 1.0f) {
                    Land();
                }
            }
        if (Input.GetKey(KeyCode.UpArrow)) {
         myRigidbody.AddRelativeForce(new Vector3(0.0f, 0.0f, MotionForce));
       }
       if (Input.GetKey(KeyCode.DownArrow)) {
        myRigidbody.AddRelativeForce(new Vector3(0.0f, 0.0f, -MotionForce));
       }
       if (Input.GetKey(KeyCode.LeftArrow)) {
        myRigidbody.AddRelativeForce(new Vector3(-MotionForce, 0.0f, 0.0f));
       }
       if (Input.GetKey(KeyCode.RightArrow)) {
        myRigidbody.AddRelativeForce(new Vector3(MotionForce, 0.0f, 0.0f));
       }
       if (Input.GetKey(KeyCode.PageUp)) {
        if (!Launched) {
            Takeoff();
        } else {
        myRigidbody.AddRelativeForce(new Vector3(0.0f, MotionForce, 0.0f));
        }
       }
       if (Input.GetKey(KeyCode.PageDown)) {
        myRigidbody.AddRelativeForce(new Vector3(0.0f, -MotionForce, 0.0f));

       }
        } else {
            myRigidbody.useGravity = true;
        }
     }
}
