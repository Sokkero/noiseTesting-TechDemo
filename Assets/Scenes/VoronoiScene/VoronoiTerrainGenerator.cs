using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiTerrainGenerator : MonoBehaviour
{
    [SerializeField]private GameObject exampleSprite;

    [Header("Terrain")]
    [SerializeField]private Texture2D grassTex;
    [SerializeField]private Texture2D pathTex; 
    [SerializeField]private float height = 1;
    [SerializeField]private int terrainDimension = 256;

    [Header("Voronoi")]
    [Tooltip("The grayscale threshold for a point to count as a path")]
	[SerializeField]private float pathThreshold = 0.8f;
	[SerializeField]private int regionAmount = 30;
    [SerializeField]private float voronoiScale = 0.2f;

    [Header("Perlin & Blue")]
    [SerializeField]private bool perlinNoise = false;
    [SerializeField]private bool blueNoise = false;
    [SerializeField]private float intensity = 1f;
    [SerializeField]private float perlinScale = 1f;
    private Terrain myTerrain;
    private int alphamapRes;
    private float[,,] splatMapData;
    [HideInInspector]public List<Vector2Int> pathPositions = new List<Vector2Int>();
    private bool fog = false;
	
	private void Start()
	{
        startGenerating();
	}

    private void Update() {
        if(Input.GetKeyDown(KeyCode.R)){
            startGenerating();
        }
        else if(Input.GetKeyDown(KeyCode.H)){
            exampleSprite.SetActive(!exampleSprite.activeSelf);
        }
        else if(Input.GetKeyDown(KeyCode.B)){
            blueNoise = !blueNoise;
            FindObjectOfType<BlueNoiseSprite>().clearTerrain();
            startGenerating();
        }
        else if(Input.GetKeyDown(KeyCode.F)){
            fog = !fog;
            RenderSettings.fog = fog;
        }
        else if(Input.GetKeyDown(KeyCode.P)){
            perlinNoise = !perlinNoise;
            startGenerating();
        }
    }

    private void startGenerating(){
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

        myTerrain.terrainData.SetHeights(0, 0, getVoronoi());

        myTerrain.terrainData.SetAlphamaps(0, 0, splatMapData);

        if(blueNoise) 
            FindObjectOfType<BlueNoiseSprite>().generateBluePoints();
    }

    void SetSplatValue(int x, int y, int splat)
    {
        for (int i = 0; i < splatMapData.GetLength(2); i++)
        {
            if (i == splat)
            {
                splatMapData[x, y, i] = 1;
            }
            else
            {
                splatMapData[x, y, i] = 0;
            }
        }
    }

	float[,] getVoronoi()
	{
		Vector2Int[] centroids = new Vector2Int[regionAmount + 1];
		//Color[] regions = new Color[regionAmount + 1]; Different Regions could have different colors each to make the result more diverse

		centroids[regionAmount] = new Vector2Int(0,0);
		//regions[regionAmount] = new Color(1f,1f,1f,1f);

		for(int i = 0; i < regionAmount; i++)
		{
			centroids[i] = new Vector2Int(Random.Range(0, terrainDimension), Random.Range(0, terrainDimension));
			//regions[i] = new Color(0f, 0f, 0f, 1f);
		}

		float[,] posValues = new float[terrainDimension, terrainDimension];
		for(int x = 0; x < terrainDimension; x++)
		{
			for(int y = 0; y < terrainDimension; y++)
			{
                float value = getVoronoiValue(x,y,centroids);

                if(value == voronoiScale){
                    SetSplatValue(x, y, 0);
                }
                else {
                    pathPositions.Add(new Vector2Int(y, x));
                    SetSplatValue(x, y, 1);
                }

                if(perlinNoise)
                    value += getPerlinValue(x, y);

				posValues[x,y] = value;
			}
		}
		return posValues;
	}

	float getVoronoiValue(int x, int y, Vector2Int[] centroids)
	{
		float smallestDst = float.MaxValue;
		float secondSmallestDst = float.MaxValue;

		int index = 0;
		for(int i = 0; i < centroids.Length; i++)
		{
			if (Vector2.Distance(new Vector2Int(x, y), centroids[i]) < smallestDst)
			{
				secondSmallestDst = smallestDst;
				smallestDst = Vector2.Distance(new Vector2Int(x, y), centroids[i]);
				index = i;
			}
		}
		return (smallestDst / secondSmallestDst) > pathThreshold ? 0f : voronoiScale;
	}

    float getPerlinValue(float x, float y){
        x *= perlinScale;
        y *= perlinScale;
        return Mathf.Clamp(Mathf.PerlinNoise(x, y) * intensity, 0f, 0.75f);
    }
}
