using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using LaserDynamics.Common;
using LaserDynamics.Calculations.FMBSsfmLaserModel;
using System.ComponentModel;
using LaserDynamics.Accessor;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace LaserDynamics.Clients.WinForms
{
    public class Presenter : ILaserCalculator
    {
        public ILaserModelAccessor Accessor { get; set; }
        public Presenter()
        {
            Accessor = new AssemblyAccessor();
            ReloadCalculationTypes();
            DefaultCalculation = CalculationTypes.First();
            OpenCalculations = new List<ICalculation>();
        }

        public async Task StartCalculation()
        {
            var calc = CurrentCalculation;
            await Task.Factory.StartNew(() => calc.Calculate());
        }
        void CalculateIt()
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            CurrentCalculation.Calculate();
            timer.Stop();
            MessageBox.Show("Success! " + timer.ElapsedMilliseconds);
        }
        public void ResumeCalculation()
        {

        }
        public void StopCalculation()
        {
            CurrentCalculation.OnCalculationStopped();
        }
        public object GetResults(string id)
        {
            var calc = OpenCalculations.FirstOrDefault(c => c.CalculationId == id);
            return calc.GetResult();
        }
        public void RemoveCalculation(string id)
        {
            var calc = OpenCalculations.FirstOrDefault(c => c.CalculationId == id);
            if (calc == null)
                throw new NullReferenceException("Не найдено вычисление с идентификатором '" + id + "'.");
            if (calc.Status == CalculationStatus.Running)
                calc.OnCalculationStopped();
            OpenCalculations.Remove(calc);
        }
        public void RemoveCalculation(ICalculation calc)
        {
            if (calc == null)
                throw new NullReferenceException("Не найдено вычисление '" + calc.Name + "'.");
            if (calc.Status == CalculationStatus.Running)
                calc.OnCalculationStopped();
            OpenCalculations.Remove(calc);
            if (CurrentCalculation == calc)
                CurrentCalculation = OpenCalculations.FirstOrDefault();
        }

        public IList<ICalculation> OpenCalculations { get; set; }
        public IList<ICalculation> CalculationTypes { get; set; }
        public ICalculation DefaultCalculation { get; set; }
        public ICalculation CurrentCalculation { get; set; }
        
        #region Loading & Saving Logics
        public void ReloadCalculationTypes()
        {
            CalculationTypes = Accessor.Load();
        }
        public ICalculation OpenCalculation(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader file = new StreamReader(path))
                {
                    var body = file.ReadToEnd();
                    int start = body.IndexOf("Title") + 8;
                    int end = body.IndexOf("ModelTitle");
                    string title = body.Substring(start , end-start).Trim(new char[] {'\"',','});
                    var calc = CalculationTypes.FirstOrDefault(c => c.View.Title == title).CloneCalculation();
                    if (!TryDeserialize(body, ref calc))
                    {
                        return null;
                    }

                    return calc;
                }
            }
            return null;
        }
        public void SaveCalculation(ICalculation calculation, string path)
        {
            string stringBody = null;
            if (!TrySerialize(calculation, out stringBody))
            {
                return;
            }
            using (StreamWriter file = new StreamWriter(path))
            {
                file.Write(stringBody);
            }
        }
        #endregion

        public ICalculation AddDefaultCalculation(string calcName)
        {
            var newCalc = DefaultCalculation.CloneCalculation();
            newCalc.Name = calcName;
            newCalc.CalculationId = Guid.NewGuid().ToString();
            OpenCalculations.Add(newCalc);
            CurrentCalculation = newCalc;
            return newCalc;
        }
        public void AddCalculation(ICalculation calc)
        {
            OpenCalculations.Add(calc);
            calc.CalculationId = Guid.NewGuid().ToString();
            CurrentCalculation = calc;
        }
        public ICalculation AddCalculationByTemplate(ICalculation calc)
        {
            calc = calc.CloneCalculation();
            OpenCalculations.Add(calc);
            calc.CalculationId = Guid.NewGuid().ToString();
            CurrentCalculation = calc;
            return calc;
        }
        public void ReplaceCalculation(ICalculation one, ICalculation byAnother)
        {
            OpenCalculations.Remove(one);
            var newCalc = byAnother.CloneCalculation();
            newCalc.Name = one.Name;
            newCalc.CalculationId = one.CalculationId;
            CurrentCalculation = newCalc;
            OpenCalculations.Add(newCalc);
        }
        public ICalculation GetTemplate(string model, string calcType, string numMethod)
        {
            return CalculationTypes.FirstOrDefault(c => c.View.ModelTitle == model && c.View.CalculationType == calcType && c.View.NumMethod == numMethod);
        }
        public string[] GetNumMethods(string model, string calcType)
        {
            return CalculationTypes.Where(c => c.View.ModelTitle == model && c.View.CalculationType == calcType).Select(c => c.View.NumMethod).ToArray();
        }
        public string[] GetCalcTypes(string model)
        {
            return CalculationTypes.Where(c => c.View.ModelTitle == model).Select(c => c.View.CalculationType).ToArray();
        }
        public ICalculationView GetView(string id)
        {
            return OpenCalculations.FirstOrDefault(c => c.CalculationId == id).View;
        }
        public static bool TryDeserialize<T>(string json, ref T value)
        {
            try
            {
                value = (T)JsonConvert.DeserializeObject(json, value.GetType(), new JsonSerializerSettings { });
                return true;
            }
            catch (Exception e)
            {
                value = default(T);
                return false;
            }
        }
        public static bool TrySerialize<T>(T obj, out string str)
        {
            try
            {
                str = JsonConvert.SerializeObject(obj);
                return true;
            }
            catch (Exception e)
            {
                str = "";
                return false;
            }
        }
    }
}
