using UnityEngine;
using System;
using System.Collections;
using LibNoise.Unity;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;
public class Chunk : MonoBehaviour
{
    public GameObject[] go;
    public int[] percentage;
    public int seed;
    public Terrain ThisTerrain;
    public Terrain TerTop;
    public Terrain TerLeft;
    public Terrain TerRight;
    public Terrain TerBottom;
    public bool Stitched = false;

    public void Starter()
    {
        Terrain terrrain = this.gameObject.AddComponent<Terrain>();
        terrrain.terrainData = new TerrainData();
        terrrain.terrainData.heightmapResolution = 256;
        terrrain.terrainData.size = new Vector3(512, 2048, 512);
        TerrainCollider terrraincollider = this.gameObject.AddComponent<TerrainCollider>();
        terrraincollider.terrainData = terrrain.terrainData;

        float fl = UnityEngine.Random.Range(0.0f, 100000.0f);
        //PrepareTerrain();
        float[,] h = new float[terrrain.terrainData.heightmapResolution, terrrain.terrainData.heightmapResolution];

        Vector3 gopos = terrrain.transform.position;
        float cwidth = terrrain.terrainData.size.x;
        int resolution = terrrain.terrainData.heightmapResolution;
        float[,] hmap = new float[resolution, resolution];
        double yoffset = 0 - (gopos.x / cwidth);
        double xoffset = (gopos.z / cwidth);

        Noise2D tmpNoiseMap = new Noise2D(resolution, resolution, new Perlin(0.25, 1, 0.5, 0, 0, QualityMode.High));
        tmpNoiseMap.GeneratePlanar(xoffset, (xoffset) + (1f / resolution) * (resolution + 1), -yoffset, (-yoffset) + (1f / resolution) * (resolution + 1));

        for (int hY = 0; hY < resolution; hY++)
        {
            for (int hX = 0; hX < resolution; hX++)
            {
                hmap[hX, hY] = (float)((((tmpNoiseMap[hX, hY] * 0.5f) + 0.5f) / 10) + 0.25f);
            }
        }

        terrrain.terrainData.SetHeights(0, 0, hmap);



        terrrain.terrainData.alphamapResolution = 256;
        terrrain.terrainData.splatPrototypes = DataBaseHandler.DataBase.SplatsPrototypes;
        float[, ,] amap = new float[terrrain.terrainData.alphamapResolution, terrrain.terrainData.alphamapResolution, terrrain.terrainData.splatPrototypes.Length];

        for (int hY = 0; hY < terrrain.terrainData.alphamapResolution; hY++)
        {
            for (int hX = 0; hX < terrrain.terrainData.alphamapResolution; hX++)
            {
                amap[hX, hY, Mathf.RoundToInt(((tmpNoiseMap[hX, hY] * 0.5f) + 0.5f))] = 1;
            }
        }

        terrrain.terrainData.SetAlphamaps(0, 0, amap);
    }
}
