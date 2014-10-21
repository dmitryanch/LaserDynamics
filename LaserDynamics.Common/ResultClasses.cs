using Jenyay.Mathematics;

namespace LaserDynamics.Common
{
    public class ImageResult
    {
        public double[,] Z { get; set; }
    }
    public class ComplexImageResult
    {
        public Complex[,] Z { get; set; }
        
        public static explicit operator ImageResult(ComplexImageResult res)
        {
            var n = new double[res.Z.GetLength(0), res.Z.GetLength(1)];
            for (int i = 0; i < res.Z.GetLength(0); i++)
                for (int j = 0; j < res.Z.GetLength(1); j++)
                {
                    n[i,j] = res.Z[i,j].Re;
                }
            return new ImageResult { Z = n };
        }
        public static explicit operator ComplexImageResult(ImageResult res)
        {
            var n = new Complex[res.Z.GetLength(0), res.Z.GetLength(1)];
            for (int i = 0; i < res.Z.GetLength(0); i++)
                for (int j = 0; j < res.Z.GetLength(1); j++)
                {
                    n[i,j] = new Complex(res.Z[i,j]);
                }
            return new ComplexImageResult { Z = n };
        }
    }
    public class VectorResult
    {
        public double[] X { get; set; }
        public double[] Y { get; set; }
    }
    public class ComplexVectorResult
    {
        public double[] X { get; set; }
        public Complex[] Y { get; set; }

        public static explicit operator VectorResult(ComplexVectorResult res)
        {
            var n = new double[res.Y.Length];
            for (int i = 0; i < res.Y.Length; i++)
            {
                n[i] = res.Y[i].Re;
            }
            return new VectorResult { X = res.X, Y = n };
        }
        public static explicit operator ComplexVectorResult(VectorResult res)
        {
            var n = new Complex[res.Y.Length];
            for (int i = 0; i < res.Y.Length; i++)
            {
                n[i] = new Complex(res.Y[i]);
            }
            return new ComplexVectorResult { X = res.X, Y = n };
        }
    }
}
