using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaserDynamics.Common;
using System.IO;
using System.Reflection;

namespace LaserDynamics.Accessor
{
    public class AssemblyAccessor : ILaserModelAccessor
    {
        public IList<ICalculation> Load()
        {
            var _Result = new List<ICalculation>();
            foreach (string assemblyFile in Directory.GetFiles("", "*LaserModel.dll"))
                try
                {
                    Assembly assembly = Assembly.LoadFrom(assemblyFile);
                    var types = assembly.GetTypes().Where(t => t.GetInterface("ICalculation") == typeof(ICalculation)).Select(t => (ICalculation)Activator.CreateInstance(t));
                    _Result.AddRange(types);
                    //foreach (Type _type in assembly.GetTypes())
                    //    if (_type.GetInterface("ICalculation") == typeof(ICalculation))
                    //        _Result.Add((ICalculation)Activator.CreateInstance(_type));
                }
                catch (Exception ex)
                {
                    //Запись сообщения об ошибке в лог
                    return null;
                }

            return _Result;
        }
    }
}
