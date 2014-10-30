using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibNoise.Generator
{
    public class CircleMask : ModuleBase
    {
        private double m_frequency = 1.0;

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
        public CircleMask()
            : base(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of Spheres.
        /// </summary>
        /// <param name="frequency">The frequency of the concentric spheres.</param>
        public CircleMask(double frequency)
            : base(0)
        {
            this.Frequency = frequency;
        }
        //TODO: Rewrite with gradient increasement, including resolution (Make the frequency together with resolution control the gradient)

        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public override double GetValue(double x, double y, double z)
        {
            z *= this.m_frequency;
            x *= this.m_frequency;
            double dfc = z * z + x * x;
            double dfss = dfc - Math.Floor(dfc);
            double dfls = 1.0 - dfss;
            double nd = Math.Min(dfss, dfls);
            return 1.0 - (dfc * 2.0);
        }
    }
}