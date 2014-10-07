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
    }

}
