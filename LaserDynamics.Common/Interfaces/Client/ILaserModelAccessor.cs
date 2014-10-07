using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserDynamics.Common
{
    public interface ILaserModelAccessor
    {
        IList<ICalculation> Load();
    }
}
