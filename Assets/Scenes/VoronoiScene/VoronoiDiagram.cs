using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiDiagram : MonoBehaviour {

    public Vector2Int imageDim = new Vector2Int(256, 256);
    public int regionAmount = 20;

    private void Start() {
        CreateTextureSprite();
    }

    public void CreateTextureSprite() {
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(GetDiagram(), new Rect(0, 0, imageDim.x, imageDim.y), Vector2.one * 0.5f);
    }

    Texture2D GetDiagram() {
        Vector2Int[] centroids = new Vector2Int[regionAmount + 1];
        Color[] regions = new Color[regionAmount + 1];

        centroids[regionAmount] = new Vector2Int(0, 0);
        regions[regionAmount] = new Color(1f, 1f, 1f, 1f);

        for (int i = 0; i < regionAmount; i++) {
            centroids[i] = new Vector2Int(Random.Range(0, imageDim.x), Random.Range(0, imageDim.y));
            regions[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }
        Color[] pixelColors = new Color[imageDim.x * imageDim.y];
        for (int x = 0; x < imageDim.x; x++) {
            for (int y = 0; y < imageDim.y; y++) {
                int index = x * imageDim.x + y;
                pixelColors[index] = regions[GetClosestCentroidIndex(new Vector2Int(x, y), centroids)];
            }
        }
        return GetImageFromColorArray(pixelColors);
    }

    int GetClosestCentroidIndex(Vector2Int pixelPos, Vector2Int[] centroids) {
        float smallestDst = float.MaxValue;
        float secondSmallestDst = float.MaxValue;

        int index = 0;
        for (int i = 0; i < centroids.Length; i++) {
            if (Vector2.Distance(pixelPos, centroids[i]) < smallestDst) {
                secondSmallestDst = smallestDst;
                smallestDst = Vector2.Distance(pixelPos, centroids[i]);
                index = i;
            }
        }
        return index;
    }

    Texture2D GetImageFromColorArray(Color[] pixelColors) {
        Texture2D tex = new Texture2D(imageDim.x, imageDim.y);
        tex.filterMode = FilterMode.Point;
        tex.SetPixels(pixelColors);
        tex.Apply();
        return tex;
    }
}
