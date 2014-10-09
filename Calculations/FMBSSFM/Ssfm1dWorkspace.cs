using Jenyay.Mathematics;
using LaserDynamics.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserDynamics.Calculations.FullMaxvellBlockSsfm
{
    [Serializable]
    public class Ssfm1dWorkspace : ICalculationWorkspace
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

        double[] R;
        double[] Sigma;
        double[] K;

        double exp1;
        Complex exp2;
        Complex[] exp3;
        Complex[] exp4;

        Complex[] E;
        Complex[] P;
        Double[] D;

        Complex[] Efl;
        Complex[] Pfl;
        double[] Dfl;

        Complex[] Ef;
        Complex[] Pf;

        Complex[] exp5;
        double[] exp6;

        Complex[] Ej;
        Complex[] Pj;
        double[] Dj;

        Complex[] Ec;
        Complex[] Pc;
        double[] Dc;

        const int Nmax = 100;
        const double eps = 1e-10;
        int jj = 0;

        public Stopwatch ffttimer = new Stopwatch();
        public Stopwatch normtimer = new Stopwatch();

        #endregion
        #region Private Methods
        public void Set(Complex[] from, Complex[] to)
        {
            for (int i = 0; i < from.Length; i++)
            {
                to[i] = from[i];
            }
        }
        public void Set(double[] from, double[] to)
        {
            for (int i = 0; i < from.Length; i++)
            {
                to[i] = from[i];
            }
        }

        public void SetSpaceFrequency()
        {
            K = new double[Nx];
            for (int i = 0; i < Nx; i++)
            {
                K[i] = (i - Nx / 2) * 2 * Math.PI / L;
                K[i] = K[i] * K[i];
            }
            Fourier.fftShift(K);
        }

        public void SetPumping(double X)
        {
            R = new double[Nx];
            for (int i = 0; i < Nx; i++)
            {
                R[i] = X;
            }
        }
        public void SetLosses(double X)
        {
            Sigma = new double[Nx];
            for (int i = 0; i < Nx; i++)
            {
                Sigma[i] = X;
            }
        }
        public void SetInitials()
        {
            for (int i = 0; i < Nx; i++)
            {
                E[i] = new Complex(Math.Sqrt(R[i] - 1 - Math.Pow(delta / (1 + Sigma[i]), 2)) * R[i] / r);
                P[i] = new Complex(E[i].Re, -E[i].Re * delta / (1 + Sigma[i]));
                D[i] = 1 + Math.Pow(delta / (1 + Sigma[i]), 2);
            }
            E[0] = E[0] + 1e-10;
        }
        public void SetExtraConstants()
        {
            exp1 = Math.Exp(-gamma * dt / 2);
            exp2 = Complex.Exp(new Complex(-dt / 2, -delta * dt / 2));
            for (int i = 0; i < Nx; i++)
            {
                exp3[i] = Complex.Exp(new Complex(-Sigma[i] * dt / 2, -a * K[i] * dt / 2));
                exp4[i] = Sigma[i] / new Complex(Sigma[i] - 1.0, a * K[i] - delta);
            }
        }
        public double NormsComparison(Complex[] A1, Complex[] A2)
        {
            var dA = new double[Nx];
            var A = new double[Nx];
            for (int i = 0; i < Nx; i++)
            {
                dA[i] = (A1[i] - A2[i]).Abs;
                A[i] = A2[i].Abs;
            }
            double res = dA.Max() / A.Max();
            return res;
        }
        public double NormsComparison(double[] A1, double[] A2)
        {
            var dA = new double[Nx];
            var A = new double[Nx];
            for (int i = 0; i < Nx; i++)
            {
                dA[i] = Math.Abs(A1[i] - A2[i]);
                A[i] = Math.Abs(A2[i]);
            }
            double res = dA.Max() / A.Max();
            return res;
        }
        #endregion

        #region Public Methods
        public void SetParameters()
        {
            exp3 = new Complex[Nx];
            exp4 = new Complex[Nx];

            E = new Complex[Nx];
            P = new Complex[Nx];
            D = new double[Nx];

            Efl = new Complex[Nx];
            Pfl = new Complex[Nx];
            Dfl = new double[Nx];

            Ef = new Complex[Nx];
            Pf = new Complex[Nx];

            exp5 = new Complex[Nx];
            exp6 = new double[Nx];

            Ej = new Complex[Nx];
            Pj = new Complex[Nx];
            Dj = new double[Nx];

            Ec = new Complex[Nx];
            Pc = new Complex[Nx];
            Dc = new double[Nx];

            SetPumping(r);
            SetLosses(sigma);
            SetSpaceFrequency();
            SetInitials();
            SetExtraConstants();
        }
        public void DoIteration()
        {
            ffttimer.Start();
            Ef = Fourier.FFT(E);
            Pf = Fourier.FFT(P);
            ffttimer.Stop();

            for (int i = 0; i < Nx; i++)
            {
                Dfl[i] = exp1 * D[i];
                Pfl[i] = exp2 * P[i];
                Pf[i] = Pf[i] * exp4[i];
                Efl[i] = exp2 * Pf[i] + exp3[i] * (Ef[i] - Pf[i]);
            }
            ffttimer.Start();
            Efl = Fourier.IFFT(Efl);
            ffttimer.Stop();

            // Start iteration process
            Set(E, Ec);
            Set(P, Pc);
            Set(D, Dc);
            Set(E, Ej);
            ffttimer.Start();
            Ef = Fourier.FFT(Efl);
            ffttimer.Stop();

            for (int i = 0; i < Nx; i++)
            {
                exp5[i] = D[i] * E[i];
                exp6[i] = 2 * R[i] - 0.5 * (Ec[i].Re * Pc[i].Re + Pc[i].Im * Ec[i].Im);
            }
            //jj = 0;
            for (int j = 0; j < Nmax; j++)
            {
                //jj++;
                for (int i = 0; i < Nx; i++)
                {
                    Pj[i] = Pfl[i] + (exp5[i] + Dc[i] * Ec[i]) / 2 * dt;
                    Dj[i] = Dfl[i] + gamma * dt * (exp6[i] - 0.5 * (Ec[i].Re * Pc[i].Re + Pc[i].Im * Ec[i].Im)) / 2;
                }
                ffttimer.Start();
                Pf = Fourier.FFT(Pj);
                ffttimer.Stop();

                for (int i = 0; i < Nx; i++)
                {
                    Pf[i] = Pf[i] * exp4[i];
                    Dj[i] = Dj[i] * exp1;
                    Pj[i] = Pj[i] * exp2;
                    Ej[i] = exp2 * Pf[i] + exp3[i] * (Ef[i] - Pf[i]);
                }
                ffttimer.Start();
                Ej = Fourier.IFFT(Ej);
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
                    if (jj != j)
                    {
                        Console.WriteLine(j);
                        jj = j;
                    }
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
        }
        #endregion
    }
}
