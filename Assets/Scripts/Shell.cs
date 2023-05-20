using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] private LayerMask SafeMask;
    [SerializeField] private LayerMask TargetMask;
    [SerializeField] private float ImpulseAmount = 100.0f;
    [SerializeField] private float Lifespan = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
       GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * ImpulseAmount, ForceMode.Impulse); 
    }
    void FixedUpdate() {
        Lifespan -= Time.deltaTime;
        if (Lifespan <= 0.0f) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Collider>().gameObject.IsInLayerMasks(SafeMask)) {
        }
        else {
            if (other.GetComponent<Collider>().gameObject.IsInLayerMasks(TargetMask)) {
                Debug.Log("Hit target - damaging " + other);
                other.GetComponentInParent<Damage>().Hit(1);
            }
            else {
                Debug.Log("hit : " + other.GetComponent<Collider>().gameObject.layer);
            }
            Destroy(gameObject);
        }
        
    }
    public void SetSafeMask(LayerMask mask) {
        SafeMask = mask;
    }
}
