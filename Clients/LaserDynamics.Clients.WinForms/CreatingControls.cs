using Jenyay.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace LaserDynamics.Clients.WinForms
{
    partial class Main
    {
        #region Controls creating  - methods
        ZedGraphControl CreatePlot(string name, object obj, Type type)
        {
            var zedGraph = new ZedGraphControl();
            zedGraph.Size = new System.Drawing.Size(500, 350);
            //zedGraph.AutoSize = true;
            zedGraph.Name = name;
            zedGraph.GraphPane = new GraphPane();
            // Создадим список точек
            PointPairList list = new PointPairList();
            var X = ((dynamic)obj).X;           // новый класс PlotResult
            var Y = ((dynamic)obj).Y;
            if (X == null || Y == null)
                return null;
            if (type == typeof(Complex[]))
            {
                var Z = Y as Complex[];
                for (int i = 0; i < (X as Array).Length; i++)
                {
                    list.Add(X[i], Z[i].Re);
                }
            }
            else
            {
                for (int i = 0; i < (X as Array).Length; i++)
                {
                    list.Add(X[i], Y[i]);
                }
            }
            // Создадим кривую с названием "Sinc", 
            // которая будет рисоваться голубым цветом (Color.Blue),
            // Опорные точки выделяться не будут (SymbolType.None)
            LineItem myCurve = new LineItem(name, list, Color.Blue, SymbolType.None);
            zedGraph.GraphPane.CurveList.Add(myCurve);
            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            // В противном случае на рисунке будет показана только часть графика, 
            // которая умещается в интервалы по осям, установленные по умолчанию
            zedGraph.AxisChange();

            // Обновляем график
            zedGraph.Invalidate();
            //zedGraph.Show();
            return zedGraph;
        }
        PictureBox CreatePictureBox(string name, object obj, Type type)
        {
            var array = ((dynamic)obj).Z;
            Bitmap bitmap;
            if (type == typeof(Complex[,]))
            {
                var complexArray = array as Complex[,];
                var newRealArray = new double[complexArray.GetLength(0), complexArray.GetLength(1)];
                for (int i = 0; i < complexArray.GetLength(0); i++)
                    for (int j = 0; j < complexArray.GetLength(1); j++)
                    {
                        newRealArray[i, j] = complexArray[i, j].Re;
                    }
                bitmap = CreateBitmap(newRealArray, Color.White);
            }
            else
            {
                bitmap = CreateBitmap(array, Color.White);
            }
            var picture = new PictureBox();
            picture.Name = name;
            picture.Image = bitmap;
            picture.SizeMode = PictureBoxSizeMode.AutoSize;
            picture.Invalidate();
            //picture.Show();
            return picture;
        }
        CheckedListBox CreateCheckListBox(string subgroup, string group)
        {
            var clb = new CheckedListBox();
            clb.Name = subgroup;
            clb.Dock = DockStyle.Fill;
            clb.SelectionMode = SelectionMode.One;
            clb.CheckOnClick = true;
            clb.Leave += (s, e) =>
            {
                var dict = new Dictionary<string, string>();
                for (int i = 0; i < clb.Items.Count; i++)
                    dict.Add(clb.Items[i].ToString(), clb.CheckedItems.Contains(clb.Items[i]).ToString());
                SetValuesToView(dict, group, subgroup);
            };
            return clb;
        }
        Button CreateButton(string name)
        {
            var btn = new Button();
            btn.Name = name;
            btn.Text = name;
            btn.AutoSize = true;
            btn.Dock = DockStyle.None;
            return btn;
        }
        ComboBox RefreshComboBoxItems(ComboBox combo, params string[] values)
        {
            combo.Items.Clear();
            combo.Items.AddRange(values);
            combo.SelectedIndex = 0;
            return combo;
        }
        ComboBox CreateComboBox(string name, params string[] values)
        {
            var newCombo = new ComboBox();
            newCombo.Name = name;
            newCombo.Items.AddRange(values);
            newCombo.SelectedIndex = 0;
            newCombo.Dock = DockStyle.Fill;
            int maxWidth = 0, temp = 0;
            foreach (string s in newCombo.Items)
            {
                temp = TextRenderer.MeasureText(s, newCombo.Font).Width;
                if (temp > maxWidth)
                {
                    maxWidth = temp;
                }
            }
            newCombo.Width = maxWidth + SystemInformation.VerticalScrollBarWidth;
            return newCombo;
        }
        TextBox CreateTextBox(string name)
        {
            var newTB = new TextBox();
            newTB.Name = name;
            newTB.Dock = DockStyle.Fill;
            return newTB;
        }
        System.Windows.Forms.Label CreateLabel(string name)
        {
            var newLabel = new System.Windows.Forms.Label();
            newLabel.Text = name;
            newLabel.Dock = DockStyle.Fill;
            newLabel.AutoSize = true;
            newLabel.TextAlign = ContentAlignment.MiddleLeft;
            newLabel.Margin = new Padding(10, 0, 0, 0);
            return newLabel;
        }
        GroupBox CreateGroupBox(string name)
        {
            var group = new GroupBox();
            group.Name = name;
            group.Text = name;
            group.AutoSize = true;
            group.Dock = DockStyle.None;
            return group;
        }
        TableLayoutPanel CreateTableLayoutPanel(string name, int rowCount, int ColumnCount)
        {
            var table = new TableLayoutPanel();
            table.AutoSize = true;
            table.ColumnCount = ColumnCount;
            for (int i = 0; i < ColumnCount; i++)
                table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.RowCount = rowCount;
            for (int i = 0; i < rowCount; i++)
                table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.Name = name;
            table.Dock = DockStyle.Fill;
            table.AutoScroll = true;
            return table;
        }
        FlowLayoutPanel CreateFlowLayoutPanel(string name)
        {
            var flowPanel = new FlowLayoutPanel();
            flowPanel.AutoScroll = true;
            flowPanel.Name = name;
            flowPanel.WrapContents = true;
            flowPanel.Dock = DockStyle.Fill;
            return flowPanel;
        }
        #endregion
    }
}
