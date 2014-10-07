using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserDynamics.Clients.WinForms.Interfaces.Client
{
    public interface ILaserModelLoader
    {
        ICalculation LoadFrom();
    }
}
