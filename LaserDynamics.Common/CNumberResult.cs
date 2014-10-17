using Jenyay.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserDynamics.Common
{
    public class CNumberResult
    {
        public double[] X { get; set; }
        public Complex[] Y { get; set; }

        public static explicit operator PlotResult(CNumberResult res)
        {
            var n = new double[res.Y.Length];
            for(int i=0;i<res.Y.Length;i++)
            {
                n[i] = res.Y[i].Re;
            }
            return new PlotResult { X = res.X, Y = n };
        }
        public static explicit operator CNumberResult(PlotResult res)
        {
            var n = new Complex[res.Y.Length];
            for (int i = 0; i < res.Y.Length; i++)
            {
                n[i] = new Complex(res.Y[i]);
            }
            return new CNumberResult { X = res.X, Y = n };
        }
    }
}
