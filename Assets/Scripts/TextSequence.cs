using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextSequence : MonoBehaviour
{
    [System.Serializable]
    public struct TextEvent {
        public string text;
        public float Duration;
        public float pauseAfter;
        public int whichBox;
        public string responseEventToSee;
    };
    public Dictionary<string, bool> eventLabelSeen;
    [SerializeField] private List<TextEvent> events;
    [SerializeField] private List<TextMeshProUGUI> textBox;
    [SerializeField] private List<Image> textBackgrounds;
    [SerializeField] private PlayerControl player;
    [SerializeField] private DroneControl drone;

    // Start is called before the first frame update
    // Update is called once per frame
    void Start()
    {
        eventLabelSeen = new Dictionary<string, bool>();
        foreach(Image image in textBackgrounds) {
            image.enabled = false;
        }
        foreach(TextMeshProUGUI text in textBox) {
            text.text = "";
        }
        player.onFireCannon += onFireCannon;
        player.onForwardBack += onForwardBack;
        player.onTurn += onTurn;
        player.onTurretTurn += onTurretTurn;
        player.onBarrelAim += onBarrelAim;
        drone.onFly += onFly;
        drone.onLand += onLand;
        drone.onTakeoff += onTakeoff;
    
        StartCoroutine(TextSequencer());
    }
    public void onFireCannon(object origin, EventArgs e) {
        eventLabelSeen["cannon_fire"] = true;
    }
    public void onForwardBack(object origin, EventArgs e) {
        eventLabelSeen["forward_back"] = true;
    }
    public void onTurn(object origin, EventArgs e) {
        eventLabelSeen["turn"] = true;
    }
    public void onTurretTurn(object origin, EventArgs e) {
        eventLabelSeen["turret_turn"] = true;
    }
    public void onBarrelAim(object origin, EventArgs e) {
        eventLabelSeen["barrel_aim"] = true;
    }
    public void onFly(object origin, EventArgs e) {
        eventLabelSeen["fly"] = true;
    }
    public void onLand(object origin, EventArgs e) {
        eventLabelSeen["land"] = true;
    }
    public void onTakeoff(object origin, EventArgs e) {
        eventLabelSeen["takeoff"] = true;
    }
    private IEnumerator TextSequencer() {
        foreach (TextEvent e in events) {
            textBox[e.whichBox].text = e.text;
            textBackgrounds[e.whichBox].enabled = true;
            float startTime = Time.time;
            if (e.responseEventToSee != "") {
                eventLabelSeen[e.responseEventToSee] = false;
            }
            while (Time.time < startTime + e.Duration) {
                yield return null;
            }
            if (e.responseEventToSee != "") {
                while (!eventLabelSeen[e.responseEventToSee]) {
                    yield return null;
                }
            }
            textBox[e.whichBox].text = "";
            textBackgrounds[e.whichBox].enabled = false;
            startTime = Time.time;
            while (Time.time < startTime + e.pauseAfter) {
                yield return null;
            }
        }
    }
}
