using UnityEngine;
using System;
using System.Collections;
using LibNoise.Unity;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;

public class SimpleTerrainGenerator : MonoBehaviour
{
    public GameObject[] go;
    public int[] percentage;
    public int seed;
    //public HeightMap HeightMap;
    //public BiomeMap BiomeMap;
    public Terrain ThisTerrain;
    public Terrain TerTop;
    public Terrain TerLeft;
    public Terrain TerRight;
    public Terrain TerBottom;

    // Use this for initialization
    public void Starter()
    {
        Terrain terrrain = this.gameObject.AddComponent<Terrain>();
        TerrainData terrraindata = terrrain.terrainData = new TerrainData();
        terrrain.terrainData.heightmapResolution = 256;
        terrrain.terrainData.size = new Vector3(512, 512, 512);
        TerrainCollider terrraincollider = this.gameObject.AddComponent<TerrainCollider>();
        terrraincollider.terrainData = terrraindata;

        float fl = UnityEngine.Random.Range(0.0f, 100000.0f);
        //PrepareTerrain();
        float[,] h = new float[terrraindata.heightmapResolution, terrraindata.heightmapResolution];

        Vector3 gopos = terrrain.transform.position;
        float cwidth = terrraindata.size.x;
        int resolution = terrraindata.heightmapResolution;
        float[,] hmap = new float[resolution, resolution];
        double yoffset = 0 - (gopos.x / cwidth);
        double xoffset = (gopos.z / cwidth);
        Noise2D tmpNoiseMap = new Noise2D(resolution, resolution, new Perlin(0.25, 1, 0.5, 4, 0, QualityMode.High));
        tmpNoiseMap.GeneratePlanar(xoffset, (xoffset) + (1f / resolution) * (resolution + 1), -yoffset, (-yoffset) + (1f / resolution) * (resolution + 1));

            for (int hY = 0; hY < resolution; hY++)
            {
                for (int hX = 0; hX < resolution; hX++)
                {
                    hmap[hX, hY] = ((tmpNoiseMap[hX, hY] * 0.5f) + 0.5f) * 1;
                }
            }
        

        terrrain.GetComponent<Terrain>().terrainData.SetHeights(0, 0, hmap);

        //for (float i = 0; i < Ter.terrainData.heightmapHeight; i++)
        //{
        //    for (float j = 0; j < Ter.terrainData.heightmapHeight; j++)
        //    {

        //        h[(int)i, (int)j] = SimplexNoise.Noise(i / 64.0f, j / 64.0f);
        //        h[(int)i, (int)j] += 1;
        //        h[(int)i, (int)j] *= 0.5f;
        //    }
        //}

        //Ter.terrainData.SetHeights(0, 0, h);


        //TODO: Build this up clean and nice :D n^2 function
        //float[,,] splat = new float[1024,1024,2];
        //for (int x = 0; x < 1024; x++)
        //    for (int y = 0; y < 1024; y++)
        //    {
        //        splat[x, y, 0] = 0.5f;
        //        splat[x, y, 1] = 0.5f;
        //    }

        //ThisTerrain.terrainData.SetAlphamaps(0, 0, splat);
    }

    //public void PrepareTerrain()
    //{
    //    ThisTerrain = GetComponent<Terrain>();
    //    //BiomeMap = new BiomeMap(ThisTerrain.terrainData.alphamapHeight);
    //    //BiomeMap.SetNoise(2);
    //    //BiomeMap.Smoothen();
    //    //BiomeMap.Normalize();
    //    //ThisTerrain.terrainData.SetAlphamaps(0, 0, TerrainTools.TerrainTexture(ThisTerrain.terrainData.alphamapHeight, ThisTerrain.terrainData.alphamapHeight, 4, BiomeMap));

    //    HeightMap = new HeightMap(ThisTerrain.terrainData.heightmapHeight);
    //    HeightMap.SetNoise(0.25f, 32, 0.5f, 2);
    //    //HeightMap.Perturb(0, 0);
    //    //HeightMap.Erode(8);
    //    //for (int i = 0; i < 16; i++)
    //    //HeightMap.Smoothen();
    //    HeightMap.Normalize();
    //    new WaitForEndOfFrame();
    //    ThisTerrain.terrainData.SetHeights(0, 0, HeightMap.Heights);
    //    ThisTerrain.SetNeighbors(TerLeft, TerTop, TerRight, TerBottom);
    //    if (TerLeft != null)
    //    {
    //        StartCoroutine(TerrainTools.StitchTerrains(ThisTerrain, TerLeft, 128, 0, 20));
    //    }
    //    if (TerTop != null)
    //    {
    //        StartCoroutine(TerrainTools.StitchTerrains(ThisTerrain, TerTop, 128, 0, 20));
    //    }
    //    if (TerRight != null)
    //    {
    //        StartCoroutine(TerrainTools.StitchTerrains(ThisTerrain, TerRight, 128, 0, 20));
    //    }
    //    if (TerBottom != null)
    //    {
    //        StartCoroutine(TerrainTools.StitchTerrains(ThisTerrain, TerBottom, 128, 0, 20));
    //    }
    //    Debug.Log(Mathf.PerlinNoise(1, 1));
    //    //GetComponent<GraphUpdateScene>().Apply();
    //    ////new AstarPath().
    //    //GameObject.Find("A*").GetComponent<AstarPath>().Scan();
    //    //Debug.Log(GameObject.Find("A*").GetComponent<AstarPath>().graphTypes[0].ToString());
    //}

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0, 0, 120, 50), "Generate"))
    //    {
    //        Starter();
    //    }
    //}
}
