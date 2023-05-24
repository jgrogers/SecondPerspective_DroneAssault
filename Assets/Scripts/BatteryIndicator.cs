using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryIndicator : MonoBehaviour
{
    [SerializeField] private Image[] indicatorBars;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material yellowMaterial;
    [SerializeField] private Material greenMaterial;
    [SerializeField] private float percentFilled = 1.0f;

    public void SetPercentFilled(float percent) {
        Material tempMaterial;
        if (percent > 0.67) tempMaterial = greenMaterial;
        else if (percent > 0.33) tempMaterial = yellowMaterial;
        else tempMaterial = redMaterial;
        int num = indicatorBars.Length;
        for(int j = 0; j < num; j++) {
            if (percent > (float)(j)/(float)(num)) {
                indicatorBars[j].enabled = true;
            } else {
                indicatorBars[j].enabled = false;
            }
            indicatorBars[j].material = tempMaterial;
        }
    }
}
