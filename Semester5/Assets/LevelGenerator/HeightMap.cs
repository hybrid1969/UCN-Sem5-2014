//using System;
//using UnityEngine;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using LibNoise.Unity.Generator;
//using LibNoise.Unity.Operator;

//namespace TerrainGenerator
//{
//    public class HeightMap
//    {
//        public float[,] Heights { get; set; }
//        public int StartY { get; private set; }
//        public int StartX { get; private set; }
//        public int Size { get; private set; }

//        public float this[int x, int y]
//        {
//            get { return Heights[x, y]; }
//            set { Heights[x, y] = value; }
//        }

//        Perlin perlin;

//        public HeightMap(int size, int startX = 0, int startY = 0)
//        {
//            perlin = new Perlin(new System.Random().Next());

//            Size = size;
//            StartX = startX;
//            StartY = startY;
//            Heights = new float[Size, Size];
//        }

//        public HeightMap(int size, int seed, int startX = 0, int startY = 0)
//        {
//            perlin = new Perlin(seed);

//            Size = size;
//            StartX = startX;
//            StartY = startY;
//            Heights = new float[Size, Size];
//        }

//        public void AlignEdges(HeightMap leftNeighbor, HeightMap rightNeighbor,
//            HeightMap topNeighbor, HeightMap bottomNeighbor, int shift = 0)
//        {
//            int x, y, counter;
//            float[,] nHeights;
//            float value;
//            int size = this.Size;

//            if (leftNeighbor != null)
//            {
//                nHeights = leftNeighbor.Heights;
//                counter = 0;

//                for (x = size - shift; x < size; x++)
//                {
//                    for (y = 0; y < size; y++)
//                    {
//                        value = (Heights[counter, y] + nHeights[x, y]) / 2f;
//                        Heights[counter, y] = nHeights[x, y] = value;
//                    }

//                    counter++;
//                }

//                x = size - 1;

//                for (y = 0; y < size; y++)
//                {
//                    value = (this[0, y] + leftNeighbor[x, y]) / 2f;
//                    this[0, y] = leftNeighbor[x, y] = value;
//                }
//            }
//            if (rightNeighbor != null)
//            {
//                nHeights = rightNeighbor.Heights;
//                counter = 0;

//                for (x = size - shift; x < size; x++)
//                {
//                    for (y = 0; y < size; y++)
//                    {
//                        value = (Heights[x, y] + nHeights[counter, y]) / 2f;
//                        Heights[x, y] = nHeights[counter, y] = value;
//                    }

//                    counter++;
//                }

//                x = size - 1;

//                for (y = 0; y < size; y++)
//                {
//                    value = (this[x, y] + rightNeighbor[0, y]) / 2f;
//                    this[x, y] = rightNeighbor[0, y] = value;
//                }
//            }
//            if (topNeighbor != null)
//            {
//                nHeights = topNeighbor.Heights;
//                counter = 0;

//                for (y = size - shift; y < size; y++)
//                {
//                    for (x = 0; x < size; x++)
//                    {
//                        value = (Heights[x, y] + nHeights[x, counter]) / 2f;
//                        Heights[x, y] = nHeights[x, counter] = value;
//                    }

//                    counter++;
//                }

//                y = size - 1;

//                for (x = 0; x < size; x++)
//                {
//                    value = (this[x, 0] + topNeighbor[x, y]) / 2f;
//                    this[x, 0] = topNeighbor[x, y] = value;
//                }
//            }
//            if (bottomNeighbor != null)
//            {
//                nHeights = bottomNeighbor.Heights;
//                counter = 0;

//                for (y = size - shift; y < size; y++)
//                {
//                    for (x = 0; x < size; x++)
//                    {
//                        value = (Heights[x, counter] + nHeights[x, y]) / 2f;
//                        Heights[x, counter] = nHeights[x, y] = value;
//                    }

//                    counter++;
//                }

//                y = size - 1;

//                for (x = 0; x < size; x++)
//                {
//                    value = (this[x, 0] + bottomNeighbor[x, y]) / 2f;
//                    this[x, 0] = bottomNeighbor[x, y] = value;
//                }
//            }
//        }

//        public void SetNoise(float frequency, byte octaves = 1, float persistence = 0.5f, float lacunarity = 2.0f, bool additive = false)
//        {
//            int size = Heights.GetLength(0);
//            int startX = StartX;
//            int startY = StartY;
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
//                        (x + startX) / fSize,
//                        (y + startY) / fSize);

//                    coord *= frequency;

//                    for (currentOctave = 0; currentOctave < octaves; currentOctave++)
//                    {
//                        signal = perlin.Noise2(coord.x, coord.y);
//                        value += signal * currentPersistence;
//                        coord *= lacunarity;
//                        currentPersistence *= persistence;
//                    }

//                    Heights[x, y] = (!additive) ? value : Heights[x, y] + value;
//                }
//            }
//        }

//        public void Perturb(float frequency, float depth)
//        {
//            int u, v, i, j;
//            int size = Heights.GetLength(0);
//            int startX = StartX;
//            int startY = StartY;
//            float[,] temp = new float[size, size];
//            float fSize = (float)size;
//            Vector2 coord;

//            for (i = 0; i < size; ++i)
//            {
//                for (j = 0; j < size; ++j)
//                {
//                    coord = new Vector2(
//                        (i + startX) / fSize,
//                        (j + startY) / fSize);

//                    coord *= frequency;

//                    u = i + (int)(perlin.Noise3(coord.x, coord.y, 0.0f) * depth);
//                    v = j + (int)(perlin.Noise3(coord.x, coord.y, 1.0f) * depth);

//                    if (u < 0) u = 0;
//                    if (u >= size) u = size - 1;
//                    if (v < 0) v = 0;
//                    if (v >= size) v = size - 1;

//                    temp[i, j] = Heights[u, v];
//                }
//            }

//            Heights = temp;
//        }

//        public void Erode(float smoothness)
//        {
//            int size = Heights.GetLength(0);

//            for(int i = 1; i < size - 1; i++)
//            {
//                int u, v;
//                float d_max, d_i, d_h;
//                int[] match;

//                for (int j = 1; j < size - 1; j++)
//                {
//                    d_max = 0.0f;
//                    match = new[] { 0, 0 };

//                    for (u = -1; u <= 1; u++)
//                    {
//                        for (v = -1; v <= 1; v++)
//                        {
//                            if (Math.Abs(u) + Math.Abs(v) > 0)
//                            {
//                                d_i = Heights[i, j] - Heights[i + u, j + v];

//                                if (d_i > d_max)
//                                {
//                                    d_max = d_i;
//                                    match[0] = u;
//                                    match[1] = v;
//                                }
//                            }
//                        }
//                    }

//                    if (0 < d_max && d_max <= (smoothness / (float)size))
//                    {
//                        d_h = 0.5f * d_max;

//                        Heights[i, j] -= d_h;
//                        Heights[i + match[0], j + match[1]] += d_h;
//                    }
//                }
//            }
//        }

//        public void Smoothen()
//        {
//            int i, j, u, v;
//            int size = Heights.GetLength(0);
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
//                            total += Heights[i + u, j + v];
//                        }
//                    }

//                    Heights[i, j] = total / 9.0f;
//                }
//            }
//        }

//        public void Normalize()
//        {
//            int size = Heights.GetLength(0);
//            float min = -1f, max = 1f;

//            for (int x = 0; x < size; x++)
//            {
//                for (int y = 0; y < size; y++)
//                {
//                    Heights[x, y] = (Heights[x, y] - min) / (max - min);
//                }
//            }
//        }

//        public void MakeFlat(float height = 0.0f)
//        {
//            int x, y;
//            int size = Heights.GetLength(0);

//            for (x = 0; x < size; x++)
//            {
//                for (y = 0; y < size; y++)
//                {
//                    Heights[x, y] = height;
//                }
//            }
//        }

//        public void Multiply(float amount)
//        {
//            int x, y;
//            int size = Heights.GetLength(0);

//            for (x = 0; x < size; x++)
//            {
//                for (y = 0; y < size; y++)
//                {
//                    Heights[x, y] *= amount;
//                }
//            }
//        }
//    }
//}