using UnityEngine;
using System;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Operator;
using LibNoise.Generator;

public class Chunk : MonoBehaviour
{
    Vector2[,] WindDirection;    
    float[,] RainShadow;
    int MaxWindSpeed = 50;
    Texture2D rain1;
    Texture2D rain2;


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

    public void Generate()
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
                hmap[hX, hY] = (float)((FlatNoiseMap[hX, hY] * 0.5f) + 0.5f);
            }
        }

        ThisTerain.terrainData.SetHeights(0, 0, hmap);
    }

    BiomeTypes[,] GenerateBiomes()
    {
        BiomeTypes[,] biomes = new BiomeTypes[ThisTerain.terrainData.alphamapResolution, ThisTerain.terrainData.alphamapResolution];

        int resolution = ThisTerain.terrainData.alphamapResolution;

        Noise2D heightMap = new Noise2D(resolution, resolution, new Perlin(0.25, 2, 0.5, 10, 0, QualityMode.High));
        heightMap.GeneratePlanar(yOffset, yOffset + 1, xOffset, xOffset + 1);

        Noise2D rainfall = new Noise2D(resolution, resolution, new Perlin(0.25, 2, 0.5, 10, 0, QualityMode.High));
        rainfall.GeneratePlanar(yOffset, yOffset + 1, xOffset, xOffset + 1);

        Noise2D WindMap = new Noise2D((resolution + (MaxWindSpeed * 2)), (resolution + (MaxWindSpeed * 2)), new Perlin(0.25, 2, 0.5, 10, 0, QualityMode.High));
        WindMap.GeneratePlanar(yOffset - ((double)MaxWindSpeed / (double)resolution), (yOffset + 1) + ((double)MaxWindSpeed / (double)resolution), xOffset - ((double)MaxWindSpeed / (double)resolution), (xOffset + 1) + ((double)MaxWindSpeed / (double)resolution));

        Noise2D RainCalcMap = new Noise2D((resolution + (MaxWindSpeed * 2)), (resolution + (MaxWindSpeed * 2)), new Perlin(0.25, 2, 0.5, 10, 0, QualityMode.High));
        RainCalcMap.GeneratePlanar(xOffset - ((double)MaxWindSpeed / (double)resolution), (xOffset + 1) + ((double)MaxWindSpeed / (double)resolution), yOffset - ((double)MaxWindSpeed / (double)resolution), (yOffset + 1) + ((double)MaxWindSpeed / (double)resolution));


        RainShadow = new float[ThisTerain.terrainData.alphamapResolution, ThisTerain.terrainData.alphamapResolution];
        WindDirection = new Vector2[(resolution + (MaxWindSpeed * 2)), (resolution + (MaxWindSpeed * 2))];
        rain2 = new Texture2D(resolution, resolution + 50);
        for (int i = 0; i < resolution + (MaxWindSpeed * 2); i++)
        {
            for (int j = 0; j < resolution + (MaxWindSpeed * 2); j++)
            {
                float val = ((((CosGradient((i - 50) + (yOffset * (float)(resolution)), (float)(resolution), 0.25f) * 0.5f) + 0.5f) + (((WindMap[i,j]) * 0.5f) + 0.5f) / 2) * 12.0f);
                if (val >= 0 && val < 2)
                {
                    if (val >= 1)
                    {
                        WindDirection[i, j] = new Vector2(-1.0f, (val - 2.0f));
                    }
                    else
                    {
                        WindDirection[i, j] = new Vector2(val * -1.0f, -1.0f);
                    }
                }
                else if (val >= 2 && val < 4)
                {
                    if (val >= 3)
                    {
                        WindDirection[i, j] = new Vector2((4.0f - val), 1.0f);
                    }
                    else
                    {
                        WindDirection[i, j] = new Vector2(1.0f, val - 2.0f);
                    }
                }
                else if (val >= 4 && val < 6)
                {
                    if (val >= 5)
                    {
                        WindDirection[i, j] = new Vector2(-1.0f, (val - 6.0f));
                    }
                    else
                    {
                        WindDirection[i, j] = new Vector2((val - 4.0f) * -1.0f, -1.0f);
                    }
                }
                else if (val >= 6 && val < 8)
                {
                    if (val >= 7)
                    {
                        WindDirection[i, j] = new Vector2(val - 8.0f, 1.0f);
                    }
                    else
                    {
                        WindDirection[i, j] = new Vector2(-1.0f, (val - 6.0f));
                    }
                }
                else if (val >= 8 && val < 10)
                {
                    if (val >= 9)
                    {
                        WindDirection[i, j] = new Vector2(1.0f, (10.0f - val) * -1.0f);
                    }
                    else
                    {
                        WindDirection[i, j] = new Vector2((val - 8.0f), -1.0f);
                    }
                }
                else if (val >= 10 && val <= 12)
                {
                    if (val >= 11)
                    {
                        WindDirection[i, j] = new Vector2((val - 12.0f), 1.0f);
                    }
                    else
                    {
                        WindDirection[i, j] = new Vector2(-1.0f, (val - 10.0f));
                    }
                }
            }
        }
        for (int i = -MaxWindSpeed; i < RainCalcMap.Width - MaxWindSpeed; i++)
        {
            for (int j = -MaxWindSpeed; j < RainCalcMap.Height - MaxWindSpeed; j++)
            {
                if (RainCalcMap[i + MaxWindSpeed, j + MaxWindSpeed] <= 0f)
                {
                    Bresenham(i, i + (int)(WindDirection[j + MaxWindSpeed, i + MaxWindSpeed].x * MaxWindSpeed), j, j + (int)(WindDirection[j + MaxWindSpeed, i + MaxWindSpeed].y * MaxWindSpeed));
                }
            }
        }
        //TODO: Remove the rain1 and rain2 textures from the game!
        rain1 = new Texture2D(128, 128);
        //Smoothen(ref RainShadow);
        for (int i = 0; i < ThisTerain.terrainData.alphamapResolution; i++)
        {
            for (int j = 0; j < ThisTerain.terrainData.alphamapResolution; j++)
            {
				try
				{
                    if (rainfall[i, j] <= 0)
                    {
                        rain1.SetPixel(j, i, Color.white);
                    }
                    biomes[i, j] = DataBaseHandler.DataBase.BiomeDiagram[Mathf.RoundToInt((((heightMap[i, j] * 0.5f) + 0.5f) + (CosGradient(i + (yOffset * (float)(resolution)), (float)(resolution), 0.5f) * 0.5f) + 0.5f) / 2), Mathf.RoundToInt((RainShadow[i, j] + ((rainfall[i, j] * 0.5f) + 0.5f) / 2) * 2)];

				}
				catch(Exception ex)
				{
				}
			}
		}
        rain1.Apply();
        System.IO.File.WriteAllBytes(this.gameObject.name + ".png", rain1.EncodeToPNG());

        return biomes;
    }

    void Bresenham(int x0, int x1, int y0, int y1)
    {
        //TODO: Add smooth Value curves - ie  the value getting smaller and smaller per point it's getting away from the original one.
        int sx = 0;
        int sy = 0;
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        if (x0 < x1) { sx = 1; } else { sx = -1; }
        if (y0 < y1) { sy = 1; } else { sy = -1; }
        int err = dx - dy;

        bool loop = true;
        while (loop)
        {
            if ((y0 >= 0 && y0 < RainShadow.GetLength(1)) && (x0 >= 0 && x0 < RainShadow.GetLength(0)))
            {
                RainShadow[y0, x0] = 1.0f;
            }
            if ((x0 == x1) && (y0 == y1)) loop = false;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err = err - dy;
                x0 = x0 + sx;
            }
            if (e2 < dx)
            {
                err = err + dx;
                y0 = y0 + sy;
            }
        }
    }

    //TODO Find a Better smooth algorithm.
    public void Smoothen(ref float[,] Heights)
    {
        int i, j, u, v;
        int size = Heights.GetLength(0);
        float total;

        for (i = 1; i < size - 1; ++i)
        {
            for (j = 1; j < size - 1; ++j)
            {
                total = 0.0f;

                for (u = -1; u <= 1; u++)
                {
                    for (v = -1; v <= 1; v++)
                    {
                        total += Heights[i + u, j + v];
                    }
                }

                Heights[i, j] = total / 9.0f;
            }
        }
    }

    float CosGradient(float f, float res, float frequency)
    {
        return ((Mathf.Cos((f / res) * Mathf.PI * frequency))); 
    }

    void GenerateAlphamap(BiomeTypes[,] Biomes)
    {
        int resolution = ThisTerain.terrainData.alphamapHeight;
        
        float[, ,] amap = new float[ThisTerain.terrainData.alphamapResolution, ThisTerain.terrainData.alphamapResolution, ThisTerain.terrainData.splatPrototypes.Length];

        for (int hX = 0; hX < ThisTerain.terrainData.alphamapResolution; hX++)
        {
            for (int hY = 0; hY < ThisTerain.terrainData.alphamapResolution; hY++)
            {
                amap[hX, hY, (int)Biomes[hX, hY]] = 1;
            }
        }
        ThisTerain.terrainData.SetAlphamaps(0, 0, amap);
    }

    void OnGUI()
    {
        //if (this.gameObject.name == "Chunk[0;-2]")
        //{
        GUI.DrawTexture(new Rect((Screen.width / 4) + (128 * xOffset), (Screen.height / 2) - (128 * yOffset), (128), (128)), rain1);
            //GUI.DrawTexture(new Rect(128 + 10, 0, 128, 128 + 50), rain2);
        //}
    }
}
