using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using LaserDynamics.Common;
using Jenyay.Mathematics;

namespace LaserDynamics.Calculations.FMBSsfmLaserModel
{
    public class Ssfm2dWorkspace : ICalculationWorkspace
    {
        #region Model Fields
        // Model Parameters
        public double ElRelax;
        public double PolRelax;
        public double InvRelax;
        public double gamma;
        public double sigma;
        public double delta;
        public double r;
        public double a;

        // Scheme Parameters
        public int Nx;
        public double dt;
        public double L;
        public int Nt;

        double[,] R;
        double[,] Sigma;
        double[,] K;

        double exp1;
        Complex exp2;
        Complex[,] exp3;
        Complex[,] exp4;

        Complex[,] E;
        Complex[,] P;
        Double[,] D;

        Complex[,] Efl;
        Complex[,] Pfl;
        double[,] Dfl;

        Complex[,] Ef;
        Complex[,] Pf;

        Complex[,] exp5;
        double[,] exp6;

        Complex[,] Ej;
        Complex[,] Pj;
        double[,] Dj;

        Complex[,] Ec;
        Complex[,] Pc;
        double[,] Dc;

        const int Nmax = 100;
        const double eps = 1e-10;
        int jj = 0;

        public Stopwatch ffttimer = new Stopwatch();
        public Stopwatch normtimer = new Stopwatch();
        #endregion

        #region Private Properties
        Complex[] LocalField { get; set; }
        double[] LocalIntensity { get; set; }
        double[] LocalInversion { get; set; }
        double[] LocalPhase { get; set; }
        Complex[] LocalPolarization { get; set; }

        string InitialCondition { get; set; }
        #endregion

        #region Private Model Methods
        void Set(Complex[,] from, Complex[,] to)
        {
            for (int i = 0; i < from.GetLength(0); i++)
                for (int j = 0; j < from.GetLength(1); j++)
                {
                    to[i,j] = from[i,j];
                }
        }
        void Set(double[,] from, double[,] to)
        {
            for (int i = 0; i < from.GetLength(0); i++)
                for (int j = 0; j < from.GetLength(1); j++)
                {
                    to[i, j] = from[i, j];
                }
        }
        void SetSpaceFrequency()
        {
            var K1 = new double[Nx];
            for (int i = 0; i < Nx; i++)
            {
                K1[i] = (i - Nx / 2) * 2 * Math.PI / L;
                K1[i] = K1[i] * K1[i];
            }
            K1 = Fourier.fftShift(K1);
            K = new double[Nx,Nx];
            for (int i = 0; i < Nx; i++)
                for (int j = 0; j < Nx; j++)
                {
                    K[i,j] = K1[i] + K1[j];
                }
            
        }
        void SetPumping(double X)
        {
            R = new double[Nx,Nx];
            for (int i = 0; i < Nx; i++)
                for (int j = 0; j < Nx; j++)
                {
                    R[i,j] = X;
                }
        }
        void SetLosses(double X)
        {
            Sigma = new double[Nx,Nx];
            for (int i = 0; i < Nx; i++)
                for (int j = 0; j < Nx; j++)
                {
                    Sigma[i,j] = X;
                }
        }
        void SetInitials()
        {
            switch (InitialCondition)
            {
                case "Around homogeneous solution":
                    for (int i = 0; i < Nx; i++)
                        for (int j = 0; j < Nx; j++)
                        {
                            E[i, j] = new Complex(Math.Sqrt(R[i, j] - 1 - Math.Pow(delta / (1 + Sigma[i, j]), 2)) * R[i, j] / r);
                            P[i, j] = new Complex(E[i, j].Re, -E[i, j].Re * delta / (1 + Sigma[i, j]));
                            D[i, j] = 1 + Math.Pow(delta / (1 + Sigma[i, j]), 2);
                        }
                    E[0, 0] = E[0, 0] + 1e-10;
                    break;  
                case "Around trivial solution":
                    for (int i = 0; i < Nx; i++)
                        for (int j = 0; j < Nx; j++)
                        {
                            E[i,j] = new Complex(0);
                            P[i,j] = new Complex(0);
                            D[i,j] = 0;
                        }
                    E[0,0] = E[0,0] + 1e-10;
                    break;
                case "Small-amplitude white Noise":
                    var rnd = new Random();
                    for (int i = 0; i < Nx; i++)
                        for (int j = 0; j < Nx; j++)
                        {
                            E[i, j] = new Complex(rnd.Next(-100, 100) / 100);
                            P[i, j] = new Complex(rnd.Next(-100, 100) / 100);
                            D[i, j] = rnd.Next(-100, 100) / 100;
                        }
                    break;
                case "Around square vortex lattice":
                    var Kt = Math.Sqrt(delta / a);
                    var Amplitude = Math.Sqrt((r - 1) / 5);
                    var pow2Amplitude = Math.Pow(Amplitude, 2);
                    for (int i = 0; i < Nx; i++)
                        for (int j = 0; j < Nx; j++)
                        {
                            E[i, j] = Amplitude * Complex.Exp(new Complex(0, -Kt * i * L / Nx)) + Amplitude * Complex.Exp(new Complex(0, Kt * i * L / Nx))
                                + Amplitude*Complex.Exp(new Complex(0, -Kt * j * L / Nx)) + Amplitude*Complex.Exp(new Complex(0, Kt * j * L / Nx + Math.PI));
                            P[i, j] = E[i, j];
                            D[i, j] = (r / 5 + 4 / 5d) - pow2Amplitude * Complex.Exp(new Complex(0, -2 * Kt * i * L / Nx)).Re - pow2Amplitude * Complex.Exp(new Complex(0, 2 * Kt * i * L / Nx)).Re
                                + pow2Amplitude * Complex.Exp(new Complex(0, -2 * Kt * j * L / Nx)).Re + pow2Amplitude * Complex.Exp(new Complex(0, 2 * Kt * j * L / Nx)).Re;
                        }
                    break;
            }
        }
        void SetExtraConstants()
        {
            exp1 = Math.Exp(-gamma * dt / 2);
            exp2 = Complex.Exp(new Complex(-dt / 2, -delta * dt / 2));
            for (int i = 0; i < Nx; i++)
                for (int j = 0; j < Nx; j++)
                {
                    exp3[i,j] = Complex.Exp(new Complex(-Sigma[i,j] * dt / 2, -a * K[i,j] * dt / 2));
                    exp4[i,j] = Sigma[i,j] / new Complex(Sigma[i,j] - 1.0, a * K[i,j] - delta);
                }
        }
        double NormInf(Complex[,] A)
        {
            var B = Transponse(A);
            var sums = new double[B.GetLength(0)];
            for (int i = 0; i < Nx; i++)
                for (int j = 0; j < Nx; j++)
                {
                    sums[i] += B[i, j].Abs;
                }
            return sums.Max();
        }
        double NormInf(double[,] A)
        {
            var B = Transponse(A);
            var sums = new double[B.GetLength(0)];
            for (int i = 0; i < Nx; i++)
                for (int j = 0; j < Nx; j++)
                {
                    sums[i] += Math.Abs(B[i, j]);
                }
            return sums.Max();
        }
        Complex[,] Transponse(Complex[,] A)
        {
            var B = new Complex[A.GetLength(1), A.GetLength(0)];
            for (int i = 0; i < Nx; i++)
                for (int j = 0; j < Nx; j++)
                {
                    B[j,i]=A[i,j];
                }
            return B;
        }
        double[,] Transponse(double[,] A)
        {
            var B = new double[A.GetLength(1), A.GetLength(0)];
            for (int i = 0; i < Nx; i++)
                for (int j = 0; j < Nx; j++)
                {
                    B[j, i] = A[i, j];
                }
            return B;
        }
        double NormsComparison(Complex[,] A1, Complex[,] A2)
        {
            var C = new Complex[A1.GetLength(0), A1.GetLength(1)];
            for (int i = 0; i < Nx; i++)
                for (int j = 0; j < Nx; j++)
                {
                    C[i,j] = A1[i, j] - A2[i, j];
                }
            return NormInf(C) / NormInf(A2);
        }
        double NormsComparison(double[,] A1, double[,] A2)
        {
            var C = new double[A1.GetLength(0), A1.GetLength(1)];
            for (int i = 0; i < Nx; i++)
                for (int j = 0; j < Nx; j++)
                {
                    C[i, j] = A1[i, j] - A2[i, j];
                }
            return NormInf(C) / NormInf(A2);
        }
        double[,] GetPow2Abs(Complex[,] A)
        {
            var R = new double[Nx, Nx];
            for (int i = 0; i < Nx; i++)
                for (int j = 0; j < Nx; j++)
                    R[i, j] = Math.Pow(A[i, j].Abs, 2);
            return R;
        }
        double[,] GetPhase(Complex[,] A)
        {
            var R = new double[Nx, Nx];
            for (int i = 0; i < Nx; i++)
                for (int j = 0; j < Nx; j++)
                    R[i, j] = A[i, j].Arg;
            return R;
        }
        double[,] GetSpectrum(Complex[,] A)
        {
            var R = new double[Nx, Nx];
            var F = Fourier.FFT2(A, Nx);
            for (int i = 0; i < Nx; i++)
                for (int j = 0; j < Nx; j++)
                    R[i, j] = Math.Pow(F[i, j].Abs, 2);
            return R;
        }
        #endregion

        #region Public Methods
        public bool IsValid()
        {
            if (ElRelax == 0 || PolRelax == 0 || InvRelax == 0 || delta == 0 || r == 0 || a == 0 || Nx == 0 || L == 0 || Nt == 0 || dt == 0)
                return false;
            return true;
        }
        public void Initialize(ICalculationView View)
        {
            var view = (View as Ssfm2dView);

            Nx = view.NumNodes;
            L = view.ApertureSize;
            dt = view.TimeStep;
            Nt = view.TotalTime;

            ElRelax = view.ElectricRelax;
            PolRelax = view.PolarizationRelax;
            InvRelax = view.InversionRelax;
            gamma = view.InversionRelax / view.PolarizationRelax;
            sigma = view.ElectricRelax / view.PolarizationRelax;
            delta = view.Detuning;
            r = view.Pumping;
            a = view.Diffraction;

            InitialCondition = view.InitialCondition;

            LocalField = view.OutLocalField ? new Complex[Nt] : null;
            LocalIntensity = view.OutLocalIntensity ? new double[Nt] : null;
            LocalInversion = view.OutLocalInversion ? new double[Nt] : null;
            LocalPhase = view.OutLocalPhase ? new double[Nt] : null;
            LocalPolarization = view.OutLocalPolarization ? new Complex[Nt] : null;
        }
        public void SetParameters()
        {
            exp3 = new Complex[Nx,Nx];
            exp4 = new Complex[Nx, Nx];

            E = new Complex[Nx, Nx];
            P = new Complex[Nx, Nx];
            D = new double[Nx, Nx];

            Efl = new Complex[Nx, Nx];
            Pfl = new Complex[Nx, Nx];
            Dfl = new double[Nx, Nx];

            Ef = new Complex[Nx, Nx];
            Pf = new Complex[Nx, Nx];

            exp5 = new Complex[Nx, Nx];
            exp6 = new double[Nx, Nx];

            Ej = new Complex[Nx, Nx];
            Pj = new Complex[Nx, Nx];
            Dj = new double[Nx, Nx];

            Ec = new Complex[Nx, Nx];
            Pc = new Complex[Nx, Nx];
            Dc = new double[Nx, Nx];

            SetPumping(r);
            SetLosses(sigma);
            SetSpaceFrequency();
            SetInitials();
            SetExtraConstants();
        }
        public void DoIteration(long numIteration)
        {
            ffttimer.Start();
            Ef = Fourier.FFT2(E,Nx);
            Pf = Fourier.FFT2(P,Nx);
            ffttimer.Stop();

            for (int i = 0; i < Nx; i++)
                for (int j = 0; j < Nx; j++)
                {
                    Dfl[i, j] = exp1 * D[i, j];
                    Pfl[i, j] = exp2 * P[i, j];
                    Pf[i, j] = Pf[i, j] * exp4[i, j];
                    Efl[i, j] = exp2 * Pf[i, j] + exp3[i, j] * (Ef[i, j] - Pf[i, j]);
                }
            ffttimer.Start();
            Efl = Fourier.IFFT2(Efl,Nx);
            ffttimer.Stop();

            // Start iteration process
            Set(E, Ec);
            Set(P, Pc);
            Set(D, Dc);
            Set(E, Ej);
            ffttimer.Start();
            Ef = Fourier.FFT2(Efl,Nx);
            ffttimer.Stop();

            for (int i = 0; i < Nx; i++)
                for (int j = 0; j < Nx; j++)
                {
                    exp5[i, j] = D[i, j] * E[i, j];
                    exp6[i, j] = 2 * R[i, j] - 0.5 * (Ec[i, j].Re * Pc[i, j].Re + Pc[i, j].Im * Ec[i, j].Im);
                }
            //jj = 0;
            for (int p = 0; p < Nmax; p++)
            {
                //jj++;
                for (int i = 0; i < Nx; i++)
                    for (int j = 0; j < Nx; j++)
                    {
                        Pj[i, j] = Pfl[i, j] + (exp5[i, j] + Dc[i, j] * Ec[i, j]) / 2 * dt;
                        Dj[i, j] = Dfl[i, j] + gamma * dt * (exp6[i, j] - 0.5 * (Ec[i, j].Re * Pc[i, j].Re + Pc[i, j].Im * Ec[i, j].Im)) / 2;
                    }
                ffttimer.Start();
                Pf = Fourier.FFT2(Pj,Nx);
                ffttimer.Stop();

                for (int i = 0; i < Nx; i++)
                    for (int j = 0; j < Nx; j++)
                    {
                        Pf[i, j] = Pf[i, j] * exp4[i, j];
                        Dj[i, j] = Dj[i, j] * exp1;
                        Pj[i, j] = Pj[i, j] * exp2;
                        Ej[i, j] = exp2 * Pf[i, j] + exp3[i, j] * (Ef[i, j] - Pf[i, j]);
                    }
                ffttimer.Start();
                Ej = Fourier.IFFT2(Ej,Nx);
                ffttimer.Stop();

                normtimer.Start();
                //double normE = NormsComparison(Ej, Ec);
                //double normP = NormsComparison(Pj, Pc);
                //double normD = NormsComparison(Dj, Dc);
                if (NormsComparison(Ej, Ec) < eps && NormsComparison(Pj, Pc) < eps && NormsComparison(Dj, Dc) < eps)
                {
                    Ec = Ej;
                    Pc = Pj;
                    Dc = Dj;
                    normtimer.Stop();
                    //if (jj != p)
                    //{
                    //    Console.WriteLine(p);
                    //    jj = p;
                    //}
                    break;
                }
                else
                {
                    Set(Ej, Ec);
                    Set(Pj, Pc);
                    Set(Dj, Dc);
                }
                normtimer.Stop();
            }
            //Console.WriteLine(jj);
            E = Ec;
            P = Pc;
            D = Dc;

            if (LocalField != null)
                LocalField[numIteration] = E[Nx / 2, Nx / 2];
            if (LocalIntensity != null)
                LocalIntensity[numIteration] = Math.Pow(E[Nx / 2, Nx / 2].Abs, 2);
            if (LocalInversion != null)
                LocalInversion[numIteration] = D[Nx / 2, Nx / 2];
            if (LocalPhase != null)
                LocalPhase[numIteration] = E[Nx / 2, Nx / 2].Arg;
            if (LocalPolarization != null)
                LocalPolarization[numIteration] = P[Nx / 2, Nx / 2];
        }
        public object GetResult(ICalculationView View)
        {
            var view = View as Ssfm2dView;
            var x = new double[Nx];
            var nt = Enumerable.Range(1, Nt).ToArray();
            var t = new double[Nt];
            for (int i = 0; i < Nt; i++)
                t[i] = dt * nt[i];
            return new
            {
                OutFieldDistibution = view.OutFieldDistibution ? new ComplexImageResult { Z = E } : null,
                OutIntensityDistibution = view.OutIntensityDistibution ? new ImageResult { Z = GetPow2Abs(E) } : null,
                OutInversionDistibution = view.OutInversionDistibution ? new ImageResult { Z = D } : null,
                OutLocalField = view.OutLocalField ? new ComplexVectorResult { X = t, Y = LocalField } : null,
                OutLocalIntensity = view.OutLocalIntensity ? new VectorResult { X = t, Y = LocalIntensity } : null,
                OutLocalInversion = view.OutLocalInversion ? new VectorResult { X = t, Y = LocalInversion } : null,
                OutLocalPhase = view.OutLocalPhase ? new VectorResult { X = t, Y = LocalPhase } : null,
                OutLocalPolarization = view.OutLocalPolarization ? new ComplexVectorResult { X = t, Y = LocalPolarization } : null,
                OutPhaseDistibution = view.OutPhaseDistibution ? new ImageResult { Z = GetPhase(E) } : null,
                OutPolarizationDistibution = view.OutPolarizationDistibution ? new ComplexImageResult { Z = P } : null,
                OutSpectrumDistibution = view.OutSpectrumDistibution ? new ImageResult { Z = GetSpectrum(E) } : null
            } as object;
        }
        #endregion
    }
}
