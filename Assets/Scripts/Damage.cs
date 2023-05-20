using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private int hitPoints = 1;
    public void Hit(int hits) {
        hitPoints -= hits;
        if (hitPoints <= 0) {
            Destroy(gameObject);
        }
    }
}
