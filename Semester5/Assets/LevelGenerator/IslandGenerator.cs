using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using System.Threading;

public class IslandGenerator : MonoBehaviour
{
    float islandFullNess = 0.15f;
    float freq = 1;
    int octaves = 4;
    float pers = 0.5f;
    float lan = 2;
    float riverFullNess = 0.4f;
    float riverDensity = 0.25f;
    float treeDensity = 2f;
    bool isgenerating = true;

    Terrain ThisTerain
    {
        get
        {
            return this.gameObject.GetComponent<Terrain>();
        }
    }

	IEnumerator Start ()
    {
        if (ThisTerain == null)
        {
            GameObject go = new GameObject("Island");
            go.AddComponent<Terrain>();
            go.GetComponent<Terrain>().terrainData = new TerrainData();
            go.AddComponent<TerrainCollider>();
            go.AddComponent<IslandGenerator>();
            go.GetComponent<TerrainCollider>().terrainData = go.GetComponent<Terrain>().terrainData;
            go.GetComponent<IslandGenerator>().Setup();
            Destroy(this.gameObject.GetComponent<IslandGenerator>());
            yield return null;
        }
        yield return StartCoroutine(GenerateHeightMap());
        isgenerating = false;
	}

    void Setup()
    {
        ThisTerain.terrainData.heightmapResolution = 2048;
        ThisTerain.terrainData.size = new Vector3(4096, 512, 4096);
        ThisTerain.terrainData.alphamapResolution = 2048;
        ThisTerain.terrainData.splatPrototypes = DataBaseHandler.DataBase.SplatsPrototypes;
        ThisTerain.terrainData.SetDetailResolution(1024, 8);
        ThisTerain.heightmapPixelError = 1;
        ThisTerain.basemapDistance = 2000;
    }

    IEnumerator GenerateHeightMap()
    {
        int hresolution = ThisTerain.terrainData.heightmapResolution;
        yield return new WaitForEndOfFrame();
        Noise2D heightMap = new Noise2D(hresolution, hresolution, 
            new Select(0, 1, 1, 
                new ScaleBias(0.125, -1, new Perlin(0.125, 2, 0.5, 4, DataBaseHandler.DataBase.Seed, QualityMode.High)),
                new Select(0, 1, 0.25,
                    new Invert(new ScaleBias(1, riverFullNess, new RidgedMultifractal(riverDensity, 2, 1, DataBaseHandler.DataBase.Seed, QualityMode.High))),
                    new ScaleBias(0.5, islandFullNess, new Perlin(freq, lan, pers, octaves, DataBaseHandler.DataBase.Seed, QualityMode.High)), 
                    new Invert(new ScaleBias(1, riverFullNess, new RidgedMultifractal(riverDensity, 2, 1, DataBaseHandler.DataBase.Seed, QualityMode.High)))), 
                new CircleMask(0.25))
                );
        yield return new WaitForEndOfFrame();

        yield return StartCoroutine(heightMap.GeneratePlanarCoRoutine(-4, 4, -4, 4));

        yield return new WaitForEndOfFrame();

        float[,] hmap = new float[hresolution, hresolution];
        for (int y = 0; y < hresolution; y++)
        {
            for (int x = 0; x < hresolution; x++)
            {
                hmap[x, y] = (((heightMap[x, y]) * 0.5f) + 0.5f);
            }
        }
        ThisTerain.terrainData.SetHeights(0, 0, hmap);
        ThisTerain.Flush();
        int aresolution = ThisTerain.terrainData.alphamapResolution;
        float[, ,] amap = new float[aresolution, aresolution, ThisTerain.terrainData.splatPrototypes.Length];
        List<TreeInstance> trees = new List<TreeInstance>();
        System.Random rnd = new System.Random(DataBaseHandler.DataBase.Seed + 1);
        for (int y = 0; y < aresolution; y++)
        {
            for (int x = 0; x < aresolution; x++)
            {
                if (hmap[(int)(x * ((float)hresolution / (float)aresolution)), (int)(y * ((float)hresolution / (float)aresolution))] < 0.4)
                {
                    amap[x, y, 2] = 1;
                }
                else if (hmap[(int)(x * ((float)hresolution / (float)aresolution)), (int)(y * ((float)hresolution / (float)aresolution))] < 0.7)
                {
                    amap[x, y, 4] = 1;
                    int r = rnd.Next(0, 100);
                    if (r < treeDensity)
                    {
                        GameObject go = (GameObject)GameObject.Instantiate(DataBaseHandler.DataBase.Trees[0]);
                        go.transform.position = new Vector3(y * 2, ThisTerain.terrainData.GetHeight(y, x), x * 2);
                        go.transform.parent = this.transform;
                        
                    }
                }
                else if (hmap[(int)(x * ((float)hresolution / (float)aresolution)), (int)(y * ((float)hresolution / (float)aresolution))] < 0.8)
                {
                    amap[x, y, 8] = 1;
                }
                else
                {
                    amap[x, y, 0] = 1;
                }
            }
        }
        yield return new WaitForEndOfFrame();

        ThisTerain.terrainData.SetAlphamaps(0, 0, amap);


        ThisTerain.Flush();
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 180, 75), "Generate") && !isgenerating)
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Destroy(this.transform.GetChild(i).gameObject);
            }
            isgenerating = true;
            StartCoroutine(Start());
        }

        GUI.Label(new Rect(0, 75, 160, 28), "Seed");
        DataBaseHandler.DataBase.Seed = (int)GUI.HorizontalSlider(new Rect(0, 95, 160, 10), DataBaseHandler.DataBase.Seed, int.MinValue, int.MaxValue);
        GUI.Label(new Rect(165, 90, 160, 28), DataBaseHandler.DataBase.Seed.ToString());

        GUI.Label(new Rect(0, 110, 160, 28), "Frequency");
        freq = GUI.HorizontalSlider(new Rect(0, 130, 160, 10), freq, 0.0001f, 4.0f);
        GUI.Label(new Rect(165, 125, 160, 28), freq.ToString());

        GUI.Label(new Rect(0, 145, 160, 28), "Lacunarity");
        lan = GUI.HorizontalSlider(new Rect(0, 165, 160, 10), lan, 0.0001f, 4.0f);
        GUI.Label(new Rect(165, 160, 160, 28), lan.ToString());

        GUI.Label(new Rect(0, 180, 160, 28), "Persistense");
        pers = GUI.HorizontalSlider(new Rect(0, 200, 160, 10), pers, 0.0001f, 1.0f);
        GUI.Label(new Rect(165, 195, 160, 28), pers.ToString());

        GUI.Label(new Rect(0, 215, 160, 28), "Octaves");
        octaves = (int)GUI.HorizontalSlider(new Rect(0, 235, 160, 10), octaves, 1.0f, 10.0f);
        GUI.Label(new Rect(165, 230, 160, 28), octaves.ToString());

        GUI.Label(new Rect(0, 250, 160, 28), "IslandFullNess");
        islandFullNess = GUI.HorizontalSlider(new Rect(0, 270, 160, 10), islandFullNess, -0.5f, 0.5f);
        GUI.Label(new Rect(165, 265, 160, 28), islandFullNess.ToString());

        GUI.Label(new Rect(0, 285, 160, 28), "RiverFullNess");
        riverFullNess = GUI.HorizontalSlider(new Rect(0, 305, 160, 10), riverFullNess, -0.5f, 0.5f);
        GUI.Label(new Rect(165, 300, 160, 28), riverFullNess.ToString());

        GUI.Label(new Rect(0, 320, 160, 28), "River Density");
        riverDensity = GUI.HorizontalSlider(new Rect(0, 340, 160, 10), riverDensity, 0, 2f);
        GUI.Label(new Rect(165, 335, 160, 28), riverDensity.ToString());

        string warning = (treeDensity <= 3) ? "" : "WARNING: MAY CAUSE LAG!";
        GUI.Label(new Rect(0, 355, 1600, 28), "Tree Density " + warning);
        treeDensity = GUI.HorizontalSlider(new Rect(0, 375, 160, 10), treeDensity, 0, 10f);
        GUI.Label(new Rect(165, 370, 160, 28), treeDensity.ToString());

        if (isgenerating)
        {
            GUIStyle gs = new GUIStyle();
            gs.fontSize = 96;

            GUI.Label(new Rect((Screen.width / 2) - 640, (Screen.height / 2) - 240, 640, 480), "Generating, Please be patience!", gs);
        }
    }
}
