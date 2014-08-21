//using UnityEngine;
//using System.Collections;
//using System.Linq;
//using LibNoise.Unity.Generator;

//namespace TerrainGenerator
//{
//    public class BiomeMap
//    {
//        public float[,] Biomes { get; set; }
//        public int Size { get; private set; }
//        Perlin perlin;

//        //public float this[int x, int y]
//        //{
//        //    get { return Biomes[x, y]; }
//        //    set { Biomes[x, y] = value; }
//        //}

//        public BiomeMap(int size)
//        {
//            perlin = new Perlin(new System.Random().Next());

//            Size = size;
//            Biomes = new float[Size, Size];
//        }

//        public BiomeMap(int size, int seed)
//        {
//            perlin = new Perlin(seed);

//            Size = size;
//            Biomes = new float[Size, Size];
//        }

//        public void SetNoise(float frequency, byte octaves = 1, float persistence = 0.5f, float lacunarity = 2.0f, bool additive = false)
//        {
//            int size = Biomes.GetLength(0);
//            float fSize = (float)size;

//            for (int x = 0; x < size; x++)
//            {
//                byte currentOctave;
//                float value, currentPersistence, signal;
//                Vector2 coord;

//                for (int y = 0; y < size; y++)
//                {
//                    value = 0.0f;
//                    currentPersistence = 1.0f;
//                    coord = new Vector2(
//                        x / fSize,
//                        y / fSize);

//                    coord *= frequency;

//                    for (currentOctave = 0; currentOctave < octaves; currentOctave++)
//                    {
//                        signal = perlin.Noise2(coord.x, coord.y);
//                        value += signal * currentPersistence;
//                        coord *= lacunarity;
//                        currentPersistence *= persistence;
//                    }

//                    Biomes[x, y] = (!additive) ? value : Biomes[x, y] + value;
//                }
//            }
//        }

//        public void Smoothen()
//        {
//            int i, j, u, v;
//            int size = Biomes.GetLength(0);
//            float total;

//            for (i = 1; i < size - 1; ++i)
//            {
//                for (j = 1; j < size - 1; ++j)
//                {
//                    total = 0.0f;

//                    for (u = -1; u <= 1; u++)
//                    {
//                        for (v = -1; v <= 1; v++)
//                        {
//                            total += Biomes[i + u, j + v];
//                        }
//                    }

//                    Biomes[i, j] = total / 9.0f;
//                }
//            }
//        }

//        public void Normalize()
//        {
//            int size = Biomes.GetLength(0);
//            float min = 0f, max = 3f;
//            float MIN = Biomes.Cast<float>().Min();
//            float MAX = Biomes.Cast<float>().Max();

//            for (int x = 0; x < size; x++)
//            {
//                for (int y = 0; y < size; y++)
//                {
//                    Biomes[x, y] = min + (Biomes[x, y] - MIN) * (max - min) / (MAX - MIN);
//                }
//            }
//        }
//    }
//}