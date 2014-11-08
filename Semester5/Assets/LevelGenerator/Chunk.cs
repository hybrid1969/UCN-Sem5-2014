using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using LibNoise;
using LibNoise.Operator;
using LibNoise.Generator;

public class Chunk : MonoBehaviour
{
    public Vector2[,] WindDirection;
    public BiomeTypes[,] biomes;
    public float[,] RainShadow;
    public float[,] Temperature;
    public float[,] Humidity;
    public float[,] Heightmap;
    int MaxWindSpeed = 50;

    NoiseHelper noisehelper;
    List<BiomeTypes> availblebiomes = new List<BiomeTypes>();

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
        noisehelper = new NoiseHelper(xOffset, yOffset);
        GenerateBiomes();
        noisehelper.SetBiomes(biomes);
        noisehelper.SetBounds(new Rect(yOffset, yOffset + 1, xOffset, xOffset + 1));
        GenerateHeightmap(biomes);
        GenerateAlphamap(biomes);
    }

    void SetupTerainData()
    {
        Terrain terrrain = this.gameObject.AddComponent<Terrain>();
        terrrain.terrainData = new TerrainData();
        terrrain.terrainData.heightmapResolution = DataBaseHandler.HeighMapSize;
        terrrain.terrainData.size = new Vector3(DataBaseHandler.ChunkSize, DataBaseHandler.ChunkSize * 2, DataBaseHandler.ChunkSize);
        TerrainCollider terrraincollider = this.gameObject.AddComponent<TerrainCollider>();
        terrraincollider.terrainData = terrrain.terrainData;
        terrrain.terrainData.alphamapResolution = DataBaseHandler.BiomeMapSize;
        terrrain.terrainData.splatPrototypes = DataBaseHandler.DataBase.SplatsPrototypes;
    }

    void GenerateHeightmap(BiomeTypes[,] biomes)
    {
        int resolution = ThisTerain.terrainData.heightmapResolution;
        Heightmap = new float[resolution, resolution];
        List <LibNoise.ModuleBase> modules = new List<ModuleBase>();
        for (int i = 0; i < availblebiomes.Count; i++)
        {
            if (!modules.Contains(Biome.FindBiome(availblebiomes[i]).Generate(noisehelper)))
            {
                modules.Add(Biome.FindBiome(availblebiomes[i]).Generate(noisehelper));
            }
        }
        
        for (int i = 1; i < modules.Count; i++)
        {
            if (modules[i] != null)
            {
                modules[i][0] = modules[i - 1];
            }
        }
        Noise2D heightMap = new Noise2D(resolution, resolution, modules[0]);
        heightMap.GeneratePlanar(yOffset, yOffset + 1.0f, xOffset, xOffset + 1.0f);

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                Heightmap[x, y] = (((heightMap[x, y]) * 0.5f) + 0.5f);// (float)((FlatNoiseMap[x, y] * 0.5f) + 0.5f);
            }
        }

        ThisTerain.terrainData.SetHeights(0, 0, Heightmap);
        ThisTerain.Flush();
    }

    void GenerateTemperature()
    {
        
    }

    void GenerateWind()
    {
        Noise2D WindMap = new Noise2D((DataBaseHandler.BiomeMapSize + (MaxWindSpeed * 2)), (DataBaseHandler.BiomeMapSize + (MaxWindSpeed * 2)), new Perlin(0.125, 2, 0.5, 10, DataBaseHandler.DataBase.Seed, QualityMode.High));
        WindMap.GeneratePlanar(yOffset - ((double)MaxWindSpeed / (double)DataBaseHandler.BiomeMapSize), (yOffset + 1) + ((double)MaxWindSpeed / (double)DataBaseHandler.BiomeMapSize), xOffset - ((double)MaxWindSpeed / (double)DataBaseHandler.BiomeMapSize), (xOffset + 1) + ((double)MaxWindSpeed / (double)DataBaseHandler.BiomeMapSize));

        WindDirection = new Vector2[(DataBaseHandler.BiomeMapSize + (MaxWindSpeed * 2)), (DataBaseHandler.BiomeMapSize + (MaxWindSpeed * 2))];
        for (int i = 0; i < DataBaseHandler.BiomeMapSize + (MaxWindSpeed * 2); i++)
        {
            for (int j = 0; j < DataBaseHandler.BiomeMapSize + (MaxWindSpeed * 2); j++)
            {
                float val = ((((CosGradient((i - 50) + (yOffset * (float)(DataBaseHandler.BiomeMapSize)), (float)(DataBaseHandler.BiomeMapSize), 0.125f) * 0.5f) + 0.5f) + (((WindMap[i, j]) * 0.5f) + 0.5f) / 2) * 12.0f);
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
    }

    void GenerateRainShadow()
    {
        RainShadow = new float[ThisTerain.terrainData.alphamapResolution, ThisTerain.terrainData.alphamapResolution];

        Noise2D RainCalcMap = new Noise2D((DataBaseHandler.BiomeMapSize + (MaxWindSpeed * 2)), (DataBaseHandler.BiomeMapSize + (MaxWindSpeed * 2)), new Perlin(0.125, 2, 0.5, 10, DataBaseHandler.DataBase.Seed, QualityMode.High));
        RainCalcMap.GeneratePlanar(xOffset - ((double)MaxWindSpeed / (double)DataBaseHandler.BiomeMapSize), (xOffset + 1) + ((double)MaxWindSpeed / (double)DataBaseHandler.BiomeMapSize), yOffset - ((double)MaxWindSpeed / (double)DataBaseHandler.BiomeMapSize), (yOffset + 1) + ((double)MaxWindSpeed / (double)DataBaseHandler.BiomeMapSize));

        for (int i = -MaxWindSpeed; i < RainCalcMap.Width - MaxWindSpeed; i++)
        {
            for (int j = -MaxWindSpeed; j < RainCalcMap.Height - MaxWindSpeed; j++)
            {
                if (RainCalcMap[i + MaxWindSpeed, j + MaxWindSpeed] <= 0.5f)
                {
                    Bresenham(i, i + (int)(WindDirection[j + MaxWindSpeed, i + MaxWindSpeed].x * MaxWindSpeed), j, j + (int)(WindDirection[j + MaxWindSpeed, i + MaxWindSpeed].y * MaxWindSpeed));
                }
            }
        }
    }

    void GenerateBiomes()
    {
        int resolution = ThisTerain.terrainData.alphamapResolution;

        Noise2D heightMap = new Noise2D(resolution, resolution, new Perlin(0.125, 2, 0.5, 10, DataBaseHandler.DataBase.Seed, QualityMode.High));
        heightMap.GeneratePlanar(yOffset, yOffset + 1.0f, xOffset, xOffset + 1.0f);

        Noise2D rainfall = new Noise2D(resolution, resolution, new Perlin(0.125, 2, 0.5, 10, DataBaseHandler.DataBase.Seed, QualityMode.High));
        rainfall.GeneratePlanar(yOffset, yOffset + 1.0f, xOffset, xOffset + 1.0f);

        Humidity = new float[ThisTerain.terrainData.alphamapResolution, ThisTerain.terrainData.alphamapResolution];
        biomes = new BiomeTypes[ThisTerain.terrainData.alphamapResolution, ThisTerain.terrainData.alphamapResolution];
        Temperature = new float[ThisTerain.terrainData.alphamapResolution, ThisTerain.terrainData.alphamapResolution];

        for (int i = 0; i < ThisTerain.terrainData.alphamapResolution; i++)
        {
            for (int j = 0; j < ThisTerain.terrainData.alphamapResolution; j++)
            {
                Temperature[i, j] = -50.0f + (((((heightMap[i, j] * 0.5f) + 0.5f) + (CosGradient(i + (yOffset * (float)(resolution)), (float)(resolution), 0.25f) * 0.5f) + 0.5f) / 2.0f) * 100.0f);
                Humidity[i, j] = ((((CosGradient(i + (yOffset * (float)(resolution)), (float)(resolution), 0.25f) * 0.5f) + 0.5f)) * 100.0f) * (((rainfall[i, j] * 0.5f) + 0.5f));
                //Humidity[i, j] = ((((rainfall[i, j] * 0.5f) + 0.5f)) * 100.0f);// *(((((heightMap[i, j] * 0.75f) + 0.5f) + (CosGradient(i + (yOffset * (float)(resolution)), (float)(resolution), 0.5f) * 0.25f) + 0.5f) / 2.0f));
                //Old Biome Selector
                //biomes[i, j] = DataBaseHandler.DataBase.BiomeDiagram[Mathf.RoundToInt((((heightMap[i, j] * 0.5f) + 0.5f) + (CosGradient(i + (yOffset * (float)(resolution)), (float)(resolution), 0.5f) * 0.5f) + 0.5f) / 2), Mathf.RoundToInt((RainShadow[i, j] + ((rainfall[i, j] * 0.5f) + 0.5f) / 2) * 2)];
                biomes[i, j] = Biome.DecideBiome(Temperature[i, j], Humidity[i, j]);
                if (!availblebiomes.Contains(biomes[i, j]))
                {
                    availblebiomes.Add(biomes[i, j]);
                }
            } //tmpNoiseMap.GeneratePlanar(xoffset, (xoffset) + (1f / resolution) * (resolution + 1), -yoffset, (-yoffset) + (1f / resolution) * (resolution + 1));

		}
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
                RainShadow[y0, x0] = 1;
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
                amap[hX, hY, Biome.FindTexture(Biomes[hX, hY])] = 1;
            }
        }
        ThisTerain.terrainData.SetAlphamaps(0, 0, amap);
        ThisTerain.Flush();
    }

    public void GenerateFoliage()
    {
        for (int i = 0; i < availblebiomes.Count; i++)
        {
            DataBaseHandler.DataBase.biomes.First(b => b.Type == availblebiomes[i]).Decorate(this);
        }
    }
}
