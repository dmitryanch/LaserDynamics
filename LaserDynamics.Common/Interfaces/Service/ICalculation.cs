using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserDynamics.Common
{
    public interface ICalculation
    {
        string CalculationId { get; set; }
        string Name { get; set; }
        ICalculationWorkspace Workspace { get; set; }
        ICalculationView View { get; set; }

        void Calculate();
        long[] GetStats();
        object GetResult();
        event EventHandler OnCalculationFinish;
        event EventHandler OnCalculationStart;
        event EventHandler OnCalculationError;
        event EventHandler OnCalculationReport;
        event EventHandler OnCalculationValidationFailed;
            
        void OnCalculationStopped();
        CalculationStatus Status { get; set; }
        string ErrorMessage { get; set; }
        string ReportMessage { get; set; }
        
        // system
        bool IsSaved { get; set; }
        string Path { get; set; }
    }
    public enum CalculationStatus
    {
        Ready,
        ValidationFailed,
        Running,
        Stopped,
        Error,
        Finished
    }
}
