using UnityEngine;
using System.Collections;

public class ExtendedFlycam : MonoBehaviour {

    [SerializeField] private float cameraSensitivity = 90;
    [SerializeField] private float climbSpeed = 4;
    [SerializeField] private float normalMoveSpeed = 10;
    [SerializeField] private float slowMoveFactor = 0.25f;
    [SerializeField] private float fastMoveFactor = 3;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    private TerrainData myTerrain;
    private bool isActive = true;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        myTerrain = FindObjectOfType<Terrain>().terrainData;
    }

    void Update() {
        if (isActive) {
            rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
            rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
            rotationY = Mathf.Clamp(rotationY, -90, 90);

            transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
                transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
            } else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
                transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
                transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
            } else {
                transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
                transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
            }


            if (Input.GetKey(KeyCode.Q)) { transform.position += transform.up * climbSpeed * Time.deltaTime; }
            if (Input.GetKey(KeyCode.E)) { transform.position -= transform.up * climbSpeed * Time.deltaTime; }

        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
            isActive = !isActive;
            Debug.Log("<color=yellow>Flycam now " + isActive + "</color>");

        }
    }
}
