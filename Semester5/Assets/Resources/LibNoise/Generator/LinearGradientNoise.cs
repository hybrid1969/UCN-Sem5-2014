using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibNoise.Generator
{
    public class LinearGradientNoise : ModuleBase
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
        public LinearGradientNoise()
            : base(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of Spheres.
        /// </summary>
        /// <param name="frequency">The frequency of the concentric spheres.</param>
        public LinearGradientNoise(double frequency, bool altitude = true)
            : base(0)
        {
            this.Frequency = frequency;
            Altitude = altitude;
        }
        //TODO: Rewrite with gradient increasement, including resolution (Make the frequency together with resolution control the gradient)

        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public override double GetValue(double x, double y, double z)
        {
            if (Altitude)
            {
                z *= this.m_frequency;
                double dfc = Math.Sqrt(z * z);
                double dfss = dfc - Math.Floor(dfc);
                double dfls = 1.0 - dfss;
                double nd = Math.Min(dfss, dfls);
                return 1.0 - (nd * 4.0);
            }
            else
            {
                x *= this.m_frequency;
                double dfc = Math.Sqrt(x * x);
                double dfss = dfc - Math.Floor(dfc);
                double dfls = 1.0 - dfss;
                double nd = Math.Min(dfss, dfls);
                return 1.0 - (nd * 4.0);
            }
        }
    }
}