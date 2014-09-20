using UnityEngine;
using System.Collections;

public class DataBaseHandler : MonoBehaviour
{
    public static DataBaseHandler DataBase { get; set; }
    public Biome[] biomes;
    public SplatPrototype[] SplatsPrototypes;
    public BiomeTypes[,] BiomeDiagram = new BiomeTypes[,] { { BiomeTypes.Desert, BiomeTypes.RainForest, BiomeTypes.Woods }, { BiomeTypes.Tundra, BiomeTypes.Tiaga, BiomeTypes.Polar } };

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
			SplatsPrototypes[i].texture = biomes[i].Texture;
            SplatsPrototypes[i].normalMap = biomes[i].Normal;
        }
    }
}
