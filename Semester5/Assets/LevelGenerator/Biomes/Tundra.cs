using UnityEngine;
using System.Collections;
using LibNoise;

public class Tundra : Biome
{
    public Tundra(Texture2D _texture, Texture2D _normal, BiomeTypes _type) 
        : base (_texture, _normal, _type)
    {
    }

    public override ModuleBase Generate(NoiseHelper noisehelper)
    {
        return noisehelper.GetTundraNoise();// noisehelper.GetHighFreqNoise(x, y) * 0.75f + noisehelper.GetMedFreqRidged(x, y) * 0.25f;
    }
}
