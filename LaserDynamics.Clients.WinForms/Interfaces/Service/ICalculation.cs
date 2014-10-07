using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserDynamics.Clients.WinForms
{
    public interface ICalculation
    {
        string Name { get; set; }
        //void SetParameters(ICalculationViewSsfm1d view);
        void Calculate();
        long[] GetStats();
        // realization of methods returning results of calculation
        void GetResults();
        void OnCalculationStopped(object s, EventArgs e);
        ICalculationView View { get; set; }
        event EventHandler OnCalculationFinish;
        event EventHandler OnCalculationStart;
        event EventHandler OnCalculationError;
    }

}
