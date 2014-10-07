using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LaserDynamics.Clients.WinForms
{
    interface ILaserCalculator
    {
        IList<ICalculation> OpenCalculations { get; set; }
        IList<ICalculation> CalculationTypes { get; set; }
        ICalculation DefaultCalculation { get; set; }
        ICalculation CurrentCalculation { get; set; }
        IList<Thread> Threads { get; set; }

        void LoadCalculations();
        void SaveCalculations();

        void CreateDefaultCalculation(string calcName);
        void DeleteCalculation(string calcName);

        void StartCalculation(string calcName);
        void ResumeCalculation(string calcName);
        void StopCalculation(string calcName);
        void ShowResults(string calcName);

        event EventHandler OnCalculationStopped;
    }
}
