using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserDynamics.Common{
    public interface ICalculationView
    {
        string Title { get; }
        string Overview { get; }
        string ModelTitle { get; }
        string CalculationType { get; }
        string NumMethod { get; }
        string[] InitialConditions { get; }
        string[] BoundaryConditions { get; }
    }
}
