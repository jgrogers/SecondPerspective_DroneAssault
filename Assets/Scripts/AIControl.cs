using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControl : MonoBehaviour
{
    private Rigidbody myRigidbody;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private GameObject target = null;
    [SerializeField] private bool targetVisible = false;
    [SerializeField] private GameObject cannonElevation;
    [SerializeField] private GameObject cannonRotation;
    [SerializeField] private Transform FiringPoint;
    [SerializeField] private GameObject Shell;
    [SerializeField] private float turretMoveRate = 15.0f;
    private float azimuth = 0.0f;
    private float elevation = -15.0f;
    [SerializeField] private float azimuthGoal = 0.0f;
    [SerializeField] private float elevationGoal = -15.0f;
    private FireControl fireControl;
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        cannonRotation.transform.localRotation = Quaternion.Euler(elevation, azimuth, 0.0f);
        fireControl = GetComponent<FireControl>();
 
    }
    // Update is called once per frame
    void Update()
    {
        if (target == null) {
            //Try to find targets
        }
        // Check raycast to see if target is still visible
        targetVisible = true;
        
    }
    void FixedUpdate() {
        //Orient turret toward target if visible
        // Fire if we are oriented correctly
        if (target) {
            if (targetVisible) {
                float dx = target.transform.position.x - transform.position.x;
                float dy = target.transform.position.y - transform.position.y;
                float dz = target.transform.position.z - transform.position.z;
                azimuthGoal = Mathf.Atan2(dx, dz);
                elevationGoal = Mathf.Atan2(dy, Mathf.Sqrt(dx*dx + dz*dz));
                float dAzimuth = (azimuthGoal - azimuth) * Time.deltaTime / turretMoveRate;
                float dElevation = (elevationGoal - elevation) * Time.deltaTime / turretMoveRate;
                if (Mathf.Abs(azimuthGoal - azimuth) < Mathf.Abs(dAzimuth)) {
                    azimuth = azimuthGoal;
                } else {
                    azimuth += dAzimuth;
                }
                if (Mathf.Abs(elevationGoal - elevation) < Mathf.Abs(dElevation)) {
                    elevation = elevationGoal;
                } else {
                    elevation += dElevation;
                }
                cannonRotation.transform.localRotation = Quaternion.Euler(elevation, azimuth, 0.0f);
            }
        } else {
            targetVisible = false;
        }
    }
}
