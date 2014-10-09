using System;
using System.Linq;
using Jenyay.Mathematics;
using System.Diagnostics;
using LaserDynamics.Common;

namespace LaserDynamics.Calculations.FullMaxvellBlockSsfm
{
    public class Ssfm1dCalculation : ICalculation 
    {
        public Ssfm1dCalculation()
        {
            View = new Ssfm1dView();
            Workspace = new Ssfm1dWorkspace();
        }

        #region Public Methods
        public string Name { get; set; }
        public ICalculationView View { get; set; }
        public ICalculationWorkspace Workspace { get; set; }
        #endregion

        public void Calculate()
        {
            try
            {
                var workspace = Workspace as Ssfm1dWorkspace;

                Status = CalculationStatus.Running;
                ErrorMessage = null;
                ReportMessage = "0";
                sumtimer.Reset();
                workspace.ffttimer.Reset();
                workspace.normtimer.Reset();
                if (OnCalculationStart != null)
                    OnCalculationStart(this, new EventArgs());
                sumtimer.Start();
                SetParametersFromViewToWorkSpace();
                for (int q = 0; q < workspace.Nt; q++)
                {
                    if (Status == CalculationStatus.Stopped)
                        break;

                    workspace.DoIteration();

                    var newReport = ((int)Math.Floor((double)q / workspace.Nt * 100)).ToString();
                    if (newReport != ReportMessage)    //  && newReport%5 == 0
                    {
                        if (OnCalculationReport != null)
                        {
                            OnCalculationReport(this, new EventArgs());
                        }
                        ReportMessage = newReport;
                    }
                }
                sumtimer.Stop();
                if (OnCalculationFinish != null)
                {
                    OnCalculationFinish(this, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;     // Как адекватно выводить здесь сообщение об ошибке?
                if (OnCalculationError != null)
                {
                    OnCalculationError(this, new EventArgs());
                }
            }
        }
        public void SetParametersFromViewToWorkSpace()
        {
            var view = (View as Ssfm1dView);
            var workspace = Workspace as Ssfm1dWorkspace;

            workspace.Nx = view.NumNodes;
            workspace.L = view.ApertureSize;
            workspace.dt = view.TimeStep;
            workspace.Nt = view.TotalTime;

            workspace.ElRelax = view.ElectricRelax;
            workspace.PolRelax = view.PolarizationRelax;
            workspace.InvRelax = view.InversionRelax;
            workspace.gamma = view.InversionRelax / view.PolarizationRelax;
            workspace.sigma = view.ElectricRelax / view.PolarizationRelax;
            workspace.delta = view.Detuning;
            workspace.r = view.Pumping;
            workspace.a = view.Diffraction;

            workspace.SetParameters();

        }
        public long[] GetStats()
        {
            var workspace = Workspace as Ssfm1dWorkspace;
            return new long[3] { sumtimer.ElapsedMilliseconds, workspace.ffttimer.ElapsedMilliseconds, workspace.normtimer.ElapsedMilliseconds };
        }
        public void GetResults()
        { }
        string ErrorMessage { get; set; }
        string ReportMessage { get; set; }
        public CalculationStatus Status { get; set; }
        public void OnCalculationStopped()
        {
            Status = CalculationStatus.Stopped;
        }
        #region Public Events
        public event EventHandler OnCalculationFinish;
        public event EventHandler OnCalculationStart;
        public event EventHandler OnCalculationError;
        public event EventHandler OnCalculationReport;
        #endregion

        public Stopwatch sumtimer = new Stopwatch();
        
    }
}
