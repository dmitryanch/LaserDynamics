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
using System.IO;

namespace LaserDynamics.Clients.WinForms
{
    public partial class Main : Form
    {
        bool IsSelectedIndexChanged = true;
        Presenter _presenter;
        IList<KeyValuePair<string, Control[]>> ControlElements = new List<KeyValuePair<string, Control[]>>();
        public Main()
        {
            InitializeComponent();
            _presenter = new Presenter();
            CreateDefault();
            CreateMenuItem.Click += (s,e) => CreateDefault();
            OpenMenuItem.Click += (s, e) => Open();
            SaveMenuItem.Click += (s, e) => Save(_presenter.CurrentCalculation);
            SaveAsMenuItem.Click += (s, e) => SaveAs(_presenter.CurrentCalculation);
            CloseMenuItem.Click += (s, e) => Close(CalculationsTabControl.SelectedTab.Name);
            ExitMenuItem.Click += (s, e) => Exit();
            RenameMenuItem.Click += (s, e) => Rename();
            CalculationsTabControl.SelectedIndexChanged += (s, e) => OnSelectedPageChanged();
        }
        void OnSelectedPageChanged()
        {
            if (!IsSelectedIndexChanged)
                return;
            if (CalculationsTabControl.SelectedTab == CalculationsTabControl.TabPages["PlusTabPage"])
                CreateDefault();
            else _presenter.CurrentCalculation = _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == CalculationsTabControl.SelectedTab.Name);
        }
        void Rename()
        {
            var oldName = _presenter.CurrentCalculation.Name;
            var newName = Microsoft.VisualBasic.Interaction.InputBox("New name:", "Rename calculation", oldName);
            if (string.IsNullOrEmpty(newName) || newName == oldName)
                return;
            CalculationsTabControl.SelectedTab.Text = newName;
            _presenter.CurrentCalculation.Name = newName;
        }
        void Open()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "clc files (*.clc)|*.clc|All files (*.*)|*.*";
            dialog.RestoreDirectory = true;
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var path in dialog.FileNames)
                {
                    var calc = _presenter.OpenCalculation(dialog.FileName);
                    _presenter.AddCalculation(calc);
                    CreatePage(calc);
                }
            }
        }
        void Save(ICalculation calc)
        {
            if (calc.Path != null)
            {
                _presenter.SaveCalculation(calc, calc.Path);
                CalculationSaved(calc);
                return;
            }
            SaveAs(calc);
        }
        void SaveAs(ICalculation calc)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "clc files (*.clc)|*.clc|All files (*.*)|*.*";
            dialog.RestoreDirectory = true;
            dialog.FileName = calc.Name;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                calc.Path = dialog.FileName;
                _presenter.SaveCalculation(calc, dialog.FileName);
                CalculationSaved(calc);
            }
        }
        void Close(string id)
        {
            var calc = _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == id);
            if (!calc.IsSaved)
                Save(calc);
            _presenter.RemoveCalculation(id);
            var controls = ControlElements.FirstOrDefault(ce => ce.Key == id);
            ControlElements.Remove(controls);
            var page = CalculationsTabControl.TabPages[id];
            int index = CalculationsTabControl.TabPages.IndexOf(page);
            CalculationsTabControl.TabPages.Remove(page);
            CalculationsTabControl.SelectTab(index - 1 > -1 ? index - 1 : 0);
        }
        void Exit()
        {
            var notSavedCalc = _presenter.OpenCalculations.Where(c => !c.IsSaved).ToList();
            if (notSavedCalc == null || !notSavedCalc.Any())
                this.Close();
            var notSavedCalcsString = String.Join(",\n", notSavedCalc.Select(c => c.Name));
            if (notSavedCalc != null && MessageBox.Show("Folowing calculations not saved:\n" + notSavedCalcsString + "\nSave changes?", "LaserDynamics", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                for (int i = 0; i < notSavedCalc.Count; i++ )
                {
                    Close(notSavedCalc[i].CalculationId);
                }
                //foreach(var calc in notSavedCalc)
                //{
                //    Close(calc.CalculationId);
                //}
            }

        }
        protected override void OnClosed(EventArgs e)
        {
            Exit();
            base.OnClosed(e);
        }
        
        #region Обработчики событий при нажатии кнопок управления вычислениями
        void OnModelChanged(TableLayoutPanel controlTable)
        {
            IsSelectedIndexChanged = false;
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
            IsSelectedIndexChanged = true;
        }
        void OnCalculationTypeChanged(TableLayoutPanel controlTable)
        {
            if (!IsSelectedIndexChanged)
                return;
            IsSelectedIndexChanged = false;
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
            IsSelectedIndexChanged = true;
        }
        void OnNumMethodChanged(TableLayoutPanel controlTable)
        {
            if (!IsSelectedIndexChanged)
                return;
            IsSelectedIndexChanged = false;
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
            IsSelectedIndexChanged = true;
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
        void CreatePage(ICalculation calc)
        {
            IsSelectedIndexChanged = false;
            var newP = CreateNewTabPage(calc);
            PushTabPage(newP);
            IsSelectedIndexChanged = true;
        }
        void CreateDefault()
        {
            IsSelectedIndexChanged = false;
            string name = "Calculation " + CalculationsTabControl.TabPages.Count;
            var calc = _presenter.AddDefaultCalculation(name);
            var newP = CreateNewTabPage(calc);
            PushTabPage(newP);
            IsSelectedIndexChanged = true;
        }
        void PushTabPage(TabPage page)
        {
            CalculationsTabControl.TabPages.Add(page);
            CalculationsTabControl.TabPages.Remove(PlusTabPage);
            CalculationsTabControl.TabPages.Add(PlusTabPage);
            CalculationsTabControl.SelectedTab = page;
        }
        TableLayoutPanel CreateControlElementsPanel(string id)
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
            
            var StartBtn = CreateButton("Start");
            StartBtn.Text = "";
            StartBtn.Image = new Bitmap(@"startEnable.png"); 
            StartBtn.Size = new Size(28, 20);
            CalculationsTabControl.SelectedTab.Controls.Add(StartBtn);
            bigTable.Controls.Add(StartBtn);
            
            var StopBtn = CreateButton("Stop");
            StopBtn.Text = "";
            StopBtn.Image = new Bitmap(@"stopEnable.png");
            StopBtn.Size = new Size(28, 20);
            StopBtn.Enabled = false;
            CalculationsTabControl.SelectedTab.Controls.Add(StopBtn);
            bigTable.Controls.Add(StopBtn);
                       
            var CloseBtn = CreateButton("Close");
            CloseBtn.Text = "";
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

            ControlElements.Add(new KeyValuePair<string, Control[]>(id, new Control[] { StartBtn, StopBtn, CloseBtn, modelCombo, calcCombo, numMethodCombo }));
            SetControlEvent(id);
            return topTable;
        }
        void SetControlEvent(string id)
        {
            var controls = ControlElements.FirstOrDefault(ce => ce.Key == id);
            var StartBtn = controls.Value.FirstOrDefault(c => c.Name == "Start") as Button;
            var StopBtn = controls.Value.FirstOrDefault(c => c.Name == "Stop") as Button;
            var CloseBtn = controls.Value.FirstOrDefault(c => c.Name == "Close") as Button;
            
            StartBtn.Click += async (s, e) =>
            {
                ReverseButtonsState(StartBtn, StopBtn);
                (CalculationsTabControl.TabPages[id].Controls[0].Controls["StateLabel"] as Label).SetTextInvoke("Started...");
                await _presenter.StartCalculation();
            };
            StopBtn.Click += (s, e) =>
            {
                _presenter.StopCalculation();
            };
            _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == id).OnCalculationValidationFailed += (s, e) =>
                {
                    ReverseButtonsState(StartBtn, StopBtn);
                    (CalculationsTabControl.TabPages[id].Controls[0].Controls["StateLabel"] as Label).SetTextInvoke("Validation Error."); 
                    MessageBox.Show("Не все поля заполнены корректно", "Validation Error");
                };
            _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == id).OnCalculationFinish += (s, e) =>
            {
                var calc = _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == id);
                var stateLabel = CalculationsTabControl.TabPages[id].Controls[0].Controls["StateLabel"] as Label;
                if (calc.ErrorMessage != null)
                {
                    stateLabel.SetTextInvoke("Error: " + calc.ErrorMessage);
                }
                else if (calc.Status == CalculationStatus.Stopped)
                {
                    stateLabel.SetTextInvoke("Stopped.");
                }
                else
                {
                    stateLabel.SetTextInvoke("Succesefully performed: " + string.Join(", ", calc.GetStats().Select(st => st.ToString() + " ms")));
                }
                ReverseButtonsState(StartBtn, StopBtn);
            };
            _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == id).OnCalculationError += (s, e) =>
            {
                var calc = _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == id);
                if (calc.ErrorMessage != null)
                {
                    (CalculationsTabControl.TabPages[id].Controls[0].Controls["StateLabel"] as Label).SetTextInvoke("Error: " + calc.ErrorMessage);
                }
                ReverseButtonsState(StartBtn, StopBtn);
            };
            _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == id).OnCalculationReport += (s, e) =>
            {
                var calc = _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == id);
                (CalculationsTabControl.TabPages[id].Controls[0].Controls["StateLabel"] as Label).SetTextInvoke("Running: " + calc.ReportMessage + "%");
            };
            CloseBtn.Click += (s, e) => Close(id);
            
        }
        TabPage CreateNewTabPage(ICalculation calc)
        {
            var newPage = new TabPage();
            var name = calc.Name;
            var template = calc.View;
            newPage.Name = calc.CalculationId;
            newPage.Text = name;
            newPage.Padding = new Padding(1, 1, 1, 1);
            newPage.AutoScroll = true;
            var tablePanel = CreateTableLayoutPanel(name, 3, 1);
            tablePanel.Margin = new Padding(1, 1, 1, 1);
            tablePanel.Padding = new Padding(0, 0, 0, 0);
            tablePanel.RowStyles[1] = new RowStyle(SizeType.Percent, 100F);
            tablePanel.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100F);
            newPage.Controls.Add(tablePanel);
            var newGroup = CreateControlElementsPanel(newPage.Name);
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
                                    clb.ItemCheck += (s, e) => CalculationChanged(_presenter.CurrentCalculation);
                                }
                                clb.Items.Add(attr.Title, (bool)member.GetValue(tempate));
                            }
                            else
                            {
                                var newLabel = CreateLabel(attr.Title);

                                var newCB = new CheckBox();
                                newCB.Name = member.Name;
                                var str = "";
                                newCB.CheckedChanged += (s, e) => TrySetValueToView(member, attr, newCB.Checked.ToString(), out str);
                                newCB.Checked = (bool)member.GetValue(tempate);

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
                                newCB.CheckedChanged += (s, e) => CalculationChanged(_presenter.CurrentCalculation);
                            }
                            break;
                        }
                    case DataType.Discrete:
                        {
                            var newCombo = CreateComboBox(member.Name, attr.ComboValues);
                            var str = "";
                            newCombo.SelectedIndexChanged += (s, e) => TrySetValueToView(member, attr, newCombo.SelectedItem.ToString(), out str);
                            newCombo.SelectedItem = member.GetValue(tempate);
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
                            newCombo.SelectedIndexChanged += (s, e) => CalculationChanged(_presenter.CurrentCalculation);
                            break;
                        }
                    default:
                        {
                            var newLabel = CreateLabel(attr.Title);
                            var newTB = CreateTextBox(member.Name);
                            newTB.Text = member.GetValue(tempate).ToString();

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
                            var errorMessage = "";
                            newTB.TextChanged += (s, e) => CalculationChanged(_presenter.CurrentCalculation);
                            newTB.Leave += (s, e) =>
                            {
                                if (!TrySetValueToView(member, attr, newTB.Text, out errorMessage))
                                {
                                    newTB.BackColor = Color.LightCoral;
                                    ErrorToolTip.SetToolTip(newTB, errorMessage);
                                    newTB.Enter += (send, ev) => newTB.BackColor = Color.White;
                                }
                            };

                            break;
                        };
                }
            }
            var smallTable = CreateFlowLayoutPanel("CalculationPanel");
            smallTable.Padding = new Padding(1, 1, 1, 1);
            smallTable.Controls.AddRange(GroupBoxes.ToArray());
            return smallTable;
        }
        void CalculationChanged(ICalculation calc)
        {
            if (calc.IsSaved)
            {
                calc.IsSaved = false;
                CalculationsTabControl.TabPages[calc.CalculationId].Text = calc.Name + "*";
                //CalculationsTabControl.SelectedTab.();
            }
        }
        void CalculationSaved(ICalculation calc)
        {
            if (!calc.IsSaved)
            {
                calc.IsSaved = true;
                CalculationsTabControl.TabPages[calc.CalculationId].Text = calc.Name;
                //CalculationsTabControl.SelectedTab.();
            }
        }
        void SetValuesToView(Dictionary<string,string> namesValues, string group, string subgroup)
        {
            foreach (var p in _presenter.CurrentCalculation.View.GetType().GetProperties())
            {
                var attr = (DisplayTitleAttribute)p.GetCustomAttribute(typeof(DisplayTitleAttribute));
                var str = "";
                if (attr!= null && namesValues.ContainsKey(attr.Title) && attr.GroupBox == group && attr.Subgroup == subgroup)
                    TrySetValueToView(p,attr,namesValues[attr.Title], out str);
            }

        }
        bool TrySetValueToView(PropertyInfo prop, DisplayTitleAttribute attr, string value, out string ErrorMessage)
        {
            ErrorMessage = null;
            value = value.Trim();
            var view = _presenter.CurrentCalculation.View;

            switch (attr.Type)
            {
                case DataType.Bool:
                    {
                        bool val = bool.Parse(value);
                        prop.SetValue(view, val, null);
                        return true;
                    }
                case DataType.Double:
                    {
                        double val;
                        if (!double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out val)
                            || !(val > attr.MoreThan && val < attr.LessThan))
                        {
                            ErrorMessage = attr.ErrorText;
                            return false;
                        }
                        prop.SetValue(view, val, null);
                        return true;
                    }
                case DataType.Integer:
                    {
                        int val;
                        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out val)
                           || !(val > attr.MoreThan && val < attr.LessThan))
                        {
                            ErrorMessage = attr.ErrorText;
                            return false;
                        }
                        prop.SetValue(view, val, null);
                        return true;
                    }
                default:
                    {
                        prop.SetValue(view, value, null);
                        return true;
                    }
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

