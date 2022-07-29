using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueNoiseSprite : MonoBehaviour {

    [SerializeField] private List<GameObject> trees = new List<GameObject>();
    [SerializeField] private GameObject debugCylinder;
    [SerializeField] private int pointAmount = 100;

    [Tooltip("Measured in coordinate units")]
    [SerializeField] private float distance = 5;
    [SerializeField] private int dimension = 256;

    public void GenerateBluePoints(bool showCylinders) {
        ClearTerrain();

        List<Vector2Int> points = new List<Vector2Int>();

        for (int i = 0; i < pointAmount; i++) {
            Vector2Int newPoint = new Vector2Int(Random.Range(1, dimension), Random.Range(1, dimension));
            bool toClose = false;
            foreach (Vector2Int compareVector in points) {
                float pointDistance = Vector2Int.Distance(compareVector, newPoint);
                if (pointDistance < distance) {
                    toClose = true;
                }
            }
            if (!toClose) {
                points.Add(newPoint);
            }
        }

        List<Vector2Int> myList = FindObjectOfType<VoronoiTerrainGenerator>().pathPositions;

        foreach (Vector2Int vectors in points) {
            if (!myList.Contains(vectors)) {
                if (showCylinders) {
                    GameObject cylinder = Instantiate(debugCylinder, new Vector3(vectors.x, 0f, vectors.y), Quaternion.identity, GameObject.Find("debug").transform);
                    cylinder.transform.localScale = new Vector3(distance, cylinder.transform.localScale.y, distance);
                } else {
                    GameObject tree = Instantiate(trees[Random.Range(0, trees.Count)], new Vector3(vectors.x, 0f, vectors.y), Quaternion.identity, GameObject.Find("debug").transform);
                }
            }
        }
    }

    public void ClearTerrain() {
        foreach (Transform x in GameObject.Find("debug").transform) {
            Destroy(x.gameObject);
        }
    }
}
