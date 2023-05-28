using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI WinText;
    [SerializeField] private LayerMask PlayerMask;
    // Start is called before the first frame update
    void Start()
    {
       WinText.enabled = false; 
    }

    public void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Collider>().gameObject.IsInLayerMasks(PlayerMask)) {
            WinText.enabled = true;
            StartCoroutine(DelayLoadIntro());
        }

    }
    public IEnumerator DelayLoadIntro() {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(1);
    }
}
