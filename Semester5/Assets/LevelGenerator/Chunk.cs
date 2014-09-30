using UnityEngine;
using System;
using System.Collections.Generic;
using LibNoise;
using LibNoise.Operator;
using LibNoise.Generator;

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
    public List<BresenhamValues>[] bresenhamvalues = new List<BresenhamValues>[4];
    float[,] RainShadow;

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
        bresenhamvalues[0] = new List<BresenhamValues>();
        bresenhamvalues[1] = new List<BresenhamValues>();
        bresenhamvalues[2] = new List<BresenhamValues>();
        bresenhamvalues[3] = new List<BresenhamValues>();

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

        Noise2D temperature = new Noise2D(resolution, resolution, new Perlin(0.25, 2, 0.5, 10, 0, QualityMode.High));
        temperature.GeneratePlanar(yOffset, yOffset + 1, xOffset, xOffset + 1);

        Noise2D rainfall = new Noise2D(resolution, resolution, new Perlin(0.25, 2, 0.5, 10, 0, QualityMode.High));
        rainfall.GeneratePlanar(yOffset, yOffset + 1, xOffset, xOffset + 1);

        Noise2D WindMap = new Noise2D(resolution, resolution, new Perlin(0.25, 2, 0.5, 10, 0, QualityMode.High));
        WindMap.GeneratePlanar(yOffset, yOffset + 1, xOffset, xOffset + 1);

        Noise2D RainCalcMap = new Noise2D(resolution, resolution + 50, new Perlin(0.25, 2, 0.5, 10, 0, QualityMode.High));
        RainCalcMap.GeneratePlanar(xOffset, (xOffset + 1), yOffset, (yOffset + 1) + (50.0 / resolution));


        RainShadow = new float[ThisTerain.terrainData.alphamapResolution, ThisTerain.terrainData.alphamapResolution];
        //Texture2D tex = new Texture2D(resolution, resolution + 50);
        for (int i = 0; i < RainCalcMap.Width; i++)
        {
            for (int j = 0; j < RainCalcMap.Height; j++)
            {
                if (RainCalcMap[i, j] <= 0f)
                {
                    //TODO: Make a call check if we can draw to any neibor chunks otherwise make all chunks check if there is lines that haven't yet benn drawn and lastly clean it up.
                    Bresenham(i, i, j - 50, j);
          //          tex.SetPixel(i, j, Color.white);
                }
            }
        }
        //tex.Apply();
        //System.IO.File.WriteAllBytes(this.gameObject.name + ".png", tex.EncodeToPNG());

        //Smoothen(ref RainShadow);
        for (int i = 0; i < ThisTerain.terrainData.alphamapResolution; i++)
        {
            for (int j = 0; j < ThisTerain.terrainData.alphamapResolution; j++)
            {
				try
				{
                    biomes[i, j] = DataBaseHandler.DataBase.BiomeDiagram[Mathf.RoundToInt((((temperature[i, j] * 0.5f) + 0.5f) + (CosGradient(i + (yOffset * (float)resolution), (float)resolution, 0.5f) * 0.5f) + 0.5f) / 2), Mathf.RoundToInt(((RainShadow[j, i] * 0.5f) + 0.5f))];
				}
				catch(Exception ex)
				{
				    //Console.WriteLine(ex.ToString() + fi + ";" + fj + ";" + i + ";" + j);
				}
			}
		}

        return biomes;
    }

    void Bresenham(int x0, int x1, int y0, int y1)
    {
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
                RainShadow[x0, y0] = 1.0f;
            }
            //else
            //{
            //    if (y0 < 0)
            //    {
            //        y0 = RainShadow.GetLength(1) - 1;
            //        bresenhamvalues[0].Add(new BresenhamValues(x0, x1, y0, y1, sx, sy, dx, dy, err));
            //    }
            //    if (x0 < 0)
            //    {
            //        x0 = RainShadow.GetLength(0) - 1;
            //        bresenhamvalues[2].Add(new BresenhamValues(x0, x1, y0, y1, sx, sy, dx, dy, err));
            //    }

            //    if (y0 >= RainShadow.GetLength(1))
            //    {
            //        y0 = 0;
            //        bresenhamvalues[1].Add(new BresenhamValues(x0, x1, y0, y1, sx, sy, dx, dy, err));
            //    }
            //    if (x0 >= RainShadow.GetLength(0))
            //    {
            //        x0 = 0;
            //        bresenhamvalues[3].Add(new BresenhamValues(x0, x1, y0, y1, sx, sy, dx, dy, err));
            //    }
            //    return;
            //}
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

    void Bresenham(int x0, int x1, int y0, int y1, int sx, int sy, int dx, int dy, int err)
    {
        bool loop = true;
        while (loop)
        {
            if ((y0 >= 0 && y0 < RainShadow.GetLength(1)) && (x0 >= 0 && x0 < RainShadow.GetLength(0)))
            {
                RainShadow[x0, y0] = 1.0f;
            }
            else
            {
                if (y0 < 0)
                {
                }
                if (x0 < 0)
                {
                }

                if (y0 >= RainShadow.GetLength(1))
                {
                }
                if (x0 >= RainShadow.GetLength(0))
                {
                }
                return;
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

        for (int hY = 0; hY < ThisTerain.terrainData.alphamapResolution; hY++)
        {
            for (int hX = 0; hX < ThisTerain.terrainData.alphamapResolution; hX++)
            {
                amap[hX, hY, (int)Biomes[hX, hY]] = 1;
            }
        }
        ThisTerain.terrainData.SetAlphamaps(0, 0, amap);
    }

    Chunk GetNeighbor(Direction dir)
    {
        switch (dir)
        {
            case Direction.North:
                return ChunkLoader.chunkloader.GetChunkAtChunkLocation((int)xOffset, (int)yOffset + 1);
            case Direction.South:
                return ChunkLoader.chunkloader.GetChunkAtChunkLocation((int)xOffset, (int)yOffset - 1);
            case Direction.West:
                return ChunkLoader.chunkloader.GetChunkAtChunkLocation((int)xOffset - 1, (int)yOffset + 1);
            case Direction.East:
                return ChunkLoader.chunkloader.GetChunkAtChunkLocation((int)xOffset + 1, (int)yOffset);
            default:
                return null;
        }
    }

    public struct NeightborChunks
    {
        public Chunk North;
        public Chunk South;
        public Chunk West;
        public Chunk East;

        //public NeightborChunks(Chunk n, Chunk s, Chunk w, Chunk e)
        //{
        //    North = n;
        //    South = s;
        //    West = w;
        //    East = e;
        //}
    }

    public struct BresenhamValues
    {
        int x0; int x1; int y0; int y1; int sx; int sy; int dx; int dy; int err;

        public BresenhamValues(int x0, int x1, int y0, int y1, int sx, int sy, int dx, int dy, int err)
        {
            this.x0 = x0;
            this.x1 = x1;
            this.y0 = y0;
            this.y1 = y1;
            this.sx = sx;
            this.sy = sy;
            this.dx = dx;
            this.dy = dy;
            this.err = err; ;
        }
    }
}
