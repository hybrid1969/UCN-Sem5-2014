using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DataBaseHandler : MonoBehaviour
{
    public static DataBaseHandler DataBase { get; set; }
    public List<Biome> biomes = new List<Biome>();
    public List<GameObject> Trees = new List<GameObject>();
    public SplatPrototype[] SplatsPrototypes;
    public TreePrototype[] TreePrototypes;
    public const int ChunkSize = 512;
    public const int HeighMapSize = 65;
    public const int BiomeMapSize = 64;
    public System.Random random;
    public int Seed = 0;

    public DataBaseHandler()
    {
        DataBase = this;
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        random = new System.Random(Seed);
        UnityEngine.Random.seed = DataBaseHandler.DataBase.Seed;

        biomes.Add(new Arctic(Resources.Load<Texture2D>("Textures/Arctic_Diff"), Resources.Load<Texture2D>("Textures/Arctic_Norm"), BiomeTypes.Arctic));
        biomes.Add(new BorealForest(Resources.Load<Texture2D>("Textures/BorealForest_Diff"), Resources.Load<Texture2D>("Textures/BorealForest_Norm"), BiomeTypes.BorealForest));
        biomes.Add(new Desert(Resources.Load<Texture2D>("Textures/Desert_Diff"), Resources.Load<Texture2D>("Textures/Desert_Norm"), BiomeTypes.Desert));
        biomes.Add(new Forest(Resources.Load<Texture2D>("Textures/Forest_Diff"), Resources.Load<Texture2D>("Textures/Forest_Norm"), BiomeTypes.Forest));
        biomes.Add(new Prairie(Resources.Load<Texture2D>("Textures/Prairie_Diff"), Resources.Load<Texture2D>("Textures/Prairie_Norm"), BiomeTypes.Prairie));
        biomes.Add(new RainForest(Resources.Load<Texture2D>("Textures/RainForest_Diff"), Resources.Load<Texture2D>("Textures/RainForest_Norm"), BiomeTypes.RainForest));
        biomes.Add(new Savanna(Resources.Load<Texture2D>("Textures/Savanna_Diff"), Resources.Load<Texture2D>("Textures/Savanna_Norm"), BiomeTypes.Savanna));
        biomes.Add(new Taiga(Resources.Load<Texture2D>("Textures/Taiga_Diff"), Resources.Load<Texture2D>("Textures/Taiga_Norm"), BiomeTypes.Taiga));
        biomes.Add(new Tundra(Resources.Load<Texture2D>("Textures/Tundra_Diff"), Resources.Load<Texture2D>("Textures/Tundra_Norm"), BiomeTypes.Tundra));

        Trees.Add(Resources.Load<GameObject>("Foilage/Palm"));

        SplatsPrototypes = new SplatPrototype[biomes.Count];
        for (int i = 0; i < biomes.Count; i ++)
        {
            SplatPrototype sp = biomes[i].GetSplatPrototype();
            sp.tileSize = new Vector2(16, 16);
            SplatsPrototypes[i] = sp;
        }
        TreePrototypes = new TreePrototype[Trees.Count];
        for (int i = 0; i < Trees.Count; i++)
        {
            TreePrototype tp = new TreePrototype();
            tp.prefab = Trees[i];
            TreePrototypes[i] = tp;
        }
    }
}