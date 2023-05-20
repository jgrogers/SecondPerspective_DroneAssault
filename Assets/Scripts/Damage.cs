using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private int hitPoints = 1;
    public void Hit(int hits) {
        hitPoints -= hits;
        if (hitPoints <= 0) {
            Debug.Log("Object destroyed, cleaning up");
            GameManager.Instance.RemoveObject(gameObject);
            Camera camera = GetComponentInChildren<Camera>();
            if(camera && camera == Camera.main) {
                Debug.Log("Unparenting camera");
                camera.transform.parent = null;
            } else {
                Debug.Log("Doesnt have a camera");
            }
            Destroy(gameObject);
        }
    }
}
