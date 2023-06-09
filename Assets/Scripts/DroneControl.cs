using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneControl : MonoBehaviour
{
    [SerializeField] private float MotionForce = 15.0f;
    [SerializeField] private Transform Mothership;
    [SerializeField] private float Charge = 120.0f;
    [SerializeField] private float MaxCharge = 120.0f;
    [SerializeField] private bool Launched = false;
    [SerializeField] private bool LandingMode = false;
    [SerializeField] private BatteryIndicator batteryIndicator;
    [SerializeField] private Button returnHomeButton;
    public event EventHandler onTakeoff;
    public event EventHandler onLand;
    public event EventHandler onFly;
    private Rigidbody myRigidbody;
    private SphereCollider myCollider;
    private DroneDock mothershipDockingPort;
    // Start is called before the first frame update
    void Awake()
    {
       myCollider = GetComponent<SphereCollider>();
       mothershipDockingPort = Mothership.GetComponent<DroneDock>();
       Launched = true; // Just so that the land operation works
       Land();
       returnHomeButton.onClick.AddListener(ReturnHomeClicked);
    }
    void Start() {
        MaxCharge = GameManager.Instance.DroneBatteryLife;
        Charge = MaxCharge;
        MotionForce = GameManager.Instance.DroneSpeed;
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
        onTakeoff?.Invoke(this, EventArgs.Empty);
    }
    void Land() {
        if (Launched) {
            transform.position = mothershipDockingPort.GetDockingPort().position;
            transform.rotation = Mothership.transform.rotation;
            transform.SetParent(Mothership);
            if (myRigidbody) Destroy(myRigidbody);
            Launched = false;
            LandingMode = false;
            myCollider.enabled = false;
            onLand?.Invoke(this, EventArgs.Empty);
        }
    }
    private void OnCollisionEnter(Collision other) {
        if (Charge <=0.0f && Launched) {
            if (other.transform.root == Mothership.transform) {
                Land();
            }
        }
    }
    public void ReturnHomeClicked() {
        if (Launched) {
            LandingMode = true;
        }
    }
    private float WrapAngle(float angle) {
        while (angle > 180.0f) {
            angle -= 360.0f;
        } while (angle < -180.0f) {
            angle += 360.0f;
        }
        return  angle;
    }
    private float GetControlTorque(float desiredAngle, float currentAngle, float currentAngularVelocity) {
        float error = WrapAngle(Mathf.MoveTowardsAngle(currentAngle, desiredAngle, 1.0f));
        
        error += Mathf.Clamp(currentAngularVelocity, -1.0f, 1.0f);
        return Mathf.Clamp(error, -1.0f, 1.0f);
    }
    private void FixedUpdate() {
        if (Launched) Charge -= Time.deltaTime; 
        else Charge += 10.0f * Time.deltaTime;
        if (Charge > MaxCharge) Charge = MaxCharge;
        if (Charge < 0f) Charge = 0f;
        batteryIndicator.SetPercentFilled(Charge / MaxCharge);

        if (Charge > 0.0f) {
            if (Launched) {
                //Add a righting force to make it upright again
            Vector3 attitude = transform.rotation.eulerAngles;
            Vector3 localAngularVelocity = transform.InverseTransformDirection(myRigidbody.angularVelocity);
            myRigidbody.AddRelativeTorque(-GetControlTorque(0.0f, attitude.x, localAngularVelocity.x), 0f, -GetControlTorque(0.0f, attitude.z, localAngularVelocity.z), ForceMode.Force);

            }
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
            LandingMode = false;
            if (Launched) {
                myRigidbody.AddRelativeForce(new Vector3(0.0f, 0.0f, MotionForce));
                onFly?.Invoke(this, EventArgs.Empty);
            }
       }
       if (Input.GetKey(KeyCode.DownArrow)) {
            LandingMode = false;
            if (Launched) {
                myRigidbody.AddRelativeForce(new Vector3(0.0f, 0.0f, -MotionForce));
                onFly?.Invoke(this, EventArgs.Empty);
            }
       }
       if (Input.GetKey(KeyCode.LeftArrow)) {
            LandingMode = false;
            if (Launched) {
                myRigidbody.AddRelativeForce(new Vector3(-MotionForce, 0.0f, 0.0f));
                onFly?.Invoke(this, EventArgs.Empty);
            }
       }
       if (Input.GetKey(KeyCode.RightArrow)) {
            LandingMode = false;
            if (Launched) {
                myRigidbody.AddRelativeForce(new Vector3(MotionForce, 0.0f, 0.0f));
                onFly?.Invoke(this, EventArgs.Empty);
            }
       }
       if (Input.GetKey(KeyCode.PageUp)) {
        if (!Launched) {
            Takeoff();
        } else {
            LandingMode = false;
            myRigidbody.AddRelativeForce(new Vector3(0.0f, MotionForce, 0.0f));
        }
       }
       if (Input.GetKey(KeyCode.PageDown)) {
            LandingMode = false;
            if (Launched) {
                myRigidbody.AddRelativeForce(new Vector3(0.0f, -MotionForce, 0.0f));
            }

       }
        } else {
            if (Launched) {
                myRigidbody.useGravity = true;
            }
        }
     }
}
