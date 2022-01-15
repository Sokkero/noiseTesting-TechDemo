using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class perlinTerrainGenerator : MonoBehaviour
{

	[Header("Basic settings")]
	[SerializeField]private bool randomize = true;
	[SerializeField]private bool scroll = false;
	[SerializeField]private int height = 100;
	[SerializeField]private int scrollSpeed = 5;
    public int terrainDimension = 256;
	[SerializeField]private bool voxel = false;
	[SerializeField]private int voxelDecimal = 2;

	[Header("Perlin 01 settings")]
    [SerializeField]private int noise01Scale = 4;
	[SerializeField]private float hardness = 0.25f;
    [SerializeField]private float offsetX01;
    [SerializeField]private float offsetY01;

	[Header("Perlin 02 settings")]
	[SerializeField]private bool includeThis = false;
	[SerializeField]private float noise02Scale = 0.8f;
	[SerializeField]private float offsetX02;
    [SerializeField]private float offsetY02;
    [HideInInspector]public Terrain terrain;

    public void Start() 
    {
        GenerateTerrain();
    }

	private void Update() {
		if(Input.GetKeyDown(KeyCode.X)){
			scroll = !scroll;
		} 
		else if(Input.GetKeyDown(KeyCode.Z)){
			randomize = !randomize;
		}
		else if(Input.GetKeyDown(KeyCode.E)){
			includeThis = !includeThis;
		}
		else if(Input.GetKeyDown(KeyCode.Plus)){
			hardness = Mathf.Clamp(hardness * 1.1f, 0f, 1f);
		}
		else if(Input.GetKeyDown(KeyCode.Minus)){
			hardness = Mathf.Clamp(hardness * 0.9f, 0f, 1f);
		}

		if(scroll){
			offsetX01 = offsetX01 + scrollSpeed;
			offsetY01 = offsetY01 + scrollSpeed;

			offsetX02 = offsetX02 - scrollSpeed;
			offsetY02 = offsetY02 - scrollSpeed;
			GenerateTerrain();
		}
		else if(Input.GetKeyDown(KeyCode.R)){
			GenerateTerrain();
		}

	}

    public void GenerateTerrain()
    {
        terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        if (randomize)
        {
            offsetX01 = Random.Range(-99999f, 99999f);
            offsetY01 = Random.Range(-99999f, 99999f);
			offsetX02 = Random.Range(-99999f, 99999f);
            offsetY02 = Random.Range(-99999f, 99999f);
        }

        terrainData.heightmapResolution = terrainDimension + 1;
		terrainData.alphamapResolution = terrainDimension;
        terrainData.size = new Vector3(terrainDimension, height, terrainDimension);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[terrainDimension , terrainDimension];
        for(int x = 1; x < terrainDimension - 1; x++)
        {
            for(int y = 1; y < terrainDimension - 1; y++)
            {
				float xCoord = ((float)x * noise01Scale + offsetX01) * 0.01f;
				float yCoord = ((float)y * noise01Scale + offsetY01) * 0.01f;

				if(includeThis){
					float xCoord2 = ((float)x * noise02Scale + offsetX02) * 0.01f;
					float yCoord2 = ((float)y * noise02Scale + offsetY02) * 0.01f;

					if(voxel){
						heights[x,y] = (float)System.Math.Round((Mathf.PerlinNoise(xCoord, yCoord) * hardness) + ((1f - hardness) * Mathf.PerlinNoise(xCoord2, yCoord2)), voxelDecimal);
					}
					else {
						heights[x,y] = (Mathf.PerlinNoise(xCoord, yCoord) * hardness) + ((1f - hardness) * Mathf.PerlinNoise(xCoord2, yCoord2));
					}
				}
				else if(voxel){
					heights[x,y] = (float)System.Math.Round(Mathf.PerlinNoise(xCoord, yCoord) * hardness, voxelDecimal);
				}
				else {
					heights[x,y] = Mathf.PerlinNoise(xCoord, yCoord) * hardness;
				}
            }
        }
        return heights;
    }
}
