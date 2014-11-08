using UnityEngine;
using System.Collections;

public class TreeHandler : MonoBehaviour
{
    void OnEnable()
    {
        ChunkLoader.OnTerrainChanged += CorectTree;
    }


    void OnDisable()
    {
        ChunkLoader.OnTerrainChanged -= CorectTree;
    }


    void CorectTree()
    {
        RaycastHit hit;
        Physics.Raycast(
            new Vector3(transform.position.x, -1, transform.position.z), Vector3.up,
            out hit, (float)DataBaseHandler.ChunkSize * 2.5f, 1);
        transform.position = hit.point;
    }
}