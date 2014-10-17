using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserDynamics.Common
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public class ResultsAttribute : Attribute
    {
        public DemoType DemoType { get; set; }
        public Type NumType { get; set; }

        public ResultsAttribute(DemoType type, Type numType = null)
        {
            DemoType = type;
            NumType = numType;
        }
    }
    public enum DemoType
    {
        None,
        Plot,
        Image
    }
}
