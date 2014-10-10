using LaserDynamics.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaserDynamics.Clients.WinForms
{
    public static class CalculationExtensions
    {
        public static T CloneCalculation<T>(this T calc) where T : ICalculation
        {
            var ctor = calc.GetType().GetConstructor(new Type[0]);
            var obj = (T)ctor.Invoke(new object[0]);
            //obj.Workspace.OnCalculationStart += (s, e) => { };
            //obj.Workspace.OnCalculationReport += (s, e) =>  { };
            //obj.Workspace.OnCalculationError += (s, e) => { };
            //obj.Workspace.OnCalculationFinish += (s, e) => { };
            obj.IsSaved = true;
            obj.Path = null;
            return obj;
        }
    }
    static public class Extensions
    {
        delegate void SetEnable(bool enabled);
        public static void SetEnableInvoke(this Button btn, bool enabled)
        {
            btn.Invoke(new SetEnable(t => { btn.Enabled = t; }), enabled);
        }
        delegate void SetText(string text);
        public static void SetTextInvoke(this Label lbl, string text)
        {
            lbl.Invoke(new SetText(t => { lbl.Text = t; }), text);
        }
    }
}
