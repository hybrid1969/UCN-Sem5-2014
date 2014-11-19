﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO: When I wrote this only me and god knew what is said, in 3 weeks from now only god will know - AKA: Comment!
public class ChunkLoader : MonoBehaviour
{
    public delegate void TreeCorrector();
    public static event TreeCorrector OnTerrainChanged;
    public static ChunkLoader chunkloader;
    int LoadDistance = 5;
    int Center { get { return (LoadDistance - 1) / 2; } }
    int ChunkSize;
    public List<List<GameObject>> Chunks = new List<List<GameObject>>();

    Terrain lt;
    Terrain rt;
    Terrain tt;
    Terrain bt;

    [SerializeField]
    GameObject Player;

    public ChunkLoader()
    {
        chunkloader = this;
    }

	// Use this for initialization
	void Start ()
    {
        ChunkSize = DataBaseHandler.ChunkSize;
        for (int i = 0; i < LoadDistance; i++)
        {
            List<GameObject> lgo = new List<GameObject>();
            for (int j = 0; j < LoadDistance; j++)
            {
                GameObject go = new GameObject();
                go.AddComponent<Chunk>();
                go.transform.position = new Vector3(((((LoadDistance - 1) / 2) - LoadDistance) + j + 1) * ChunkSize, 0, (((LoadDistance - 1) / 2 - LoadDistance) + i + 1) * ChunkSize);
                go.name = "Chunk" + "[" + (j - Center) + ";" + (i - Center) + "]";
                lgo.Add(go);
                go.GetComponent<Chunk>().Generate();
            }
            Chunks.Add(lgo);
        }
        for (int i = 0; i < LoadDistance; i++)
        {
            for (int j = 0; j < LoadDistance; j++)
            {
                for (int k = 0; k < 1; k++)
                {
                    if (i != 0)
                    {
                        bt = Chunks[i - 1][j].GetComponent<Terrain>();
                        //TerrainStitcher.stitch(bt, Chunks[i][j].GetComponent<Terrain>(), 16, 0);
                        //TerrainTools.BlendData(Chunks[i][j].GetComponent<Terrain>(), bt, TerrainTools.Direction.Z, 16);
                    }
                    if (i != LoadDistance - 1)
                    {
                        tt = Chunks[i + 1][j].GetComponent<Terrain>();
                        //TerrainStitcher.stitch(tt, Chunks[i][j].GetComponent<Terrain>(), 16, 0);
                        //TerrainTools.BlendData(tt, Chunks[i][j].GetComponent<Terrain>(), TerrainTools.Direction.Z, 16);
                    }
                    if (j != 0)
                    {
                        lt = Chunks[i][j - 1].GetComponent<Terrain>();
                        //TerrainStitcher.stitch(lt, Chunks[i][j].GetComponent<Terrain>(), 16, 0);
                        //TerrainTools.BlendData(lt, Chunks[i][j].GetComponent<Terrain>(), TerrainTools.Direction.X, 16);
                    }
                    if (j != LoadDistance - 1)
                    {
                        rt = Chunks[i][j + 1].GetComponent<Terrain>();
                        //TerrainStitcher.stitch(rt, Chunks[i][j].GetComponent<Terrain>(), 16, 0);
                        //TerrainTools.BlendData(Chunks[i][j].GetComponent<Terrain>(), rt, TerrainTools.Direction.X, 16);
                    }
                }
                Chunks[i][j].GetComponent<Terrain>().SetNeighbors(lt, tt, rt, bt);
                Chunks[i][j].GetComponent<Terrain>().Flush();
                bt = null;
                tt = null;
                lt = null;
                rt = null;
            }
        }

        TerrainTools.AutoFill();
        TerrainTools.StitchTerrains();

	    for (int i = 0; i < LoadDistance; i++)
	    {
	        for (int j = 0; j < LoadDistance; j++)
	        {
                Chunks[i][j].GetComponent<Chunk>().GenerateFoliage();
	        }
	    }
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.transform.position.z < Chunks[Center - 1][Center].transform.position.z + (ChunkSize / 2))
        {
            AddColumnBack();
            DeleteColumnFront();
            TerrainTools.AutoFill();
            TerrainTools.StitchTerrains();
            OnTerrainChanged();
        }
        if (Player.transform.position.z > Chunks[Center + 1][Center].transform.position.z + (ChunkSize / 2))
        {
            AddColumnFront();
            DeleteColumnBack();
            TerrainTools.AutoFill();
            TerrainTools.StitchTerrains();
            OnTerrainChanged();
        }
        if (Player.transform.position.x < Chunks[Center][Center - 1].transform.position.x + (ChunkSize / 2))
        {
            AddRowBack();
            DeleteRowFront();
            TerrainTools.AutoFill();
            TerrainTools.StitchTerrains();
            OnTerrainChanged();
        }
        if (Player.transform.position.x > Chunks[Center][Center + 1].transform.position.x + (ChunkSize / 2))
        {
            AddRowFront();
            DeleteRowBack();
            TerrainTools.AutoFill();
            TerrainTools.StitchTerrains();
            OnTerrainChanged();
        }
	}

    void AddColumnFront()
    {
        Vector3 vec = Chunks[Chunks.Count - 1][Center].transform.position;
        List<GameObject> lgo = new List<GameObject>();
        int c = -(Center * ChunkSize);
        for (int i = 0; i < Chunks.Count; i++)
        {
            GameObject go = new GameObject();
            go.transform.position = new Vector3(vec.x + c, 0, vec.z + ChunkSize);
            go.AddComponent<Chunk>();
            go.name = "Chunk" + "[" + go.transform.position.x / ChunkSize + ";" + go.transform.position.z / ChunkSize + "]";
            lgo.Add(go);
            go.GetComponent<Chunk>().Generate();
            c += ChunkSize;
        }

        Chunks.Add(lgo);

        for (int i = 0; i < Chunks.Count - 1; i++)
        {

            if (i == 0)
            {
                //TerrainStitcher.stitch(Chunks[Chunks.Count - 1][i].GetComponent<Terrain>(), Chunks[Chunks.Count - 1][i + 1].GetComponent<Terrain>(), 16, 0);
                //TerrainStitcher.stitch(Chunks[Chunks.Count - 1][i].GetComponent<Terrain>(), Chunks[Chunks.Count - 2][i].GetComponent<Terrain>(), 16, 0);

                Chunks[Chunks.Count - 1][i].GetComponent<Terrain>().SetNeighbors(null, null, Chunks[Chunks.Count - 1][i + 1].GetComponent<Terrain>(), Chunks[Chunks.Count - 2][i].GetComponent<Terrain>());
                Chunks[Chunks.Count - 1][i].GetComponent<Terrain>().Flush();
                Chunks[Chunks.Count - 2][i].GetComponent<Terrain>().SetNeighbors(null, Chunks[Chunks.Count - 1][i].GetComponent<Terrain>(), Chunks[Chunks.Count - 2][i + 1].GetComponent<Terrain>(), Chunks[Chunks.Count - 3][i].GetComponent<Terrain>());
                Chunks[Chunks.Count - 2][i].GetComponent<Terrain>().Flush();
            }
            else if (i == Chunks.Count - 2)
            {
                //TerrainStitcher.stitch(Chunks[Chunks.Count - 1][i].GetComponent<Terrain>(), Chunks[Chunks.Count - 1][i - 1].GetComponent<Terrain>(), 16, 0);
                //TerrainStitcher.stitch(Chunks[Chunks.Count - 1][i].GetComponent<Terrain>(), Chunks[Chunks.Count - 2][i].GetComponent<Terrain>(), 16, 0);

                Chunks[Chunks.Count - 1][i].GetComponent<Terrain>().SetNeighbors(Chunks[Chunks.Count - 1][i - 1].GetComponent<Terrain>(), null, null, Chunks[Chunks.Count - 2][i].GetComponent<Terrain>());
                Chunks[Chunks.Count - 1][i].GetComponent<Terrain>().Flush();
                Chunks[Chunks.Count - 2][i].GetComponent<Terrain>().SetNeighbors(Chunks[Chunks.Count - 2][i - 1].GetComponent<Terrain>(), Chunks[Chunks.Count - 1][i].GetComponent<Terrain>(), null, Chunks[Chunks.Count - 3][i].GetComponent<Terrain>());
                Chunks[Chunks.Count - 2][i].GetComponent<Terrain>().Flush();
            }
            else
            {
                //TerrainStitcher.stitch(Chunks[Chunks.Count - 1][i].GetComponent<Terrain>(), Chunks[Chunks.Count - 1][i - 1].GetComponent<Terrain>(), 16, 0);
                //TerrainStitcher.stitch(Chunks[Chunks.Count - 1][i].GetComponent<Terrain>(), Chunks[Chunks.Count - 1][i + 1].GetComponent<Terrain>(), 16, 0);
                //TerrainStitcher.stitch(Chunks[Chunks.Count - 1][i].GetComponent<Terrain>(), Chunks[Chunks.Count - 2][i].GetComponent<Terrain>(), 16, 0);

                Chunks[Chunks.Count - 1][i].GetComponent<Terrain>().SetNeighbors(Chunks[Chunks.Count - 1][i - 1].GetComponent<Terrain>(), null, Chunks[Chunks.Count - 1][i + 1].GetComponent<Terrain>(), Chunks[Chunks.Count - 2][i].GetComponent<Terrain>());
                Chunks[Chunks.Count - 1][i].GetComponent<Terrain>().Flush();
                Chunks[Chunks.Count - 2][i].GetComponent<Terrain>().SetNeighbors(Chunks[Chunks.Count - 2][i - 1].GetComponent<Terrain>(), Chunks[Chunks.Count - 1][i].GetComponent<Terrain>(), Chunks[Chunks.Count - 2][i + 1].GetComponent<Terrain>(), Chunks[Chunks.Count - 3][i].GetComponent<Terrain>());
                Chunks[Chunks.Count - 2][i].GetComponent<Terrain>().Flush();
            }
        }

        for (int i = 0; i < Chunks[Chunks.Count - 1].Count - 1; i++)
        {
            Chunks[Chunks.Count - 1][i].GetComponent<Chunk>().GenerateFoliage();
        }
    }

    void AddColumnBack()
    {
        Vector3 vec = Chunks[0][Center].transform.position;
        List<GameObject> lgo = new List<GameObject>();
        int c = -(Center * ChunkSize);
        for (int i = 0; i < Chunks.Count; i++)
        {
            GameObject go = new GameObject();
            go.transform.position = new Vector3(vec.x + c, 0, vec.z - ChunkSize);
            go.AddComponent<Chunk>();
            go.name = "Chunk" + "[" + go.transform.position.x / ChunkSize + ";" + go.transform.position.z / ChunkSize + "]";
            lgo.Add(go);
            go.GetComponent<Chunk>().Generate();
            c += ChunkSize;
        }
        Chunks.Insert(0, lgo);

        for (int i = 0; i < Chunks.Count - 1; i++)
        {
            if (i == 0)
            {
                //TerrainStitcher.stitch(Chunks[0][i].GetComponent<Terrain>(), Chunks[0][i + 1].GetComponent<Terrain>(), 16, 0);
                //TerrainStitcher.stitch(Chunks[0][i].GetComponent<Terrain>(), Chunks[1][i].GetComponent<Terrain>(), 16, 0);

                Chunks[0][i].GetComponent<Terrain>().SetNeighbors(null, Chunks[1][i].GetComponent<Terrain>(), Chunks[0][i + 1].GetComponent<Terrain>(), null);
                Chunks[0][i].GetComponent<Terrain>().Flush();
                Chunks[1][i].GetComponent<Terrain>().SetNeighbors(null, Chunks[2][i].GetComponent<Terrain>(), Chunks[1][i + 1].GetComponent<Terrain>(), Chunks[0][i].GetComponent<Terrain>());
                Chunks[1][i].GetComponent<Terrain>().Flush();
            }
            else if (i == Chunks.Count - 2)
            {
                //TerrainStitcher.stitch(Chunks[0][i].GetComponent<Terrain>(), Chunks[0][i - 1].GetComponent<Terrain>(), 16, 0);
                //TerrainStitcher.stitch(Chunks[0][i].GetComponent<Terrain>(), Chunks[1][i].GetComponent<Terrain>(), 16, 0);

                Chunks[0][i].GetComponent<Terrain>().SetNeighbors(Chunks[0][i - 1].GetComponent<Terrain>(), Chunks[1][i].GetComponent<Terrain>(), null, null);
                Chunks[0][i].GetComponent<Terrain>().Flush();
                Chunks[1][i].GetComponent<Terrain>().SetNeighbors(Chunks[1][i - 1].GetComponent<Terrain>(), Chunks[2][i].GetComponent<Terrain>(), null, Chunks[0][i].GetComponent<Terrain>());
                Chunks[1][i].GetComponent<Terrain>().Flush();
            }
            else
            {
                //TerrainStitcher.stitch(Chunks[0][i].GetComponent<Terrain>(), Chunks[0][i - 1].GetComponent<Terrain>(), 16, 0);
                //TerrainStitcher.stitch(Chunks[0][i].GetComponent<Terrain>(), Chunks[0][i + 1].GetComponent<Terrain>(), 16, 0);
                //TerrainStitcher.stitch(Chunks[0][i].GetComponent<Terrain>(), Chunks[1][i].GetComponent<Terrain>(), 16, 0);

                Chunks[0][i].GetComponent<Terrain>().SetNeighbors(Chunks[0][i - 1].GetComponent<Terrain>(), Chunks[1][i].GetComponent<Terrain>(), Chunks[0][i + 1].GetComponent<Terrain>(), null);
                Chunks[0][i].GetComponent<Terrain>().Flush();
                Chunks[1][i].GetComponent<Terrain>().SetNeighbors(Chunks[1][i - 1].GetComponent<Terrain>(), Chunks[2][i].GetComponent<Terrain>(), Chunks[1][i + 1].GetComponent<Terrain>(), Chunks[0][i].GetComponent<Terrain>());
                Chunks[1][i].GetComponent<Terrain>().Flush();
            }
        }

        for (int i = 0; i < Chunks[0].Count - 1; i++)
        {
            Chunks[0][i].GetComponent<Chunk>().GenerateFoliage();
        }
    }

    void AddRowFront()
    {
        Vector3 vec = Chunks[Center][Chunks[1].Count - 1].transform.position;
        int c = -(Center * ChunkSize);
        foreach (List<GameObject> lgo in Chunks)
        {
            GameObject go = new GameObject();
            go.transform.position = new Vector3(vec.x + ChunkSize, 0, vec.z + c);
            go.AddComponent<Chunk>();
            go.name = "Chunk" + "[" + go.transform.position.x / ChunkSize + ";" + go.transform.position.z / ChunkSize + "]";
            go.GetComponent<Chunk>().Generate();
            lgo.Add(go);
            c += ChunkSize;
        }


        for (int i = 0; i < Chunks.Count; i++)
        {
            if (i == 0)
            {
                //TerrainStitcher.stitch(Chunks[i][Chunks.Count].GetComponent<Terrain>(), Chunks[i][Chunks.Count - 1].GetComponent<Terrain>(), 16, 0);
                //TerrainStitcher.stitch(Chunks[i][Chunks.Count].GetComponent<Terrain>(), Chunks[i + 1][Chunks.Count].GetComponent<Terrain>(), 16, 0);

                Chunks[i][Chunks.Count].GetComponent<Terrain>().SetNeighbors(Chunks[i][Chunks.Count - 1].GetComponent<Terrain>(), Chunks[i + 1][Chunks.Count].GetComponent<Terrain>(), null, null);
                Chunks[i][Chunks.Count].GetComponent<Terrain>().Flush();
                Chunks[i][Chunks.Count - 1].GetComponent<Terrain>().SetNeighbors(Chunks[i][Chunks.Count - 2].GetComponent<Terrain>(), Chunks[i + 1][Chunks.Count - 1].GetComponent<Terrain>(), Chunks[i][Chunks.Count].GetComponent<Terrain>(), null);
                Chunks[i][Chunks.Count - 1].GetComponent<Terrain>().Flush();
            }
            else if (i == Chunks.Count - 1)
            {
                //TerrainStitcher.stitch(Chunks[i][Chunks.Count].GetComponent<Terrain>(), Chunks[i][Chunks.Count - 1].GetComponent<Terrain>(), 16, 0);
                //TerrainStitcher.stitch(Chunks[i][Chunks.Count].GetComponent<Terrain>(), Chunks[i - 1][Chunks.Count].GetComponent<Terrain>(), 16, 0);

                Chunks[i][Chunks.Count].GetComponent<Terrain>().SetNeighbors(Chunks[i][Chunks.Count - 1].GetComponent<Terrain>(), null, null, Chunks[i - 1][Chunks.Count].GetComponent<Terrain>());
                Chunks[i][Chunks.Count].GetComponent<Terrain>().Flush();
                Chunks[i][Chunks.Count - 1].GetComponent<Terrain>().SetNeighbors(Chunks[i][Chunks.Count - 2].GetComponent<Terrain>(), null, Chunks[i][Chunks.Count].GetComponent<Terrain>(), Chunks[i - 1][Chunks.Count - 1].GetComponent<Terrain>());
                Chunks[i][Chunks.Count - 1].GetComponent<Terrain>().Flush();
            }
            else
            {
                //TerrainStitcher.stitch(Chunks[i][Chunks.Count].GetComponent<Terrain>(), Chunks[i][Chunks.Count - 1].GetComponent<Terrain>(), 16, 0);
                //TerrainStitcher.stitch(Chunks[i][Chunks.Count].GetComponent<Terrain>(), Chunks[i - 1][Chunks.Count].GetComponent<Terrain>(), 16, 0);
                //TerrainStitcher.stitch(Chunks[i][Chunks.Count].GetComponent<Terrain>(), Chunks[i + 1][Chunks.Count].GetComponent<Terrain>(), 16, 0);

                Chunks[i][Chunks.Count].GetComponent<Terrain>().SetNeighbors(Chunks[i][Chunks.Count - 1].GetComponent<Terrain>(), Chunks[i + 1][Chunks.Count].GetComponent<Terrain>(), null, Chunks[i - 1][Chunks.Count].GetComponent<Terrain>());
                Chunks[i][Chunks.Count].GetComponent<Terrain>().Flush();
                Chunks[i][Chunks.Count - 1].GetComponent<Terrain>().SetNeighbors(Chunks[i][Chunks.Count - 2].GetComponent<Terrain>(), Chunks[i + 1][Chunks.Count - 1].GetComponent<Terrain>(), Chunks[i][Chunks.Count].GetComponent<Terrain>(), Chunks[i - 1][Chunks.Count - 1].GetComponent<Terrain>());
                Chunks[i][Chunks.Count - 1].GetComponent<Terrain>().Flush();
            }
        }

        for (int i = 0; i < Chunks.Count; i++)
        {
            Chunks[i][Chunks.Count].GetComponent<Chunk>().GenerateFoliage();
        }
    }

    void AddRowBack()
    {
        Vector3 vec = Chunks[Center][0].transform.position;
        int c = -(Center * ChunkSize);
        foreach (List<GameObject> lgo in Chunks)
        {
            GameObject go = new GameObject();
            go.transform.position = new Vector3(vec.x - ChunkSize, 0, vec.z + c);
            go.AddComponent<Chunk>();
            go.name = "Chunk" + "[" + go.transform.position.x / ChunkSize + ";" + go.transform.position.z / ChunkSize + "]";
            go.GetComponent<Chunk>().Generate();
            lgo.Insert(0, go);
            c += ChunkSize;
        }


        for (int i = 0; i < Chunks.Count; i++)
        {
            if (i == 0)
            {
                //TerrainStitcher.stitch(Chunks[i][0].GetComponent<Terrain>(), Chunks[i][1].GetComponent<Terrain>(), 16, 0);
                //TerrainStitcher.stitch(Chunks[i][0].GetComponent<Terrain>(), Chunks[i + 1][0].GetComponent<Terrain>(), 16, 0);

                Chunks[i][0].GetComponent<Terrain>().SetNeighbors(null, Chunks[i + 1][0].GetComponent<Terrain>(), Chunks[i][1].GetComponent<Terrain>(), null);
                Chunks[i][0].GetComponent<Terrain>().Flush();
                Chunks[i][1].GetComponent<Terrain>().SetNeighbors(Chunks[i][0].GetComponent<Terrain>(), Chunks[i + 1][1].GetComponent<Terrain>(), Chunks[i][2].GetComponent<Terrain>(), null);
                Chunks[i][1].GetComponent<Terrain>().Flush();
            }
            else if (i == Chunks.Count - 1)
            {
                //TerrainStitcher.stitch(Chunks[i][0].GetComponent<Terrain>(), Chunks[i][1].GetComponent<Terrain>(), 16, 0);
                //TerrainStitcher.stitch(Chunks[i][0].GetComponent<Terrain>(), Chunks[i - 1][0].GetComponent<Terrain>(), 16, 0);

                Chunks[i][0].GetComponent<Terrain>().SetNeighbors(null, null, Chunks[i][1].GetComponent<Terrain>(), Chunks[i - 1][0].GetComponent<Terrain>());
                Chunks[i][0].GetComponent<Terrain>().Flush();
                Chunks[i][1].GetComponent<Terrain>().SetNeighbors(Chunks[i][0].GetComponent<Terrain>(), null, Chunks[i][2].GetComponent<Terrain>(), Chunks[i - 1][1].GetComponent<Terrain>());
                Chunks[i][1].GetComponent<Terrain>().Flush();
            }
            else
            {
                //TerrainStitcher.stitch(Chunks[i][0].GetComponent<Terrain>(), Chunks[i][1].GetComponent<Terrain>(), 16, 0);
                //TerrainStitcher.stitch(Chunks[i][0].GetComponent<Terrain>(), Chunks[i - 1][0].GetComponent<Terrain>(), 16, 0);
                //TerrainStitcher.stitch(Chunks[i][0].GetComponent<Terrain>(), Chunks[i + 1][0].GetComponent<Terrain>(), 16, 0);

                Chunks[i][0].GetComponent<Terrain>().SetNeighbors(null, Chunks[i + 1][0].GetComponent<Terrain>(), Chunks[i][1].GetComponent<Terrain>(), Chunks[i - 1][0].GetComponent<Terrain>());
                Chunks[i][0].GetComponent<Terrain>().Flush();
                Chunks[i][1].GetComponent<Terrain>().SetNeighbors(Chunks[i][0].GetComponent<Terrain>(), Chunks[i + 1][1].GetComponent<Terrain>(), Chunks[i][2].GetComponent<Terrain>(), Chunks[i - 1][1].GetComponent<Terrain>());
                Chunks[i][1].GetComponent<Terrain>().Flush();
            }
        }
        for (int i = 0; i < Chunks.Count; i++)
        {
            Chunks[i][0].GetComponent<Chunk>().GenerateFoliage();
        }
    }

    void DeleteColumnFront()
    {
        for (int i = 0; i < Chunks[Chunks.Count - 1].Count; i++)
        {
            if (i == 0)
            {
                Chunks[Chunks.Count - 2][i].GetComponent<Terrain>().SetNeighbors(null, null, Chunks[Chunks.Count - 2][i + 1].GetComponent<Terrain>(), Chunks[Chunks.Count - 3][i].GetComponent<Terrain>());
            }
            else if (i == Chunks[0].Count - 1)
            {
                Chunks[Chunks.Count - 2][i].GetComponent<Terrain>().SetNeighbors(Chunks[Chunks.Count - 2][i - 1].GetComponent<Terrain>(), null, null, Chunks[Chunks.Count - 3][i].GetComponent<Terrain>());
            }
            else
            {
                Chunks[Chunks.Count - 2][i].GetComponent<Terrain>().SetNeighbors(Chunks[Chunks.Count - 2][i - 1].GetComponent<Terrain>(), null, Chunks[Chunks.Count - 2][i + 1].GetComponent<Terrain>(), Chunks[Chunks.Count - 3][i].GetComponent<Terrain>());
            }
            Chunks[Chunks.Count - 2][i].GetComponent<Terrain>().Flush();
            GameObject.Destroy(Chunks[Chunks.Count - 1][i]);
        }
        Chunks.RemoveAt(Chunks.Count - 1);
    }

    void DeleteColumnBack()
    {
        for (int i = 0; i < Chunks[0].Count; i++)
        {
            if (i == 0)
            {
                Chunks[1][i].GetComponent<Terrain>().SetNeighbors(null, Chunks[2][i].GetComponent<Terrain>(), Chunks[1][i + 1].GetComponent<Terrain>(), null);
            }
            else if (i == Chunks[0].Count - 1)
            {
                Chunks[1][i].GetComponent<Terrain>().SetNeighbors(Chunks[1][i - 1].GetComponent<Terrain>(), Chunks[2][i].GetComponent<Terrain>(), null, null);
            }
            else
            {
                Chunks[1][i].GetComponent<Terrain>().SetNeighbors(Chunks[1][i - 1].GetComponent<Terrain>(), Chunks[2][i].GetComponent<Terrain>(), Chunks[1][i + 1].GetComponent<Terrain>(), null);
            }
            Chunks[1][i].GetComponent<Terrain>().Flush();
            GameObject.Destroy(Chunks[0][i]);
        }
        Chunks.RemoveAt(0);
    }

    void DeleteRowFront()
    {
        for (int i = 0; i < Chunks.Count; i++)
        {
            if (i == 0)
            {
                Chunks[i][Chunks.Count - 1].GetComponent<Terrain>().SetNeighbors(Chunks[i][Chunks.Count - 2].GetComponent<Terrain>(), Chunks[i + 1][Chunks.Count - 1].GetComponent<Terrain>(), null, null);
			}
			else if (i == Chunks.Count - 1)
			{

                Chunks[i][Chunks.Count - 1].GetComponent<Terrain>().SetNeighbors(Chunks[i][Chunks.Count - 2].GetComponent<Terrain>(), null, null, Chunks[i - 1][Chunks.Count - 1].GetComponent<Terrain>());
			}
            else
            {
                Chunks[i][Chunks.Count - 1].GetComponent<Terrain>().SetNeighbors(Chunks[i][Chunks.Count - 2].GetComponent<Terrain>(), Chunks[i + 1][Chunks.Count - 1].GetComponent<Terrain>(), null, Chunks[i - 1][Chunks.Count - 1].GetComponent<Terrain>());
            }
			Chunks[i][Chunks.Count - 1].GetComponent<Terrain>().Flush();
		}
		for (int i = 0; i < Chunks.Count; i++)
        {
            GameObject.Destroy(Chunks[i][Chunks.Count]);
            Chunks[i].RemoveAt(Chunks.Count);
        }
    }

    void DeleteRowBack()
    {
        for (int i = 0; i < Chunks.Count; i++)
        {
            if (i == 0)
            {
                Chunks[i][1].GetComponent<Terrain>().SetNeighbors(null, Chunks[i + 1][1].GetComponent<Terrain>(), Chunks[i][2].GetComponent<Terrain>(), null);
            }
			else if (i == Chunks.Count - 1)
			{
                Chunks[i][1].GetComponent<Terrain>().SetNeighbors(null, null, Chunks[i][2].GetComponent<Terrain>(), Chunks[i - 1][1].GetComponent<Terrain>());
            }
            else
            {
                Chunks[i][1].GetComponent<Terrain>().SetNeighbors(null, Chunks[i + 1][1].GetComponent<Terrain>(), Chunks[i][2].GetComponent<Terrain>(), Chunks[i - 1][1].GetComponent<Terrain>());
            }
            Chunks[i][1].GetComponent<Terrain>().Flush();
            
        }
        for (int i = 0; i < Chunks.Count; i++)
        {
            GameObject.Destroy(Chunks[i][0]);
            Chunks[i].RemoveAt(0);
        }
    }

    public Chunk GetChunkAtPlayer()
    {
        return GameObject.Find("Chunk[" + ((int)(Player.transform.position.x / ChunkSize)).ToString() + ";" + ((int)(Player.transform.position.z / ChunkSize)).ToString() + "]").GetComponent<Chunk>();
    }

    public Chunk GetChunkAtWorldLocation(float x, float y)
    {
        return GameObject.Find("Chunk[" + ((int)(x / ChunkSize)).ToString() + ";" + ((int)(y / ChunkSize)).ToString() + "]").GetComponent<Chunk>();
    }

    public Chunk GetChunkAtChunkLocation(int x, int y)
    {
        return GameObject.Find("Chunk[" + (x).ToString() + ";" + (y).ToString() + "]").GetComponent<Chunk>();
    }
}