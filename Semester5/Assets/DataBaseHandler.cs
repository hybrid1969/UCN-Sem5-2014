using UnityEngine;
using System.Collections;

public class DataBaseHandler : ScriptableObject
{
    public static DataBaseHandler DataBase { get; set; }
    //public Texture[] SplatMaps;
    public Biome[] biomes;

    public DataBaseHandler()
    {
        DataBase = this;
    }
}
