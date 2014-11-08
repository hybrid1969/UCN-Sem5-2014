using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DataBaseHandler : MonoBehaviour
{
    public static DataBaseHandler DataBase { get; set; }
    public List<Biome> biomes = new List<Biome>();
    public List<GameObject> Foliage = new List<GameObject>();
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

        Foliage.Add(Resources.Load<GameObject>("Foliage/Palm"));


        biomes.Add(new Arctic(Resources.Load<Texture2D>("Textures/Arctic_Diff"), Resources.Load<Texture2D>("Textures/Arctic_Norm"), BiomeTypes.Arctic,
            new GameObject[]
            {
                Resources.Load<GameObject>("Foliage/Arctic_Tree1"),
                Resources.Load<GameObject>("Foliage/Arctic_Tree2"),
                Resources.Load<GameObject>("Foliage/Arctic_Tree3"),
                Resources.Load<GameObject>("Foliage/Arctic_Tree4"),
                Resources.Load<GameObject>("Foliage/Arctic_Tree5"),
                Resources.Load<GameObject>("Foliage/Arctic_Tree6"),
                Resources.Load<GameObject>("Foliage/Arctic_Tree7"),
                Resources.Load<GameObject>("Foliage/Arctic_Tree8"),
            },
            new int[]
            {
                5,5,5,5,5,5,5,5,
            }));
        biomes.Add(new BorealForest(Resources.Load<Texture2D>("Textures/BorealForest_Diff"), Resources.Load<Texture2D>("Textures/BorealForest_Norm"), BiomeTypes.BorealForest, 
            new GameObject[]
            {
                Resources.Load<GameObject>("Foliage/BorealForest_Tree1"),
                Resources.Load<GameObject>("Foliage/BorealForest_Tree2"),
                Resources.Load<GameObject>("Foliage/BorealForest_Tree3"),
                Resources.Load<GameObject>("Foliage/BorealForest_Tree4"),
                Resources.Load<GameObject>("Foliage/BorealForest_Tree5"),
            },
            new int[]
            {
                100,100,100,100,100
            }));
        biomes.Add(new Desert(Resources.Load<Texture2D>("Textures/Desert_Diff"), Resources.Load<Texture2D>("Textures/Desert_Norm"), BiomeTypes.Desert,
            new GameObject[]
            {
                Resources.Load<GameObject>("Foliage/Desert_Tree1"),
                Resources.Load<GameObject>("Foliage/Desert_Tree2"),
                Resources.Load<GameObject>("Foliage/Desert_Tree3"),                
            },
            new int[]
            {
                20,20,50,
            }));
        biomes.Add(new Forest(Resources.Load<Texture2D>("Textures/Forest_Diff"), Resources.Load<Texture2D>("Textures/Forest_Norm"), BiomeTypes.Forest, 
            new GameObject[]
            {
                Resources.Load<GameObject>("Foliage/Forest_Tree1"),
                Resources.Load<GameObject>("Foliage/Forest_Tree2"),
                Resources.Load<GameObject>("Foliage/Forest_Tree3"),
                Resources.Load<GameObject>("Foliage/Forest_Tree4"),
                Resources.Load<GameObject>("Foliage/Forest_Tree5"),
                Resources.Load<GameObject>("Foliage/Forest_Tree6"),
                Resources.Load<GameObject>("Foliage/Forest_Tree7"),
                Resources.Load<GameObject>("Foliage/Forest_Tree8"),
                Resources.Load<GameObject>("Foliage/Forest_Tree9"),
                Resources.Load<GameObject>("Foliage/Forest_Tree10"),
                Resources.Load<GameObject>("Foliage/Forest_Tree11"),
                Resources.Load<GameObject>("Foliage/Forest_Tree12"),
                Resources.Load<GameObject>("Foliage/Forest_Tree13"),
                Resources.Load<GameObject>("Foliage/Forest_Tree14"),
                Resources.Load<GameObject>("Foliage/Forest_Tree15"),
                Resources.Load<GameObject>("Foliage/Forest_Tree16"),
                Resources.Load<GameObject>("Foliage/Forest_Tree17"),
                Resources.Load<GameObject>("Foliage/Forest_Tree18"),                
            },
            new int[]
            {
                50,50,25,25,25,25,25,25,25,25,25,25,25,25,25,25,25,25,
            }));
        biomes.Add(new Prairie(Resources.Load<Texture2D>("Textures/Prairie_Diff"), Resources.Load<Texture2D>("Textures/Prairie_Norm"), BiomeTypes.Prairie,            
            new GameObject[]
            {
                Resources.Load<GameObject>("Foliage/Prairie_Tree1"),
                Resources.Load<GameObject>("Foliage/Prairie_Tree2"),
                Resources.Load<GameObject>("Foliage/Prairie_Tree3"),
                Resources.Load<GameObject>("Foliage/Prairie_Tree4"),
                Resources.Load<GameObject>("Foliage/Prairie_Tree5"),
                Resources.Load<GameObject>("Foliage/Prairie_Tree6"),
                Resources.Load<GameObject>("Foliage/Prairie_Tree7"),
                Resources.Load<GameObject>("Foliage/Prairie_Tree8"),
                Resources.Load<GameObject>("Foliage/Prairie_Tree9"),
                Resources.Load<GameObject>("Foliage/Prairie_Tree10"),
                
            },
            new int[]
            {
                200,50,20,20,20,20,100,150,200,200,
            }));
        biomes.Add(new RainForest(Resources.Load<Texture2D>("Textures/RainForest_Diff"), Resources.Load<Texture2D>("Textures/RainForest_Norm"), BiomeTypes.RainForest,
            new GameObject[]
            {
                Resources.Load<GameObject>("Foliage/RainForest_Tree1"),
                Resources.Load<GameObject>("Foliage/RainForest_Tree2"),
                Resources.Load<GameObject>("Foliage/RainForest_Tree3"),
                Resources.Load<GameObject>("Foliage/RainForest_Tree4"),
                Resources.Load<GameObject>("Foliage/RainForest_Tree5"),
                Resources.Load<GameObject>("Foliage/RainForest_Tree6"),
                Resources.Load<GameObject>("Foliage/RainForest_Tree7"),
                Resources.Load<GameObject>("Foliage/RainForest_Tree8"),
                Resources.Load<GameObject>("Foliage/RainForest_Tree9"),
                Resources.Load<GameObject>("Foliage/RainForest_Tree10"),
                Resources.Load<GameObject>("Foliage/RainForest_Tree11"),
                Resources.Load<GameObject>("Foliage/RainForest_Tree12"),
                Resources.Load<GameObject>("Foliage/RainForest_Tree13"),
                Resources.Load<GameObject>("Foliage/RainForest_Tree14"),

            },
            new int[]
            {
                100, 100, 25, 25, 40, 40, 40, 25, 25, 100, 100, 100, 30, 40,
            }));
        biomes.Add(new Savanna(Resources.Load<Texture2D>("Textures/Savanna_Diff"), Resources.Load<Texture2D>("Textures/Savanna_Norm"), BiomeTypes.Savanna,             
            new GameObject[]
            {
                Resources.Load<GameObject>("Foliage/Savanna_Tree1"),
                Resources.Load<GameObject>("Foliage/Savanna_Tree2"),
                Resources.Load<GameObject>("Foliage/Savanna_Tree3"),
                Resources.Load<GameObject>("Foliage/Savanna_Tree4"),
                Resources.Load<GameObject>("Foliage/Savanna_Tree5"),
                Resources.Load<GameObject>("Foliage/Savanna_Tree6"),
                Resources.Load<GameObject>("Foliage/Savanna_Tree7"),
                Resources.Load<GameObject>("Foliage/Savanna_Tree8"),
            },
            new int[]
            {
                25,25,200,10,50,50,100,100,
            }));
        biomes.Add(new Taiga(Resources.Load<Texture2D>("Textures/Taiga_Diff"), Resources.Load<Texture2D>("Textures/Taiga_Norm"), BiomeTypes.Taiga,             
            new GameObject[]
            {
                Resources.Load<GameObject>("Foliage/Taiga_Tree1"),
                Resources.Load<GameObject>("Foliage/Taiga_Tree2"),
                Resources.Load<GameObject>("Foliage/Taiga_Tree3"),
                Resources.Load<GameObject>("Foliage/Taiga_Tree4"),
                Resources.Load<GameObject>("Foliage/Taiga_Tree5"),
                Resources.Load<GameObject>("Foliage/Taiga_Tree6"),
                Resources.Load<GameObject>("Foliage/Taiga_Tree7"),
                Resources.Load<GameObject>("Foliage/Taiga_Tree8"),
            },
            new int[]
            {
                100, 50, 100, 50, 25, 25, 50, 50,
            }));
        biomes.Add(new Tundra(Resources.Load<Texture2D>("Textures/Tundra_Diff"), Resources.Load<Texture2D>("Textures/Tundra_Norm"), BiomeTypes.Tundra,             
            new GameObject[]
            {
                Resources.Load<GameObject>("Foliage/Tundra_Tree1"),                
                Resources.Load<GameObject>("Foliage/Tundra_Tree2"),
                Resources.Load<GameObject>("Foliage/Tundra_Tree3"),
                Resources.Load<GameObject>("Foliage/Tundra_Tree4"),
                Resources.Load<GameObject>("Foliage/Tundra_Tree5"),
                Resources.Load<GameObject>("Foliage/Tundra_Tree6"),
                Resources.Load<GameObject>("Foliage/Tundra_Tree7"),
                Resources.Load<GameObject>("Foliage/Tundra_Tree8"),
                Resources.Load<GameObject>("Foliage/Tundra_Tree9"),
                Resources.Load<GameObject>("Foliage/Tundra_Tree10"),
            },
            new int[]
            {
                10,10,10,10,10,10,10,10,25,25,
            }));

        SplatsPrototypes = new SplatPrototype[biomes.Count];
        for (int i = 0; i < biomes.Count; i ++)
        {
            SplatPrototype sp = biomes[i].GetSplatPrototype();
            sp.tileSize = new Vector2(16, 16);
            SplatsPrototypes[i] = sp;
        }
        TreePrototypes = new TreePrototype[Foliage.Count];
        for (int i = 0; i < Foliage.Count; i++)
        {
            TreePrototype tp = new TreePrototype();
            tp.prefab = Foliage[i];
            TreePrototypes[i] = tp;
        }
    }
}