using System;
using System.Linq;
using Jenyay.Mathematics;
using System.Diagnostics;
using LaserDynamics.Common;

namespace LaserDynamics.Calculations.FullMaxvellBlockSsfm
{
    public class Ssfm1dCalculation : ICalculation
    {
        public string CalculationId { get; set; }
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

                workspace.Initialize(View);
                if (!workspace.IsValid())
                {
                    Status = CalculationStatus.ValidationFailed;
                    if (OnCalculationValidationFailed != null)
                        OnCalculationValidationFailed(this, new EventArgs());
                    return;
                }
                workspace.SetParameters();
                Status = CalculationStatus.Running;
                ErrorMessage = null;
                ReportMessage = "0";
                sumtimer.Reset();
                workspace.ffttimer.Reset();
                workspace.normtimer.Reset();
                if (OnCalculationStart != null)
                    OnCalculationStart(this, new EventArgs());
                sumtimer.Start();
                for (int q = 0; q < workspace.Nt; q++)
                {
                    if (Status == CalculationStatus.Stopped)
                        break;

                    workspace.DoIteration(q);

                    var newReport = ((int)Math.Floor((double)q / workspace.Nt * 100)).ToString();
                    if (newReport != ReportMessage)
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
                    if (Status == CalculationStatus.Running)
                        Status = CalculationStatus.Finished;
                    OnCalculationFinish(this, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;     // Как адекватно выводить здесь сообщение об ошибке?
                Status = CalculationStatus.Error;
                if (OnCalculationError != null)
                {
                    OnCalculationError(this, new EventArgs());
                }
            }
        }
        public long[] GetStats()
        {
            var workspace = Workspace as Ssfm1dWorkspace;
            return new long[3] { sumtimer.ElapsedMilliseconds, workspace.ffttimer.ElapsedMilliseconds, workspace.normtimer.ElapsedMilliseconds };
        }
        public object GetResult()
        {
            if (Status != CalculationStatus.Running)
                return null;
            return Workspace.GetResult(View);
        }
        public string ErrorMessage { get; set; }
        public string ReportMessage { get; set; }
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
        public event EventHandler OnCalculationValidationFailed;
        #endregion

        public Stopwatch sumtimer = new Stopwatch();
        public bool IsSaved { get; set; }
        public string Path { get; set; }
    }
}
