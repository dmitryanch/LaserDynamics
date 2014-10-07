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
            foreach (string _command in Directory.GetFiles("", "*LaserModel.dll"))
                try
                {
                    Assembly _assm;
                    _assm = Assembly.LoadFrom(_command);

                    foreach (Type _type in _assm.GetTypes())
                        if (_type.GetInterface("ICalculation") == typeof(ICalculation))
                            _Result.Add((ICalculation)Activator.CreateInstance(_type));
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
