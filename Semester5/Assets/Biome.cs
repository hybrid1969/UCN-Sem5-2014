using UnityEngine;
using System.Collections;

[System.Serializable]
public class Biome
{
    public Texture tex;
    public BiomeTypes Type;
    public float mountainPower = 1;
    public float minHeight = 10;
    public float maxHeight = 10;

    public float mountainPowerBonus = 0;
}

public enum BiomeTypes
{
    Grassy,
    Woody,
}