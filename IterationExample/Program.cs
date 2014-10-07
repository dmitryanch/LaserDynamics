using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jenyay.Mathematics;
using System.Diagnostics;


namespace IterationExample
{
    class Program
    {
        static int Nx = 256;
        static int Nt = 100;
        static double dt = 0.001;
        static double L = 5;
        static double gamma = 0.01;
        static double sigma = 0.1;
        static double delta = -1;
        static double r = 2;
        static double a = 0.01;
        static double[] R;
        static double[] Sigma;
        static double[] K;

        static Complex[] E = new Complex[Nx];
        static Complex[] P = new Complex[Nx];
        static Double[] D = new double[Nx];

        static double exp1 = Math.Exp(-gamma * dt / 2);
        static Complex exp2 = Complex.Exp(new Complex(-dt / 2, -delta * dt / 2));
        static Complex[] exp3 = new Complex[Nx];
        static Complex[] exp4 = new Complex[Nx];

        static double[] Dfl = new double[Nx];
        static Complex[] Pfl = new Complex[Nx];
        static Complex[] Efl = new Complex[Nx];

        static Complex[] Pf = new Complex[Nx];
        static Complex[] Ef = new Complex[Nx];

        static Complex[] exp5 = new Complex[Nx];
        static double[] exp6 = new double[Nx];

        static Complex[] Pj = new Complex[Nx];
        static Complex[] Ej = new Complex[Nx];
        static double[] Dj = new double[Nx];

        static double[] Dc = new double[Nx];
        static Complex[] Pc = new Complex[Nx];
        static Complex[] Ec = new Complex[Nx];

        static int Nmax = 100;
        static double eps = 1e-10;

        static Stopwatch sumtimer = new Stopwatch();
        static Stopwatch ffttimer = new Stopwatch();
        static Stopwatch normtimer = new Stopwatch();

        static int jj;
        static void Main()
        {
            //var A1 = new double[Nx];
            //var A2 = new double[Nx];
            //for (int i = 0; i < Nx; i++)
            //{
            //    A1[i] = i;
            //    A2[i] = A1[i];
            //}
            //A2[Nx - 1] = 253;
            //double res = NormsComparison(A2, A1);
            int jj = 0;
            SetPumping(r);
            SetLosses(sigma);
            SetSpaceFrequency();
            SetInitials();
            SetExtraConstants();
            for (int i = 0; i < Nx; i++)
                Console.WriteLine(E[i].Abs * E[i].Abs);
            sumtimer.Start();
            for (int q = 0; q < Nt; q++)
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
                Set(D , Dc);
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
                    double normE = NormsComparison(Ej, Ec);
                    double normP = NormsComparison(Pj, Pc) ;
                    double normD  = NormsComparison(Dj, Dc) ;
                    if (normE < eps && normP< eps && normD< eps)
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
            sumtimer.Stop();
            
            Console.WriteLine("Sum time: " + sumtimer.ElapsedMilliseconds / Nt);
            Console.WriteLine("FFT time: " + ffttimer.ElapsedMilliseconds / Nt);
            Console.WriteLine("Norm time: " + normtimer.ElapsedMilliseconds / Nt);
            for (int i = 0; i < Nx; i++)
                Console.WriteLine(E[i].Abs * E[i].Abs);
            Console.ReadKey();
        }

        static void Set(Complex[] from, Complex[] to)
        {
            for(int i=0;i<from.Length;i++)
            {
                to[i]=from[i];
            }
        }
        static void Set(double[] from, double[] to)
        {
            for (int i = 0; i < from.Length; i++)
            {
                to[i] = from[i];
            }
        }

        static void SetSpaceFrequency()
        {
            K = new double[Nx];
            for (int i = 0; i < Nx; i++)
            {
                K[i] = (i - Nx / 2) * 2 * Math.PI / L;
                K[i] = K[i] * K[i];
            }
            Fourier.fftShift(K);
        }

        static void SetPumping(double X)
        {
            R = new double[Nx];
            for (int i = 0; i < Nx; i++)
            {
                R[i] = X;
            }
        }
        static void SetLosses(double X)
        {
            Sigma = new double[Nx];
            for (int i = 0; i < Nx; i++)
            {
                Sigma[i] = X;
            }
        }
        static void SetInitials()
        {
            for (int i = 0; i < Nx; i++)
            {
                E[i] = new Complex(Math.Sqrt(R[i] - 1 - Math.Pow(delta / (1 + Sigma[i]), 2)) * R[i] / r);
                P[i] = new Complex(E[i].Re, -E[i].Re * delta / (1 + Sigma[i]));
                D[i] = 1 + Math.Pow(delta / (1 + Sigma[i]), 2);
            }
            E[0] = E[0] + 1e-10;
        }
        static void SetExtraConstants()
        {
            for (int i = 0; i < Nx; i++)
            {
                exp3[i] = Complex.Exp(new Complex(-Sigma[i] * dt / 2, -a * K[i] * dt / 2));
                exp4[i] = Sigma[i]/new Complex(Sigma[i] - 1.0, a * K[i] - delta);
            }
        }
        static double NormsComparison(Complex[] A1, Complex[] A2)
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
        static double NormsComparison(double[] A1, double[] A2)
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
    }
}
