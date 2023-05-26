using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour
{
    public void OnClickControls() {
        SceneManager.LoadScene(2);

    }
    public void OnClickTutorial() {
        SceneManager.LoadScene(3);

    }
    public void OnClickMission() {
        SceneManager.LoadScene(4);

    }

}
