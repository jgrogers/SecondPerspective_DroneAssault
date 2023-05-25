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
    [SerializeField] private WheelCollider[] leftTracks;
    [SerializeField] private WheelCollider[] rightTracks;
    [SerializeField] private float motorTorque = 150.0f;
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
    [SerializeField] private Transform goalPoint;
    [SerializeField] private int waypointNum = 0;
    [SerializeField] private int waypointMissionNum = -1;
    [SerializeField] private int TreeLayer = 9;
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        cannonRotation.transform.localRotation = Quaternion.Euler(0.0f, azimuth, 0.0f);
        cannonElevation.transform.localRotation = Quaternion.Euler(elevation, 0.0f, 0.0f);
        fireControl = GetComponent<FireControl>();
        SampleDistanceError();
        Debug.Log("My waypoint mission num is " + waypointMissionNum);
        if (waypointMissionNum != -1) {
            goalPoint = PatrolPaths.Instance.GetPatrolPath(waypointMissionNum).waypoints[waypointNum];
        }
    }
    private void SampleDistanceError() {
        currentError = Random.insideUnitCircle * ErrorRadius;
        
    }
    // Update is called once per frame
    void Update()
    {
        if (target) {
            VerifyTargetStillExists();
        }
        if (!target) {
            FindNewTarget();
        }
        if (target) {
            ComputeFiringSolution();
        } else {
            //Continue on primary mission: get to goalPoint
        }
    }
    void FixedUpdate() {
        //Orient turret toward target if visible
        // Fire if we are oriented correctly
        if (targetInRange && targetVisible && hostile) {
            AimAndFire();
        } else if (goalPoint) {
            //Continue on primary mission: get to goalPoint
            if(TurnToGoal()) {
                if(MoveToGoal()) {
                    waypointNum ++;
                    if (waypointNum >= PatrolPaths.Instance.GetPatrolPath(waypointMissionNum).waypoints.Count) {
                        waypointNum = 0;
                    }
                    goalPoint = PatrolPaths.Instance.GetPatrolPath(waypointMissionNum).waypoints[waypointNum];
                }
            }
        }
    }
    private bool TurnToGoal() {
        //Does a turn-in-place behavior if we are not facing pretty close to the goal, otherwise return true;
        Vector3 direction = goalPoint.transform.position - transform.position;
        Vector3 localDirection = transform.InverseTransformDirection(direction);
        float yaw_error = Mathf.Atan2(localDirection.x, localDirection.z);
        if (Mathf.Abs(yaw_error) < Mathf.Deg2Rad * 30.0f) return true;
        else {
            float leftTorque = 0.0f;
            float rightTorque = 0.0f;
            if (WrapToPi(yaw_error) > 0) {
                leftTorque = motorTorque;
                rightTorque = -motorTorque;
            } else {
                leftTorque = -motorTorque;
                rightTorque = motorTorque;
            }
            foreach(WheelCollider wheel in leftTracks) {
                wheel.motorTorque = leftTorque;
            }
            foreach(WheelCollider wheel in rightTracks) {
                wheel.motorTorque = rightTorque;
            }
        }
        return false;
    }
    private bool MoveToGoal() {
        Vector3 direction = goalPoint.transform.position - transform.position;
        Vector3 localDirection = transform.InverseTransformDirection(direction);
        float yaw_error = Mathf.Atan2(localDirection.x, localDirection.z);
        
        float leftTorque = motorTorque;
        float rightTorque = motorTorque;
        if (yaw_error < 0) leftTorque = leftTorque * Mathf.Min(1.0f, 0.3f/ yaw_error);
        else if (yaw_error > 0) rightTorque = rightTorque * Mathf.Min(1.0f, 0.3f/ yaw_error);
        foreach(WheelCollider wheel in leftTracks) {
            wheel.motorTorque = leftTorque;
        }
        foreach(WheelCollider wheel in rightTracks) {
            wheel.motorTorque = rightTorque;
        }
        return direction.magnitude < 10.0f;
    }
    private void VerifyTargetStillExists() {
        if (!GameManager.Instance.CheckObjectExists(target)) {
            target = null;
        }
    }
    private void FindNewTarget() {
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
                }
            }
        }
        if (target == null) {
            targetVisible = false;
        }
    }
    private void ComputeFiringSolution() {
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
            float asin_value = horiz_dist * 9.81f / (30.0f * 30.0f);
            if (asin_value >= 1.0f || asin_value < 0) targetInRange = false;
            else targetInRange = true;
            if(targetInRange) {
                elevationGoal = Mathf.Rad2Deg * (0.5f * Mathf.Asin(horiz_dist * 9.81f / (30.0f * 30.0f)));
            } else {
                elevationGoal = 45.0f;
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
    private void AimAndFire() {
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
                GameObject shell = Instantiate(Shell, FiringPoint.position, FiringPoint.rotation);
                shell.GetComponent<Shell>().SetSafeMask(gameObject.layer);
                SampleDistanceError();
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
