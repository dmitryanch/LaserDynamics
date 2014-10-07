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

namespace LaserDynamics.Clients.WinForms
{
    public class Presenter : ILaserCalculator
    {
        readonly string AllTypeListFile = "AllTypeList.xml";
        readonly string OpenTypeListFile = "OpenTypeList.xml";

        public event EventHandler OnCalculationStopped;

        public Presenter()
        {
            LoadCalculations();
        }

        public async Task StartCalculation(string calcName)
        {
            //if (!_view.IsValid()) // реализовать методом презентера
            //return;
            var calc = CurrentCalculation;
            if (calc == null)
                throw new NullReferenceException("Не найдено вычисление с именем '" + calcName + "'.");
            await Task.Factory.StartNew(() => calc.Workspace.Calculate());
        }
        void CalculateIt()
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            CurrentCalculation.Workspace.Calculate();
            timer.Stop();
            MessageBox.Show("Success! " + timer.ElapsedMilliseconds);
        }
        public void ResumeCalculation(string calcName)
        {

        }
        public void StopCalculation(string calcName)
        {
            CurrentCalculation.Workspace.OnCalculationStopped();
        }
        public void ShowResults(string calcName)
        {

        }
        public void ClosePage(string calcName)
        {
            var calc = OpenCalculations.FirstOrDefault(c => c.Name == calcName);
            OpenCalculations.Remove(calc);
            CurrentCalculation.Workspace.OnCalculationStopped();
        }

        public IList<ICalculation> OpenCalculations { get; set; }
        public IList<ICalculation> CalculationTypes { get; set; }
        public ICalculation DefaultCalculation { get; set; }
        public ICalculation CurrentCalculation { get; set; }
        IList<KeyValuePair<string, Type>> Types { get; set; }
        IList<KeyValuePair<string, Type>> OpenTypes { get; set; }

        public void LoadCalculations()
        {
            Types = new List<KeyValuePair<string, Type>>();
            if (File.Exists(AllTypeListFile))
            {
                using (StreamReader file = new StreamReader(AllTypeListFile))
                {
                    XmlSerializer reader = new XmlSerializer(typeof(List<KeyValuePair<string, Type>>));
                    Types = (IList<KeyValuePair<string, Type>>)reader.Deserialize(file);
                }
            }
            OpenTypes = new List<KeyValuePair<string, Type>>();
            if (File.Exists(OpenTypeListFile))
            {
                using (StreamReader file = new StreamReader(OpenTypeListFile))
                {
                    XmlSerializer reader = new XmlSerializer(typeof(List<KeyValuePair<string, Type>>));
                    OpenTypes = (IList<KeyValuePair<string, Type>>)reader.Deserialize(file);
                }
            }
            CalculationTypes = new List<ICalculation>();
            foreach (var calcTypeFile in Types)
            {
                if (File.Exists(calcTypeFile.Key))
                {
                    using (StreamReader file = new StreamReader(calcTypeFile.Key))
                    {
                        XmlSerializer reader = new XmlSerializer(calcTypeFile.Value);
                        CalculationTypes.Add((ICalculation)reader.Deserialize(file));
                    }
                }
            }
            OpenCalculations = new List<ICalculation>();
            foreach (var openTypeFile in OpenTypes)
            {
                if (File.Exists(openTypeFile.Key))
                {
                    using (StreamReader file = new StreamReader(openTypeFile.Key))
                    {
                        XmlSerializer reader = new XmlSerializer(openTypeFile.Value);
                        OpenCalculations.Add((ICalculation)reader.Deserialize(file));
                    }
                }
            }

            //if (CalculationTypes == null)
            //    CalculationTypes = new List<ICalculation>();
            CalculationTypes.Add(new Ssfm1dCalculation());
            if (DefaultCalculation == null)
                DefaultCalculation = CalculationTypes[0];
            if (OpenCalculations == null)// || OpenCalculations.Count() == 0)
            {
                OpenCalculations = new List<ICalculation>();
                OpenCalculations.Add(DefaultCalculation.GenerateNewCalculation());
            }
        }
        public void SaveCalculations()
        {
            if (File.Exists(AllTypeListFile))
            {
                using (StreamWriter file = new StreamWriter(AllTypeListFile))
                {
                    XmlSerializer writer = new XmlSerializer(typeof(List<KeyValuePair<string, Type>>));
                    writer.Serialize(file, Types);
                }
            }
            if (File.Exists(OpenTypeListFile))
            {
                using (StreamWriter file = new StreamWriter(AllTypeListFile))
                {
                    XmlSerializer writer = new XmlSerializer(typeof(List<KeyValuePair<string, Type>>));
                    writer.Serialize(file, OpenTypes);
                }
            }
            foreach (var calcType in CalculationTypes)
            {
                var fileName = Path.Combine("CalculationTypes", calcType.View.Title + ".xml");
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                if (!File.Exists(fileName))
                {
                    using (StreamWriter file = new StreamWriter(fileName))
                    {
                        XmlSerializer reader = new XmlSerializer(calcType.GetType());
                        reader.Serialize(file, calcType);
                    }
                }
            }
            //OpenCalculations = new List<ICalculation>();
            foreach (var openCalc in OpenCalculations)
            {
                var fileName = Path.Combine("OpenCalculations", openCalc.Name + ".xml");
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                using (StreamWriter file = new StreamWriter(fileName))
                {
                    XmlSerializer reader = new XmlSerializer(openCalc.GetType());
                    reader.Serialize(file, openCalc);
                }
            }
        }
        void AddCalculationType(ICalculation calc)
        {
            CalculationTypes.Add(calc);
            Types.Add(new KeyValuePair<string, Type>(Path.Combine("CalculationTypes", calc.View.Title + ".xml"), calc.GetType()));
        }
        void AddOpenCalculation(ICalculation calc)
        {
            OpenCalculations.Add(calc);
            Types.Add(new KeyValuePair<string, Type>(Path.Combine("OpenCalculations", calc.Name + ".xml"), calc.GetType()));
        }
        public void CreateDefaultCalculation(string calcName)
        {
            var newCalc = DefaultCalculation.GenerateNewCalculation();
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
            var newCalc = byAnother.GenerateNewCalculation();
            newCalc.Name = one.Name;
            CurrentCalculation = newCalc;
            OpenCalculations.Add(newCalc);
        }
    }
}
