using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

enum Direction {Across, Down}

internal class TerrainTools : ScriptableWizard
{
    private static int across;
    private static int down;
    private static int tWidth;
    private static int tHeight;
    private static Terrain[] terrains;
    private static int stitchWidth = 32;
    private static string message;
    private static int terrainRes;
    private static Texture2D lineTex;
    private static int maxStitchWidth = 100;
    private static bool playError = false;
    private static int gridPixelHeight = 28;
    private static int gridPixelWidth = 121;
    private static int y = 84;

    [MenuItem("Terrain/StitchC#...")]
    private static void CreateWizard()
    {
        if (lineTex == null)
        {
            across = down = tWidth = tHeight = 2;
            stitchWidth = 10;
            SetNumberOfTerrains();
            lineTex = EditorGUIUtility.whiteTexture;
        }
        message = "";
        playError = false;
        ScriptableWizard.DisplayWizard("Stitch Terrains", typeof (TerrainTools));
    }

    private new void OnGUI()
    {
        GUILayout.BeginHorizontal(GUILayout.Width(220));
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal(GUILayout.Width(190));
        GUILayout.Label("Number of terrains across:");
        across = Mathf.Max(EditorGUILayout.IntField(across, GUILayout.Width(30)), 1);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(GUILayout.Width(190));
        GUILayout.Label("Number of terrains down:");
        down = Mathf.Max(EditorGUILayout.IntField(down, GUILayout.Width(30)), 1);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Space(12);
        if (GUILayout.Button("Apply"))
        {
            tWidth = across;
            tHeight = down;
            SetNumberOfTerrains();
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.Space(7);

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        if (GUILayout.Button("Autofill from scene", GUILayout.Width(gridPixelWidth*tWidth + 2)))
        {
            AutoFill();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(9);

        int counter = 0;
        for (int h = 0; h < tHeight; h++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            for (int w = 0; w < tWidth; w++)
            {
                terrains[counter] =
                    (Terrain)
                        EditorGUILayout.ObjectField(terrains[counter++], typeof (Terrain), true, GUILayout.Width(112));
                GUILayout.Space(5);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(9);
        }
        DrawGrid(Color.black, 1, y);
        DrawGrid(Color.white, 0, y);

        GUI.Label(new Rect(2, y - 4, 20, 20), "Z");

        GUI.Label(new Rect(gridPixelWidth*tWidth + 10, y + 2 + gridPixelHeight*tHeight, 20, 20), "X");
        GUI.color = Color.black;
        GUI.DrawTexture(new Rect(7, y + 12, 1, gridPixelHeight*tHeight - 2), lineTex);
        GUI.DrawTexture(new Rect(7, y + 10 + gridPixelHeight*tHeight, gridPixelWidth*tWidth, 1), lineTex);
        GUI.color = Color.white;

        GUILayout.Space(15);

        GUILayout.BeginHorizontal();
        if (terrains[0] != null)
        {
            maxStitchWidth = terrains[0].terrainData.heightmapWidth/2;
        }
        GUILayout.Label("Stitch width: " + stitchWidth, GUILayout.Width(90));
        stitchWidth = (int) GUILayout.HorizontalSlider((float) stitchWidth, 2, maxStitchWidth);
        GUILayout.EndHorizontal();

        GUILayout.Space(8);

        GUILayout.Label(message);

        GUILayout.Space(1);

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (GUILayout.Button("Clear"))
        {
            SetNumberOfTerrains();
        }
        if (GUILayout.Button("Stitch"))
        {
            StitchTerrains();
        }
        GUILayout.Space(10);
        GUILayout.EndHorizontal();
    }

    public static void AutoFill()
    {
        Terrain[] sceneTerrains = FindObjectsOfType<Terrain>();
        if (sceneTerrains.Length == 0)
        {
            message = "No terrains found";
            return;
        }

        List<float> xPositions = new List<float>();
        List<float> zPositions = new List<float>();
        Vector3 tPosition = sceneTerrains[0].transform.position;
        xPositions.Add(tPosition.x);
        zPositions.Add(tPosition.z);
        for (int i = 0; i < sceneTerrains.Length; i++)
        {
            tPosition = sceneTerrains[i].transform.position;
            if (!ListContains(xPositions, tPosition.x))
            {
                xPositions.Add(tPosition.x);
            }
            if (!ListContains(zPositions, tPosition.z))
            {
                zPositions.Add(tPosition.z);
            }
        }
        if (xPositions.Count*zPositions.Count != sceneTerrains.Length)
        {
            message = "Unable to autofill. Terrains should line up closely in the form of a grid.";
            return;
        }

        xPositions.Sort();
        zPositions.Sort();
        zPositions.Reverse();
        across = tWidth = xPositions.Count;
        down = tHeight = zPositions.Count;
        terrains = new Terrain[tWidth*tHeight];
        int count = 0;
        for (int z = 0; z < zPositions.Count; z++)
        {
            for (int x = 0; x < xPositions.Count; x++)
            {
                for (int i = 0; i < sceneTerrains.Length; i++)
                {
                    tPosition = sceneTerrains[i].transform.position;
                    if (Approx(tPosition.x, xPositions[x]) && Approx(tPosition.z, zPositions[z]))
                    {
                        terrains[count++] = sceneTerrains[i];
                        break;
                    }
                }
            }
        }
    }

    public static bool ListContains(List<float> list, float pos)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (Approx(pos, list[i]))
            {
                return true;
            }
        }
        return false;
    }

    public static bool Approx(float pos1, float pos2)
    {
        if (pos1 >= pos2 - 1.0f && pos1 <= pos2 + 1.0f)
        {
            return true;
        }
        return false;
    }

    public void DrawGrid(Color color, int offset, int top)
    {
        GUI.color = color;
        for (int i = 0; i < tHeight + 1; i++)
        {
            GUI.DrawTexture(new Rect(15 + offset, top + offset + gridPixelHeight*i, gridPixelWidth*tWidth, 1), lineTex);
        }
        for (int i = 0; i < tWidth + 1; i++)
        {
            GUI.DrawTexture(new Rect(15 + offset + gridPixelWidth*i, top + offset, 1, gridPixelHeight*tHeight + 1),
                lineTex);
        }
    }

    public static void SetNumberOfTerrains()
    {
        terrains = new Terrain[tWidth*tHeight];
        message = "";
    }

    public static void StitchTerrains()
    {
        foreach (Terrain t in terrains)
        {
            if (t == null)
            {
                message = "All terrain slots must have a terrain assigned";
                return;
            }
        }

        terrainRes = terrains[0].terrainData.heightmapWidth;
        if (terrains[0].terrainData.heightmapHeight != terrainRes)
        {
            message = "Heightmap width and height must be the same";
            return;
        }

        foreach (Terrain t in terrains)
        {
            if (t.terrainData.heightmapWidth != terrainRes || t.terrainData.heightmapHeight != terrainRes)
            {
                message = "All heightmaps must be the same resolution";
                return;
            }
        }

        foreach (Terrain t in terrains)
        {
            Undo.RegisterUndo(t.terrainData, "Stitch");
        }

        stitchWidth = Mathf.Clamp(stitchWidth, 1, (terrainRes - 1)/2);
        int counter = 0;
        int total = tHeight*(tWidth - 1) + (tHeight - 1)*tWidth;

        if (tWidth == 1 && tHeight == 1)
        {
            BlendData(terrains[0].terrainData, terrains[0].terrainData, Direction.Across, true);
            BlendData(terrains[0].terrainData, terrains[0].terrainData, Direction.Down, true);
            message = "Terrain has been made repeatable with itself";
        }
        else
        {
            for (int h = 0; h < tHeight; h++)
            {
                for (int w = 0; w < tWidth - 1; w++)
                {
                    EditorUtility.DisplayProgressBar("Stitching...", "", Mathf.InverseLerp(0, total, ++counter));
                    BlendData(terrains[h*tWidth + w].terrainData, terrains[h*tWidth + w + 1].terrainData,
                        Direction.Across, false);
                }
            }
            for (int h = 0; h < tHeight - 1; h++)
            {
                for (int w = 0; w < tWidth; w++)
                {
                    EditorUtility.DisplayProgressBar("Stitching...", "", Mathf.InverseLerp(0, total, ++counter));
                    BlendData(terrains[h*tWidth + w].terrainData, terrains[(h + 1)*tWidth + w].terrainData,
                        Direction.Down, false);
                }
            }
            message = "Terrains stitched successfully";
        }

        EditorUtility.ClearProgressBar();
    }

    public static void BlendData(TerrainData terrain1, TerrainData terrain2, Direction thisDirection, bool singleTerrain)
    {
        float[,] heightmapData = terrain1.GetHeights(0, 0, terrainRes, terrainRes);
        float[,] heightmapData2 = terrain2.GetHeights(0, 0, terrainRes, terrainRes);
        int pos = terrainRes - 1;

        if (thisDirection == Direction.Across)
        {
            for (int i = 0; i < terrainRes; i++)
            {
                for (int j = 1; j < stitchWidth; j++)
                {
                    float mix = Mathf.Lerp(heightmapData[i, pos - j], heightmapData2[i, j], 0.5f);
                    if (j == 1)
                    {
                        heightmapData[i, pos] = mix;
                        heightmapData2[i, 0] = mix;
                    }
                    float t = Mathf.SmoothStep(0.0f, 1.0f, Mathf.InverseLerp(1, stitchWidth - 1, j));
                    heightmapData[i, pos - j] = Mathf.Lerp(mix, heightmapData[i, pos - j], t);
                    if (!singleTerrain)
                    {
                        heightmapData2[i, j] = Mathf.Lerp(mix, heightmapData2[i, j], t);
                    }
                    else
                    {
                        heightmapData[i, j] = Mathf.Lerp(mix, heightmapData2[i, j], t);
                    }
                }
            }
            if (singleTerrain)
            {
                for (int i = 0; i < terrainRes; i++)
                {
                    heightmapData[i, 0] = heightmapData[i, pos];
                }
            }
        }
        else
        {
            for (int i = 0; i < terrainRes; i++)
            {
                for (int j = 1; j < stitchWidth; j++)
                {
                    float mix = Mathf.Lerp(heightmapData2[pos - j, i], heightmapData[j, i], 0.5f);
                    if (j == 1)
                    {
                        heightmapData2[pos, i] = mix;
                        heightmapData[0, i] = mix;
                    }
                    float t = Mathf.SmoothStep(0.0f, 1.0f, Mathf.InverseLerp(1, stitchWidth - 1, j));
                    if (!singleTerrain)
                    {
                        heightmapData2[pos - j, i] = Mathf.Lerp(mix, heightmapData2[pos - j, i], t);
                    }
                    else
                    {
                        heightmapData[pos - j, i] = Mathf.Lerp(mix, heightmapData2[pos - j, i], t);
                    }
                    heightmapData[j, i] = Mathf.Lerp(mix, heightmapData[j, i], t);
                }
            }
            if (singleTerrain)
            {
                for (int i = 0; i < terrainRes; i++)
                {
                    heightmapData[pos, i] = heightmapData[0, i];
                }
            }
        }

        terrain1.SetHeights(0, 0, heightmapData);
        if (!singleTerrain)
        {
            terrain2.SetHeights(0, 0, heightmapData2);
        }
    }
}