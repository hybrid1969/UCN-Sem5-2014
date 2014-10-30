using System.Diagnostics;

namespace LibNoise.Operator
{
    public class Mask : ModuleBase
    {
        double MinMaskValue = 0;
        double MaxMaskValue = 0;
        double smoothlenght = 32;
        bool Smooth = true;
        public Mask()
            : base(1)
        {
        }

        public Mask(ModuleBase inputA, double min, double max)
            : base(1)
        {
            Modules[0] = inputA;
            MinMaskValue = min;
            MaxMaskValue = max;
            smoothlenght = smoothlenght / (double)DataBaseHandler.HeighMapSize;
        }

        public Mask(ModuleBase inputA, double val) : this (inputA, val, val)
        {
        }

        public override double GetValue(double x, double y, double z)
        {
            Debug.Assert(Modules[0] != null);
            double sum = 0;
            double divider = 0;

            if (Modules[0].GetType() == typeof(BiomeTranslator))
            {
                BiomeTranslator bt = (BiomeTranslator)Modules[0];
                return 1;
            }
            else
            {
                if (Smooth)
                {
                    for (double i = x - smoothlenght; i < x + smoothlenght; i += smoothlenght)
                    {
                        for (double j = z - smoothlenght; j < z + smoothlenght; j += smoothlenght)
                        {
                            if (!double.IsNaN(Modules[0].GetValue(i, 0, j)))
                            {
                                if (Modules[0].GetValue(i, 0, j) >= MinMaskValue && Modules[0].GetValue(i, 0, j) <= MaxMaskValue)
                                {
                                    sum++;
                                }
                                else
                                {
                                    sum--;
                                }
                                divider++;
                            }
                        }
                    }
                    if (double.IsNaN(sum / divider))
                        return -1;
                    else
                        return sum / divider;
                }


                if (Modules[0].GetValue(x, y, z) >= MinMaskValue && Modules[0].GetValue(x, y, z) <= MaxMaskValue)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}