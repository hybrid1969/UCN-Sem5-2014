﻿using UnityEngine;
using System.Collections;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;

public class NoiseHelper
{
    Noise2D HeightMap;
    Noise2D RainFallMap;
    Noise2D TemperatureMap;

    Noise2D FinalTerrainNoise;

    Select TundraNoise;
    Select TaigaNoise;

    Perlin StandartNoise = new Perlin(0.125, 2, 0.5, 2, DataBaseHandler.DataBase.Seed, QualityMode.High);

    BiomeTypes[,] Biomes;
    Rect Bounds;

    double xoffset;
    double yoffset;

    public NoiseHelper(double x, double y)
    {
        xoffset = x;
        yoffset = y;
    }

    public void SetBiomes(BiomeTypes[,] bt)
    {
        Biomes = bt;
    }

    public void SetBounds(Rect r)
    {
        Bounds = r;
    }

    public ModuleBase GetTundraNoise()
    {
        if (TundraNoise == null)
        {
            TundraNoise = new Select(0, 1, 1, StandartNoise, new Perlin(1, 2, 0.5, 4, DataBaseHandler.DataBase.Seed, QualityMode.High), new BiomeTranslator(Biomes, BiomeTypes.Tundra, Bounds.left, Bounds.top, Bounds.width, Bounds.height));
        }
        return TundraNoise;
    }

    public ModuleBase GetTaigaNoise()
    {
        if (TaigaNoise == null)
        {
            TaigaNoise = new Select(0, 1, 1, StandartNoise, new Perlin(0.5, 4, 0.25, 2, DataBaseHandler.DataBase.Seed, QualityMode.High), new BiomeTranslator(Biomes, BiomeTypes.Taiga, Bounds.left, Bounds.top, Bounds.width, Bounds.height));
        }
        return TaigaNoise;
    }

    //public float GetLowFreqNoise(int x, int y)
    //{
    //    if (LowFreqNoise == null)
    //    {
    //        LowFreqNoise = new Noise2D(DataBaseHandler.HeighMapSize, DataBaseHandler.HeighMapSize, new Perlin(0.125, 2, 0.5, 2, DataBaseHandler.DataBase.Seed, QualityMode.High));
    //        LowFreqNoise.GeneratePlanar(yoffset, yoffset + 1, xoffset, xoffset + 1);
    //    }
    //    return ((LowFreqNoise[x, y] * 0.5f) + 0.5f);
    //}
    //public float GetMedFreqNoise(int x, int y)
    //{
    //    if (MedFreqNoise == null)
    //    {
    //        MedFreqNoise = new Noise2D(DataBaseHandler.HeighMapSize, new Perlin(0.25, 2, 0.5, 4, DataBaseHandler.DataBase.Seed, QualityMode.High));
    //        MedFreqNoise.GeneratePlanar(yoffset, yoffset + 1, xoffset, xoffset + 1);
    //    }
    //    return ((MedFreqNoise[x, y] * 0.5f) + 0.5f);
    //}
    //public float GetHighFreqNoise(int x, int y)
    //{
    //    if (HighFreqNoise == null)
    //    {
    //        HighFreqNoise = new Noise2D(DataBaseHandler.HeighMapSize, new Perlin(0.5, 4, 0.25, 8, DataBaseHandler.DataBase.Seed, QualityMode.High));
    //        HighFreqNoise.GeneratePlanar(yoffset, yoffset + (1f / DataBaseHandler.HeighMapSize) * (DataBaseHandler.HeighMapSize + 1), xoffset, xoffset + (1f / DataBaseHandler.HeighMapSize) * (DataBaseHandler.HeighMapSize + 1));
    //    }
    //    return ((HighFreqNoise[x, y] * 0.5f) + 0.5f);
    //}

    //public float GetLowFreqRidged(int x, int y)
    //{
    //    if (LowFreqRidged == null)
    //    {
    //        LowFreqRidged = new Noise2D(DataBaseHandler.HeighMapSize, new RidgedMultifractal(0.1, 1, 2, DataBaseHandler.DataBase.Seed, QualityMode.High));
    //        LowFreqRidged.GeneratePlanar(yoffset, yoffset + (1f / DataBaseHandler.HeighMapSize) * (DataBaseHandler.HeighMapSize + 1), xoffset, xoffset + (1f / DataBaseHandler.HeighMapSize) * (DataBaseHandler.HeighMapSize + 1));
    //    }
    //    return ((LowFreqRidged[x, y] * 0.5f) + 0.5f);
    //}
    //public float GetMedFreqRidged(int x, int y)
    //{
    //    if (MedFreqRidged == null)
    //    {
    //        MedFreqRidged = new Noise2D(DataBaseHandler.HeighMapSize, new RidgedMultifractal(0.15, 2, 4, DataBaseHandler.DataBase.Seed, QualityMode.High));
    //        MedFreqRidged.GeneratePlanar(yoffset, yoffset + (1f / DataBaseHandler.HeighMapSize) * (DataBaseHandler.HeighMapSize + 1), xoffset, xoffset + (1f / DataBaseHandler.HeighMapSize) * (DataBaseHandler.HeighMapSize + 1));
    //    }
    //    return ((MedFreqRidged[x, y]* 0.5f) + 0.5f);
    //}
    //public float GetHighFreqRidged(int x, int y)
    //{
    //    if (HighFreqRidged == null)
    //    {
    //        HighFreqRidged = new Noise2D(DataBaseHandler.HeighMapSize, new RidgedMultifractal(0.2, 4, 8, DataBaseHandler.DataBase.Seed, QualityMode.High));
    //        HighFreqRidged.GeneratePlanar(yoffset, yoffset + (1f / DataBaseHandler.HeighMapSize) * (DataBaseHandler.HeighMapSize + 1), xoffset, xoffset + (1f / DataBaseHandler.HeighMapSize) * (DataBaseHandler.HeighMapSize + 1));
    //    }
    //    return ((HighFreqRidged[x, y] * 0.5f) + 0.5f);
    //}
}