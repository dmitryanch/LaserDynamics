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
            obj.IsSaved = true;
            obj.Path = null;
            return obj;
        }
    }
    // Extension with delegates for multithreading operations with UI-controls
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
        delegate void AddControl(TableLayoutPanel lbl);
        public static void AddControlInvoke(this Control control, TableLayoutPanel lbl)
        {
            //lbl.Invoke(new AddControl(t => { lbl.Controls.Add(t, 0, 2); }), control);
            //control.Invoke(new AddControl(t => { t.Controls.Add(control, 0, 2); }), lbl);
            var t = control.Handle;
            control.Invoke(new MethodInvoker(() => {lbl.Controls.Add(control, 0, 2); }));
        }
        delegate void RemoveByKeyControl(string text);
        public static void RemoveByKeyControlInvoke(this TableLayoutPanel lbl, string text)
        {
            lbl.Invoke(new RemoveByKeyControl(t => { lbl.Controls.RemoveByKey(t); }), text);
        }
    }
}
