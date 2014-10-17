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

        #region Private Properties
        Complex[] LocalField { get; set; }
        double[] LocalIntensity { get; set; }
        double[] LocalInversion { get; set; }
        double[] LocalPhase { get; set; }
        Complex[] LocalPolarization { get; set; }
        #endregion

        #region Private Model Methods
        void Set(Complex[] from, Complex[] to)
        {
            for (int i = 0; i < from.Length; i++)
            {
                to[i] = from[i];
            }
        }
        void Set(double[] from, double[] to)
        {
            for (int i = 0; i < from.Length; i++)
            {
                to[i] = from[i];
            }
        }
        void SetSpaceFrequency()
        {
            K = new double[Nx];
            for (int i = 0; i < Nx; i++)
            {
                K[i] = (i - Nx / 2) * 2 * Math.PI / L;
                K[i] = K[i] * K[i];
            }
            Fourier.fftShift(K);
        }
        void SetPumping(double X)
        {
            R = new double[Nx];
            for (int i = 0; i < Nx; i++)
            {
                R[i] = X;
            }
        }
        void SetLosses(double X)
        {
            Sigma = new double[Nx];
            for (int i = 0; i < Nx; i++)
            {
                Sigma[i] = X;
            }
        }
        void SetInitials()
        {
            for (int i = 0; i < Nx; i++)
            {
                E[i] = new Complex(Math.Sqrt(R[i] - 1 - Math.Pow(delta / (1 + Sigma[i]), 2)) * R[i] / r);
                P[i] = new Complex(E[i].Re, -E[i].Re * delta / (1 + Sigma[i]));
                D[i] = 1 + Math.Pow(delta / (1 + Sigma[i]), 2);
            }
            E[0] = E[0] + 1e-10;
        }
        void SetExtraConstants()
        {
            exp1 = Math.Exp(-gamma * dt / 2);
            exp2 = Complex.Exp(new Complex(-dt / 2, -delta * dt / 2));
            for (int i = 0; i < Nx; i++)
            {
                exp3[i] = Complex.Exp(new Complex(-Sigma[i] * dt / 2, -a * K[i] * dt / 2));
                exp4[i] = Sigma[i] / new Complex(Sigma[i] - 1.0, a * K[i] - delta);
            }
        }
        double NormsComparison(Complex[] A1, Complex[] A2)
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
        double NormsComparison(double[] A1, double[] A2)
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
        double[] GetPow2Abs(Complex[] A)
        {
            var R = new double[Nx];
            for (int i = 0; i < Nx; i++)
                R[i] = Math.Pow(A[i].Abs, 2);
            return R;
        }
        double[] GetPhase(Complex[] A)
        {
            var R = new double[Nx];
            for (int i = 0; i < Nx; i++)
                R[i] = A[i].Arg;
            return R;
        }
        double[] GetSpectrum(Complex[] A)
        {
            var R = new double[Nx];
            var F = Fourier.FFT(A);
            for (int i = 0; i < Nx; i++)
                R[i] = Math.Pow(F[i].Abs, 2);
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
            var view = (View as Ssfm1dView);

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

            LocalField = view.OutLocalField ? new Complex[Nt] : null;
            LocalIntensity = view.OutLocalIntensity ? new double[Nt] : null;
            LocalInversion = view.OutLocalInversion ? new double[Nt] : null;
            LocalPhase = view.OutLocalPhase ? new double[Nt] : null;
            LocalPolarization = view.OutLocalPolarization ? new Complex[Nt] : null;
        }
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
        public void DoIteration(long numIteration)
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

            if(LocalField != null)
                LocalField[numIteration] = E[Nx / 2];
            if (LocalIntensity != null) 
                LocalIntensity[numIteration] = Math.Pow(E[Nx / 2].Abs, 2);
            if (LocalInversion != null) 
                LocalInversion[numIteration] = D[Nx / 2];
            if (LocalPhase != null) 
                LocalPhase[numIteration] = E[Nx / 2].Arg;
            if (LocalPolarization != null) 
                LocalPolarization[numIteration] = P[Nx / 2];
        }
        public object GetResult(ICalculationView View)
        {
            var view = View as Ssfm1dView;
            //List<object> list = new List<object>();
            //if (view.OutFieldDistibution)
            //    list.Add(E);
            //if (view.OutIntensityDistibution)
            //    list.Add(GetPow2Abs(E));
            //if (view.OutInversionDistibution)
            //    list.Add(D);
            //if (view.OutLocalField)
            //    list.Add(LocalField);
            //if (view.OutLocalIntensity)
            //    list.Add(LocalIntensity);
            //if (view.OutLocalInversion)
            //    list.Add(LocalInversion);
            //if (view.OutLocalPhase)
            //    list.Add(LocalPhase);
            //if (view.OutLocalPolarization)
            //    list.Add(LocalPolarization);
            //if (view.OutPhaseDistibution)
            //    list.Add(GetPhase(E));
            //if (view.OutPolarizationDistibution)
            //    list.Add(P);
            //if (view.OutSpectrumDistibution)
            //    list.Add(GetSpectrum(E));
            //return list.ToArray();
            var x = new double[Nx];
            for (int i = 0; i < Nx; i++)
                x[i] = i + 1;
            var nt = Enumerable.Range(1, Nt).ToArray();
            var t = new double[Nt];
            for (int i = 0; i < Nt; i++)
                t[i] = dt*nt[i];
            return new
            {
                OutFieldDistibution = view.OutFieldDistibution ? new CNumberResult{ X = x, Y = E } : null,
                OutIntensityDistibution = view.OutIntensityDistibution ? new PlotResult{ X = x, Y = GetPow2Abs(E) } : null,
                OutInversionDistibution = view.OutInversionDistibution ? new PlotResult { X = x, Y = D } : null,
                OutLocalField = view.OutLocalField ? new CNumberResult { X = t, Y = LocalField } : null,
                OutLocalIntensity = view.OutLocalIntensity ? new PlotResult { X = t, Y = LocalIntensity } : null,
                OutLocalInversion = view.OutLocalInversion ? new PlotResult { X = t, Y = LocalInversion } : null,
                OutLocalPhase = view.OutLocalPhase ? new PlotResult { X = t, Y = LocalPhase } : null,
                OutLocalPolarization = view.OutLocalPolarization ? new CNumberResult { X = t, Y = LocalPolarization } : null,
                OutPhaseDistibution = view.OutPhaseDistibution ? new PlotResult { X = x, Y = GetPhase(E) } : null,
                OutPolarizationDistibution = view.OutPolarizationDistibution ? new CNumberResult { X = x, Y = P } : null,
                OutSpectrumDistibution = view.OutSpectrumDistibution ? new PlotResult { X = x, Y = GetSpectrum(E) } : null
            } as object;
        }
        #endregion
    }
}
