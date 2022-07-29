using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GradientNoiseTerrainGenerator : MonoBehaviour {

    [Header("Basic settings")]
    [SerializeField] private bool randomize = false;
    [SerializeField] private bool scroll = false;
    [SerializeField] private float noiseBaseScale = 0.01f;
    [SerializeField] private int height = 100;
    [SerializeField] private float scrollSpeed = 5;
    [SerializeField] private bool voxel = false;
    [SerializeField] private int voxelDecimal = 2;
    [SerializeField] private bool is3DNoiseModeEnabled = false;
    [SerializeField] private NoiseType noiseType = NoiseType.SimplexType;
    [SerializeField] private float scrollOffset;
    public int terrainDimension = 256;

    [Header("Layer 01 settings")]
    [SerializeField] private long noise01Seed = 0;
    [SerializeField] private int noise01Scale = 4;
    [SerializeField] private float hardness = 0.25f;
    [SerializeField] private float offsetX01;
    [SerializeField] private float offsetY01;

    [Header("Layer 02 settings")]
    [SerializeField] private bool includeLayer2 = false;
    [SerializeField] private long noise02Seed = 1;
    [SerializeField] private float noise02Scale = 0.8f;
    [SerializeField] private float offsetX02;
    [SerializeField] private float offsetY02;
    [HideInInspector] public Terrain terrain;

    public enum NoiseType {
        SimplexType,
        UnmitigatedPerlin,
        DomainRotatedPerlin,
    }

    public void Start() {
        GenerateTerrain();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.X)) {
            scroll = !scroll;
        }
        if (Input.GetKeyDown(KeyCode.Z)) {
            randomize = !randomize;
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            includeLayer2 = !includeLayer2;
        }
        if (Input.GetKeyDown(KeyCode.V)) {
            is3DNoiseModeEnabled = !is3DNoiseModeEnabled;
        }
        if (Input.GetKeyDown(KeyCode.M)) {
            if (noiseType == NoiseType.UnmitigatedPerlin) noiseType = NoiseType.DomainRotatedPerlin;
            else if (noiseType == NoiseType.DomainRotatedPerlin) noiseType = NoiseType.UnmitigatedPerlin;
        }
        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus)) {
            hardness = Mathf.Clamp(hardness * 1.1f, 0f, 1f);
        }
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) {
            hardness = Mathf.Clamp(hardness * 0.9f, 0f, 1f);
        }

        if (randomize) {
            noise01Seed = Random.Range(int.MinValue, int.MaxValue);
            noise02Seed = Random.Range(int.MinValue + 1, int.MaxValue) + (noise01Seed - int.MinValue);
            offsetX01 = Random.Range(int.MinValue, int.MaxValue);
            offsetY01 = Random.Range(int.MinValue, int.MaxValue);
            offsetX02 = Random.Range(int.MinValue, int.MaxValue);
            offsetY02 = Random.Range(int.MinValue, int.MaxValue);
        }

        if (scroll) {
            scrollOffset += scrollSpeed * noiseBaseScale;
        }

        if (scroll || Input.GetKeyDown(KeyCode.R)) {
            GenerateTerrain();
        }
    }

    public void GenerateTerrain() {
        terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    TerrainData GenerateTerrain(TerrainData terrainData) {
        terrainData.heightmapResolution = terrainDimension + 1;
        terrainData.alphamapResolution = terrainDimension;
        terrainData.size = new Vector3(terrainDimension, height, terrainDimension);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float GetNoise(long seed, double xCoord, double yCoord) {
        float value;
        switch (noiseType) {
            default:
            case NoiseType.SimplexType:
                if (is3DNoiseModeEnabled) value = OpenSimplex2.Noise3_ImproveXY(seed, xCoord, yCoord, scrollOffset);
                else value = OpenSimplex2.Noise2(seed, xCoord + scrollOffset, yCoord + scrollOffset);
                break;
            case NoiseType.DomainRotatedPerlin:
                if (is3DNoiseModeEnabled) value = DomainRotatedPerlin.Noise3_ImproveXY(seed, xCoord, yCoord, scrollOffset);
                else value = DomainRotatedPerlin.Noise3_ImproveXY(seed, xCoord + scrollOffset, yCoord + scrollOffset, 0.0);
                break;
            case NoiseType.UnmitigatedPerlin:
                if (is3DNoiseModeEnabled) value = DomainRotatedPerlin.Noise3_UnrotatedBase(seed, xCoord, yCoord, scrollOffset);
                else value = DomainRotatedPerlin.Noise3_UnrotatedBase(seed, xCoord + scrollOffset, yCoord + scrollOffset, 0.0);
                break;
        }
        return value * 0.5f + 0.5f;
    }

    float[,] GenerateHeights() {
        float[,] heights = new float[terrainDimension, terrainDimension];
        for (int x = 1; x < terrainDimension - 1; x++) {
            for (int y = 1; y < terrainDimension - 1; y++) {
                double xCoord = (x * noise01Scale + offsetX01) * noiseBaseScale;
                double yCoord = (y * noise01Scale + offsetY01) * noiseBaseScale;
                float noiseValue = GetNoise(noise01Seed, xCoord, yCoord) * hardness;

                if (includeLayer2) {
                    double xCoord2 = (x * noise02Scale + offsetX02) * noiseBaseScale;
                    double yCoord2 = (y * noise02Scale + offsetY02) * noiseBaseScale;
                    noiseValue += GetNoise(noise02Seed, xCoord2, yCoord2) * (1f - hardness);
                }

                if (voxel)
                    noiseValue = (float)System.Math.Round(noiseValue, voxelDecimal);

                heights[x, y] = noiseValue;
            }
        }
        return heights;
    }
}
