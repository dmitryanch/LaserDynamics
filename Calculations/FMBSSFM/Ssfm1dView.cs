using LaserDynamics.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserDynamics.Calculations.FullMaxvellBlockSsfm
{
    [Serializable]
    public class Ssfm1dView : ICalculationView
    {
        #region ICalculationViewSsfm1d-realization
        string _title = "Maxvell-Block 1d SSFM";
        static string _modelTitle = "Maxvell-Bloch";
        static string _calculationType = "Transverse dynamic (1d)";
        static string _numericalMethod = "SSFM";
        static string[] _initialConditions = new string[3] { "Around homogeneous solution", "Around trivial solution", "Around square vortex lattice" };
        string _initialCondition;
        static string[] _boundaryConditions = new string[1] { "Periodic" };
        string _boundaryCondition;

        public string Title { get { return _title; } }
        // Common
        public string ModelTitle { get { return _modelTitle; } }
        public string CalculationType { get { return _calculationType; } }
        public string NumMethod { get { return _numericalMethod; } }

        // Model
        [DisplayTitle("Скорость релаксации опт. поля, нс", DataType.Double, "Параметры модели",  0.001, 1000)]
        public double ElectricRelax { get; set; }
        [DisplayTitle("Скорость релаксации поляризации, нс", DataType.Double, "Параметры модели",  0.001, 1000)]
        public double PolarizationRelax { get; set; }
        [DisplayTitle("Скорость релаксации инверсии, нс", DataType.Double, "Параметры модели",  0.001, 1000)]
        public double InversionRelax { get; set; }
        [DisplayTitle("Остройка частоты, отн. ед.", DataType.Double, "Параметры модели", -10,  10)]
        public double Detuning { get; set; }
        [DisplayTitle("Накачка, отн. ед.", DataType.Double, "Параметры модели",  0,  200)]
        public double Pumping { get; set; }
        [DisplayTitle("Дифракционный параметр, отн. ед.", DataType.Double, "Параметры модели",  0.000001, 10)]
        public double Diffraction { get; set; }

        // Scheme
        [DisplayTitle("Размер расчетной области, отн. ед.", DataType.Double, "Параметры схемы",  0.01,  10)]
        public double ApertureSize { get; set; }
        [DisplayTitle("Число расчетных точек, ед.", DataType.Integer, "Параметры схемы",  10,  1024)]
        public int NumNodes { get; set; }
        [DisplayTitle("Шаг по времени, нс", DataType.Double, "Параметры схемы",  0.000001,  10)]
        public double TimeStep { get; set; }
        [DisplayTitle("Количество временных итераций, ед.", DataType.Integer, "Параметры схемы", 1,  1e6)]
        public int TotalTime { get; set; }

        [DisplayTitle("Начальное условие", DataType.Discrete, "Условия", "", new string[3] { "Around homogeneous solution", "Around trivial solution", "Around square vortex lattice" })]
        public string InitialCondition 
        {
            get 
            { 
                return String.IsNullOrEmpty(_initialCondition) ? _initialConditions[0] : _initialCondition;
            } 
            set 
            { _initialCondition = value; } 
        }
        public string[] InitialConditions { get { return _initialConditions; } }
        [DisplayTitle("Граничное условие", DataType.Discrete, "Условия", "", new string[1] { "Periodic" })]
        public string BoundaryCondition
        {
            get
            {
                return String.IsNullOrEmpty(_boundaryCondition) ? _boundaryConditions[0] : _boundaryCondition;
            }
            set
            { _boundaryCondition = value; }
        }
        public string[] BoundaryConditions { get { return _boundaryConditions; } }

        // Results
        [DisplayTitle("Интенсивность", DataType.Bool, "Вывод результатов", "Локальная динамика")]
        public bool OutLocalIntensity { get; set; }
        [DisplayTitle("Оптическое поле", DataType.Bool, "Вывод результатов", "Локальная динамика")]
        public bool OutLocalField { get; set; }
        [DisplayTitle("Фаза", DataType.Bool, "Вывод результатов", "Локальная динамика")]
        public bool OutLocalPhase { get; set; }
        [DisplayTitle("Поляризация", DataType.Bool, "Вывод результатов", "Локальная динамика")]
        public bool OutLocalPolarization { get; set; }
        [DisplayTitle("Инверсия", DataType.Bool, "Вывод результатов", "Локальная динамика")]
        public bool OutLocalInversion { get; set; }

        [DisplayTitle("Интенсивность", DataType.Bool, "Вывод результатов", "Распределение")]
        public bool OutIntensityDistibution { get; set; }
        [DisplayTitle("Оптическое поле", DataType.Bool, "Вывод результатов", "Распределение")]
        public bool OutFieldDistibution { get; set; }
        [DisplayTitle("Фаза", DataType.Bool, "Вывод результатов", "Распределение")]
        public bool OutPhaseDistibution { get; set; }
        [DisplayTitle("Пространственный спектр", DataType.Bool, "Вывод результатов", "Распределение")]
        public bool OutSpectrumDistibution { get; set; }
        [DisplayTitle("Поляризация", DataType.Bool, "Вывод результатов", "Распределение")]
        public bool OutPolarizationDistibution { get; set; }
        [DisplayTitle("Инверсия", DataType.Bool, "Вывод результатов", "Распределение")]
        public bool OutInversionDistibution { get; set; }
        #endregion
    }
}
