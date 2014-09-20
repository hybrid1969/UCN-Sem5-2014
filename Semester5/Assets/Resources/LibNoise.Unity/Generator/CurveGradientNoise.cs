using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibNoise.Unity.Generator
{
    public class CurveGradientNoise : ModuleBase
    {
        private double m_frequency = 1.0;
        private bool Altitude;

        /// <summary>
        /// Gets or sets the frequency of the concentric spheres.
        /// </summary>
        public double Frequency
        {
            get { return this.m_frequency; }
            set { this.m_frequency = value; }
        }

        /// <summary>
        /// Initializes a new instance of Spheres.
        /// </summary>
        public CurveGradientNoise()
            : base(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of Spheres.
        /// </summary>
        /// <param name="frequency">The frequency of the concentric spheres.</param>
        public CurveGradientNoise(double frequency, bool altitude = true)
            : base(0)
        {
            this.Frequency = frequency;
            Altitude = altitude;
        }

        public override double GetValue(double x, double y, double z)
        {
            if (Altitude)
            {
                x *= this.m_frequency;
                double dfc = Math.Sqrt(x * x);
                double dfss = dfc - Math.Cos(dfc);
                double dfls = 1.0 - dfss;
                double nd = Math.Min(dfss, dfls);
                return 1.0 - (nd * 4.0);
            }
            else
            {
                z *= this.m_frequency;
                double dfc = Math.Sqrt(z * z);
                double dfss = dfc - Math.Floor(dfc);
                double dfls = 1.0 - dfss;
                double nd = Math.Min(dfss, dfls);
                return 1.0 - (nd * 4.0);
            }
        }
    }
}