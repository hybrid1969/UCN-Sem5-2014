using UnityEngine;
using System;
using System.Collections;
using LibNoise.Unity;
using LibNoise.Unity.Operator;
using LibNoise.Unity.Generator;

public class Chunk : MonoBehaviour
{
    //public GameObject[] go;
    //public int[] percentage;
    //public int seed;
    //public Terrain ThisTerrain;
    //public Terrain TerTop;
    //public Terrain TerLeft;
    //public Terrain TerRight;
    //public Terrain TerBottom;
    //public bool Stitched = false;

    float xOffset
    {
        get
        {
            return this.transform.position.x / ThisTerain.terrainData.size.x;
        }
    }

    float yOffset
    {
        get
        {
            return this.transform.position.z / ThisTerain.terrainData.size.x;
        }
    }

    Terrain ThisTerain
    {
        get
        {
            return this.gameObject.GetComponent<Terrain>();
        }
    }

    public void Starter()
    {
        SetupTerainData();
        BiomeTypes[,] Biomes = GenerateBiomes();
        //GenerateHeightmap(Biomes);
        GenerateAlphamap(Biomes);
        
    }

    void SetupTerainData()
    {
        Terrain terrrain = this.gameObject.AddComponent<Terrain>();
        terrrain.terrainData = new TerrainData();
        terrrain.terrainData.heightmapResolution = 128;
        terrrain.terrainData.size = new Vector3(512, 512, 512);
        TerrainCollider terrraincollider = this.gameObject.AddComponent<TerrainCollider>();
        terrraincollider.terrainData = terrrain.terrainData;
        terrrain.terrainData.alphamapResolution = 128;
        terrrain.terrainData.splatPrototypes = DataBaseHandler.DataBase.SplatsPrototypes;
    }

    void GenerateHeightmap(BiomeTypes[,] Biome)
    {
        int resolution = ThisTerain.terrainData.heightmapResolution;
        float[,] hmap = new float[resolution, resolution];
        Noise2D FlatNoiseMap = new Noise2D(resolution, resolution, new Perlin(0.25, 1, 0.5, 0, 0, QualityMode.High));

        FlatNoiseMap.GeneratePlanar(yOffset, (yOffset) + (1f / resolution) * (resolution + 1), xOffset, (xOffset) + (1f / resolution) * (resolution + 1));

        for (int hY = 0; hY < resolution; hY++)
        {
            for (int hX = 0; hX < resolution; hX++)
            {
                //hmap[hX, hY] = (float)((((FlatNoiseMap[hX, hY] * 0.5f) + 0.5f) / 10) + 0.25f);
                hmap[hX, hY] = (float)((FlatNoiseMap[hX, hY] * 0.5f) + 0.5f);
            }
        }

        ThisTerain.terrainData.SetHeights(0, 0, hmap);
    }

    BiomeTypes[,] GenerateBiomes()
    {
        BiomeTypes[,] biomes = new BiomeTypes[ThisTerain.terrainData.alphamapResolution, ThisTerain.terrainData.alphamapResolution];

        int resolution = ThisTerain.terrainData.alphamapResolution;

        Noise2D temp = new Noise2D(resolution, resolution, new Perlin(0.5, 2, 0.5, 20, 0, QualityMode.High));
        temp.GeneratePlanar(yOffset, (yOffset) + (1f / resolution) * (resolution + 1), xOffset, (xOffset) + (1f / resolution) * (resolution + 1));

        Noise2D RainFall = new Noise2D(resolution, resolution, new Billow(0.5, 2, 0.5, 20, 0, QualityMode.High));
        RainFall.GeneratePlanar(yOffset, (yOffset) + (1f / resolution) * (resolution + 1), xOffset, (xOffset) + (1f / resolution) * (resolution + 1));


        for (int i = 0; i < ThisTerain.terrainData.alphamapResolution; i++)
        {
            //float cg = CosGradient((float)(i + (yOffset * (float)resolution)), (float)resolution, 0.5f);

            for (int j = 0; j < ThisTerain.terrainData.alphamapResolution; j++)
            {
				//float fi = Mathf.RoundToInt((float)((Temperature.GetValue(i, 0, j) * 0.25f) + 0.5f));
				//float fj = Mathf.RoundToInt((((float)RainFall[i, j] * 0.5f) + 0.5f) * 2);
				//if (fi >= 2 || fi < 0)
					//Debug.Log(fi); 
				//if (fi == 0 && fj == 0)
					//Debug.Log(i + ";" + j);
				try
				{
                    biomes[i, j] = DataBaseHandler.DataBase.BiomeDiagram[Mathf.RoundToInt((((temp[i, j] * 0.5f) + 0.5f) + (CosGradient(i + (yOffset * (float)resolution), resolution, 0.5f))) / 2), Mathf.RoundToInt((float)(((RainFall[i,j]* 0.5f) + 0.5f) * 2))];
                    //biomes[i, j] = DataBaseHandler.DataBase.BiomeDiagram[Mathf.RoundToInt((float)((Temperature.GetValue(i + (yOffset * resolution), 0, j + (xOffset * resolution)) * 0.25f) + 0.5f)), 1];
					//Debug.Log(i + ";" + j);
				}
				catch(Exception ex)
				{
				    //Console.WriteLine(ex.ToString() + fi + ";" + fj + ";" + i + ";" + j);
				}
			}

		}

        return biomes;
    }

    float CosGradient(float f, float res, float frequency)
    {
        return (((Mathf.Cos((f / res) * Mathf.PI * 0.5f)) * 0.5f) + 0.5f); 
    }

    void GenerateAlphamap(BiomeTypes[,] Biomes)
    {
        int resolution = ThisTerain.terrainData.alphamapHeight;
        
        float[, ,] amap = new float[ThisTerain.terrainData.alphamapResolution, ThisTerain.terrainData.alphamapResolution, ThisTerain.terrainData.splatPrototypes.Length];

        for (int hY = 0; hY < ThisTerain.terrainData.alphamapResolution; hY++)
        {
            for (int hX = 0; hX < ThisTerain.terrainData.alphamapResolution; hX++)
            {
                amap[hX, hY, (int)Biomes[hX, hY]] = 1;
            }
        }
        ThisTerain.terrainData.SetAlphamaps(0, 0, amap);
    }
}
