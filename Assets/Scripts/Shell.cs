using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] private int SafeMask;
    [SerializeField] private LayerMask TargetMask;
    [SerializeField] private int TreeLayer;
    private float ImpulseAmount;
    private float Lifespan;
    [SerializeField] private ParticleSystem explosionEffect;
    private Vector3 lastPhysicsPosition;
    private Rigidbody myRigidbody;
    // Start is called before the first frame update
    void Start()
    {
       myRigidbody = GetComponent<Rigidbody>();
       ImpulseAmount = GameManager.Instance.ShellVelocity;
       myRigidbody.AddRelativeForce(Vector3.forward * ImpulseAmount, ForceMode.Impulse); 
       lastPhysicsPosition = transform.position;
       Lifespan = GameManager.Instance.ShellLifetime;
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
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Collider>().gameObject.layer == SafeMask) {
        }
        else {
            if (other.GetComponent<Collider>().gameObject.IsInLayerMasks(TargetMask)) {
                other.GetComponentInParent<Damage>().Hit(1);
            }
            else if (other.GetComponent<Collider>().gameObject.layer == TreeLayer) {
                other.gameObject.AddComponent<Rigidbody>();
                other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(10f, transform.position, 10.0f*5f, 3.0f, ForceMode.Impulse);
                other.gameObject.layer = 10;
            }
            else {
            }
            ParticleSystem explosion = ParticleSystem.Instantiate(explosionEffect, transform.position, Quaternion.identity);
            explosion.Play();
            if (TerrainCraterer.Instance) TerrainCraterer.Instance.MakeCraterAtWorld(transform.position);
            Destroy(gameObject);
        }
        
    }
    public void SetSafeMask(int mask) {
        SafeMask = mask;
    }
}
