using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePlacement : MonoBehaviour {
    private void Awake() {
        Terrain myTerrrain = FindObjectOfType<Terrain>();

        this.transform.Rotate(0f, Random.Range(0f, 360f), 0f);

        Vector3 newPos = this.transform.position;
        newPos.y = myTerrrain.SampleHeight(this.transform.position);
        this.transform.position = newPos;
    }
}