using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private float cameraSensivity = 200f;
    [SerializeField] private float cameraDistance = 260f;
    [SerializeField] private float cameraMoveSpeed = 50f;
    [SerializeField] private float zoomFactor = 0.05f;

    private float rotationX = -60f;
    private float rotationY = 0f;
    private GameObject lookTarget;
    private GradientNoiseTerrainGenerator myGen;

    private void Start() {
        myGen = FindObjectOfType<GradientNoiseTerrainGenerator>();
        lookTarget = new GameObject();
        lookTarget.name = "cameraHolder";
        this.transform.parent = lookTarget.transform;
        lookTarget.transform.position = new Vector3(myGen.terrainDimension / 2, 0f, myGen.terrainDimension / 2);

    }

    private void Update() {
        if (Input.GetKey(KeyCode.LeftShift)) {
            float scrollAmount = (Input.GetAxis("Vertical") * zoomFactor) * (cameraDistance * 0.3f);
            cameraDistance += scrollAmount * -1f;
            cameraDistance = Mathf.Clamp(cameraDistance, 10f, 1000f);
        } else {
            rotationY += (Input.GetAxis("Horizontal") * -1f) * cameraMoveSpeed * Time.deltaTime;
            rotationX += Input.GetAxis("Vertical") * cameraMoveSpeed * Time.deltaTime;
        }

        Quaternion qt = Quaternion.Euler(rotationX, rotationY, 0);
        lookTarget.transform.rotation = Quaternion.Lerp(lookTarget.transform.rotation, qt, Time.deltaTime * 10f);

        transform.localPosition = new Vector3(0f, Mathf.Lerp(transform.localPosition.y, cameraDistance, Time.deltaTime * 10f), 0f);
    }
}
