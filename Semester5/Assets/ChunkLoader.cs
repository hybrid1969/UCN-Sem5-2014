using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkLoader : MonoBehaviour
{
    int LoadDistance = 5;
    int Center { get { return (LoadDistance - 1) / 2; } }
    int ChunkSize = 512;
    List<List<GameObject>> Chunks = new List<List<GameObject>>();

    Terrain lt;
    Terrain rt;
    Terrain tt;
    Terrain bt;

    [SerializeField]
    GameObject Player;

	// Use this for initialization
	void Start ()
    {
        for (int i = 0; i < LoadDistance; i++)
        {
            List<GameObject> lgo = new List<GameObject>();
            for (int j = 0; j < LoadDistance; j++)
            {
                GameObject go = new GameObject();
                go.AddComponent<SimpleTerrainGenerator>();
                go.transform.position = new Vector3(((((LoadDistance - 1) / 2) - LoadDistance) + j + 1) * ChunkSize, 0, (((LoadDistance - 1) / 2 - LoadDistance) + i + 1) * ChunkSize);
                go.name = go.name + "[" + (j - Center) + ";" + (i - Center) + "]";
                lgo.Add(go);
                go.GetComponent<SimpleTerrainGenerator>().Starter();
            }
            Chunks.Add(lgo);
        }
        for (int i = 0; i < LoadDistance; i++)
        {
            for (int j = 0; j < LoadDistance; j++)
            {
                if (i != 0)
                {
                    bt = Chunks[i - 1][j].GetComponent<Terrain>();
                    TerrainTools.StitchTerrains(bt, Chunks[i][j].GetComponent<Terrain>(), 64, 0, 20);
                }
                if (i != LoadDistance - 1)
                {
                    tt = Chunks[i + 1][j].GetComponent<Terrain>();
                    TerrainTools.StitchTerrains(tt, Chunks[i][j].GetComponent<Terrain>(), 64, 0, 20);
                }
                if (j != 0)
                {
                    lt = Chunks[i][j - 1].GetComponent<Terrain>();
                    TerrainTools.StitchTerrains(lt, Chunks[i][j].GetComponent<Terrain>(), 64, 0, 20);
                }
                if (j != LoadDistance - 1)
                {
                    rt = Chunks[i][j + 1].GetComponent<Terrain>();
                    TerrainTools.StitchTerrains(rt, Chunks[i][j].GetComponent<Terrain>(), 64, 0, 20);
                }

                Chunks[i][j].GetComponent<Terrain>().SetNeighbors(lt, tt, rt, bt);
                Chunks[i][j].GetComponent<Terrain>().Flush();
                bt = null;
                tt = null;
                lt = null;
                rt = null;
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
        }
        if (Player.transform.position.z > Chunks[Center + 1][Center].transform.position.z - (ChunkSize / 2))
        {
            AddColumnFront();
            DeleteColumnBack();
        }
        if (Player.transform.position.x < Chunks[Center][Center - 1].transform.position.x + (ChunkSize / 2))
        {
            AddRowBack();
            DeleteRowFront();
        }
        if (Player.transform.position.x > Chunks[Center][Center + 1].transform.position.x - (ChunkSize / 2))
        {
            AddRowFront();
            DeleteRowBack();
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
            go.AddComponent<SimpleTerrainGenerator>();
            go.name = go.name + "[" + go.transform.position.x / ChunkSize + ";" + go.transform.position.z / ChunkSize + "]";
            lgo.Add(go);
            go.GetComponent<SimpleTerrainGenerator>().Starter();
            c += ChunkSize;
        }

        Chunks.Add(lgo);
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
            go.AddComponent<SimpleTerrainGenerator>();
            go.name = go.name + "[" + go.transform.position.x / ChunkSize + ";" + go.transform.position.z / ChunkSize + "]";
            lgo.Add(go);
            go.GetComponent<SimpleTerrainGenerator>().Starter();
            c += ChunkSize;
        }
        Chunks.Insert(0, lgo);
    }

    void AddRowFront()
    {
        Vector3 vec = Chunks[Center][Chunks[1].Count - 1].transform.position;
        int c = -(Center * ChunkSize);
        foreach (List<GameObject> lgo in Chunks)
        {
            GameObject go = new GameObject();
            go.transform.position = new Vector3(vec.x + ChunkSize, 0, vec.z + c);
            go.AddComponent<SimpleTerrainGenerator>();
            go.name = go.name + "[" + go.transform.position.x / ChunkSize + ";" + go.transform.position.z / ChunkSize + "]";
            go.GetComponent<SimpleTerrainGenerator>().Starter();
            lgo.Add(go);
            c += ChunkSize;
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
            go.AddComponent<SimpleTerrainGenerator>();
            go.name = go.name + "[" + go.transform.position.x / ChunkSize + ";" + go.transform.position.z / ChunkSize + "]";
            go.GetComponent<SimpleTerrainGenerator>().Starter();
            lgo.Insert(0, go);
            c += ChunkSize;
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
            //TODO: Fix this copy pasta
            if (i == 0)
            {
                //Chunks[i][Chunks.Count - 1].GetComponent<Terrain>().SetNeighbors(null, null, Chunks[i + 1][Chunks.Count - 1].GetComponent<Terrain>(), Chunks[i][Chunks.Count - 2].GetComponent<Terrain>());
				Debug.Log(" Middle: " + Chunks[i][Chunks.Count - 1].name + " Right: " + Chunks[i + 1][Chunks.Count - 1].name + " Bottom: " + Chunks[i][Chunks.Count - 2].name);
			}
			else if (i == Chunks.Count - 1)
			{
				//Chunks[i][Chunks.Count - 1].GetComponent<Terrain>().SetNeighbors(Chunks[i - 1][Chunks.Count - 1].GetComponent<Terrain>(), null, null, Chunks[i][Chunks.Count - 2].GetComponent<Terrain>());
				Debug.Log(" Middle: " + Chunks[i][Chunks.Count - 1].name + " Left: " + Chunks[i - 1][Chunks.Count - 1].name + " Bottom: " + Chunks[i][Chunks.Count - 2].name);
			}
            else
            {
                //Chunks[i][Chunks.Count - 1].GetComponent<Terrain>().SetNeighbors(Chunks[i - 1][Chunks.Count - 1].GetComponent<Terrain>(), null, Chunks[i + 1][Chunks.Count - 1].GetComponent<Terrain>(), Chunks[i][Chunks.Count - 2].GetComponent<Terrain>());
				Debug.Log(" Middle: " + Chunks[i][Chunks.Count - 1].name + " Left: " + Chunks[i - 1][Chunks.Count - 1].name + " Right: " + Chunks[i + 1][Chunks.Count - 1].name + " Bottom: " + Chunks[i][Chunks.Count - 2].name);
            }
            //Chunks[i][Chunks.Count - 2].GetComponent<Terrain>().SetNeighbors(null, null, null, null);
            //Chunks[i][Chunks.Count - 2].GetComponent<Terrain>().Flush();
			Chunks[i][Chunks.Count - 1].GetComponent<Terrain>().SetNeighbors(null, null, null, null);
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
            //TODO: Fix this copy pasta
            if (i == 0)
            {
                //Chunks[i][1].GetComponent<Terrain>().SetNeighbors(null, Chunks[i][2].GetComponent<Terrain>(), Chunks[i + 1][1].GetComponent<Terrain>(), null);
            }
			else if (i == Chunks.Count - 1)
			{
				//Chunks[i][1].GetComponent<Terrain>().SetNeighbors(Chunks[i - 1][i].GetComponent<Terrain>(), Chunks[i][2].GetComponent<Terrain>(), null, null);
            }
            else
            {
                //Chunks[i][1].GetComponent<Terrain>().SetNeighbors(Chunks[i - 1][i].GetComponent<Terrain>(), Chunks[i][2].GetComponent<Terrain>(), Chunks[i + 1][1].GetComponent<Terrain>(), null);
            }
            Chunks[i][1].GetComponent<Terrain>().SetNeighbors(null, null, null, null);
            Chunks[i][1].GetComponent<Terrain>().Flush();
            
        }
        for (int i = 0; i < Chunks.Count; i++)
        {
            GameObject.Destroy(Chunks[i][0]);
            Chunks[i].RemoveAt(0);
        }
    }
}