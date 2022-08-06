using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiTerrainGenerator : MonoBehaviour {

    [SerializeField] private GameObject exampleSprite;

    [Header("Terrain")]
    [SerializeField] private Texture2D grassTex;
    [SerializeField] private Texture2D pathTex;
    [SerializeField] private float height = 1;
    [SerializeField] private int terrainDimension = 256;

    [Header("Voronoi")]
    [Tooltip("The grayscale threshold for a point to count as a path")]
    [SerializeField] private float pathThreshold = 0.8f;
    [SerializeField] private int regionAmount = 30;
    [SerializeField] private float voronoiScale = 0.2f;

    [Header("Gradient Noise")]
    [SerializeField] private bool gradientNoise = false;
    [SerializeField] private float intensity = 0.8f;
    [SerializeField] private float gradientNoiseScale = 0.0225f;

    [Header("Blue Noise")]
    [SerializeField] private bool blueNoise = false;
    [SerializeField] private bool showBlueNoiseCylinders = true;

    private Terrain myTerrain;
    private int alphamapRes;
    private float[,,] splatMapData;
    [HideInInspector] public List<Vector2Int> pathPositions = new List<Vector2Int>();
    private bool fog = false;

    private void Start() {
        StartGenerating();
    }

    private void Update() {
        bool shouldRegenerate = false;
        if (Input.GetKeyDown(KeyCode.R)) {
            shouldRegenerate = true;
        }
        if (Input.GetKeyDown(KeyCode.H)) {
            exampleSprite.SetActive(!exampleSprite.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.B)) {
            blueNoise = !blueNoise;
            FindObjectOfType<BlueNoiseSprite>().ClearTerrain();
            shouldRegenerate = true;
        }
        if (Input.GetKeyDown(KeyCode.F)) {
            fog = !fog;
            RenderSettings.fog = fog;
        }
        if (Input.GetKeyDown(KeyCode.G)) {
            gradientNoise = !gradientNoise;
            shouldRegenerate = true;
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            showBlueNoiseCylinders = !showBlueNoiseCylinders;
            shouldRegenerate = true;
        }

        if (shouldRegenerate) {
            StartGenerating();
        }
    }

    private void StartGenerating() {
        myTerrain = GetComponent<Terrain>();

        pathPositions = new List<Vector2Int>();

        TerrainLayer[] newTextures = new TerrainLayer[6];
        newTextures[0] = new TerrainLayer();
        newTextures[0].diffuseTexture = grassTex;
        newTextures[1] = new TerrainLayer();
        newTextures[1].diffuseTexture = pathTex;
        myTerrain.terrainData.terrainLayers = newTextures;

        myTerrain.terrainData.heightmapResolution = terrainDimension + 1;
        myTerrain.terrainData.alphamapResolution = terrainDimension;
        myTerrain.terrainData.size = new Vector3(terrainDimension, height, terrainDimension);

        alphamapRes = myTerrain.terrainData.alphamapResolution;
        splatMapData = myTerrain.terrainData.GetAlphamaps(0, 0, alphamapRes, alphamapRes);

        myTerrain.terrainData.SetHeights(0, 0, GetVoronoi());

        myTerrain.terrainData.SetAlphamaps(0, 0, splatMapData);

        if (blueNoise)
            FindObjectOfType<BlueNoiseSprite>().GenerateBluePoints(showBlueNoiseCylinders);
    }

    void SetSplatValue(int x, int y, int splat) {
        for (int i = 0; i < splatMapData.GetLength(2); i++) {
            if (i == splat) {
                splatMapData[x, y, i] = 1;
            } else {
                splatMapData[x, y, i] = 0;
            }
        }
    }

    float[,] GetVoronoi() {
        Vector2Int[] centroids = new Vector2Int[regionAmount + 1];
        //Color[] regions = new Color[regionAmount + 1]; Different Regions could have different colors each to make the result more diverse

        centroids[regionAmount] = new Vector2Int(0, 0);
        //regions[regionAmount] = new Color(1f,1f,1f,1f);

        for (int i = 0; i < regionAmount; i++) {
            centroids[i] = new Vector2Int(Random.Range(0, terrainDimension), Random.Range(0, terrainDimension));
            //regions[i] = new Color(0f, 0f, 0f, 1f);
        }

        float[,] posValues = new float[terrainDimension, terrainDimension];
        for (int x = 0; x < terrainDimension; x++) {
            for (int y = 0; y < terrainDimension; y++) {
                float value = GetVoronoiValue(x, y, centroids);

                if (value == voronoiScale) {
                    SetSplatValue(x, y, 0);
                } else {
                    pathPositions.Add(new Vector2Int(y, x));
                    SetSplatValue(x, y, 1);
                }

                if (gradientNoise)
                    value += GetGradientNoiseValue(x, y);

                posValues[x, y] = value;
            }
        }
        return posValues;
    }

    float GetVoronoiValue(int x, int y, Vector2Int[] centroids) {
        float smallestDst = float.MaxValue;
        float secondSmallestDst = float.MaxValue;

        int index = 0;
        for (int i = 0; i < centroids.Length; i++) {
            if (Vector2.Distance(new Vector2Int(x, y), centroids[i]) < smallestDst) {
                secondSmallestDst = smallestDst;
                smallestDst = Vector2.Distance(new Vector2Int(x, y), centroids[i]);
                index = i;
            }
        }
        return (smallestDst / secondSmallestDst) > pathThreshold ? 0f : voronoiScale;
    }

    float GetGradientNoiseValue(float x, float y) {
        x *= gradientNoiseScale;
        y *= gradientNoiseScale;
        return Mathf.Clamp((OpenSimplex2.Noise2(0, x, y) * 0.5f + 0.5f) * intensity, 0f, 0.75f);
    }
}
