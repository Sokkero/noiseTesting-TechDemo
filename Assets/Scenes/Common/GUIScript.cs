using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUIScript : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI fpsCounter;

    void Update() {
        fpsCounter.text = System.Math.Round(1 / Time.deltaTime, 2) + "fps";
    }
}
