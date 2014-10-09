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
using LaserDynamics.Calculations.FullMaxvellBlockSsfm;
using System.ComponentModel;
using LaserDynamics.Accessor;
using Newtonsoft.Json;

namespace LaserDynamics.Clients.WinForms
{
    public class Presenter : ILaserCalculator
    {
        readonly string AllTypeListFile = "AllTypeList.xml";
        readonly string OpenTypeListFile = "OpenTypeList.xml";

        public ILaserModelAccessor Accessor { get; set; }
        public event EventHandler OnCalculationStopped;

        public Presenter()
        {
            Accessor = new AssemblyAccessor();
            LoadCalculations();
        }

        public async Task StartCalculation(string calcName)
        {
            //if (!_view.IsValid()) // реализовать методом презентера
            //return;
            var calc = CurrentCalculation;
            if (calc == null)
                throw new NullReferenceException("Не найдено вычисление с именем '" + calcName + "'.");
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
        public void ResumeCalculation(string calcName)
        {

        }
        public void StopCalculation(string calcName)
        {
            CurrentCalculation.OnCalculationStopped();
        }
        public void ShowResults(string calcName)
        {

        }
        public void RemoveCalculation(string calcName)
        {
            var calc = OpenCalculations.FirstOrDefault(c => c.Name == calcName);
            OpenCalculations.Remove(calc);
            CurrentCalculation.OnCalculationStopped();
        }

        public IList<ICalculation> OpenCalculations { get; set; }
        public IList<ICalculation> CalculationTypes { get; set; }
        public ICalculation DefaultCalculation { get; set; }
        public ICalculation CurrentCalculation { get; set; }
        IList<KeyValuePair<string, Type>> OpenTypes { get; set; }

        #region Loading & Saving Logics
        public void LoadCalculations()
        {
            CalculationTypes = Accessor.Load();
            
            OpenTypes = new List<KeyValuePair<string, Type>>();
            var openCalculations = new List<ICalculation>();
            if (File.Exists(OpenTypeListFile))
            {
                using (StreamReader file = new StreamReader(OpenTypeListFile))
                {
                    var str = file.ReadToEnd();
                    if(!TryDeserialize(str, out openCalculations))
                    {
                        
                    }
                    OpenCalculations = openCalculations;
                }
            }

            if (DefaultCalculation == null)
                DefaultCalculation = CalculationTypes[0];
            if (OpenCalculations == null || !OpenCalculations.Any())
            {
                OpenCalculations = new List<ICalculation>();
                OpenCalculations.Add(DefaultCalculation.CloneCalculation());
            }
        }
        public void SaveCalculation(ICalculation calculation, string path)
        {

        }
        public void SaveOpenCalculations()
        {
            //if (File.Exists(OpenTypeListFile))
            //{
            string str = null;
                
            foreach (var calc in OpenCalculations)
            {
                string str1 = null;
                if (!TrySerialize(calc, out str1))
                {
                    //continue;
                }

            }
                using (StreamWriter file = new StreamWriter(OpenTypeListFile))
                {
                    file.Write(str);
                }
        }

        #endregion

        public void AddDefaultCalculation(string calcName)
        {
            var newCalc = DefaultCalculation.CloneCalculation();
            newCalc.Name = calcName;
            OpenCalculations.Add(newCalc);
        }
        public void DeleteCalculation(string calcName)
        {
            var calc = OpenCalculations.FirstOrDefault(c => c.Name == calcName);
            if (calc == null)
                throw new NullReferenceException("Не найдено вычисление с именем '" + calcName + "'.");
            OpenCalculations.Remove(calc);
        }
        public void ReplaceCalculation(ICalculation one, ICalculation byAnother)
        {
            OpenCalculations.Remove(one);
            var newCalc = byAnother.CloneCalculation();
            newCalc.Name = one.Name;
            CurrentCalculation = newCalc;
            OpenCalculations.Add(newCalc);
        }

        public static bool TryDeserialize<T>(string json, out T value)
        {
            try
            {
                value = JsonConvert.DeserializeObject<T>(json);
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
