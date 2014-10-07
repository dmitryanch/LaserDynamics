using System;
using System.Linq;
using Jenyay.Mathematics;
using System.Diagnostics;
using LaserDynamics.Common;

namespace LaserDynamics.Calculations.FullMaxvellBlockSsfm
{
    public class Ssfm1dCalculation : ICalculation 
    {
        public Ssfm1dCalculation()
        {
            View = new Ssfm1dView { Parent = this };
            Workspace = new Ssfm1dWorkspace { Parent = this };
        }

        #region Public Methods
        public string Name { get; set; }
        public ICalculationView View { get; set; }
        public ICalculationWorkspace Workspace { get; set; }
        #endregion
    }
}
