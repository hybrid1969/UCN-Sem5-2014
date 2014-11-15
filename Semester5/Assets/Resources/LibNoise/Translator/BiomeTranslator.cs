using UnityEngine;
using System;
using System.Collections;

namespace LibNoise.Operator
{
    public class BiomeTranslator : ModuleBase
    {
        public double[,] BiomeMap;
        public double xminval;
        public double xmaxval;
        public double yminval;
        public double ymaxval;

        public BiomeTranslator(BiomeTypes[,] biomemap, BiomeTypes mask, double _xminval, double _xmaxval, double _yminval, double _ymaxval)
            : base(0)
        {
            BiomeMap = new double[DataBaseHandler.HeighMapSize, DataBaseHandler.HeighMapSize];
            xminval = _xminval;
            xmaxval = _xmaxval;
            yminval = _yminval;
            ymaxval = _ymaxval;

            for (int i = 0; i < DataBaseHandler.HeighMapSize; i++)
            {
                for (int j = 0; j < DataBaseHandler.HeighMapSize; j++)
                {
                    BiomeTypes bt = biomemap[(int)(((double)DataBaseHandler.BiomeMapSize / (double)DataBaseHandler.HeighMapSize) * (double)i), (int)(((double)DataBaseHandler.BiomeMapSize / (double)DataBaseHandler.HeighMapSize) * (double)j)];

                    if (bt == mask)
                    {
                        BiomeMap[i, j] = 1.0;
                    }
                    else
                    {
                        BiomeMap[i, j] = -1.0;
                    }
                }
            }

            //int[,] weight = new int[,]
            //{
            //    {05, 04, 2,},
            //    {12, 09, 4,},
            //    {15, 12, 5,},
            //    {28, 21, 15, 12}
            //    {15, 12, 5,},
            //    {12, 09, 4,},
            //    {05, 04, 2,},
            //};

            int smoothlenght = 8;
            for (int q = 0; q < 1; q++)
            {
                double[,] temp = new double[DataBaseHandler.HeighMapSize, DataBaseHandler.HeighMapSize];
                for (int i = 0; i < BiomeMap.GetLength(0); i++)
                {
                    for (int j = 0; j < BiomeMap.GetLength(1); j++)
                    {
                        double sum = 0;
                        double divider = 0;
                        for (int k = -smoothlenght; k <= smoothlenght; k++)
                        {
                            for (int l = -smoothlenght; l <= smoothlenght; l++)
                            {
                                if (i + k >= 0 && j + l >= 0 && i + k < BiomeMap.GetLength(0) - 1 && j + l < BiomeMap.GetLength(1) - 1)
                                {
                                    int c = (int)((((Mathf.Cos(((float)Mathf.Abs(k) / 16.0f) * Mathf.PI) * 32) + 32) + ((Mathf.Cos(((float)Mathf.Abs(l) / 16.0f) * Mathf.PI) * 32) + 32)) / 2);
                                    sum += BiomeMap[i + k, j + l] * c;
                                    divider += c;
                                }
                            }
                        }
                        temp[i, j] = sum / divider;
                    }
                }
                BiomeMap = temp;
            }
        }

        public override double GetValue(double x, double y, double z)
        {
            try
            {
                double dx = (x - xminval) / (xmaxval - xminval);
                double dy = (z - yminval) / (ymaxval - yminval);
                double ds = (double)BiomeMap[(int)(dx * ((BiomeMap.GetLength(0) - 2))), (int)(dy * ((BiomeMap.GetLength(1) - 2)))];
                return ds;

            }
            catch(Exception ex)
            {
                return 0;
            }
        }
    }
}