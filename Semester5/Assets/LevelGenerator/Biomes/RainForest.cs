using UnityEngine;
using System.Collections;
using LibNoise;

public class RainForest : Biome
{
    public RainForest(Texture2D _texture, Texture2D _normal, BiomeTypes _type) 
        : base (_texture, _normal, _type)
    {
    }

    public override ModuleBase Generate(NoiseHelper noisehelper)
    {
        return noisehelper.GetTundraNoise();
    }
}
