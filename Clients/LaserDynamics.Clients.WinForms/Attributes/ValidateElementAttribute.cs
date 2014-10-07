using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserDynamics.Clients.WinForms
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidateElementAttribute : Attribute
    {
        public double MoreThan { get; set; }
        public double LessThan { get; set; }
        public string[] DiscreteValues { get; set; }
        public DataType Type { get; set; }
        public string ErrorText 
        { 
            get
            {
                if (Type == DataType.Double || Type == DataType.Integer)
                {
                    return String.Format("Field has to be '{0}'-number from interval [{1} .. {2}]",Type.ToString(),MoreThan,LessThan);
                }
                return null;
            }
        }

        public ValidateElementAttribute(DataType type = DataType.String, double moreThan = 0, double lessThan = 0, params string[] discreteValues)
        {
            Type = type; 
            MoreThan = moreThan;
            LessThan = lessThan;
            DiscreteValues = discreteValues;
        }
        public ValidateElementAttribute(DataType type = DataType.String, params string[] discreteValues)
        {
            Type = type;
            DiscreteValues = discreteValues;
        }
    }

    //public enum DataType
    //{
    //    Bool,
    //    Double,
    //    Integer,
    //    String,
    //    Discrete
    //}
}
