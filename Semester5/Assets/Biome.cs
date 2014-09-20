using UnityEngine;
using System.Collections;

[System.Serializable]
public class Biome
{
    public Texture2D Texture;
    public Texture2D Normal;
    public BiomeTypes Type;
    public float mountainPower = 1;
    public float minHeight = 10;
    public float maxHeight = 10;

    public float mountainPowerBonus = 0;
}

public enum BiomeTypes
{
    Polar = 0,
    Tundra = 1,
    Tiaga = 2,
    Woods = 3,
    RainForest = 4,
    Desert = 5,

    Ocean = 6,
}