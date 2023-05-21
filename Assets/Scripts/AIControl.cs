using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControl : MonoBehaviour
{
    private Rigidbody myRigidbody;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private GameObject target = null;
    [SerializeField] private bool targetVisible = false;
    [SerializeField] private bool targetInRange = false;
    [SerializeField] private GameObject cannonElevation;
    [SerializeField] private GameObject cannonRotation;
    [SerializeField] private Transform FiringPoint;
    [SerializeField] private GameObject Shell;
    [SerializeField] private float turretMoveRate = 4.0f;
    [SerializeField] private float azimuth = 0.0f;
    [SerializeField] private float elevation = -15.0f;
    [SerializeField] private float azimuthGoal = 0.0f;
    [SerializeField] private float elevationGoal = -15.0f;
    [SerializeField] private float detectionRange = 10.0f;
    [SerializeField] private Transform sensorPoint;
    private FireControl fireControl;
    [SerializeField] private bool hostile = false;
    [SerializeField] private float learnedDistanceScale = 1.0f;
    [SerializeField] private float ErrorRadius = 15.0f;
    [SerializeField] private Vector2 currentError;
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        cannonRotation.transform.localRotation = Quaternion.Euler(0.0f, azimuth, 0.0f);
        cannonElevation.transform.localRotation = Quaternion.Euler(elevation, 0.0f, 0.0f);
        fireControl = GetComponent<FireControl>();
        SampleDistanceError();
    }
    private void SampleDistanceError() {
        currentError = Random.insideUnitCircle * ErrorRadius;
        
    }
    // Update is called once per frame
    void Update()
    {
        if (target) {
            if (!GameManager.Instance.CheckObjectExists(target)) {
                Debug.Log("Target gone");
                target = null;
            }
        }
        if (target == null) {
            //Try to find targets
            foreach (GameObject allyTeamMember in GameManager.Instance.PlayerTeam) {
                Vector3 direction = allyTeamMember.transform.position - transform.position;
                Debug.DrawRay(sensorPoint.position, direction);
                if (Physics.Raycast(sensorPoint.position, direction, out RaycastHit hit, detectionRange)) {
                    if (hit.transform == allyTeamMember.transform) {
                        target = allyTeamMember;
                        Debug.Log("Found target " + target);
                        break;
                    } else {
                        Debug.Log("Trying to get target but hit " + hit.transform);
                    }
                }
            }
            if (target == null) {
                targetVisible = false;
            }
        }
        if (target) {
            // Check raycast to see if target is still visible
            Vector3 direction = target.transform.position - transform.position;
            Vector3 localDirection = transform.InverseTransformDirection(direction);
            if (Physics.Raycast(sensorPoint.position, direction, out RaycastHit hit, Mathf.Infinity)) {
                if (hit.transform == target.transform) {
                    targetVisible = true;
                } else {
                    targetVisible = false;
                }
            }
            if (targetVisible) {
                
                float dx = localDirection.x + currentError.x;
                float dy = localDirection.y;
                float dz = localDirection.z + currentError.y;
                azimuthGoal = Mathf.Rad2Deg * Mathf.Atan2(dx, dz);
                float horiz_dist = learnedDistanceScale * Mathf.Sqrt(dx*dx + dz*dz);
//                elevationGoal = Mathf.Rad2Deg * (Mathf.Atan2(dy, horiz_dist) - elevationAimGain * horiz_dist);
                float asin_value = horiz_dist * 9.81f / (30.0f * 30.0f);
                if (asin_value >= 1.0f || asin_value < 0) targetInRange = false;
                else targetInRange = true;
                if(targetInRange) {
                    elevationGoal = Mathf.Rad2Deg * (0.5f * Mathf.Asin(horiz_dist * 9.81f / (30.0f * 30.0f)));
                } else {
                    elevationGoal = 45.0f;
                }
            } else {
                Debug.Log("I'd move towards target");
            }
        }
    }
    float WrapToPi(float ang_in) {
        while (ang_in > 180.0f) {
            ang_in -= 360.0f;
        }
        while (ang_in < -180.0f) {
            ang_in += 360.0f;
        }
        return ang_in;
    }
    void FixedUpdate() {
        //Orient turret toward target if visible
        // Fire if we are oriented correctly
        bool onTarget = false;
        float azimuthError = WrapToPi(azimuthGoal - azimuth);
        float dAzimuth =  azimuthError * Time.deltaTime * turretMoveRate;
        float elevationError = WrapToPi(elevationGoal - elevation);
        float dElevation = elevationError * Time.deltaTime * turretMoveRate;
        if (Mathf.Abs(azimuthError) < Mathf.Abs(Time.deltaTime * turretMoveRate)) {
            azimuth = azimuthGoal;
            onTarget = true;
        } else {
            azimuth += dAzimuth;
        }
        if (Mathf.Abs(elevationError) < Mathf.Abs(Time.deltaTime * turretMoveRate)) {
            elevation = elevationGoal;
        } else {
            elevation += dElevation;
            onTarget = false;
        }
        cannonRotation.transform.localRotation = Quaternion.Euler(0.0f,  azimuth, 0.0f);
        cannonElevation.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, elevation);
        if (targetInRange && targetVisible && onTarget && hostile) {
            if (fireControl.Fire()) {
                Debug.Log("Firing shell!");
                GameObject shell = Instantiate(Shell, FiringPoint.position, FiringPoint.rotation);
                shell.GetComponent<Shell>().SetSafeMask(gameObject.layer);
                SampleDistanceError();
            } 
        }
    }
}
