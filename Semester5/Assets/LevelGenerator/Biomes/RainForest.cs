using UnityEngine;
using System.Collections.Generic;
using LibNoise;

public class RainForest : Biome
{
    public RainForest(Texture2D _texture, Texture2D _normal, BiomeTypes _type, GameObject[] _decoration, int[] _decorationcount)
        : base(_texture, _normal, _type, _decoration, _decorationcount)
    {
    }

    public override ModuleBase Generate(NoiseHelper noisehelper)
    {
        return noisehelper.GetRainForestNoise();
    }
}
