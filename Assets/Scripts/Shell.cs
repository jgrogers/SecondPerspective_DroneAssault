using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] private int SafeMask;
    [SerializeField] private LayerMask TargetMask;
    [SerializeField] private float ImpulseAmount = 100.0f;
    [SerializeField] private float Lifespan = 10.0f;
    [SerializeField] private ParticleSystem explosionEffect;
    private Vector3 lastPhysicsPosition;
    private Rigidbody myRigidbody;
    // Start is called before the first frame update
    void Start()
    {
       myRigidbody = GetComponent<Rigidbody>();
       myRigidbody.AddRelativeForce(Vector3.forward * ImpulseAmount, ForceMode.Impulse); 
       lastPhysicsPosition = transform.position;
    }
    void FixedUpdate() {
        Lifespan -= Time.deltaTime;
        if (Lifespan <= 0.0f) {
            Destroy(gameObject);
        }
        //Do a raycast between last physics position and this one, and trigger colliders that were missed by fast moving shell
        Vector3 direction = transform.position - lastPhysicsPosition;

        if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, direction.magnitude)) {
            OnTriggerEnter(hit.collider);
        }
        lastPhysicsPosition = transform.position;
        Debug.Log("Shell velocity : " + myRigidbody.velocity.magnitude);
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Shell safe mask is " + SafeMask);
        Debug.Log("Hitting target with mask " + other.GetComponent<Collider>().gameObject.layer);
        if (other.GetComponent<Collider>().gameObject.layer == SafeMask) {
        }
        else {
            if (other.GetComponent<Collider>().gameObject.IsInLayerMasks(TargetMask)) {
                Debug.Log("Hit target - damaging " + other);
                other.GetComponentInParent<Damage>().Hit(1);
            }
            else {
                Debug.Log("hit : " + other.GetComponent<Collider>().gameObject.layer);
            }
            ParticleSystem explosion = ParticleSystem.Instantiate(explosionEffect, transform.position, Quaternion.identity);
            explosion.Play();
            TerrainCraterer.Instance.MakeCraterAtWorld(transform.position);
            Destroy(gameObject);
        }
        
    }
    public void SetSafeMask(int mask) {
        SafeMask = mask;
    }
}
