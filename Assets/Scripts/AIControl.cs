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
    [SerializeField] private float turretMoveRate = 4.0f;
    [SerializeField] private float azimuth = 0.0f;
    [SerializeField] private float elevation = -15.0f;
    [SerializeField] private float azimuthGoal = 0.0f;
    [SerializeField] private float elevationGoal = -15.0f;
    [SerializeField] private float detectionRange = 10.0f;
    private FireControl fireControl;
    [SerializeField] private bool hostile = false;
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
                Debug.DrawRay(transform.position, direction);
                if (Physics.Raycast(transform.position, direction, out RaycastHit hit, detectionRange)) {
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
        if (target) {
            // Check raycast to see if target is still visible
            Vector3 direction = target.transform.position - transform.position;
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, Mathf.Infinity)) {
                if (hit.transform == target.transform) {
                    targetVisible = true;
                } else {
                    targetVisible = false;
                }
            }
            if (targetVisible) {
                float dx = target.transform.position.x - transform.position.x;
                float dy = target.transform.position.y - transform.position.y;
                float dz = target.transform.position.z - transform.position.z;
                azimuthGoal = Mathf.Rad2Deg * Mathf.Atan2(dx, dz);
                elevationGoal = Mathf.Rad2Deg * Mathf.Atan2(dy, Mathf.Sqrt(dx*dx + dz*dz));
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
        cannonRotation.transform.localRotation = Quaternion.Euler(0.0f, azimuth, 0.0f);
        cannonElevation.transform.localRotation = Quaternion.Euler(elevation, 0.0f, 0.0f);
        if (targetVisible && onTarget && hostile) {
            if (fireControl.Fire()) {
                Debug.Log("Firing shell!");
                GameObject shell = Instantiate(Shell, FiringPoint.position, FiringPoint.rotation);
                shell.GetComponent<Shell>().SetSafeMask(gameObject.layer);
            } 
        }
    }
}
