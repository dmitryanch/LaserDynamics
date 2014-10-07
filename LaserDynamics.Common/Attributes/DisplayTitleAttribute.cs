using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserDynamics.Common
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public class DisplayTitleAttribute : Attribute
    {
        public string Title { get; set; }
        //public ElementType Type { get; set; }
        public string GroupBox { get; set; }
        public string Subgroup { get; set; }
        public string[] ComboValues { get; set; }
        public double MoreThan { get; set; }
        public double LessThan { get; set; }
        public DataType Type { get; set; }
        public string ErrorText
        {
            get
            {
                if (Type == DataType.Double || Type == DataType.Integer)
                {
                    return String.Format("Field has to be '{0}'-number from interval [{1} .. {2}]", Type.ToString(), MoreThan, LessThan);
                }
                return null;
            }
        }
        public DisplayTitleAttribute(string title, DataType type, string groupBox, double moreThan, double lessThan, string subgroup = null)
        {
            Title = title;
            Type = type;
            GroupBox = groupBox;
            Subgroup = subgroup;
            MoreThan = moreThan;
            LessThan = lessThan;
        }
        public DisplayTitleAttribute(string title, DataType type, string groupBox, string subgroup = null)
        {
            Title = title;
            Type = type;
            GroupBox = groupBox;
            Subgroup = subgroup;
        }
        public DisplayTitleAttribute(string title, DataType type, string groupBox, string subgroup = null, params string[] comboValues)
        {
            Title = title;
            Type = type;
            GroupBox = groupBox;
            Subgroup = subgroup;
            ComboValues = comboValues;
        }
    }
        public enum DataType
        {
            Bool,
            Double,
            Integer,
            String,
            Discrete
        }
        public enum ElementType
        {
            TextBox,
            ComboBox,
            CheckBox
        }
}
