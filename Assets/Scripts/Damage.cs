using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private int hitPoints = 1;
    public void Hit(int hits) {
        hitPoints -= hits;
        if (hitPoints <= 0) {
            GameManager.Instance.RemoveObject(gameObject);
            Camera camera = GetComponentInChildren<Camera>();
            if(camera && camera == Camera.main) {
                camera.transform.parent = null;
            } else {
            }
            Destroy(gameObject);
        }
    }
}
