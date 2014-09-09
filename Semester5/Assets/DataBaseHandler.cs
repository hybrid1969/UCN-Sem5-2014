using UnityEngine;
using System.Collections;

public class DataBaseHandler : MonoBehaviour
{
    public static DataBaseHandler DataBase { get; set; }
    //public Texture[] SplatMaps;
    public Biome[] biomes;
    public SplatPrototype[] SplatsPrototypes;
    //public Texture[] 

    public DataBaseHandler()
    {
        DataBase = this;
    }

    void Awake()
    {
        SplatsPrototypes = new SplatPrototype[biomes.Length];
        for (int i = 0; i < biomes.Length; i++)
        {
			SplatsPrototypes[i] = new SplatPrototype();
			SplatsPrototypes[i].texture = biomes[i].tex;
        }
    }
}
