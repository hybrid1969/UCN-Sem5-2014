using System;
using UnityEngine;
using System.Collections;

    public static class TerrainTools
    {
        static float[] smoothModTable = null;
        static float[] smoothDYTable = null;
        static bool memoizationTablesFilled = false;

        static Vector3 prevSize = new Vector3(0, 0, 0);
        static int prevHeight = 0;
        static int prevWidth = 0;
        static int prevNumSamples = 0;

        delegate float GetYMod(int domTerrain, int terrainToMod, double dY, int curCellX, int maxCellsX, int curCellY, int maxCellsY);
        public static IEnumerator StitchTerrains(Terrain terrain1, Terrain terrain2, int numSamples, int domTerrain, int stitchTimes)
        {

            var dX = terrain2.transform.position.x - terrain1.transform.position.x;
            var dZ = terrain2.transform.position.z - terrain1.transform.position.z;

            var height = terrain1.terrainData.heightmapHeight;
            var width = terrain1.terrainData.heightmapWidth;

            float[,] heights1 = terrain1.terrainData.GetHeights(0, 0, width, height);
            float[,] heights2 = terrain2.terrainData.GetHeights(0, 0, width, height);

            if (height != prevHeight || width != prevWidth || terrain1.terrainData.size != prevSize || numSamples != prevNumSamples)
            {
                prevHeight = height;
                prevWidth = width;
                prevSize = terrain1.terrainData.size;
                prevNumSamples = numSamples;

                memoizationTablesFilled = false;
                smoothModTable = new float[height];
                smoothDYTable = new float[numSamples];
            }
            GetYMod getYMod;
            if (memoizationTablesFilled)
                getYMod = getYModMemoized;
            else
                getYMod = getYModDynamic;

            if (Mathf.Abs(dX) > Mathf.Abs(dZ))
            {
                int xDir = terrain2.transform.position.x > terrain1.transform.position.x ? 1 : -1;

                // Align terrains
                terrain2.transform.position = terrain1.transform.position;
                terrain2.transform.position = new Vector3(terrain1.transform.position.x + terrain1.terrainData.size.x * xDir, terrain2.transform.position.y, terrain2.transform.position.z);

                /* Stitch */
                // Loop over each sample point on the Z axis
                for (int z = 0; z < height; z++)
                {
                    // Get height difference between sample point on the two terrains ([0, 1.0f])
                    float dY = 0.0f;
                    if (xDir == 1)
                        dY = heights2[z, 0] - heights1[z, width - 1];
                    else
                        dY = heights2[z, width - 1] - heights1[z, 0];

                    // Move a configured number of samples towards the point in the middle of the height difference
                    // the amount to move them is reduced the farther away they are from the edge
                    for (int i = 0; i < numSamples; i++)
                    {
                        if (xDir == 1)
                        {
                            heights1[z, width - 1 - i] = heights1[z, width - 1 - i] + getYMod(domTerrain, 1, dY, z, height, i, numSamples);
                            heights2[z, i] = heights2[z, i] - getYMod(domTerrain, 2, dY, z, height, i, numSamples);
                        }
                        else
                        {
                            heights1[z, i] = heights1[z, i] + getYMod(domTerrain, 1, dY, z, height, i, numSamples);
                            heights2[z, width - 1 - i] = heights2[z, width - 1 - i] - getYMod(domTerrain, 2, dY, z, height, i, numSamples);
                        }
                    }
                    if (z % 64 == 0)
                    {
                        //Debug.Log(z);
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
            // Stitch along X axis
            else
            {
                int zDir = terrain2.transform.position.z > terrain1.transform.position.z ? 1 : -1;

                // Align terrains
                terrain2.transform.position = terrain1.transform.position;
                terrain2.transform.position = new Vector3(terrain2.transform.position.x, terrain2.transform.position.y, terrain1.transform.position.z + terrain1.terrainData.size.z * zDir);

                /* Stitch */
                // Loop over each sample point on the X axis
                for (int x = 0; x < height; x++)
                {
                    // Get height difference between sample point on the two terrains ([0, 1.0f])
                    float dY = 0.0f;
                    if (zDir == 1)
                        dY = heights2[0, x] - heights1[width - 1, x];
                    else
                        dY = heights2[width - 1, x] - heights1[0, x];

                    // Move a configured number of samples towards the point in the middle of the height difference
                    // the amount to move them is reduced the farther away they are from the edge
                    for (int i = 0; i < numSamples; i++)
                    {
                        if (zDir == 1)
                        {
                            heights1[width - 1 - i, x] = heights1[width - 1 - i, x] + getYMod(domTerrain, 1, dY, x, height, i, numSamples);
                            heights2[i, x] = heights2[i, x] - getYMod(domTerrain, 2, dY, x, height, i, numSamples);
                        }
                        else
                        {
                            heights1[i, x] = heights1[i, x] + getYMod(domTerrain, 1, dY, x, height, i, numSamples);
                            heights2[width - 1 - i, x] = heights2[width - 1 - i, x] - getYMod(domTerrain, 2, dY, x, height, i, numSamples);
                        }
                        if (x % 50 == 0 && i != 0)
                        {
                            //Debug.Log("TESTER");
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }


                //yield return new WaitForEndOfFrame();
            }

            terrain1.terrainData.SetHeights(0, 0, heights1);
            terrain2.terrainData.SetHeights(0, 0, heights2);
            terrain1.Flush();
            terrain2.Flush();
        }

        static float getYModDynamic(int domTerrain, int terrainToMod, double dY, int curCellX, int maxCellsX, int curCellY, int maxCellsY)
        {
            float yMod = (float)dY / 2.0f;

            if (terrainToMod == 1)
            {
                if (domTerrain == 1)
                    yMod = (float)dY * smoothMod(curCellX, maxCellsX);
                else if (domTerrain == 2)
                    yMod = (float)dY * (1.0f - smoothMod(curCellX, maxCellsX));
            }
            else if (terrainToMod == 2)
            {
                if (domTerrain == 1)
                    yMod = (float)dY * (1.0f - smoothMod(curCellX, maxCellsX));
                else if (domTerrain == 2)
                    yMod = (float)dY * smoothMod(curCellX, maxCellsX);
            }
            else
            {
                Debug.LogError("terrainToMod must be either 1 or 2! (found: " + terrainToMod + ")");
            }

            return (float)(yMod * smoothDY(curCellY, maxCellsY));
        }

        // Get the height (Y) difference that needs to be applied at this point to match the terrains' height
        // X: along the edge
        // Y: into the terrain
        static float getYModMemoized(int domTerrain, int terrainToMod, double dY, int curCellX, int maxCellsX, int curCellY, int maxCellsY)
        {
            float yMod = (float)dY / 2.0f;

            if (terrainToMod == 1)
            {
                if (domTerrain == 1)
                    yMod = (float)dY * smoothModTable[curCellX];
                else if (domTerrain == 2)
                    yMod = (float)dY * (1.0f - smoothModTable[curCellX]);
            }
            else if (terrainToMod == 2)
            {
                if (domTerrain == 1)
                    yMod = (float)dY * (1.0f - smoothModTable[curCellX]);
                else if (domTerrain == 2)
                    yMod = (float)dY * smoothModTable[curCellX];
            }
            else
            {
                Debug.LogError("terrainToMod must be either 1 or 2! (found: " + terrainToMod + ")");
            }

            return (float)(yMod * smoothDYTable[curCellY]);
        }

        static float smoothDY(int current, float max)
        {
            float x = 1.0f - (float)((current) / (max - 1));

            /*return 0.05f*((x*(x-0.3f)*(x-0.5f)*(x-0.7f)*(x-0.85f)*(x-1.0f)) / -0.0025770937499999995f) +
                0.2f*((x*(x-0.15f)*(x-0.5f)*(x-0.7f)*(x-0.85f)*(x-1.0f)) / 0.0013859999999999999f) +
                0.5f*((x*(x-0.15f)*(x-0.3f)*(x-0.7f)*(x-0.85f)*(x-1.0f)) / -0.0012249999999999995f) +
                0.8f*((x*(x-0.15f)*(x-0.3f)*(x-0.5f)*(x-0.85f)*(x-1.0f)) / 0.0013859999999999999f) +
                0.95f*((x*(x-0.15f)*(x-0.3f)*(x-0.5f)*(x-0.7f)*(x-1.0f)) / -0.0025770937500000004f) +
                (x*(x-0.15f)*(x-0.3f)*(x-0.5f)*(x-0.7f)*(x-0.85f)) / 0.013387500000000004f;*/

            /*Polynomial (Lagrange) interpolated from (x, y):
            0 0
            0.5f 0.5f
            1 1
            .30  .20
            .70 .80
            .15  .05
            .85 .95
            */
            float result = (float)(236706659320000.0f * Math.Pow(x, 6) + 99115929736349168000.0f * Math.Pow(x, 5) - 247790818523389713100.0f * Math.Pow(x, 4)
                + 80036273392876468420.0f * Math.Pow(x, 3) + 127736693875550215551.0f * Math.Pow(x, 2) - 713647159857677493.0f * x) / 58384668816317760000.0f;
            smoothDYTable[current] = result;
            return result;
        }

        // When one terrain needs to stay the same height in the corners we want to gradually transition
        // from not modifying a tile (at the corners) to modifying tiles for 50% (the other 50% is done
        // by the other terrain to meet in the middle)
        static float smoothMod(int current, float max)
        {
            float x = (float)((current) / (max - 1));
            //return 0.5f * Mathf.Cos(-Mathf.PI * x + 0.5f * Mathf.PI);

            /*Polynomial (Lagrange) interpolated from (x, y):
            0.0f,0.0f
            0.01f,1.0fE-4
            0.05f,0.0050f
            0.1f,0.04f
            0.5f,0.5f
            0.9f,0.04f
            0.95f,0.0050f
            0.99f,1.0fE-4
            1.0f,0.0f
            */
            float result = 1.0E-4f * ((1 * (x - 0.0f) * (x - 0.05f) * (x - 0.1f) * (x - 0.5f) * (x - 0.9f) * (x - 0.95f) * (x - 0.99f) * (x - 1.0f)) / -1.4317846804800003E-5f) +
                0.0050f * ((1 * (x - 0.0f) * (x - 0.01f) * (x - 0.1f) * (x - 0.5f) * (x - 0.9f) * (x - 0.95f) * (x - 0.99f) * (x - 1.0f)) / 3.074152499999999E-5f) +
                0.04f * ((1 * (x - 0.0f) * (x - 0.01f) * (x - 0.05f) * (x - 0.5f) * (x - 0.9f) * (x - 0.95f) * (x - 0.99f) * (x - 1.0f)) / -9.804240000000002E-5f) +
                0.5f * ((1 * (x - 0.0f) * (x - 0.01f) * (x - 0.05f) * (x - 0.1f) * (x - 0.9f) * (x - 0.95f) * (x - 0.99f) * (x - 1.0f)) / 0.0019448099999999997f) +
                0.04f * ((1 * (x - 0.0f) * (x - 0.01f) * (x - 0.05f) * (x - 0.1f) * (x - 0.5f) * (x - 0.95f) * (x - 0.99f) * (x - 1.0f)) / -9.804239999999985E-5f) +
                0.0050f * ((1 * (x - 0.0f) * (x - 0.01f) * (x - 0.05f) * (x - 0.1f) * (x - 0.5f) * (x - 0.9f) * (x - 0.99f) * (x - 1.0f)) / 3.0741525000000005E-5f) +
                1.0E-4f * ((1 * (x - 0.0f) * (x - 0.01f) * (x - 0.05f) * (x - 0.1f) * (x - 0.5f) * (x - 0.9f) * (x - 0.95f) * (x - 1.0f)) / -1.4317846804800018E-5f);
            smoothModTable[current] = result;
            return result;
        }

        //public static float[, ,] TerrainTexture(int sizeX, int sizeY, int texturecount, BiomeMap biomap)
        //{
        //    float[, ,] textures = new float[sizeX, sizeY, texturecount];
        //    for (int x = 0; x < sizeX; x++)
        //    {
        //        for (int y = 0; y < sizeY; y++)
        //        {
        //            textures[x, y, Mathf.RoundToInt(biomap.Biomes[x,y])] = 1;
        //        }
        //    }
        //    return textures;
        //}

        public static SimpleTerrainGenerator GetGenerator(this Terrain ter)
        {
            if (ter.GetComponent<SimpleTerrainGenerator>() != null)
                return ter.GetComponent<SimpleTerrainGenerator>();
            else
                return ter.gameObject.AddComponent<SimpleTerrainGenerator>();

        }
    }
