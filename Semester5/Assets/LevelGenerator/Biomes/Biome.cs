using UnityEngine;
using System.Linq;
using LibNoise;

public abstract class Biome
{
    public Texture2D Texture { get; set; }
    public Texture2D Normal { get; set; }
    public BiomeTypes Type { get; set; }

    public GameObject[] Grass { get; set; }
    public GameObject[] Trees { get; set; }

    //public float minHeight = 10;
    //public float maxHeight = 10;
    //public float LowFreqNoise;
    //public float MedFrqNoise;
    //public float HighFreqNoise;
    //public float LowFreqRidged;
    //public float MedFreqRidged;
    //public float HighFreqRidged;
    //public float PerturbDepth;
    //public float ErosionFreq;
    //public float ErosionSmooth;
    //public float SmoothenLenght;

    public Biome(Texture2D _texture, Texture2D _normal, BiomeTypes _type)
    {
        Texture = _texture;
        Normal = _normal;
        Type = _type;
    }

    public abstract ModuleBase Generate(NoiseHelper noisehelper);

    public virtual void GenerateGrass()
    {

    }

    public virtual void GenerateTress()
    {

    }

    public virtual SplatPrototype GetSplatPrototype()
    {
        SplatPrototype splatsPrototypes = new SplatPrototype();
        splatsPrototypes.texture = Texture;
        splatsPrototypes.normalMap = Normal;
        return splatsPrototypes;
    }

    public static BiomeTypes DecideBiome(float temperature, float humidity)
    {
        //TODO: Change to new boimes!
        //Temperature is always between -50 and +50, humidity always between 0 and 100.

        if (temperature <= -27.0f)
        {
            return BiomeTypes.Arctic;
        }
        else if (temperature <= -20.0f)
        {
            return BiomeTypes.Tundra;
        }
        else if (temperature <= 7.0f)
        {
            if (humidity <= 50.0f)
            {
                return BiomeTypes.Taiga;
            }
            else
            {
                return BiomeTypes.BorealForest;
            }
        }
        else if (temperature <= 22.5f)
        {
            //if (humidity <= 7.0f)
            //{
            //    return BiomeTypes.Heathland;
            //}
            //else 
            if (humidity <= 50.0f)
            {
                return BiomeTypes.Prairie;
            }
            else
            {
                return BiomeTypes.Forest;
            }
        }
        //else if (temperature <= 25.0f)
        //{
        //    if (humidity <= 25)
        //    {
        //        return BiomeTypes.ColdDesert;
        //    }
        //    else if (humidity <= 50)
        //    {
        //        return BiomeTypes.Chaparral;
        //    }
        //    else if (humidity <= 75)
        //    {
        //        return BiomeTypes.Steppe;
        //    }
        //    else
        //    {
        //        return BiomeTypes.TemperatedForest;
        //    }
        //}
        else
        {
            
            if (humidity <= 40)
            {
                return BiomeTypes.Desert;
            }
            //else if (humidity <= 40)
            //{
            //    return BiomeTypes.Shrubland;
            //}
            else if (humidity <= 60)
            {
                return BiomeTypes.Savanna;
            }
            //else if (humidity <= 70)
            //{
            //    return BiomeTypes.SeasonalForest;
            //}
            else
            {
                return BiomeTypes.RainForest;
            }
        }
    }

    public static Biome FindBiome(BiomeTypes biome)
    {
        return DataBaseHandler.DataBase.biomes.First(b => b.Type == biome);
    }

    public static int FindTexture(BiomeTypes biome)
    {
        return DataBaseHandler.DataBase.biomes.FindIndex(b => b.Type == biome);
    }

    //public static int FindTexture(BiomeTypes biome)
    //{
    //    if (biome == BiomeTypes.Arctic)
    //    {
    //        return 0;
    //    }
    //    if (biome == BiomeTypes.BorealForest)
    //    {
    //        return 1;
    //    }
    //    if (biome == BiomeTypes.Chaparral)
    //    {
    //        return 2;
    //    }
    //    if (biome == BiomeTypes.ColdDesert)
    //    {
    //        return 3;
    //    }
    //    if (biome == BiomeTypes.Desert)
    //    {
    //        return 4;
    //    }
    //    if (biome == BiomeTypes.Forest)
    //    {
    //        return 5;
    //    }
    //    if (biome == BiomeTypes.Heathland)
    //    {
    //        return 6;
    //    }
    //    if (biome == BiomeTypes.Prairie)
    //    {
    //        return 7;
    //    }
    //    if (biome == BiomeTypes.RainForest)
    //    {
    //        return 8;
    //    }
    //    if (biome == BiomeTypes.Savanna)
    //    {
    //        return 9;
    //    }
    //    if (biome == BiomeTypes.SeasonalForest)
    //    {
    //        return 10;
    //    }
    //    if (biome == BiomeTypes.Shrubland)
    //    {
    //        return 11;
    //    }
    //    if (biome == BiomeTypes.Steppe)
    //    {
    //        return 12;
    //    }
    //    if (biome == BiomeTypes.Taiga)
    //    {
    //        return 13;
    //    }
    //    if (biome == BiomeTypes.TemperatedForest)
    //    {
    //        return 14;
    //    }
    //    if (biome == BiomeTypes.Tundra)
    //    {
    //        return 15;
    //    }
    //    return 0;
    //}
}

public enum BiomeTypes
{
    Ocean,

    //Icy
    Tundra,
    IceWasteland,
    Arctic,
    Polar,
    

    //Cold Biomes
    PineForrest,
    BorealForest,
    ConiferousForest,
    Taiga,

    //Temperated Biomes
    Heathland,
    Prairie,
    Forest,
    Swamp,

    //Warm Biomes
    ColdDesert,
    Chaparral,
    Steppe,
    TemperatedForest,

    //Hot Biomes
    Desert,
    Shrubland,
    Savanna,
    SeasonalForest,
    RainForest,
}