using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserDynamics.Common
{
    public interface ICalculation
    {
        string Name { get; set; }
        ICalculationWorkspace Workspace { get; set; }
        ICalculationView View { get; set; }

        void Calculate();
        long[] GetStats();
        void GetResults();
        event EventHandler OnCalculationFinish;
        event EventHandler OnCalculationStart;
        event EventHandler OnCalculationError;
        event EventHandler OnCalculationReport;
        void OnCalculationStopped();
        CalculationStatus Status { get; set; }
        string ErrorMessage { get; set; }
        string ReportMessage { get; set; }
    }
    public enum CalculationStatus
    {
        Ready,
        Running,
        Stopped,
        Error,
        Finished
    }
}
