using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;

namespace LaserDynamics.Common
{
    public interface ILaserCalculator
    {
        ILaserModelAccessor Accessor { get; set; }
        IList<ICalculation> OpenCalculations { get; set; }
        IList<ICalculation> CalculationTypes { get; set; }
        ICalculation DefaultCalculation { get; set; }
        ICalculation CurrentCalculation { get; set; }
        
        ICalculation OpenCalculation(string path);
        void SaveCalculation(ICalculation calculation, string path);

        ICalculation AddDefaultCalculation(string calcName);
        void RemoveCalculation(string calcName);

        Task StartCalculation();
        void ResumeCalculation();
        void StopCalculation();
        void ShowResults();
    }
}
