using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Reflection;
using LaserDynamics.Common;

namespace LaserDynamics.Clients.WinForms
{
    public partial class Main : Form
    {
        bool Loadable = true;
        Presenter _presenter;
        public Main()
        {
            InitializeComponent();
            _presenter = new Presenter();

            StartBtn.Click += async (s, e) => await _presenter.StartCalculation(CalculationsTabControl.SelectedTab.Name);
            ResumeBtn.Click += (s, e) => _presenter.ResumeCalculation(CalculationsTabControl.SelectedTab.Name);
            StopBtn.Click += (s, e) => _presenter.StopCalculation(CalculationsTabControl.SelectedTab.Name);
            ResultsBtn.Click += (s, e) => _presenter.ShowResults(CalculationsTabControl.SelectedTab.Name);
            ClosePageBtn.Click += (s, e) => _presenter.ClosePage(CalculationsTabControl.SelectedTab.Name);
            CalculationsTabControl.SelectedIndexChanged += (s,e)=>
                {
                    if (!Loadable)
                        return;
                    if (CalculationsTabControl.SelectedTab == CalculationsTabControl.TabPages["PlusTabPage"])
                        CreateDefaultCalculationTabPage();
                    _presenter.CurrentCalculation = _presenter.OpenCalculations.FirstOrDefault(c => c.Name == CalculationsTabControl.SelectedTab.Name);
                };
            // реализовать функциональность наполнения _presenter.OpenPages и _presenter.Models
        }

        protected override void OnClosed(EventArgs e)
        {
            _presenter.SaveCalculations();
            base.OnClosed(e);
        }
        
        #region Обработчики событий при нажатии кнопок управления вычислениями
        void OnModelChanged(TableLayoutPanel controlTable)
        {
            Loadable = false;
            var name = CalculationsTabControl.SelectedTab.Name;
            var table = CalculationsTabControl.SelectedTab.Controls[name];
            var flowPanel = table.Controls["CalculationPanel"];
            if (flowPanel != null)
                table.Controls.Remove(flowPanel);
            var modelCombo = (controlTable.Controls["ModelTitle"] as ComboBox);
            var calcTypeCombo = (controlTable.Controls["CalculationType"] as ComboBox);
            calcTypeCombo = RefreshComboBoxItems(calcTypeCombo, _presenter.CalculationTypes
                .Where(c => c.View.ModelTitle == modelCombo.SelectedItem.ToString())
                .Select(c => c.View.CalculationType).ToArray());
            var numMethodCombo = (controlTable.Controls["NumMethod"] as ComboBox);
            numMethodCombo = RefreshComboBoxItems(numMethodCombo, _presenter.CalculationTypes
                .Where(c => c.View.ModelTitle == modelCombo.SelectedItem.ToString() && c.View.CalculationType == calcTypeCombo.SelectedItem.ToString())
                .Select(c => c.View.NumMethod).ToArray());

            var template = _presenter.CalculationTypes
                .FirstOrDefault(c => c.View.ModelTitle == modelCombo.SelectedItem.ToString() && c.View.CalculationType == calcTypeCombo.SelectedItem.ToString() && c.View.NumMethod == numMethodCombo.SelectedItem.ToString());
            _presenter.ReplaceCalculation(_presenter.CurrentCalculation,template);
            var smallTable = CreateCalculationPanel(template.View);
            table.Controls.Add(smallTable);
            Loadable = true;
        }
        void OnCalculationTypeChanged(TableLayoutPanel controlTable)
        {
            if (!Loadable)
                return;
            Loadable = false;
            var name = CalculationsTabControl.SelectedTab.Name;
            var table = CalculationsTabControl.SelectedTab.Controls[name];
            var flowPanel = table.Controls["CalculationPanel"];
            if (flowPanel != null)
                table.Controls.Remove(flowPanel);

            var modelCombo = (controlTable.Controls["ModelTitle"] as ComboBox);
            var calcTypeCombo = (controlTable.Controls["CalculationType"] as ComboBox);
            var numMethodCombo = (controlTable.Controls["NumMethod"] as ComboBox);
            numMethodCombo = RefreshComboBoxItems(numMethodCombo, _presenter.CalculationTypes
                .Where(c => c.View.ModelTitle == modelCombo.SelectedItem.ToString() && c.View.CalculationType == calcTypeCombo.SelectedItem.ToString())
                .Select(c => c.View.NumMethod).ToArray());

            var template = _presenter.CalculationTypes
                .FirstOrDefault(c => c.View.ModelTitle == modelCombo.SelectedItem.ToString() && c.View.CalculationType == calcTypeCombo.SelectedItem.ToString() && c.View.NumMethod == numMethodCombo.SelectedItem.ToString());
            _presenter.ReplaceCalculation(_presenter.CurrentCalculation, template);
            var smallTable = CreateCalculationPanel(template.View);
            table.Controls.Add(smallTable);
            Loadable = true;
        }
        void OnNumMethodChanged(TableLayoutPanel controlTable)
        {
            if (!Loadable)
                return;
            Loadable = false;
            var name = CalculationsTabControl.SelectedTab.Name;
            var table = CalculationsTabControl.SelectedTab.Controls[name];
            var flowPanel = table.Controls["CalculationPanel"];
            if (flowPanel != null)
                table.Controls.Remove(flowPanel);

            var modelCombo = (controlTable.Controls["ModelTitle"] as ComboBox);
            var calcTypeCombo = (controlTable.Controls["CalculationType"] as ComboBox);
            var numMethodCombo = (controlTable.Controls["NumMethod"] as ComboBox);

            var template = _presenter.CalculationTypes
                .FirstOrDefault(c => c.View.ModelTitle == modelCombo.SelectedItem.ToString() && c.View.CalculationType == calcTypeCombo.SelectedItem.ToString() && c.View.NumMethod == numMethodCombo.SelectedItem.ToString());
            _presenter.ReplaceCalculation(_presenter.CurrentCalculation, template);
            var smallTable = CreateCalculationPanel(template.View);
            table.Controls.Add(smallTable);
            Loadable = true;
        }
        #endregion

        #region Методы создания и редактирования элементов управления форм
        void ReverseButtonsState(params Button[] btns)
        {
            foreach (var btn in btns)
            {
                btn.SetEnableInvoke(!btn.Enabled);
                //string path = btn.Name + (btn.Enabled ? "Enable" : "Disable") + ".png";
                //btn.Image = new Bitmap(path);
            }
        }
        void CreateDefaultCalculationTabPage()
        {
            Loadable = false;
            var calcView = _presenter.DefaultCalculation.View;
            string name = "Calculation " + CalculationsTabControl.TabPages.Count;
            _presenter.AddDefaultCalculation(name);
            var newP = CreateNewTabPage(calcView, name);
            CalculationsTabControl.TabPages.Add(newP);
            CalculationsTabControl.SelectedTab = newP;
            CalculationsTabControl.TabPages.Remove(PlusTabPage);
            CalculationsTabControl.TabPages.Add(PlusTabPage);
            Loadable = true;
        }
        void OpenCalculationTabPage(ICalculation calc)
        {
            Loadable = false;
            string name = calc.Name;
            var newP = CreateNewTabPage(calc.View, name);
            //_presenter.CreateDefaultCalculation(name);
            CalculationsTabControl.TabPages.Add(newP);
            CalculationsTabControl.SelectedTab = newP;
            CalculationsTabControl.TabPages.Remove(PlusTabPage);
            CalculationsTabControl.TabPages.Add(PlusTabPage);
            Loadable = true;
        }
        TableLayoutPanel CreateControlElementsPanel(string name)
        {
            var bigTable = CreateTableLayoutPanel("", 1, 8);
            bigTable.Margin = new Padding(0, 0, 0, 0);
            bigTable.Dock = DockStyle.None;
            var topTable = CreateTableLayoutPanel("", 1, 2);
            topTable.Padding = new Padding(0, 0, 0, 0);
            topTable.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100F);
            topTable.AutoScroll = false;
            bigTable.AutoScroll = false;
            topTable.Controls.Add(bigTable);
            
            var StartBtn = CreateButton("start");
            StartBtn.Text = "";
            StartBtn.Image = new Bitmap(@"startEnable.png"); 
            StartBtn.Size = new Size(28, 20);
            CalculationsTabControl.SelectedTab.Controls.Add(StartBtn);
            bigTable.Controls.Add(StartBtn);
            
            var StopBtn = CreateButton("stop");
            StopBtn.Text = "";
            StopBtn.Image = new Bitmap(@"stopEnable.png");
            StopBtn.Size = new Size(28, 20);
            StopBtn.Enabled = false;
            CalculationsTabControl.SelectedTab.Controls.Add(StopBtn);
            bigTable.Controls.Add(StopBtn);
            
            StartBtn.Click += async (s, e) =>
            {
                ReverseButtonsState(StartBtn, StopBtn);
                CalculationsTabControl.TabPages[name].Controls[0].Controls["StateLabel"].Text = "Started...";
                await _presenter.StartCalculation(name);
            };
            StopBtn.Click += (s, e) =>
            {
                _presenter.StopCalculation(name);
            };
            _presenter.OpenCalculations.FirstOrDefault(c => c.Name == name).Workspace.OnCalculationFinish += (s, e) =>
                {
                    var calc = _presenter.OpenCalculations.FirstOrDefault(c => c.Name == name);
                    if (calc.Workspace.Error != null)
                    {
                        (CalculationsTabControl.TabPages[name].Controls[0].Controls["StateLabel"]as Label).SetTextInvoke("Error: " + calc.Workspace.Error);
                    }
                    else if (calc.Workspace.Stopped)
                    {
                        (CalculationsTabControl.TabPages[name].Controls[0].Controls["StateLabel"] as Label).SetTextInvoke("Stopped.");
                    }
                    else
                    {
                        (CalculationsTabControl.TabPages[name].Controls[0].Controls["StateLabel"] as Label).SetTextInvoke("Succesefully performed: " + string.Join(", ", calc.Workspace.GetStats().Select(st => st.ToString() + " ms")));
                        // реализовать отображение результатов
                    }
                    ReverseButtonsState(StartBtn, StopBtn);
                };
            _presenter.OpenCalculations.FirstOrDefault(c => c.Name == name).Workspace.OnCalculationError += (s, e) =>
            {
                var calc = _presenter.OpenCalculations.FirstOrDefault(c => c.Name == name);
                if (calc.Workspace.Error != null)
                {
                    (CalculationsTabControl.TabPages[name].Controls[0].Controls["StateLabel"] as Label).SetTextInvoke("Error: " + calc.Workspace.Error);
                }
                ReverseButtonsState(StartBtn, StopBtn);
            };
            _presenter.OpenCalculations.FirstOrDefault(c => c.Name == name).Workspace.OnCalculationReport += (s, e) =>
            {
                var calc = _presenter.OpenCalculations.FirstOrDefault(c => c.Name == name);
                (CalculationsTabControl.TabPages[name].Controls[0].Controls["StateLabel"] as Label).SetTextInvoke("Running: " + calc.Workspace.Report + "%");
            };
            
            //var ResumeBtn = CreateButton("Resume");
            //ResumeBtn.Click += (s, e) => _presenter.ResumeCalculation(name);
            //bigTable.Controls.Add(ResumeBtn, 2, 0);

            var CloseBtn = CreateButton("");
            CloseBtn.Click += (s, e) =>
                {
                    _presenter.ClosePage(name);
                    int index = CalculationsTabControl.TabPages.IndexOf( CalculationsTabControl.SelectedTab);
                    CalculationsTabControl.TabPages.Remove(CalculationsTabControl.SelectedTab);
                    CalculationsTabControl.SelectTab(index - 1);
                };
            CloseBtn.Image = new Bitmap(@"close.png");
            CloseBtn.Size = new Size(20, 20);
            topTable.Controls.Add(CloseBtn);

            // ComboBoxes
            var newLabel = CreateLabel("Модель");
            bigTable.Controls.Add(newLabel);

            var modelCombo = CreateComboBox("ModelTitle", _presenter.CalculationTypes
                .Select(c => c.View.ModelTitle).ToArray());
            modelCombo.SelectedIndexChanged += (s, e) => OnModelChanged(bigTable);
            bigTable.Controls.Add(modelCombo);
            modelCombo.Anchor = AnchorStyles.Top & AnchorStyles.Bottom;

            newLabel = CreateLabel("Тип расчетов");
            bigTable.Controls.Add(newLabel);

            var calcCombo = CreateComboBox("CalculationType", _presenter.CalculationTypes
                .Where(c => c.View.ModelTitle == modelCombo.Items[0].ToString())
                .Select(c => c.View.CalculationType).ToArray());
            calcCombo.SelectedIndexChanged += (s, e) => OnCalculationTypeChanged(bigTable);
            calcCombo.Anchor = AnchorStyles.Top & AnchorStyles.Bottom;
            bigTable.Controls.Add(calcCombo);

            newLabel = CreateLabel("Численный метод");
            bigTable.Controls.Add(newLabel);

            var numMethodCombo = CreateComboBox("NumMethod", _presenter.CalculationTypes
                .Where(c => c.View.ModelTitle == modelCombo.Items[0].ToString() && c.View.CalculationType == calcCombo.Items[0].ToString())
                .Select(c => c.View.NumMethod).ToArray());
            numMethodCombo.SelectedIndexChanged += (s, e) => OnNumMethodChanged(bigTable);
            numMethodCombo.Anchor = AnchorStyles.Top & AnchorStyles.Bottom;
            bigTable.Controls.Add(numMethodCombo);

            return topTable;
        }
        TabPage CreateNewTabPage(ICalculationView template, string name)
        {
            var newPage = new TabPage();
            newPage.Name = name;
            newPage.Text = name;
            newPage.Padding = new Padding(1, 1, 1, 1);
            newPage.AutoScroll = true;
            var tablePanel = CreateTableLayoutPanel(name, 3, 1);
            tablePanel.Margin = new Padding(1, 1, 1, 1);
            tablePanel.Padding = new Padding(0, 0, 0, 0);
            tablePanel.RowStyles[1] = new RowStyle(SizeType.Percent, 100F);
            tablePanel.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100F);
            newPage.Controls.Add(tablePanel);
            var newGroup = CreateControlElementsPanel(name);
            newGroup.Margin = new Padding(0, 0, 0, 0);
            tablePanel.Controls.Add(newGroup);
            var smallTable = CreateCalculationPanel(template);
            tablePanel.Controls.Add(smallTable, 0, 1);
            var stateLabel = CreateLabel("Ready");
            stateLabel.Name = "StateLabel";
            tablePanel.Controls.Add(stateLabel, 0, 2);
            return newPage;
        }
        // Reflection is used HERE
        FlowLayoutPanel CreateCalculationPanel(ICalculationView tempate)
        {
            List<GroupBox> GroupBoxes = new List<GroupBox>();
            List<CheckedListBox> CLB = new List<CheckedListBox>();
            List<TableLayoutPanel> Tables = new List<TableLayoutPanel>();
            foreach (var member in tempate.GetType().GetProperties())
            {
                var attr = (DisplayTitleAttribute)member.GetCustomAttribute(typeof(DisplayTitleAttribute));
                if (attr == null)
                    continue;
                switch (attr.Type)
                {
                    case DataType.Bool:
                        {
                            if (!String.IsNullOrEmpty(attr.Subgroup))
                            {
                                var clb = CLB.FirstOrDefault(c => c.Name == attr.Subgroup);
                                var group = GroupBoxes.FirstOrDefault(g => g.Name == attr.GroupBox);
                                var table = Tables.FirstOrDefault(g => g.Name == attr.GroupBox);
                                if (group == null)
                                {
                                    group = CreateGroupBox(attr.GroupBox);
                                    GroupBoxes.Add(group);
                                    table = CreateTableLayoutPanel(attr.GroupBox, 1, 2);
                                    table.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
                                    Tables.Add(table);
                                    group.Controls.Add(table);
                                }
                                if (clb == null)
                                {
                                    var newLbl = CreateLabel(attr.Subgroup);
                                    clb = CreateCheckListBox(attr.Subgroup, attr.GroupBox);
                                    var tbl = CreateTableLayoutPanel("", 2, 1);
                                    table.Controls.Add(tbl);
                                    tbl.Controls.Add(newLbl);
                                    tbl.Controls.Add(clb);
                                    CLB.Add(clb);
                                }
                                clb.Items.Add(attr.Title);
                            }
                            else
                            {
                                var newLabel = CreateLabel(attr.Title);

                                var newCB = new CheckBox();
                                newCB.Name = member.Name;
                                newCB.CheckedChanged += (s, e) => SetValueToView(member, attr, newCB.Checked.ToString());

                                var group = GroupBoxes.FirstOrDefault(g => g.Name == attr.GroupBox);
                                var table = Tables.FirstOrDefault(g => g.Name == attr.GroupBox);
                                if (group == null)
                                {
                                    group = CreateGroupBox(attr.GroupBox);
                                    GroupBoxes.Add(group);
                                    table = CreateTableLayoutPanel(attr.GroupBox, 1, 2);
                                    Tables.Add(table);
                                    group.Controls.Add(table);
                                }
                                if (table.Controls.Count > 0)
                                    table.RowCount++;
                                table.Controls.Add(newLabel);
                                table.Controls.Add(newCB);
                            }
                            break;
                        }
                    case DataType.Discrete:
                        {
                            var newCombo = CreateComboBox(member.Name, attr.ComboValues);
                            newCombo.SelectedIndexChanged += (s, e) => SetValueToView(member, attr, newCombo.SelectedItem.ToString());
                            var newLabel = CreateLabel(attr.Title);


                            var group = GroupBoxes.FirstOrDefault(g => g.Name == attr.GroupBox);
                            var table = Tables.FirstOrDefault(g => g.Name == attr.GroupBox);
                            if (group == null)
                            {
                                group = CreateGroupBox(attr.GroupBox);
                                GroupBoxes.Add(group);
                                table = CreateTableLayoutPanel(attr.GroupBox, 1, 2);
                                Tables.Add(table);
                                group.Controls.Add(table);
                            }
                            if (table.Controls.Count > 0)
                                table.RowCount++;
                            table.Controls.Add(newLabel);
                            table.Controls.Add(newCombo);

                            break;
                        }
                    default:
                        {
                            var newLabel = CreateLabel(attr.Title);
                            var newTB = CreateTextBox(member.Name);
                            newTB.Leave += (s, e) => SetValueToView(member, attr, newTB.Text);

                            var group = GroupBoxes.FirstOrDefault(g => g.Name == attr.GroupBox);
                            var table = Tables.FirstOrDefault(g => g.Name == attr.GroupBox);

                            if (group == null)
                            {
                                group = CreateGroupBox(attr.GroupBox);
                                table = CreateTableLayoutPanel(attr.GroupBox, 1, 2);
                                GroupBoxes.Add(group);
                                Tables.Add(table);
                                group.Controls.Add(table);
                            }
                            if (table.Controls.Count > 0)
                                table.RowCount++;

                            table.Controls.Add(newLabel, 0, table.RowCount - 1);
                            table.Controls.Add(newTB, 1, table.RowCount - 1);
                            break;
                        };
                }
            }
            var smallTable = CreateFlowLayoutPanel("CalculationPanel");
            smallTable.Padding = new Padding(1, 1, 1, 1);
            smallTable.Controls.AddRange(GroupBoxes.ToArray());
            return smallTable;
        }
        void SetValuesToView(Dictionary<string,string> namesValues, string group, string subgroup)
        {
            foreach (var p in _presenter.CurrentCalculation.View.GetType().GetProperties())
            {
                var attr = (DisplayTitleAttribute)p.GetCustomAttribute(typeof(DisplayTitleAttribute));
                if (attr!= null && namesValues.ContainsKey(attr.Title) && attr.GroupBox == group && attr.Subgroup == subgroup)
                    SetValueToView(p,attr,namesValues[attr.Title]);
            }

        }
        void SetValueToView(PropertyInfo prop, DisplayTitleAttribute attr, string value)
        {
            value = value.Trim();
            var view = _presenter.CurrentCalculation.View;

            switch (attr.Type)
            {
                case DataType.Bool:
                    {
                        bool val = bool.Parse(value);
                        prop.SetValue(view, val, null);
                        break;
                    }
                case DataType.Double:
                    {
                        double val;
                        if (!double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out val)
                            || !(val > attr.MoreThan && val < attr.LessThan))
                        {
                            MessageBox.Show(attr.Title + " - " + attr.ErrorText);
                            break;
                        }
                        prop.SetValue(view, val, null);
                        break;
                    }
                case DataType.Integer:
                    {
                        int val;
                        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out val)
                           || !(val > attr.MoreThan && val < attr.LessThan))
                        {
                            MessageBox.Show(attr.Title + " - " + attr.ErrorText);
                            break;
                        }
                        prop.SetValue(view, val, null);
                        break;
                    }
                default:
                    {
                        //if(!attr.DiscreteValues.Contains(value))
                        //    string ErrorText;
                        prop.SetValue(view, value, null);
                    }
                    break;
            }
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
                for (int i = 0; i < clb.Items.Count;i++ )
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
        Label CreateLabel(string name)
        {
            var newLabel = new Label();
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

